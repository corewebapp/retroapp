using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using retrowebcore.Handlers.Boards;
using retrowebcore.Models;
using retrowebcore.Persistences;

namespace retrowebcore.Controllers
{
    [Authorize]
    public class BoardController : BaseController<BoardController>
    {
        #region consts
        const string BoardList = nameof(BoardList);
        const string BoardView = nameof(BoardView);
        const string BoardEdit = nameof(BoardEdit);
        const string BoardSearch = nameof(BoardSearch);
        #endregion
        public BoardController(ILogger<BoardController> l, IMediator m) : base(l, m) { }

        public async Task<IActionResult> List()
        {
            var boards = await _mediator.Send(new BoardListRequest());
            return View(BoardList, boards);
        }

        public async Task<IActionResult> View(Guid id) 
        {
            var board = await _mediator.Send(new ViewBoardRequest(id));
            return View(BoardView, board);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var board = await _mediator.Send(new ViewBoardRequest(id));
            return View(BoardEdit, board);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Board board)
        {
            var result = await _mediator.Send(new EditBoardRequest(board));
            return View(BoardEdit, result);
        }

        public async Task<IActionResult> Search(string q)
        {
            TempData[R.BoardQueryKey] = q;
            var boards = await _mediator.Send(new SearchBoardRequest(q));
            return View(BoardList, boards);
        }

        public IActionResult Privacy() => View();
    }
}
