using Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MessagePortal
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var builder = Host.CreateDefaultBuilder();
            builder.UseConsoleLifetime();

            builder.UseNServiceBus(ctx =>
            {
                var endpointConfiguration = new EndpointConfiguration("SDC3_Portal");
                var transportDefinition = new SqlServerTransport(ctx.Configuration.GetConnectionString("QueueDatabase"));
                transportDefinition.TransportTransactionMode = TransportTransactionMode.SendsAtomicWithReceive;
                var transport = endpointConfiguration.UseTransport<SqlServerTransport>(transportDefinition);
                transport.RouteToEndpoint(typeof(StartProcessChangesSaga).Assembly, "Messages", "SDC3_Processor");
                endpointConfiguration.UseSerialization<XmlSerializer>();
                endpointConfiguration.DefineCriticalErrorAction(OnCriticalError);
                endpointConfiguration.EnableInstallers();
                return endpointConfiguration;
            });

            builder.ConfigureServices((ctx, services) =>
            {
                services.AddSingleton<StartSagaService>();
                services.AddSingleton<Portal>();
            });

            var host = builder.Build();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var mainform = services.GetRequiredService<Portal>();
                Application.Run(mainform);
            }
            host.Run();
        }

        static async Task OnCriticalError(ICriticalErrorContext context, CancellationToken cancellationToken)
        {
            var fatalMessage = $"The following critical error was encountered:{Environment.NewLine}{context.Error}{Environment.NewLine}Process is shutting down. StackTrace: {Environment.NewLine}{context.Exception.StackTrace}";

            try
            {
                await context.Stop(cancellationToken);
            }
            finally
            {
                Environment.FailFast(fatalMessage, context.Exception);
            }
        }
    }
}