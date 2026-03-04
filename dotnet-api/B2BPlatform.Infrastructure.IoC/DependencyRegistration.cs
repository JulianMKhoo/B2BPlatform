using System.Data;
using B2BPlatform.Infrastructure.Contexts;
using B2BPlatform.Infrastructure.Repositories;
using B2BPlatform.Infrastructure.Services;
using B2BPlatform.Service.Authentication.Services;
using B2BPlatform.Service.Dashboard.Services;
using B2BPlatform.Service.Employees.Services;
using B2BPlatform.Services.BusinessUnit.Services;
using B2BPlatform.Services.Chats.Services;
using B2BPlatform.Services.CompanyOwner.Services;
using B2BPlatform.Shared.Interfaces.Repositories;
using B2BPlatform.Shared.Interfaces.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using StackExchange.Redis;

namespace B2BPlatform.Infrastructure.IoC;

public static class DependencyRegistration
{
    public static IServiceCollection Register(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("WRITE"));
        });

        services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(configuration.GetConnectionString("WRITE")));

        // Redis
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("REDIS") ?? "localhost:6379"));

        services
            .AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();

        // Service Registrations
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IBusinessUnitService, BusinessUnitService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<ICompanyOwnerService, CompanyOwnerService>();
        services.AddSingleton<IChatEventPublisher, RedisChatEventPublisher>();
        services.AddSingleton<ICacheService, RedisCacheService>();

        // Repository Registrations
        services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IBusinessUnitRepository, BusinessUnitRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ICompanyOwnerRepository, CompanyOwnerRepository>();

        return services;
    }
}
