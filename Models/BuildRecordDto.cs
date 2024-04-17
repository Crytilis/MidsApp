namespace MidsApp.Models
{
    /// <summary>
    /// Represents a data transfer object for a character build record.
    /// This DTO includes the metadata necessary for creating or updating a build
    /// but excludes server-managed fields such as the unique identifier, shortcode, and expiration date.
    /// It is used to transfer data between the client and the server without directly exposing database models.
    /// </summary>
    public class BuildRecordDto
    {
        /// <summary>
        /// Gets or sets the character name for the build. Can be null.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the archetype of the build.
        /// </summary>
        [RequiredForSubmission]
        public string? Archetype { get; set; }

        /// <summary>
        /// Gets or sets a comment/description for the build. Can be null
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the primary powerset chosen for the build.
        /// </summary>
        [RequiredForSubmission]
        public string? Primary { get; set; }

        /// <summary>
        /// Gets or sets the secondary powerset chosen for the build.
        /// </summary>
        [RequiredForSubmission]
        public string? Secondary { get; set; }

        /// <summary>
        /// Gets or sets the base64 encoded serialized data representing the build details.
        /// </summary>
        [RequiredForSubmission]
        [RequiredForUpdate]
        public string? BuildData { get; set; }

        /// <summary>
        /// Gets or sets the base64 encoded image data associated with the build.
        /// </summary>
        [RequiredForSubmission]
        [RequiredForUpdate]
        public string? ImageData { get; set; }
    }
}
