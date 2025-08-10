using LIN.Cloud.OpenAssistant.Persistence.Data;
using LIN.Cloud.OpenAssistant.Services.Assistants;
using LIN.OpenAI.Connector.Models;
using LIN.Types.Cloud.OpenAssistant.Models;

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
    public List<MessageModel> Messages { get; set; } = [];

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

        // Agregar prompt del usuario.
        Messages.Add(new MessageModel()
        {
            role = "user",
            content = prompt,
        });

        var client = new LIN.OpenAI.Connector.Client()
        {
            Messages = [new MessageModel() {
                role = "system",
                content = systemMessage
            }, ..Messages],
            ToolFunctions = OpenAIConnector.Toolsl,
            Tools = OpenAIConnector.Tools,
        };

        var response = await client.SendRequest();

        Messages.AddRange(response.GeneratedMessages);

        Messages.Add(new MessageModel()
        {
            role = "assistant",
            content = response.LastMessage
        });

        return new()
        {
            UserText = response.LastMessage,
        };
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