using B2BPlatform.API.Controllers;
using B2BPlatform.Infrastructure.Contexts;
using B2BPlatform.Infrastructure.IoC;
using Microsoft.EntityFrameworkCore;

namespace B2BPlatform.API;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        builder.Services.AddSwaggerGen(options =>
        {
            options.OrderActionsBy(d => $"{d.ActionDescriptor.RouteValues["controller"]}/{d.RelativePath}");
        });

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        builder.Services.Register(builder.Configuration);

        var app = builder.Build();

        // Auto-migrate database on startup
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
        }

        app.UseCors();
        app.UseAuthorization();
        app.UseExceptionHandler();
        app.MapControllers();
        app.Run();
    }
}
