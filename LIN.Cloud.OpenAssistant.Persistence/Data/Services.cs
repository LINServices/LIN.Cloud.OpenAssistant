using LIN.Cloud.OpenAssistant.Persistence.Context;
using LIN.Types.Cloud.OpenAssistant.Models;

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

}