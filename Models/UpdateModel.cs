using System.ComponentModel.DataAnnotations;

namespace MidsApp.Models
{
    /// <summary>
    /// Update Model
    /// </summary>
    public class UpdateModel
    {
        /// <summary>
        /// Build Code
        /// </summary>
        [Required]
        public string Code { get; set; }
        /// <summary>
        /// Html Page Data (Base64 Encoded)
        /// </summary>
        [Required]
        public string PageData { get; set; }
    }
}
