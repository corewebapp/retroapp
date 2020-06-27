﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using retrowebcore.Models;
using retrowebcore.Persistences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace retrowebcore.Handlers.Mediators
{
    #region BoardListRequest
    public class BoardListRequest : IRequest<BoardListResponse> 
    {
        public int Offset { get; set; } = 0;
        public int Limit { get; set; } = 100;
    }

    public class BoardListHandler : IRequestHandler<BoardListRequest, BoardListResponse>
    {
        static readonly string BoardListHandlerQuery = nameof(BoardListHandlerQuery);
        readonly AppDbContext _context;
        public BoardListHandler(AppDbContext c) 
        {
            _context = c;
        }

        public async Task<BoardListResponse> Handle(BoardListRequest r, CancellationToken ct)
        {
            var data = await _context.Boards
                .TagWith(BoardListHandlerQuery)
                .Where(x => x.Deletor == null && x.DeletedAt == null)
                .OrderByDescending(x => x.Created)
                .Take(r.Limit)
                .Skip(r.Offset)
                .ToListAsync();
            var hasMore = false;
            var hasPrev = false;
            if (data.Count > 0)
            {
                var soonest = data.First().Created;
                var oldest = data.Last().Created;
                hasMore = await _context.Boards.AnyAsync(x => x.Created < oldest);
                hasPrev = await _context.Boards.AnyAsync(x => x.Created > soonest);
            }

            return new BoardListResponse { HasPrev = hasPrev, HasNext = hasMore, Data = data };
        }
    }

    public class BoardListResponse 
    {
        public bool HasPrev { get; set; }
        public bool HasNext { get; set; }
        public List<Board> Data { get; set; }
    }
    #endregion

    #region CreateBoardRequest
    public class CreateBoardRequest : IRequest<Board> 
    {
        public string Squad { get; set; }
        public string Sprint { get; set; }
    }

    public class CreateBoardHandler : IRequestHandler<CreateBoardRequest, Board>
    {
        readonly AppDbContext _context;
        public CreateBoardHandler(AppDbContext c)
        {
            _context = c;
        }

        public async Task<Board> Handle(CreateBoardRequest request, CancellationToken ct)
        {
            var newBoard = new Board { Name = $"{request.Squad} <[<[]>]> {request.Sprint}" };
            await _context.AddAsync(newBoard);
            await _context.SaveChangesAsync(ct);
            return newBoard;
        }
    }
    #endregion
}
