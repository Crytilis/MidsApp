using System.Collections.Generic;

namespace MidsApp.Models
{
    [Collection("Users")]
    public class DiscordUser
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        public string? Avatar { get; set; }
        public bool Mfa { get; set; }
        public bool Verified { get; set; }
        public string? Email { get; set; }
        public List<DiscordServer> Servers { get; set; }

        public struct DiscordServer
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
            public bool Owner { get; set; }
            public string? Permissions { get; set; }
        }
    }
}
