
using System.ComponentModel.DataAnnotations;

namespace WebApplication6.Models
{
    public class Users
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Username is required")]
        [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        public string Username { get; set; }
        public string Password { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
        public string Fname { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
        public string Lname { get; set; }
        [Required(ErrorMessage = "Role is required")]
        [MaxLength(50, ErrorMessage = "Role cannot exceed 50 characters")]
        public string Role { get; set; }
        [MaxLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        public string? Phone {  get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
