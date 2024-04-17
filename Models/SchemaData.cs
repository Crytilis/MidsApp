namespace MidsApp.Models
{
    public class SchemaData
    {
        public string? Data { get; set; }

        public SchemaData(string? importData)
        {
            Data = importData;
        }
    }
}
