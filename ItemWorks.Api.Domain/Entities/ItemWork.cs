using ItemWorks.Api.Domain.Enums;
using User.Api.Domain.Common;

namespace ItemWorks.Api.Domain.Entities
{
    public class ItemWork : SqlBaseEntity
    {
        public string Description { get; set; }
        public DateTime DeliveryDate { get; set; }
        public RelevanceType Relevance { get; set; }
        public ItemStatus Status { get; set; }
        public int UserId { get; set; }
    }
}
