using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class JobOffer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public decimal Salary { get; set; } 

        [Required]
        public string ContractType { get; set; } = string.Empty;

        [Required]
        public DateTime DatePosted { get; set; } = DateTime.UtcNow;

        // Relación de uno a muchos con JobApplication
        public ICollection<JobApplication>? JobApplications { get; set; } = new List<JobApplication>();
    }
}
