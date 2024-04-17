namespace MidsApp.Models
{
    /// <summary>
    /// Represents the data necessary to create a file for a character build, including metadata and content.
    /// </summary>
    public class FileData
    {
        /// <summary>
        /// Gets or sets the name of the character. If null, the file name is generated without a character name.
        /// </summary>
        public string? CharacterName { get; set; }

        /// <summary>
        /// Gets or sets the archetype of the character. This is a required field.
        /// </summary>
        public required string Archetype { get; set; }

        /// <summary>
        /// Gets or sets the primary power set of the character. This is a required field.
        /// </summary>
        public required string Primary { get; set; }

        /// <summary>
        /// Gets or sets the secondary power set of the character. This is a required field.
        /// </summary>
        public required string Secondary { get; set; }

        /// <summary>
        /// Generates a file name based on the character's name, archetype, and power sets.
        /// If the character name is not provided, the file name only includes the archetype and power sets.
        /// </summary>
        /// <value>
        /// The file name for the build data.
        /// </value>
        public string FileName => CharacterName is null ? 
            $"{Archetype} ({Primary} - {Secondary}).mbd" : 
            $"{CharacterName} [{Archetype}] ({Primary} - {Secondary}).mbd";

        /// <summary>
        /// Gets or sets the byte array that represents the serialized form of the character build data. This is a required field.
        /// </summary>
        public required byte[] DataBytes { get; set; }
    }
}
