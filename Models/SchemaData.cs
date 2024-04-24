namespace MidsApp.Models
{
    /// <summary>
    /// Represents a schema-specific data container used to encapsulate build data.
    /// </summary>
    public class SchemaData
    {
        /// <summary>
        /// Gets or sets the serialized build data.
        /// </summary>
        /// <value>
        /// The serialized data of a build, which may include various attributes of the build such as configuration, properties, and other relevant information.
        /// This property can be null if no data is available to encapsulate.
        /// </value>
        public string? Data { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaData"/> class.
        /// </summary>
        /// <param name="importData">The initial serialized data to encapsulate within this instance.</param>
        public SchemaData(string? importData)
        {
            Data = importData;
        }
    }

}
