using LIN.Types.Cloud.OpenAssistant.Models;

namespace LIN.Cloud.OpenAssistant.Services;

public class ContextManager
{

    /// <summary>
    /// Lista de contextos.
    /// </summary>
    private Dictionary<int, Context> _context = [];


    /// <summary>
    /// Obtener o crear un contexto.
    /// </summary>
    /// <param name="profile">Modelo del perfil.</param>
    public Context GetOrCreate(ProfileModel profile)
    {

        _context.TryGetValue(profile.Id, out var context);

        if (context == null)
        {
            context = new Context(profile);
            _context.Add(profile.Id, context);
        }

        context.ProfileModel = profile;
        return context;
    }


}