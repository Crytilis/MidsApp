using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ThirdParty.Json.LitJson;

namespace MidsApp.Models.BuildFile
{
    /// <summary>
    /// Represents the character build data for serialization and deserialization 
    /// in preparation for creating a .mbd file for the user.
    /// </summary>
    public class BuildFile
    {
        /// <summary>
        /// Meta information about the build, including version details.
        /// </summary>
        [JsonProperty]
        public MetaData? BuiltWith { get; set; }

        /// <summary>
        /// The level of the character in the build.
        /// </summary>
        [JsonProperty]
        public string Level { get; set; } = string.Empty;

        /// <summary>
        /// The class/archetype of the character in the build.
        /// </summary>
        [Required]
        [JsonProperty]
        public string Class { get; set; } = string.Empty;

        /// <summary>
        /// The origin of the character in the build.
        /// </summary>
        [Required]
        [JsonProperty]
        public string Origin { get; set; } = string.Empty;

        /// <summary>
        /// The alignment of the character in the build (e.g., Hero, Villain).
        /// </summary>
        [Required]
        [JsonProperty]
        public string Alignment { get; set; } = string.Empty;

        /// <summary>
        /// The name of the character in the build.
        /// </summary>
        [JsonProperty]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Additional comments or notes associated with the build.
        /// </summary>
        [JsonProperty]
        public string? Comment { get; set; }

        /// <summary>
        /// A list of powersets included in the build.
        /// </summary>
        [Required]
        [JsonProperty]
        public List<string> PowerSets { get; set; } = [];

        /// <summary>
        /// Indicates the last power selected in the build.
        /// </summary>
        [Required]
        [JsonProperty]
        public int LastPower { get; set; }

        /// <summary>
        /// A detailed list of powers, including configurations and selections, in the build.
        /// </summary>
        [Required]
        [JsonProperty]
        public List<PowerData?> PowerEntries { get; set; } = [];
    }
}
