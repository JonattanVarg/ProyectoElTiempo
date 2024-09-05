using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class JobApplication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CandidateName { get; set; } = string.Empty;

        [Required]
        public string CandidateEmail { get; set; } = string.Empty;

        [Required]
        public int JobOfferId { get; set; }

        public JobOffer? JobOffer { get; set; }  = new JobOffer();

        [Required]
        public DateTime DateApplied { get; set; } = DateTime.UtcNow;
    }
}
