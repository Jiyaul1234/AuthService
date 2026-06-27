using Ecommerce.AuthService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.AuthService.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
