using MassTransit;
using Messages;
using Microsoft.Extensions.Logging;

namespace Handler;

public class HelloMessageConsumer : IConsumer<HelloMessage>
{
    private readonly ILogger<HelloMessageConsumer> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public HelloMessageConsumer(ILogger<HelloMessageConsumer> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task Consume(ConsumeContext<HelloMessage> context)
    {
        using var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri("http://emailapi:8080");

        var response = await client.PostAsync("/email", null, context.CancellationToken);

        response.EnsureSuccessStatusCode();
        
        _logger.LogInformation("Hello {message}!", context.Message.Hello);
    }
}
