namespace CustomerInsights.ServiceDefaults.Models
{
    public sealed class RabbitMqConnectionOptions
    {
        public string HostName { get; set; } = "messaging";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
    }

}
