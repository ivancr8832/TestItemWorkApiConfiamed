using ItemWorks.Api.Domain.Entities;
using ItemWorks.Api.Domain.Repositories.Base;

namespace ItemWorks.Api.Domain.Repositories
{
    public interface IItemWorkRepository : IRepository<ItemWork>
    {
        Task<Dictionary<int, List<ItemWork>>> GetPendings();
        Task<IEnumerable<ItemWork>> GetPendingByUser(int userId);
    }
}
