using LIN.Cloud.OpenAssistant.Persistence.Data;
using LIN.Cloud.OpenAssistant.Services.Assistants;
using LIN.OpenAI.Connector;
using LIN.OpenAI.Connector.Models;
using LIN.Types.Cloud.OpenAssistant.Models;

namespace LIN.Cloud.OpenAssistant.Services.Context;

public class UserContext(ProfileModel profile, IGptOrchestrator orchestrator)
{

    /// <summary>
    /// Perfil del usuario.
    /// </summary>
    public ProfileModel ProfileModel { get; set; } = profile;

    /// <summary>
    /// Lista de mensajes.
    /// </summary>
    public List<ChatMessage> Messages { get; set; } = [];

    /// <summary>
    /// Responder.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="prompt">Entrada del usuario.</param>
    /// <param name="contextApp">Aplicación actual del usuario.</param>
    /// <param name="profileService">Servicio de datos de usuario.</param>
    public async Task<EmmaSchemaResponse> Reply(string token, string prompt, string contextApp, Profiles profileService, IServiceProvider scope)
    {
        // Obtener texto de comportamiento personalizado.
        string systemMessage = DynamicMessageManager.GetHeader(ProfileModel);

        // Agregar prompt del usuario.
        Messages.Add(ChatMessage.User(prompt));

        // Ejecutar orquestador.
        var result = await orchestrator.RunAsync(token, [ChatMessage.System(systemMessage), .. Messages], [.. OpenAIConnector.Tools, .. Connector.GetBySession(scope)]);

        if (result is null || result.FinalAssistant is null)
            return new();

        // Agregar el mensaje de respuesta.
        Messages.Add(result.FinalAssistant);

        return new()
        {
            UserText = result.FinalAssistant.content ?? string.Empty,
        };
    }
}