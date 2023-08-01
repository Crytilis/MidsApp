using MongoDB.Bson.Serialization.Attributes;

namespace MidsApp.Models
{
    /// <summary>
    /// Short Url Record
    /// </summary>
    [Collection("Builds")]
    public class BuildRecord
    {
        /// <summary>
        /// Record Id
        /// </summary>
        [BsonId]
        public string Id { get; init; }
        /// <summary>
        /// Build Data (Base 64 Encoded)
        /// </summary>
        public string BuildData { get; init; }

        /// <summary>
        /// Image Data (Base 64 Encoded)
        /// </summary>
        public string ImageData { get; init; }

        /// <summary>
        /// Html Data (Base64 Url Encoded)
        /// </summary>
        public string PageData { get; init; }

        /// <summary>
        /// ShortUrl Code
        /// </summary>
        public string Code { get; init; }
    }
}
