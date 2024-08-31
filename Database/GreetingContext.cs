using Domain;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public class GreetingContext : DbContext
    {
        public GreetingContext(DbContextOptions<GreetingContext> options) : base(options) { }

        public DbSet<Greeting> Greetings { get; set; }
    }
}
