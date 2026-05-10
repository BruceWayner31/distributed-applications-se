using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication6.Models
{
    public class Events
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(50, ErrorMessage = "Title cannot exceed 50 characters")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [MaxLength(300, ErrorMessage = "Description cannot exceed 300 characters")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Ticket price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
        public double TicketPrice { get; set; }
        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")]
        public int MaxCapacity { get; set; }
        [Required(ErrorMessage = "Status is required")]
        [MaxLength(20)]
        public string Status { get; set; }
        [MaxLength(300)]
        public string? ImageUrl { get; set; }
        [Required(ErrorMessage = "Venue is required")]
        public int VenuesId { get; set; }
        [ForeignKey("VenuesId")]

        public Venues? Venues { get; set; }
        [Required(ErrorMessage = "Organizer is required")]

        public int OrganizerId { get; set; }
        [ForeignKey("OrganizerId")]
        public Users? Users { get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.Now;
        // one Event has many EventCategory rows
        public ICollection<EventCategories> EventCategories { get; set; }
            = new List<EventCategories>();

        // one Event has many Registrations
        public ICollection<Registrations> Registrations { get; set; }
            = new List<Registrations>();


    }
}
