using Microsoft.Extensions.Configuration;
using System;

namespace MidsApp.Services
{
    /// <summary>
    /// Provides utility methods for building URLs using configured base URL and protocol.
    /// </summary>
    public class UrlBuilder : IUrlBuilder
    {
        private readonly string _baseUrl;
        private readonly string _customProtocol;

        /// <summary>
        /// Initializes a new instance of the UrlBuilder class.
        /// </summary>
        /// <param name="configuration">Configuration containing the base URL and custom protocol.</param>
        /// <exception cref="InvalidOperationException">Thrown if the base URL or custom protocol is not configured.</exception>
        public UrlBuilder(IConfiguration configuration)
        {
            _baseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl") ?? throw new InvalidOperationException("Base URL must be configured.");
            _customProtocol = configuration.GetValue<string>("ApiSettings:AppProtocol") ?? throw new InvalidOperationException("Protocol must be configured.");
        }

        /// <summary>
        /// Builds a URL for downloading a build based on the provided shortcode.
        /// </summary>
        /// <param name="shortcode">The shortcode identifying the build.</param>
        /// <returns>A string representing the full URL to download the build.</returns>
        public string BuildDownloadUrl(string shortcode)
        {
            var uriBuilder = new UriBuilder(_baseUrl)
            {
                Path = $"/build/download/{shortcode}"
            };
            return uriBuilder.ToString();
        }

        /// <summary>
        /// Builds a URL for accessing the image of a build based on the provided shortcode.
        /// </summary>
        /// <param name="shortcode">The shortcode identifying the build's image.</param>
        /// <returns>A string representing the full URL to the image.</returns>
        public string BuildImageUrl(string shortcode)
        {
            var uriBuilder = new UriBuilder(_baseUrl)
            {
                Path = $"/build/image/{shortcode}.png"
            };
            return uriBuilder.ToString();
        }

        /// <summary>
        /// Builds a URL using a custom protocol for accessing the build schema based on the provided shortcode.
        /// </summary>
        /// <param name="shortcode">The shortcode identifying the build.</param>
        /// <returns>A string representing the custom URL to access the build schema.</returns>
        public string BuildSchemaUrl(string shortcode)
        {
            return $"{_customProtocol}://{shortcode}";
        }
    }
}
