using LIN.Access.OpenIA.Models;
using LIN.Cloud.OpenAssistant.Persistence.Data;
using LIN.Types.Cloud.OpenAssistant.Models;

namespace LIN.Cloud.OpenAssistant.Services;

public class Context(ProfileModel profile)
{

    /// <summary>
    /// Perfil.
    /// </summary>
    public ProfileModel ProfileModel { get; set; } = profile;


    /// <summary>
    /// Lista de mensajes.
    /// </summary>
    public List<Message> Messages { get; set; } = [];


    /// <summary>
    /// Responder.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="prompt">Entrada del usuario.</param>
    /// <param name="appLocal">App local.</param>
    public async Task<string> Reply(string token, string prompt, string appLocal, Profiles profileService)
    {

        // Obtener modelo.
        string header = DynamicMessageManager.GetHeader(ProfileModel);

        // Model builder.
        Access.OpenIA.IAModelBuilder modelBuilder = new();

        // Agregar mensaje.
        Messages.Add(Message.FromUser(prompt));

        // Cargar mensajes.
        modelBuilder.Load([Message.FromSystem(header), .. Messages.TakeLast(10)]);

        // Responder.
        var response = await modelBuilder.Reply();

        // Validaciones.
        if (response.Content.StartsWith('"') && response.Content.EndsWith('"') && response.Content.Length > 2 && response.Content[1] == '#')
        {
            response.Content = response.Content.Remove(0, 1);
            response.Content = response.Content[..^1];
        }

        // Si es un método servidor.
        if (response.Content.StartsWith("#require", StringComparison.CurrentCultureIgnoreCase) || response.Content.StartsWith("#force", StringComparison.CurrentCultureIgnoreCase))
        {
            // Aplicación SILF.
            var silfApp = new SILF.Script.App(response.Content.Remove(0, 1));

            // Agregar métodos.
            silfApp.AddDefaultFunctions(Scripts.Build(token, ProfileModel.Id, prompt, appLocal, profileService));

            // Obtener data.
            var result = silfApp.RunProfit()?.ToString() ?? "";

            // Agregar a la lista de mensajes.
            Messages.Add(Message.FromAssistant(result));

            return result;
        }

        // Agregar mensaje del asistente.
        Messages.Add(Message.FromAssistant(response.Content));
        return response.Content;
    }



}