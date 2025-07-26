using Microsoft.EntityFrameworkCore;

namespace CldvPOEnew.Models
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<Venue> Venue { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<Booking> Booking { get; set; }

        public DbSet<EventType> EventTypes
        {
            get; set;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>().ToTable("Booking");
            modelBuilder.Entity<Event>().ToTable("Event");
            modelBuilder.Entity<Venue>().ToTable("Venue");
            modelBuilder.Entity<EventType>().ToTable("EventTypes");
        }
    }
}
