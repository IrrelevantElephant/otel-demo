namespace Messages;

public record GreetingCreated(Guid GreetingId);
public record GreetingNotFound(Guid GreetingId);
public record GreetingMalformed(Guid GreetingId, string greetingBody);