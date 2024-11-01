using ItemWorks.Api.Domain.Enums;

namespace ItemWorks.Api.Contracts.DTOs
{
    public class ItemWorkDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime DeliveryDate { get; set; }
        public RelevanceType Relevance { get; set; }
        public ItemStatus Status { get; set; }
        public int UserId { get; set; }
    }
}
