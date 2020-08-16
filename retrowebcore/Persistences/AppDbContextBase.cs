using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using NpgsqlTypes;
using retrowebcore.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace retrowebcore.Persistences
{
    public static class StringExtension
    {
        private const string Controller = "controller";
        private const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";

        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) { return input; }

            var startUnderscores = Regex.Match(input, @"^_+");
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }
    }

    public partial class AppDbContext : IdentityDbContext<AppUser, IdentityRole<long>, long>
    {
        const string Added = "Added";
        const string Modified = "Modified";
        const string Deleted = "Deleted";
        const string InvalidPrefix = "asp_net_";
        const string ValidPrefix = "app_";
        public const string SearchablePropertyVector = "search_vector";
        public const string SearchableContent = "search_content";

        /// <summary>
        /// well, this is quite hacky, for testing purpose. Dont touch this in production, keep it null
        /// </summary>
        protected long? ScopedUserId = null;
        private ILoggerFactory _loggerFactory = null;

        public AppDbContext(DbContextOptions<AppDbContext> options, ILoggerFactory loggerFactory)
            : base(options)
        {
            _loggerFactory = loggerFactory;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(_loggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            AddShadowPropertyToSearchables(builder);

            base.OnModelCreating(builder);
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                var tablename = entity.GetTableName().ToSnakeCase();
                if (tablename.StartsWith(InvalidPrefix))
                    entity.SetTableName(tablename.Replace(InvalidPrefix, ValidPrefix));
                else
                    entity.SetTableName(tablename);
            }

            builder.Entity<Board>().HasIndex(g => g.Slug).IsUnique();


        }

        private static void AddShadowPropertyToSearchables(ModelBuilder builder)
        {
            var type = typeof(ISearchable);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            foreach (var eachType in types)
            {
                if (eachType.IsInterface) continue;
                var searchable = builder.Entity(eachType);
                searchable.Property<NpgsqlTsVector>(SearchablePropertyVector);
                searchable.HasIndex(SearchablePropertyVector)
                    .HasMethod("GIN");

                searchable.Property<string>(SearchableContent);
            }
        }

        protected string GetScopedUserId() 
        {
            if (ScopedUserId.HasValue)
                return $"{ScopedUserId.Value}";

            var httpContextAccessor = this.GetService<IHttpContextAccessor>();
            return httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            BeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            BeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public Task<int> SaveChangesHardDeleteAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            BeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        void BeforeSaving()
        {
            var userId = GetScopedUserId();
            long? userLong = long.TryParse(userId, out var result) ? result : default;

            foreach (var entry in ChangeTracker.Entries())
            {
                var state = entry.State.ToString();

                if (entry.Entity is IAuditable auditable)
                {
                    if (state == Added)
                    {
                        auditable.Created = auditable.Updated = DateTime.Now;
                        if (userLong.HasValue)
                            auditable.Creator = auditable.Updator = userLong.Value;
                    }
                    else
                    {
                        entry.Property(nameof(IAuditable.Creator)).IsModified = false;
                        entry.Property(nameof(IAuditable.Created)).IsModified = false;
                        if (state == Modified)
                        {

                            auditable.Updated = DateTime.Now;
                            if (userLong.HasValue)
                                auditable.Updator = userLong.Value;
                        }

                    }
                }

                if (entry.Entity is ISearchable searchable) 
                {
                    if (state == Added || state == Modified) 
                    {
                        var content = searchable.GetSearchableContent();
                        if (!string.IsNullOrWhiteSpace(content))
                            entry.Property(SearchableContent).CurrentValue = content;
                    }
                }

                if (entry.Entity is ISoftDeletable softdeletable)
                {
                    if (state == Deleted)
                    {
                        entry.State = EntityState.Modified;
                        softdeletable.DeletedAt = DateTime.Now;
                        if (userLong.HasValue)
                            softdeletable.Deletor = userLong.Value;
                    }
                }
            }
        }
    }
}
