using System;

namespace MidsApp.Models
{
    /// <summary>
    /// Represents the result of a creation or update operation, providing necessary details about the newly created or updated resource.
    /// </summary>
    public class TransactionResult
    {
        /// <summary>
        /// Gets the unique shortcode associated with the created resource.
        /// </summary>
        /// <value>The shortcode that uniquely identifies the resource.</value>
        public string Shortcode { get; }

        /// <summary>
        /// Gets the URL where the submitted build can be downloaded.
        /// </summary>
        /// <value>The absolute URL to download the build associated with the shortcode.</value>
        public string DownloadUrl { get; }

        /// <summary>
        /// Gets the URL for the image associated with the created build.
        /// </summary>
        /// <value>The absolute URL to the image related to the build for preview or display purposes.</value>
        public string ImageUrl { get; }

        /// <summary>
        /// Gets the custom URL that can be used to access the build schema directly via a custom protocol.
        /// </summary>
        /// <value>The custom URL (mrb://{shortcode}) that can be used by applications configured to handle the mrb protocol
        /// to load and display the build directly from the schema data.</value>
        public string SchemaUrl { get; }

        /// <summary>
        /// Gets the expiration date and time of the build in string format.
        /// </summary>
        /// <value>The date and time when the build expires, formatted as a string using ISO 8601 format with Z indicating UTC.</value>
        public string ExpiresAt { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionResult"/> class.
        /// </summary>
        /// <param name="shortcode">The unique shortcode identifying the created resource.</param>
        /// <param name="downloadUrl">The URL where the build can be downloaded.</param>
        /// <param name="imageUrl">The URL where the image associated with the build can be accessed.</param>
        /// <param name="schemaUrl">The custom URL using a mrb scheme for accessing the build schema.</param>
        /// <param name="expiresAt">The expiration date and time of the build.</param>
        public TransactionResult(string shortcode, string downloadUrl, string imageUrl, string schemaUrl, string expiresAt)
        {
            Shortcode = shortcode;
            DownloadUrl = downloadUrl;
            ImageUrl = imageUrl;
            SchemaUrl = schemaUrl;
            ExpiresAt = expiresAt;
        }
    }
}
