using LIN.Access.OpenIA;
using LIN.Access.OpenIA.Models;
using LIN.Cloud.OpenAssistant.Persistence.Data;
using LIN.Cloud.OpenAssistant.Services.Context;
using LIN.Types.Cloud.OpenAssistant.Abstractions;
using LIN.Types.Cloud.OpenAssistant.Models;

namespace LIN.Cloud.OpenAssistant.Services.Responders;

public class OpenIAResponder : IIAResponder
{

    /// <summary>
    /// Respondedor de Open IA.
    /// </summary>
    /// <param name="system">Prompt del sistema.</param>
    /// <param name="token">Token de LIN Identity</param>
    /// <param name="currentApp">Aplicación actual.</param>
    /// <param name="context">Contexto de usuario.</param>
    /// <param name="profiles">Data de usuario.</param>
    public async Task<string?> Reply(string system, string token, string currentApp, UserContext context, Profiles profiles)
    {

        // Model builder.
        IAModelBuilder modelBuilder = new()
        {
            Schema = File.ReadAllText("wwwroot/schema.json")
        };

        // Cargar mensajes.
        modelBuilder.Load([Message.FromSystem(system), .. context.Messages]);

        // Responder.
        var response = await modelBuilder.Reply();

        // Obtener la respuesta de la IA.
        var openResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EmmaSchemaResponse>(response.Content);

        foreach (var action in openResponse?.Actions ?? [])
        {
            // Crear la app.
            SILF.Script.App app = new(action);

            // Cargar los métodos al motor.
            app.AddDefaultFunctions(Scripts.Build(token, context.Messages.LastOrDefault()?.Content ?? string.Empty, currentApp, profiles, context));

            // Obtener data.
            var schema = app.RunProfit()?.ToString() ?? string.Empty;

            var applicationResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EmmaSchemaResponse>(schema);

            if (applicationResponse != null && !string.IsNullOrWhiteSpace(applicationResponse.UserText) && openResponse is not null)
            {
                // Agregar a la lista de mensajes.
                openResponse.UserText = applicationResponse.UserText;
                openResponse.Commands.AddRange(applicationResponse.Commands);
            }
        }

        if (openResponse is not null)
        {
            context.Messages.Add(Message.FromAssistant(Newtonsoft.Json.JsonConvert.SerializeObject(openResponse)));
            openResponse.Actions = [];
        }

        return Newtonsoft.Json.JsonConvert.SerializeObject(openResponse);
    }

}