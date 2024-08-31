using Database;
using Domain;
using MassTransit;
using Messages;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Handler;

public class GreetingCreatedConsumer : IConsumer<GreetingCreated>
{
    private readonly ILogger<GreetingCreatedConsumer> _logger;
    private readonly IDatabase _database;
    private readonly GreetingContext _greetingContext;

    public GreetingCreatedConsumer(ILogger<GreetingCreatedConsumer> logger, IConnectionMultiplexer muxer, GreetingContext greetingContext)
    {
        _logger = logger;
        _database = muxer.GetDatabase();
        _greetingContext = greetingContext;
    }

    public async Task Consume(ConsumeContext<GreetingCreated> context)
    {
        var message = context.Message;
        var json = await _database.StringGetAsync(message.GreetingId.ToString());

        if (json.IsNullOrEmpty)
        {
            _logger.LogError("Greeting {GreetingId} could not be found in cache.", message.GreetingId);
            await context.Publish(new GreetingNotFound(message.GreetingId));
            return;
        }

        var greeting = JsonSerializer.Deserialize<Greeting>(json);

        if (greeting == null)
        {
            _logger.LogError("Greeting {GreetingId} could not be deserialized.", message.GreetingId);
            await context.Publish(new GreetingMalformed(message.GreetingId, json));
            return;
        }

        await _greetingContext.Greetings.AddAsync(greeting);
        await _greetingContext.SaveChangesAsync();

        _logger.LogInformation("{GreetingBody} {GreetingSubject}!", greeting.Message, greeting.Subject);
    }
}
