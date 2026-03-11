using ChitMeo.Host.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.ConfigureWebApplication();

WebApplication app = builder.Build();
await app.ConfigureRequestPipelineAsync();


// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.

// var app = builder.Build();

// // Configure the HTTP request pipeline.

// app.UseHttpsRedirection();

// app.Run();