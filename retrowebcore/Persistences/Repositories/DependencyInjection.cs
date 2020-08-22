using Microsoft.Extensions.DependencyInjection;
using retrowebcore.Models;

namespace retrowebcore.Persistences.Repositories
{
    public static class DependencyInjection 
    {
        public static void AddRepositories(this IServiceCollection s) 
        {
            s.AddScoped(typeof(IRepository<Board>),typeof(BoardRepository));
        }
    }
}
