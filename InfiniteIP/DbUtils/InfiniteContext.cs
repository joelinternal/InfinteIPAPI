using InfiniteIP.Models;
using Microsoft.EntityFrameworkCore;

namespace InfiniteIP.DbUtils
{
    public class InfiniteContext:DbContext
    {
        public InfiniteContext(DbContextOptions dbContextOptions):base(dbContextOptions)
        {
            
        }

        public DbSet<GmSheet> GmSheet { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Sow> Sow { get; set; }
        public DbSet<GmRunsheet> GmRunsheet { get; set; }
    }
}
