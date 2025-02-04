using ChangeProcessor;
using Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using System.Data.SqlClient;

var builder = Host.CreateDefaultBuilder(args);
builder.UseConsoleLifetime();
builder.ConfigureServices((ctx, services) =>
{
    services.AddHostedService<ChangeProcessorService>();
});
builder.UseNServiceBus(ctx =>
{
    var endpointConfiguration = new EndpointConfiguration("SDC3_Processor");
    var transportDefinition = new SqlServerTransport(ctx.Configuration.GetConnectionString("QueueDatabase"));
    transportDefinition.TransportTransactionMode = TransportTransactionMode.SendsAtomicWithReceive;
    var transport = endpointConfiguration.UseTransport<SqlServerTransport>(transportDefinition);
    transport.RouteToEndpoint(typeof(ChangeCommand).Assembly, "Messages", "SDC3_Processor");
    endpointConfiguration.UseSerialization<XmlSerializer>();
    endpointConfiguration.DefineCriticalErrorAction(OnCriticalError);
    endpointConfiguration.EnableInstallers();
    var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
    persistence.SqlDialect<SqlDialect.MsSqlServer>();
    persistence.ConnectionBuilder(() => new SqlConnection(ctx.Configuration.GetConnectionString("QueueDatabase")));
    return endpointConfiguration;
});

var host = builder.Build();
host.Run();

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

