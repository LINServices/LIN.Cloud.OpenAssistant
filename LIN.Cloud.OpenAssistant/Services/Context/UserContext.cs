using LIN.Cloud.OpenAssistant.Persistence.Data;
using LIN.Types.Cloud.OpenAssistant.Abstractions;
using LIN.Types.Cloud.OpenAssistant.Models;
using System.Text.Json;

namespace LIN.Cloud.OpenAssistant.Services.Context;

public class UserContext(ProfileModel profile)
{

    /// <summary>
    /// Perfil del usuario.
    /// </summary>
    public ProfileModel ProfileModel { get; set; } = profile;

    /// <summary>
    /// Lista de mensajes.
    /// </summary>
    public List<Message> Messages { get; set; } = [];

    /// <summary>
    /// Modelo de IA.
    /// </summary>
    public IAModel? Model { get; private set; } = null;


    /// <summary>
    /// Responder.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="prompt">Entrada del usuario.</param>
    /// <param name="contextApp">Aplicación actual del usuario.</param>
    /// <param name="profileService">Servicio de datos de usuario.</param>
    public async Task<EmmaSchemaResponse?> Reply(string token, string prompt, string contextApp, Profiles profileService)
    {
        // Iniciar.
        Start();

        // Obtener texto de comportamiento personalizado.
        string systemMessage = DynamicMessageManager.GetHeader(ProfileModel);

        // Respondedor.
        Responders.IIAResponder? responder = GetResponder();

        // Agregar prompt del usuario.
        Messages.Add(Message.FromUser(prompt));

        // Si no hay manejador.
        if (responder is null)
            return null;

        // Responder.
        EmmaSchemaResponse? reply = JsonSerializer.Deserialize<EmmaSchemaResponse>(await responder.Reply(systemMessage, token, contextApp, this, profileService));

        // Limpiar mensajes
        if (Messages.Count > 10)
            Messages = Messages.Skip(2).ToList();

        return reply;
    }



    public Responders.IIAResponder? GetResponder()
    {
        switch (Model)
        {
            case IAModel.OpenIA:
                return new Responders.OpenIAResponder();
            case IAModel.Gemini:
                return new Responders.GeminiResponder();
            default:
                break;
        }
        return null;
    }

    private void Start()
    {
        if (Model is not null)
            return;
        try
        {
            Model = (IAModel)ProfileModel.Model;
            return;
        }
        catch { }
        Model = IAModel.OpenIA;
    }

}