namespace MidsApp.Models
{
    public class DataFile
    {
        public string? ImportData { get; set; }

        public DataFile(string? data)
        {
            ImportData = data;
        }
    }
}
