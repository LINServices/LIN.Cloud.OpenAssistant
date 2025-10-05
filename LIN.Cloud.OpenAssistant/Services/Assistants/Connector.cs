using LIN.OpenAI.Connector.Models;
using System.Text.Json.Nodes;

namespace LIN.Cloud.OpenAssistant.Services.Assistants;

public partial class Connector
{
    public IServiceScope Scope { get; set; } = default!;

    public static IEnumerable<ToolDefinition> GetBySession(IServiceProvider scope)
    {
        var connector = new Connector
        {
            Scope = scope.CreateScope()
        };

        yield return ToolDefinition.Create(
            name: "set_city",
            description: "Establecer o actualizar la ciudad donde se encuentra ubicado el usuario.",
            parameters: new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject
                {
                    ["city"] = new JsonObject { ["type"] = "string", ["description"] = "Nombre de la ciudad" }
                },
                ["required"] = new JsonArray("city")
            },
            callback: connector.UpdateCity
        );

        yield return ToolDefinition.Create(
            name: "set_alias",
            description: "Establecer o actualizar el alias del usuario.",
            parameters: new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject
                {
                    ["alias"] = new JsonObject { ["type"] = "string", ["description"] = "Nuevo alias para el usuario" }
                },
                ["required"] = new JsonArray("alias")
            },
            callback: connector.UpdateAlias
        );

    }
}