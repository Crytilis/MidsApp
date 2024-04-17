namespace MidsApp.Models.BuildFile
{
    /// <summary>
    /// Represents data for a sub-power associated with a main power in a character build,
    /// including its name and whether statistics should be included in calculations.
    /// </summary>
    public class SubPowerData
    {
        /// <summary>
        /// Gets or sets the name of the sub-power. The name is expected to be a unique identifier within the context of the build.
        /// </summary>
        public string PowerName { get; set; } = "";

        /// <summary>
        /// Gets or sets a value indicating whether statistics (stats) should be included
        /// in the calculations for this sub-power. This can affect how the sub-power
        /// contributes to the overall effectiveness of the character build.
        /// </summary>
        public bool StatInclude { get; set; }
    }
}
