namespace MidsApp.Settings
{
    /// <summary>
    /// Discord client data
    /// </summary>
    public interface IDiscordSettings
    {
        /// <summary>
        /// The client id of the Discord application
        /// </summary>
        string? ClientId { get; set; }
        /// <summary>
        /// The client secret of the Discord application
        /// </summary>
        string? ClientSecret { get; set; }
    }

    /// <inheritdoc />
    public class DiscordSettings : IDiscordSettings
    {
        /// <inheritdoc />
        public string? ClientId { get; set; }

        /// <inheritdoc />
        public string? ClientSecret { get; set; }
    }
}
