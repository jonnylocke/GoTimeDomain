using Microsoft.Extensions.Configuration;

namespace GoTimeDomain.Host
{
    internal class Settings
    {
        public string EsConnection { get; private set; }
        public string StreamName { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        public Settings(IConfiguration config)
        {
            EsConnection = config["EsConnection"];
            StreamName = config["StreamName"];
            Username = config["Username"];
            Password = config["Password"];
        }
    }
}