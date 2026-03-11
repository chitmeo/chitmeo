using ChitMeo.Host.Extensions;

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.ConfigureWebApplication();

    var app = builder.Build();
    await app.ConfigureRequestPipelineAsync();
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex);
    throw;
}