using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Database;

internal class GreetingContextFactory : IDesignTimeDbContextFactory<GreetingContext>
{
    public GreetingContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GreetingContext>();
        optionsBuilder.UseNpgsql(args[0]);
        return new GreetingContext(optionsBuilder.Options);
    }

}
