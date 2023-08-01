namespace MidsApp.Settings
{
    public interface IJwtSettings
    {
        string AccessKey { get; set; }
        string RefreshKey { get; set; }
        string Audience { get; set; }
        string Issuer { get; set; }
        double AccessExpires { get; set; }
        double RefreshExpires { get; set; }
    }

    /// <summary>
    /// JWT Data
    /// </summary>
    public class JwtSettings : IJwtSettings
    {
        public string AccessKey { get; set; }
        public string RefreshKey { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public double AccessExpires { get; set; }
        public double RefreshExpires { get; set; }
    }
}
