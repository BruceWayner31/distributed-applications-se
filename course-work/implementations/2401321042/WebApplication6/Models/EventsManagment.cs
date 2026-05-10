using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApplication6.Models
{
    public class EventsManagment
    {
           
        public class EventsContext : DbContext
        {
            public EventsContext(DbContextOptions<EventsContext> options) : base(options)
            {
            }
            public DbSet<Events> Events { get; set; }
         public DbSet<Categories> Categories { get; set; }
            public DbSet<EventCategories> EventCategories { get; set; }
            public DbSet<Users> Users { get; set; }
            public DbSet<Venues> Venues { get; set; }
            public DbSet<Registrations> Registrations { get; set; }

            public DbSet<ErrorResponse> ErrorResponses { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\molaa\Documents\EventManagment.mdf;Integrated Security=True;Connect Timeout=30");

           }
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Events>()
       .HasOne(e => e.Users)
       .WithMany()
       .HasForeignKey(e => e.OrganizerId)
       .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
