using LIN.Cloud.OpenAssistant.Persistence.Data;
using System.Text.Json.Nodes;

namespace LIN.Cloud.OpenAssistant.Services.Assistants;

public partial class Connector
{
    public async Task<JsonObject> UpdateCity(string token, JsonObject @params)
    {
        // Validar el token.
        var authInformation = await Access.Auth.Controllers.Authentication.Login(token);

        var profileService = Scope.ServiceProvider.GetService<Profiles>();

        var profile = await profileService.ReadByAccount(authInformation.Model.Id);

        await profileService.Update(new()
        {
            Id = profile.Model.Id,
            City = @params["city"]!.GetValue<string>()
        });

        string data = $$"""
        {
            "ok": "ok"
        }
        """;
        return JsonNode.Parse(data)!.AsObject();
    }

    public async Task<JsonObject> UpdateAlias(string token, JsonObject @params)
    {
        // Validar el token.
        var authInformation = await Access.Auth.Controllers.Authentication.Login(token);

        var profileService = Scope.ServiceProvider.GetService<Profiles>();

        var profile = await profileService.ReadByAccount(authInformation.Model.Id);

        await profileService.Update(new()
        {
            Id = profile.Model.Id,
            Alias = @params["alias"]!.GetValue<string>()
        });

        string data = $$"""
        {
            "ok": "ok"
        }
        """;
        return JsonNode.Parse(data)!.AsObject();
    }
}