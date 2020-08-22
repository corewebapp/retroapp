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

        readonly IRepository<Board> _boardRepo;

        public BoardListHandler(AppDbContext c, IRepository<Board> b) : base(c) => _boardRepo = b;

        public async Task<BoardListResponse> Handle(BoardListRequest r, CancellationToken ct)
        {
            var repoQuery = new RepoQuery<Board>
            {
                Predicate = x => x.Deletor == null && x.DeletedAt == null,
                Skip = r.Offset,
                Take = r.Limit,
                OrderAscending = x => x.Created
            };

            var data = await _boardRepo.FindAll(repoQuery, BoardListHandlerQuery);
            #region some other logic
            var hasMore = false;
            var hasPrev = false;
            if (data.Count > 0)
            {
                var soonest = data.First();
                var oldest = data.Last();
                hasMore = await _context.Boards
                    .OrderBy(x => x.Created)
                    .AnyAsync();
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
