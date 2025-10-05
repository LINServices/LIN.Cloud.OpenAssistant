using LIN.Cloud.OpenAssistant.Services.Context;
using LIN.OpenAI.Connector;
using LIN.Types.Cloud.OpenAssistant.Models;

namespace LIN.Cloud.OpenAssistant.Services;

public class ContextManager
{
    /// <summary>
    /// Lista de contextos.
    /// </summary>
    private readonly Dictionary<int, UserContext> _context = [];

    /// <summary>
    /// Obtener o crear un contexto.
    /// </summary>
    /// <param name="profile">Modelo del perfil.</param>
    public UserContext GetOrCreate(ProfileModel profile, IGptOrchestrator gptOrchestrator)
    {
        // Obtener contexto.
        _context.TryGetValue(profile.Id, out var context);

        if (context is null)
        {
            context = new UserContext(profile, gptOrchestrator);
            _context.Add(profile.Id, context);
        }

        context.ProfileModel = profile;
        return context;
    }

    /// <summary>
    /// Obtener o crear un contexto.
    /// </summary>
    /// <param name="profile">Modelo del perfil.</param>
    public void Delete(int profile)
    {
        // Obtener contexto.
        _context.Remove(profile);
    }

}