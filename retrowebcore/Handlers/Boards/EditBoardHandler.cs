using MediatR;
using Microsoft.EntityFrameworkCore;
using retrowebcore.Models;
using retrowebcore.Persistences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace retrowebcore.Handlers.Boards
{
    public class EditBoardRequest : IRequest<Board> 
    {
        public Board Board { get; set; }
    }

    public class EditBoardHandler : BoardHandlerBase, IRequestHandler<EditBoardRequest, Board>
    {
        public EditBoardHandler(AppDbContext c) : base(c) { }

        public async Task<Board> Handle(EditBoardRequest r, CancellationToken ct)
        {
            var board = await _context.Boards.FirstOrDefaultAsync(x => x.Slug == r.Board.Slug);
            if (board == null)
                return null;

            board.Name = r.Board.Name;
            board.Description = r.Board.Description;
            await _context.SaveChangesAsync();
            return board;
        }
    }
}
