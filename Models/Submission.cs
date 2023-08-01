using System.ComponentModel.DataAnnotations;

namespace MidsApp.Models
{
    /// <summary>
    /// Submission Model
    /// </summary>
    public class Submission
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Data { get; set; }

        [Required]
        public string Image { get; set; }
    }
}
