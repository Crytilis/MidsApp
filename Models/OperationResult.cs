namespace MidsApp.Models
{
    public class OperationResult
    {
        internal bool Success;

        public string Status => Success ? "Success" : "Failed";
        public string? ErrorMessage { get; set; }
        public string? Id { get; set; }
        public string? Code { get; set; }
        public string? BuildUrl { get; set; }
        public string? ImageUrl { get; set; }
        public string? PageUrl { get; set; }
    }
}
