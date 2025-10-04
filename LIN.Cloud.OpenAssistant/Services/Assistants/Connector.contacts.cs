using Global.Http.Services;
using System.Text.Json.Nodes;

namespace LIN.Cloud.OpenAssistant.Services.Assistants;

public partial class Connector
{

    /// <summary>
    /// Buscar contactos.
    /// </summary>
    /// <param name="token">Token de acceso LIN Identity.</param>
    public static async Task<JsonObject> FindContacts(string token, JsonObject @params)
    {
        Client client = new("https://api.linplatform.com/contacts/Assistant/all/");
        client.AddHeader("token", token);

        // Retornar todos los contactos.
        var data = await client.Get();
        return JsonNode.Parse(data)!.AsObject();
    }

}