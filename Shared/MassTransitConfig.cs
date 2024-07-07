namespace Shared;

public class MassTransitConfig
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Uri { get; set; }
    public string VirtualHost { get; set; } = "/";
}