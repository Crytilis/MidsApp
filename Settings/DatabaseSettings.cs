namespace MidsApp.Settings
{
    /// <summary>
    /// MongoDB connection data
    /// </summary>
    public interface IDatabaseSettings
    {
        /// <summary>
        /// The MongoDB instance connection string
        /// </summary>
        string? ConnectionString { get; set; }
        /// <summary>
        /// Name of the MongoDB Database
        /// </summary>
        string? DatabaseName { get; set; }
    }

    /// <inheritdoc />
    public class DatabaseSettings : IDatabaseSettings
    {
        /// <inheritdoc />
        public string? ConnectionString { get; set; }

        /// <inheritdoc />
        public string? DatabaseName { get; set; }
    }
}
