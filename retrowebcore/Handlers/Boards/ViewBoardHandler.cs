using MediatR;
using Microsoft.EntityFrameworkCore;
using retrowebcore.Models;
using retrowebcore.Persistences;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace retrowebcore.Handlers.Boards
{
    #region View Board
    public class ViewBoardRequest : IRequest<BoardDetail>
    { 
        public Guid Slug { get; set; }
        public ViewBoardRequest() { }
        public ViewBoardRequest(Guid s) => Slug = s;
    }

    public class ViewBoardHandler : IRequestHandler<ViewBoardRequest, BoardDetail> 
    {
        const string ViewBoardQuery = nameof(ViewBoardQuery);
        readonly IRepository<Board> _boardRepo;
        public ViewBoardHandler(IRepository<Board> b) => _boardRepo = b;

        public async Task<BoardDetail> Handle(ViewBoardRequest request, CancellationToken ct)
        {
            var board = await _boardRepo.FirstOrDefault(x => x.Slug == request.Slug, ViewBoardQuery);
            return new BoardDetail(board);
        }
    }
    #endregion
}
