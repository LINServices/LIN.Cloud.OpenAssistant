using LIN.Cloud.OpenAssistant.Persistence.Context;
using LIN.Cloud.OpenAssistant.Persistence.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LIN.Cloud.OpenAssistant.Persistence.Extensions;

public static class DataExtensions
{
    /// <summary>
    /// Agregar servicios de persistence.
    /// </summary>
    /// <param name="services">Services.</param>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfigurationManager configuration)
    {

        services.AddScoped<Profiles, Profiles>();
        services.AddScoped<Services, Services>();

        services.AddDbContextPool<DataContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("cloud"));
        });

        return services;
    }



    /// <summary>
    /// Habilitar el servicio de base de datos.
    /// </summary>
    public static IApplicationBuilder UseDataBase(this IApplicationBuilder app)
    {
        try
        {
            var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetService<DataContext>();
            context?.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        return app;
    }


}