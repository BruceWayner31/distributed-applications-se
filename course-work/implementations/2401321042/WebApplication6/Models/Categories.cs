using System.ComponentModel.DataAnnotations;

namespace WebApplication6.Models
{
    public class Categories
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }
  
        [MaxLength(300, ErrorMessage = "Description cannot exceed 300 characters")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Color is required")]
        [MaxLength(7, ErrorMessage = "Color must be in #RRGGBB format")]
        [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Must be valid hex color e.g. #FF5733")]
        public string ColorHex { get; set; }
        [Required]
        public Boolean isActived { get; set; } = true;
        public DateTime Created { get; set; } = DateTime.Now;
    }
}
