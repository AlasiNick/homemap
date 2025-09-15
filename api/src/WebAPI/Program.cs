using Homemap.ApplicationCore;
using Homemap.ApplicationCore.Interfaces.Seeders;
using Homemap.ApplicationCore.Models.Auth;
using Homemap.Infrastructure.Data;
using Homemap.Infrastructure.Data.Contexts;
using Homemap.Infrastructure.Data.Seeds;
using Homemap.Infrastructure.Messaging;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

const string devCorsPolicy = "devNuxtCorsPolicy";

builder.Services
    .AddDatabase(builder.Configuration)
    .AddRepositories()
    .AddMessagingServices(builder.Configuration)
    .AddMappers()
    .AddValidators()
    .Configure<GoogleAuthSettings>(
    builder.Configuration.GetSection("Authentication:Google"))
    .AddApplicationServices()
    .AddAuthenticationServices(builder.Configuration)
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddCors(opts => opts.AddPolicy(devCorsPolicy, policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "http://localhost:3080")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    }))
    .AddControllers()
    .AddJsonOptions(opts =>
    {
        // allow `$type` to be anywhere in request body
        opts.JsonSerializerOptions.AllowOutOfOrderMetadataProperties = true;
    });

var app = builder.Build();
app.UseHttpsRedirection();
// app.UseMiddleware<TokenExtractionMiddleware>();

if (app.Environment.IsDevelopment())
{
    // add swagger
    app.UseSwagger().UseSwaggerUI(opts =>
    {
        opts.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        opts.RoutePrefix = string.Empty;
    });

    // seed database
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    new List<ISeeder>
    {
        new ProjectSeeder(context),
        new ReceiverSeeder(context),
        new DeviceSeeder(context),
        new UserSeeder(context, new PasswordHasher<User>())
    }.ForEach(async seeder => await seeder.SeedAsync());

    // enable cors
    app.UseCors(devCorsPolicy);
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
