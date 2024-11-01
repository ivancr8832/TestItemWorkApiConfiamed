using ItemWorks.Api.Domain.Entities;
using ItemWorks.Api.Domain.Enums;
using ItemWorks.Api.Domain.Repositories;
using ItemWorks.Api.Infrastructure.Data;
using ItemWorks.Api.Infrastructure.RepositorieImpl.Base;
using Microsoft.EntityFrameworkCore;

namespace ItemWorks.Api.Infrastructure.RepositorieImpl
{
    public class ItemWorkRespository : Repository<ItemWork>, IItemWorkRepository
    {
        public ItemWorkRespository(ItemWorkDbContext itemWorkContext) : base(itemWorkContext)
        {
        }

        public async Task<IEnumerable<ItemWork>> GetPendingByUser(int userId)
        {
            var pendingItems = await _itemWorkContext.Set<ItemWork>()
                    .Where(x => x.Status == ItemStatus.Pending && x.UserId == userId).ToListAsync();

            return pendingItems;
        }

        public async Task<Dictionary<int, List<ItemWork>>> GetPendings()
        {
            var pendingItems = await _itemWorkContext.Set<ItemWork>().Where(x => x.Status == ItemStatus.Pending).ToListAsync();

            var pendingByUser = pendingItems
                .Where(i => i.Status != ItemStatus.Pending)
                .GroupBy(i => i.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => g
                    .OrderByDescending(i => i.Relevance == RelevanceType.High)
                    .ThenBy(i => i.DeliveryDate)
                    .ToList()
                );

            return pendingByUser;
        }
    }
}
