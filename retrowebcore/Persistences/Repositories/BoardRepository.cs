using Microsoft.EntityFrameworkCore;
using retrowebcore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace retrowebcore.Persistences.Repositories
{
    public class BoardRepository : RepositoryWithEFContext<Board>
    {


        public BoardRepository(AppDbContext c) : base(c) { }

        public override async Task<List<Board>> FindAll(Expression<Func<Board, bool>> p, string tag = "")
        {
            return await _context.Boards
                .TagWith(tag)
                .Include(x => x.Cards)
                .Where(p)
                .ToListAsync();
        }

        public override async Task<List<Board>> FindAll(RepoQuery<Board> q, string tag = "")
        {
            return await _context.Boards
                .TagWith(tag)
                .Include(x => x.Cards)
                .Where(q.Predicate)
                .OrderBy(q.OrderAscending)
                .Skip(q.Skip)
                .Take(q.Take)
                .ToListAsync();
        }

        public override async Task<Board> FirstOrDefault(Expression<Func<Board, bool>> p, string tag = "") =>
            await _context.Boards
            .TagWith(tag)
            .Include(x => x.Cards)
            .FirstOrDefaultAsync(p);
    }
}
