using PetFamily.Api.Extensions;
using PetFamily.Api.Middlewares;
using PetFamily.Application;
using PetFamily.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = LoggerConfigurationFactory.Create(builder.GetSeqConnectionString());

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSerilog();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

app.UseExceptionMiddleware();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "PetFamily API"));

    await app.ApplyMigrations();
}

app.UseAuthorization();

app.MapControllers();

app.Run();