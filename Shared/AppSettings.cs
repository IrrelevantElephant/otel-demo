namespace Shared;

public class AppSettings
{
    public required MassTransitConfig MassTransitConfig { get; set; }
    public required string RedisConnectionString { get; set; }
    public required string DatabaseConnectionString { get; set; }
}
