using retrowebcore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace retrowebcore.Persistences.Repositories
{
    public abstract class RepositoryWithEFContext<T> : IRepository<T>
    {
        protected AppDbContext _context;

        public RepositoryWithEFContext(AppDbContext c) => _context = c;
        public abstract Task<List<T>> FindAll(Expression<Func<T, bool>> predicate, string tag = "");
        public abstract Task<List<T>> FindAll(RepoQuery<T> q, string tag = "");
        public abstract Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate, string tag = "");
        public async Task SaveChanges() => await _context.SaveChangesAsync();
    }
}
