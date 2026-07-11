using System.ComponentModel.DataAnnotations;

namespace VoteHub.Models
{
    /// <summary>
    /// Represents a user in the VoteHub system
    /// </summary>
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "User"; // Admin or User

        public string? ProfilePicture { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime LastLogin { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<Election> Elections { get; set; } = new List<Election>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}