using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MidsApp.Models
{
    /// <summary>
    /// Represents a build record stored in the MongoDB database.
    /// This includes both the metadata for a character build and the dynamic fields managed by the server.
    /// </summary>
    [Collection("Builds")]
    public class BuildRecord
    {
        /// <summary>
        /// Gets or sets the unique identifier for the build record, represented as an ObjectId.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the character name for the build.
        /// </summary>
        [BsonElement("name")]
        public string? Name { get; init; }

        /// <summary>
        /// Gets or sets the archetype for the build.
        /// </summary>
        [BsonElement("archetype")]
        public required string Archetype { get; init; }

        /// <summary>
        /// Gets or sets a comment/description for the build.
        /// </summary>
        [BsonElement("description")]
        public string? Description { get; init; }

        /// <summary>
        /// Gets or sets the primary powerset chosen for the build.
        /// </summary>
        [BsonElement("primary")]
        public required string Primary { get; init; }

        /// <summary>
        /// Gets or sets the secondary powerset chosen for the build.
        /// </summary>
        [BsonElement("secondary")]
        public required string Secondary { get; init; }

        /// <summary>
        /// Gets or sets the base64 encoded serialized data representing the build details.
        /// </summary>
        [BsonElement("buildData")]
        public required string BuildData { get; init; }

        /// <summary>
        /// Gets or sets the base64 encoded image data associated with the build.
        /// </summary>
        [BsonElement("imageData")]
        public required string ImageData { get; init; }

        /// <summary>
        /// Gets or sets the shortcode generated for the build, used for easier sharing and retrieval.
        /// </summary>
        [BsonElement("shortcode")]
        public string? Code { get; set; }

        /// <summary>
        /// Gets or sets the expiration date of the build. After this date, the build may be automatically deleted from the database.
        /// </summary>
        [BsonElement("expiresAt")]
        public DateTime ExpiresAt { get; init; }
    }
}
