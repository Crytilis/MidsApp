using System;

namespace MidsApp.Models.BuildFile
{
    /// <summary>
    /// Contains metadata information about the application and its associated database,
    /// including version details.
    /// </summary>
    public class MetaData
    {
        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        public string App { get; set; }

        /// <summary>
        /// Gets or sets the version of the application.
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets the name of the database used by the application.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the version of the database.
        /// </summary>
        public Version DatabaseVersion { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaData"/> class with specified application and database information.
        /// </summary>
        /// <param name="app">The name of the application.</param>
        /// <param name="version">The version of the application.</param>
        /// <param name="database">The name of the database.</param>
        /// <param name="databaseVersion">The version of the database.</param>
        public MetaData(string app, Version version, string database, Version databaseVersion)
        {
            App = app;
            Version = version;
            Database = database;
            DatabaseVersion = databaseVersion;
        }
    }
}
