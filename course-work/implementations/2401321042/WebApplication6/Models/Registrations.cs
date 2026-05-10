using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication6.Models
{
    public class Registrations
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "User is required")]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public Users? Users { get; set; }
        [Required(ErrorMessage = "Event is required")]
        public int EventId { get; set; }
        [ForeignKey("EventId")]
        public Events? Events { get; set; }
        [Required(ErrorMessage = "Ticket count is required")]
        [Range(1, 100, ErrorMessage = "Ticket count must be between 1 and 100")]
        public int TicketCount { get; set; }
        public double TotalPrice { get; set; }
        [Required(ErrorMessage = "Status is required")]
        [MaxLength(20)]
        public string Status { get; set; }
        [MaxLength(300, ErrorMessage = "Notes cannot exceed 300 characters")]
        public string? Notes { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
}
