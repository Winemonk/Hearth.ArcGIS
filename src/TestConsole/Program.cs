using Microsoft.Extensions.DependencyInjection;
using Serilog;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TestConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServiceCollection services = new ServiceCollection();
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/app-{Date}.log")
                .CreateLogger();
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            Container container = new Container(rules => rules.With(FactoryMethod.ConstructorWithResolvableArgumentsIncludingNonPublic));
            container.Populate(services);

            ILogger<Program> logger = container.Resolve<ILogger<Program>>();
            logger.LogInformation("asdasd");
        }
    }
}
