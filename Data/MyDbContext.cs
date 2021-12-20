using Microsoft.EntityFrameworkCore;
using SignalR.Model;

namespace SignalR.Data
{
    public class MyDbContext:DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
            
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}