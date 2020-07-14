using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using retrowebcore.Handlers.Boards;
using retrowebcore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace retrowebcore.Handlers.Boards.Tests
{
    [TestFixture()]
    public class BoardListHandlerTests
    {
        [Test()]
        public async Task HandleTest()
        {
            var options = TestDbContext.NewDefaultOption();
            var slugs = new Guid[] { Guid.NewGuid(), Guid.NewGuid() };

            using var context = new TestDbContext(options, long.MaxValue);
            context.Boards.Add(new Board { Slug = slugs[0] });
            await context.SaveChangesAsync();
            
            context.Boards.Add(new Board { Slug = slugs[1] });
            await context.SaveChangesAsync();

            var handler = new BoardListHandler(context);
            var ct = CancellationToken.None;
            var result = await handler.Handle(new BoardListRequest { Limit = 1, Offset = 0}, ct);

            Assert.AreEqual(1,result.Data.Count);
            Assert.AreEqual(true, result.HasNext);
            Assert.AreEqual(false, result.HasPrev);
            var board = result.Data.FirstOrDefault(x => x.Slug == slugs[0] || x.Slug == slugs[1]);
            Assert.IsNotNull(board);

        }
    }
}