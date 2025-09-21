using Infrastructure.Postgres;
using Infrastructure.S3;
using PetFamily.Framework.Middlewares;
using Serilog;
using Web;
using Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = LoggerConfigurationFactory.Create(builder.GetSeqConnectionString());

builder.Services.AddSerilog();

builder.Services.AddS3Infrastructure(builder.Configuration);
builder.Services.AddPostgres(builder.Configuration);
builder.Services.AddProgramDependencies(builder.Configuration);

var app = builder.Build();

app.UseExceptionMiddleware();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;