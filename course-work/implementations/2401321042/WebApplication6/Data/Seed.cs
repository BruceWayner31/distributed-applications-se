using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;
using static WebApplication6.Models.EventsManagment;

namespace WebApplication6.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(EventsContext context)
        {
            // ако вече има данни — не seed-вай
            if (await context.Users.AnyAsync(u => u.Username == "admin")) return;

            // ── USERS ──
            var admin = new Users
            {
                Username = "admin",
                Email = "admin@eventify.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Fname = "Admin",
                Lname = "User",
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };
            var organizer = new Users
            {
                Username = "organizer",
                Email = "organizer@eventify.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Organizer123!"),
                Fname = "John",
                Lname = "Doe",
                Role = "Organizer",
                CreatedAt = DateTime.UtcNow
            };
            var attendee = new Users
            {
                Username = "attendee",
                Email = "attendee@eventify.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Attendee123!"),
                Fname = "Jane",
                Lname = "Smith",
                Role = "Attendee",
                CreatedAt = DateTime.UtcNow
            };
            context.Users.AddRange(admin, organizer, attendee);
            await context.SaveChangesAsync();

            // ── VENUES ──
            var venue1 = new Venues
            {
                Name = "Grand Concert Hall",
                Address = "123 Main Street",
                City = "Sofia",
                Capacity = 500,
                Description = "A stunning concert hall in the heart of Sofia.",
                ContactEmail = "contact@grandconcert.bg",
                CreatedAt = DateTime.UtcNow
            };
            var venue2 = new Venues
            {
                Name = "Tech Hub Conference Center",
                Address = "456 Innovation Blvd",
                City = "Plovdiv",
                Capacity = 200,
                Description = "Modern conference center for tech events.",
                ContactEmail = "info@techhub.bg",
                CreatedAt = DateTime.UtcNow
            };
            var venue3 = new Venues
            {
                Name = "Outdoor Arena",
                Address = "789 Park Avenue",
                City = "Varna",
                Capacity = 2000,
                Description = "Open air arena perfect for festivals.",
                ContactEmail = "arena@varna.bg",
                CreatedAt = DateTime.UtcNow
            };
            context.Venues.AddRange(venue1, venue2, venue3);
            await context.SaveChangesAsync();

            // ── CATEGORIES ──
            var catMusic = new Categories
            {
                Name = "Music",
                Description = "Concerts and live music events",
                ColorHex = "#E8FF47",
                isActived = true,
                Created = DateTime.UtcNow
            };
            var catTech = new Categories
            {
                Name = "Technology",
                Description = "Tech conferences and workshops",
                ColorHex = "#3B82F6",
                isActived = true,
                Created = DateTime.UtcNow
            };
            var catFestival = new Categories
            {
                Name = "Festival",
                Description = "Outdoor festivals and fairs",
                ColorHex = "#FF6B35",
                isActived = true,
                Created = DateTime.UtcNow
            };
            var catFree = new Categories
            {
                Name = "Free Entry",
                Description = "Events with free admission",
                ColorHex = "#2ECC71",
                isActived = true,
                Created = DateTime.UtcNow
            };
            context.Categories.AddRange(catMusic, catTech, catFestival, catFree);
            await context.SaveChangesAsync();

            // ── EVENTS ──
            var event1 = new Events
            {
                Title = "Summer Jazz Night",
                Description = "An unforgettable evening of jazz under the stars featuring top local artists.",
                StartDate = DateTime.UtcNow.AddDays(14),
                EndDate = DateTime.UtcNow.AddDays(14).AddHours(4),
                TicketPrice = 25.00,
                MaxCapacity = 300,
                Status = "Published",
                VenuesId = venue1.Id,
                OrganizerId = organizer.Id,
                CreatedAt = DateTime.UtcNow
            };
            var event2 = new Events
            {
                Title = "AI & Future Tech Conference 2026",
                Description = "Leading experts discuss the future of artificial intelligence and its impact on society.",
                StartDate = DateTime.UtcNow.AddDays(21),
                EndDate = DateTime.UtcNow.AddDays(21).AddHours(8),
                TicketPrice = 99.00,
                MaxCapacity = 150,
                Status = "Published",
                VenuesId = venue2.Id,
                OrganizerId = organizer.Id,
                CreatedAt = DateTime.UtcNow
            };
            var event3 = new Events
            {
                Title = "Varna Summer Festival",
                Description = "Three days of music, food and culture at the beautiful Varna seafront.",
                StartDate = DateTime.UtcNow.AddDays(30),
                EndDate = DateTime.UtcNow.AddDays(33),
                TicketPrice = 0,
                MaxCapacity = 2000,
                Status = "Published",
                VenuesId = venue3.Id,
                OrganizerId = organizer.Id,
                CreatedAt = DateTime.UtcNow
            };
            var event4 = new Events
            {
                Title = "Web Development Workshop",
                Description = "Hands-on workshop covering modern web development with ASP.NET Core and React.",
                StartDate = DateTime.UtcNow.AddDays(7),
                EndDate = DateTime.UtcNow.AddDays(7).AddHours(6),
                TicketPrice = 49.00,
                MaxCapacity = 50,
                Status = "Published",
                VenuesId = venue2.Id,
                OrganizerId = organizer.Id,
                CreatedAt = DateTime.UtcNow
            };
            context.Events.AddRange(event1, event2, event3, event4);
            await context.SaveChangesAsync();

            // ── EVENT CATEGORIES ──
            context.EventCategories.AddRange(
                new EventCategories { EventId = event1.Id, CategoryId = catMusic.Id, AssignedAt = DateTime.UtcNow },
                new EventCategories { EventId = event2.Id, CategoryId = catTech.Id, AssignedAt = DateTime.UtcNow },
                new EventCategories { EventId = event3.Id, CategoryId = catFestival.Id, AssignedAt = DateTime.UtcNow },
                new EventCategories { EventId = event3.Id, CategoryId = catFree.Id, AssignedAt = DateTime.UtcNow },
                new EventCategories { EventId = event4.Id, CategoryId = catTech.Id, AssignedAt = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();

            // ── REGISTRATIONS ──
            context.Registrations.AddRange(
                new Registrations
                {
                    UserId = attendee.Id,
                    EventId = event1.Id,
                    TicketCount = 2,
                    TotalPrice = 2 * event1.TicketPrice,
                    Status = "Confirmed",
                    Notes = "Front row seats please",
                    RegisteredAt = DateTime.UtcNow
                },
                new Registrations
                {
                    UserId = attendee.Id,
                    EventId = event2.Id,
                    TicketCount = 1,
                    TotalPrice = 1 * event2.TicketPrice,
                    Status = "Pending",
                    RegisteredAt = DateTime.UtcNow
                }
            );
            await context.SaveChangesAsync();
        }
    }
}