using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteHub.Models
{
    /// <summary>
    /// Represents a vote cast by a user
    /// </summary>
    public class Vote
    {
        [Key]
        public int VoteId { get; set; }

        [Required]
        public int ElectionId { get; set; }

        [Required]
        public int CandidateId { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime DateVoted { get; set; } = DateTime.Now;

        [ForeignKey(nameof(ElectionId))]
        public Election Election { get; set; }

        [ForeignKey(nameof(CandidateId))]
        public Candidate Candidate { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}