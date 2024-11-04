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
    /// Esquema
    /// </summary>
    public string Schema { get; set; } = "";


    /// <summary>
    /// Responder.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="prompt">Entrada del usuario.</param>
    /// <param name="appLocal">App local.</param>
    public async Task<(bool isSuccess, EmmaSchemaResponse response)> Reply(string token, string prompt, string appLocal, Profiles profileService)
    {

        // Obtener modelo.
        string header = DynamicMessageManager.GetHeader(ProfileModel);

        // Model builder.
        Access.OpenIA.IAModelBuilder modelBuilder = new()
        {
            Schema = Schema
        };

        // Agregar mensaje.
        Messages.Add(Message.FromUser(prompt));

        // Cargar mensajes.
        modelBuilder.Load([Message.FromSystem(header), .. Messages.TakeLast(10)]);

        // Responder.
        var response = await modelBuilder.Reply();

        // Parsear.
        var x = Newtonsoft.Json.JsonConvert.DeserializeObject<EmmaSchemaResponse>(response.Content);

        foreach (var e in x.Actions)
        {
            // Aplicación SILF.
            var silfApp = new SILF.Script.App(e);

            // Agregar métodos.
            silfApp.AddDefaultFunctions(Scripts.Build(token, ProfileModel.Id, prompt, appLocal, profileService, this));

            // Obtener data.
            var result = silfApp.RunProfit()?.ToString() ?? "";

            var x2 = Newtonsoft.Json.JsonConvert.DeserializeObject<EmmaSchemaResponse>(result);

            if (!string.IsNullOrWhiteSpace(x2.UserText))
            {
                // Agregar a la lista de mensajes.
                x.UserText = x2.UserText;
                x.Commands.AddRange(x2.Commands);
            }

            Messages.Add(Message.FromAssistant(Newtonsoft.Json.JsonConvert.SerializeObject(x)));

        }

        x.Actions = [];
        return (true, x);
    }

}