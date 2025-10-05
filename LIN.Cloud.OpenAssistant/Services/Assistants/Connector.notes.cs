using Global.Http.Services;
using System.Text.Json.Nodes;

namespace LIN.Cloud.OpenAssistant.Services.Assistants;

public partial class Connector
{

    /// <summary>
    /// Buscar en las notas.
    /// </summary>
    /// <param name="token">Token de acceso LIN Identity.</param>
    public static async Task<JsonObject> FindNotes(string token, JsonObject @params)
    {
        Client client = new("https://api.linplatform.com/notes/Connector/");
        client.AddHeader("token", token);

        // Retornar todos los contactos.
        var data = await client.Get();
        return JsonNode.Parse(data)!.AsObject();
    }

}