using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication6.Models
{
    public class EventCategories
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Event is required")]
        public int EventId { get; set; }
        [ForeignKey("EventId")]
        public Events? Events { get; set; }
        [Required(ErrorMessage = "Category is required")]

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Categories? Categories { get; set; } 
        public DateTime AssignedAt { get; set; } = DateTime.Now;    
    }
}
