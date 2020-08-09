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
        const string BoardList = nameof(BoardList);
        const string BoardView = nameof(BoardView);
        const string BoardEdit = nameof(BoardEdit);
        const string BoardSearch = nameof(BoardSearch);

        AppDbContext c;

        public BoardController(ILogger<BoardController> l, IMediator m, AppDbContext c) : base(l,m) 
        {
            this.c = c;
        }

        public async Task<IActionResult> List() =>
            View(BoardList, await _mediator.Send(new BoardListRequest()));

        public async Task<IActionResult> View(Guid id) 
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var board = await _mediator.Send(new ViewBoardRequest{ Slug = id });
            return View(BoardView, board);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var board = await _mediator.Send(new ViewBoardRequest { Slug = id });
            return View(BoardEdit, board);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Board board)
        {
            var result = await _mediator.Send(new EditBoardRequest { Board = board });
            return View(BoardEdit, result);
        }

        public IActionResult Privacy() => View();

        public IActionResult Search(string q) => View(BoardSearch, q);

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
