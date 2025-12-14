using Auth.Api.Extentions;
using Auth.Api.Extentions.MinimalApi;
using Auth.Api.Middlewares;
using Auth.Repository.Context;
using Auth.Repository.Seed;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddDebug();
builder.Logging.AddConsole();


// Add services to the container.
builder.Services.AddCustomConfiguration(builder.Configuration);
//builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddAllMinimalApisFromAssembly();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger"; // swagger UI available at /swagger
    });
}

// 1?? Register minimal API endpoints
app.UseCors("CorsPolicy");
app.UseCors("AllowAngular");

// 2?? Build endpoint routing table BEFORE seeding
app.UseRouting();
app.RegisterMinimalApisEndPoints();
// 3?? Seed API URLs (NOW endpoint list is available)

// 4?? Middlewares
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    using var scope = app.Services.CreateScope();
    var urlList = ApiSeedExtention.DiscoverCurrentEndpoints(app);

    await scope.ServiceProvider.CreateUrlsForProjectAsync(
        projectTitle: "Authentication",
    host: "localhost",
    port: "5025",
    urls: urlList
    );
    });
await app.Services.AddDbContextSeedDataAsync();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();
app.UseMiddleware<AuthorizationMiddleware>();
app.Run();
