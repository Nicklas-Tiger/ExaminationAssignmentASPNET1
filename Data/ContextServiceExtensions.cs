using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Data;

public static class ContextServiceExtensions
{
    public static IServiceCollection AddContexts(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<DataContext>(x => x.UseSqlServer(connectionString));
        return services;
    }
}
