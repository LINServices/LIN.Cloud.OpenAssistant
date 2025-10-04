using LIN.OpenAI.Connector.Models;
using System.Text.Json.Nodes;

namespace LIN.Cloud.OpenAssistant.Services.Assistants;

public class OpenAIConnector
{

    public static List<ToolDefinition> Tools = [];

    static OpenAIConnector()
    {
        Tools.Add(ToolDefinition.Create(
            name: "weather",
            description: "Obtener el clima de una ciudad",
            parameters: new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject
                {
                    ["city"] = new JsonObject { ["type"] = "string", ["description"] = "Nombre de la ciudad" }
                },
                ["required"] = new JsonArray("city")
            },
            callback: async (token, args) =>
            {
                string city = args["city"]!.GetValue<string>();
                await Task.Delay(10); // simulación I/O
                return new JsonObject { ["city"] = city, ["value"] = "17 grados" };
            }
        ));

        Tools.Add(ToolDefinition.Create(
            name: "get_contacts",
            description: "Obtener la información de los contactos",
            parameters: new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject
                {
                }
            },
            callback: Connector.FindContacts
        ));
    }
}