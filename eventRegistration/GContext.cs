using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace eventRegistration
{
    public class GContext : DbContext
    {
        public GContext(DbContextOptions<GContext> options) : base(options) { }
        public DbSet<Guest> Guest { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Guest>().HasIndex(c => c.Email).IsUnique(true);
            modelBuilder.Entity<Guest>().Property(c => c.Id).HasDefaultValueSql("newid()");

            base.OnModelCreating(modelBuilder);

        }

    }
}
