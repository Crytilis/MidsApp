namespace MidsApp.Services;

/// <summary>
/// Defines a set of methods for constructing URLs within the application. 
/// This interface abstracts the logic for generating URLs that access various resources 
/// like build downloads, images, and schemas based on a shortcode.
/// </summary>
public interface IUrlBuilder
{
    /// <summary>
    /// Builds a URL for downloading a build based on the provided shortcode.
    /// </summary>
    /// <param name="shortcode">The shortcode identifying the build.</param>
    /// <returns>A string representing the full URL to download the build.</returns>
    string BuildDownloadUrl(string shortcode);

    /// <summary>
    /// Builds a URL for accessing the image of a build based on the provided shortcode.
    /// </summary>
    /// <param name="shortcode">The shortcode identifying the build's image.</param>
    /// <returns>A string representing the full URL to the image.</returns>
    string BuildImageUrl(string shortcode);

    /// <summary>
    /// Builds a URL using a custom protocol for accessing the build schema based on the provided shortcode.
    /// </summary>
    /// <param name="shortcode">The shortcode identifying the build.</param>
    /// <returns>A string representing the custom URL to access the build schema.</returns>
    string BuildSchemaUrl(string shortcode);
}