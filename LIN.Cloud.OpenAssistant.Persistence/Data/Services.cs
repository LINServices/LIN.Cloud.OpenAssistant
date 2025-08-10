using LIN.Cloud.OpenAssistant.Persistence.Context;
using LIN.Types.Cloud.OpenAssistant.Models;
using Microsoft.EntityFrameworkCore;

namespace LIN.Cloud.OpenAssistant.Persistence.Data;

public class Services(DataContext context)
{

    /// <summary>
    /// Crear servicio.
    /// </summary>
    /// <param name="service">Service model</param>
    public async Task<CreateResponse> Create(EmmaService service)
    {
        try
        {
            await context.Services.AddAsync(service);
            context.SaveChanges();
            return new(Responses.Success, service.Id);
        }
        catch (Exception)
        {
        }
        return new(Responses.Undefined);
    }

    /// <summary>
    /// Obtener servicios
    /// </summary>
    public async Task<ReadAllResponse<EmmaService>> ReadAll()
    {
        try
        {
            // Obtener los servicios.
            var services = await (from s in context.Services
                                  select new EmmaService
                                  {
                                      Name = s.Name,
                                      Url = s.Url,
                                  }).ToListAsync();

            // Retornar.
            return new(Responses.Success, services);
        }
        catch (Exception)
        {
        }
        return new(Responses.Undefined);
    }

    /// <summary>
    /// Obtener un servicio
    /// </summary>
    public async Task<ReadOneResponse<EmmaService>> Read(string name)
    {
        try
        {
            // Obtener los servicios.
            var service = await (from s in context.Services
                                  where s.Name == name
                                  select s).FirstOrDefaultAsync();

            // Retornar.
            if (service is null)
                return new(Responses.NotRows);

            return new(Responses.Success, service);
        }
        catch (Exception)
        {
        }
        return new(Responses.Undefined);
    }

}