using MassTransit;
using Messages;
using Microsoft.Extensions.Logging;

namespace Handler;

public class HelloMessageConsumer : IConsumer<HelloMessage>
{
    private readonly ILogger<HelloMessageConsumer> _logger;

    public HelloMessageConsumer(ILogger<HelloMessageConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<HelloMessage> context)
    {
        _logger.LogInformation("Hello {message}!", context.Message.Hello);
        return Task.CompletedTask;
    }
}
