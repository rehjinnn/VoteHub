using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteHub.Models
{
    /// <summary>
    /// Represents a candidate in an election
    /// </summary>
    public class Candidate
    {
        [Key]
        public int CandidateId { get; set; }

        [Required]
        public int ElectionId { get; set; }

        [Required]
        [StringLength(150)]
        public required string Name { get; set; }

        [Required]
        [StringLength(100)]
        public required string Position { get; set; }

        [Required]
        [StringLength(50)]
        public required string Party { get; set; }

        [Required]
        [StringLength(100)]
        public required string Course { get; set; }

        [Required]
        [StringLength(20)]
        public required string YearLevel { get; set; }

        [StringLength(500)]
        public required string Biography { get; set; }

        [StringLength(1000)]
        public required string Platform { get; set; }

        [Required]
        [StringLength(255)]
        public required string Photo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey(nameof(ElectionId))]
        public required Election Election { get; set; }

        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}