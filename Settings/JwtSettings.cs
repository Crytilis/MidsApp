namespace MidsApp.Settings
{
    /// <summary>
    /// Interface representing JWT settings.
    /// </summary>
    public interface IJwtSettings
    {
        /// <summary>
        /// Gets or sets the access key used for JWT token generation.
        /// </summary>
        string AccessKey { get; set; }

        /// <summary>
        /// Gets or sets the refresh key used for JWT token generation.
        /// </summary>
        string RefreshKey { get; set; }

        /// <summary>
        /// Gets or sets the audience for JWT tokens.
        /// </summary>
        string Audience { get; set; }

        /// <summary>
        /// Gets or sets the issuer of JWT tokens.
        /// </summary>
        string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the expiration time in minutes for access tokens.
        /// </summary>
        double AccessExpires { get; set; }

        /// <summary>
        /// Gets or sets the expiration time in minutes for refresh tokens.
        /// </summary>
        double RefreshExpires { get; set; }
    }

    /// <summary>
    /// Implementation of JWT settings.
    /// </summary>
    public class JwtSettings : IJwtSettings
    {
        /// <inheritdoc/>
        public string AccessKey { get; set; }

        /// <inheritdoc/>
        public string RefreshKey { get; set; }

        /// <inheritdoc/>
        public string Audience { get; set; }

        /// <inheritdoc/>
        public string Issuer { get; set; }

        /// <inheritdoc/>
        public double AccessExpires { get; set; }

        /// <inheritdoc/>
        public double RefreshExpires { get; set; }
    }

}
