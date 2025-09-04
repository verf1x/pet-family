using Infrastructure.Postgres;
using Infrastructure.S3;
using PetFamily.Framework.Middlewares;
using Serilog;
using Web;
using Web.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Log.Logger = LoggerConfigurationFactory.Create(builder.GetSeqConnectionString());

builder.Services.AddSerilog();

builder.Services.AddS3Infrastructure(builder.Configuration);
builder.Services.AddPostgres(builder.Configuration);
builder.Services.AddProgramDependencies(builder.Configuration);

WebApplication app = builder.Build();

app.UseExceptionMiddleware();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

namespace Web
{
    public class Program;
}