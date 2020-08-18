using MediatR;
using Microsoft.EntityFrameworkCore;
using retrowebcore.Models;
using retrowebcore.Persistences;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace retrowebcore.Handlers.Boards
{
    public class BoardListRequest : IRequest<BoardListResponse>
    {
        public int Offset { get; set; } = 0;
        public int Limit { get; set; } = 100;
    }
    public class BoardListResponse
    {
        public bool HasPrev { get; set; }
        public bool HasNext { get; set; }
        public List<Board> Data { get; set; }
    }
    public class BoardListHandler : BoardHandlerBase, IRequestHandler<BoardListRequest, BoardListResponse>
    {
        static readonly string BoardListHandlerQuery = nameof(BoardListHandlerQuery);
        public BoardListHandler(AppDbContext c): base(c) { }

        public async Task<BoardListResponse> Handle(BoardListRequest r, CancellationToken ct)
        {
            var data = await _context.Boards
                .TagWith(BoardListHandlerQuery)
                .Where(x => x.Deletor == null && x.DeletedAt == null)
                .OrderBy(x => x.Created)
                .Skip(r.Offset)
                .Take(r.Limit)
                .ToListAsync();
            #region some other logic
            var hasMore = false;
            var hasPrev = false;
            if (data.Count > 0)
            {
                var soonest = data.First();
                var oldest = data.Last();
                hasMore = await _context.Boards
                    .OrderBy(x => x.Created)
                    .AnyAsync(x => 
                        x.DeletedAt == null && 
                        x.Deletor == null && 
                        x.Created > oldest.Created &&
                        x.Id != oldest.Id);
                hasPrev = await _context.Boards
                    .OrderBy(x => x.Created)
                    .Skip(r.Offset)
                    .AnyAsync(x => 
                        x.DeletedAt == null && 
                        x.Deletor == null && 
                        x.Created < soonest.Created &&
                        x.Id != soonest.Id);
            }
            #endregion

            return new BoardListResponse { HasPrev = hasPrev, HasNext = hasMore, Data = data };
        }
    }
    
}
