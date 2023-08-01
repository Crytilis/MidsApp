namespace MidsApp.Models
{
    /// <summary>
    /// Snowflake Id 
    /// </summary>
    public class Snowflake
    {
        /// <summary>
        /// Id
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="id"></param>
        public Snowflake(string id)
        {
            Id = id;
        }
    }
}
