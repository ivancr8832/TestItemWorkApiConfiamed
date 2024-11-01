using ItemWorks.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ItemWorks.Api.Infrastructure.Data
{
    public class ItemWorkDbContext : DbContext
    {
        public ItemWorkDbContext(DbContextOptions<ItemWorkDbContext> options) : base (options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public DbSet<ItemWork> ItemWorks { get; set; }
    }
}
