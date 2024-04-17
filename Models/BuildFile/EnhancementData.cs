namespace MidsApp.Models.BuildFile
{
    /// <summary>
    /// Represents the data for an enhancement applied to a power slot in a character build.
    /// </summary>
    public class EnhancementData
    {
        /// <summary>
        /// Gets or sets the unique identifier (UID) for the enhancement.
        /// </summary>
        public string Uid { get; set; } = "";

        /// <summary>
        /// Gets or sets the grade of the enhancement (e.g., "None", "Common", "Rare"). Default is "None".
        /// </summary>
        public string Grade { get; set; } = "None";

        /// <summary>
        /// Gets or sets the invention level of the enhancement, applicable to Invention Origin enhancements. Default is 1.
        /// </summary>
        public int IoLevel { get; set; } = 1;

        /// <summary>
        /// Gets or sets the relative level of the enhancement compared to the character's level (e.g., "Even", "+1", "-1"). Default is "Even".
        /// </summary>
        public string RelativeLevel { get; set; } = "Even";

        /// <summary>
        /// Gets or sets a value indicating whether the enhancement has been obtained by the character. Default is false.
        /// </summary>
        public bool Obtained { get; set; } = false;
    }
}
