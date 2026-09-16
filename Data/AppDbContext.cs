using Microsoft.EntityFrameworkCore;
using NetWaveSupportAPI.Models;

namespace NetWaveSupportAPI.Data
{
 
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options)
                : base(options)
            {
            }

            public DbSet<Customer> Customers { get; set; }

            public DbSet<SupportTicket> SupportTickets { get; set; }

        }
}
