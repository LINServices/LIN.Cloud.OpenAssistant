using LIN.Access.OpenIA.Models;
using LIN.Cloud.OpenAssistant.Persistence.Data;
using LIN.Cloud.OpenAssistant.Services;
using LIN.Types.Cloud.OpenAssistant.Api;
using LIN.Types.Cloud.OpenAssistant.Models;
using LIN.Types.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LIN.Cloud.OpenAssistant.Controllers;

[Route("[Controller]")]
[Route("Emma")]
public class AssistantController(Profiles profilesData, ContextManager contextManager) : ControllerBase
{

    /// <summary>
    /// Asistente.
    /// </summary>
    /// <param name="request">Request.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpPost]
    public async Task<ReadOneResponse<EmmaSchemaResponse>> Assistant([FromBody] AssistantRequest request, [FromHeader] string token)
    {

        // Obtener datos de autenticación.
        var authData = await LIN.Access.Auth.Controllers.Authentication.Login(token);

        // Validar.
        if (authData.Response != Types.Responses.Responses.Success)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "No tienes autorización."
            };

        // Obtener perfil.
        var profile = await profilesData.ReadByAccount(authData.Model.Id);

        // Crear perfil.
        if (profile.Response != Types.Responses.Responses.Success)
        {

            // Modelo.
            ProfileModel model = new()
            {
                AccountId = authData.Model.Id,
                Alias = authData.Model.Name,
                City = "Bogotá"
            };

            // Crear perfil.
            var response = await profilesData.Create(model);

            // Validar.
            if (response.Response != Types.Responses.Responses.Success)
                return new()
                {
                    Response = Responses.Undefined,
                };

            model.Id = response.LastID;
            profile.Model = model;
        }

        // Obtener header.
        Context context = contextManager.GetOrCreate(profile.Model);

        // Responder.
        (bool isSuccess, EmmaSchemaResponse responseEmma) = await context.Reply(token, request.Prompt, request.App, profilesData);


        // Parsear.




        return new()
        {
            Response = isSuccess ? Responses.Success : Responses.Undefined,
            Model = responseEmma
        };

    }


    /// <summary>
    /// Asistente.
    /// </summary>
    /// <param name="request">Request.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpPut]
    public async Task<ResponseBase> Assistant([FromHeader] string token)
    {

        // Obtener datos de autenticación.
        var authData = await LIN.Access.Auth.Controllers.Authentication.Login(token);

        // Validar.
        if (authData.Response != Types.Responses.Responses.Success)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "No tienes autorización."
            };

        // Obtener perfil.
        var profile = await profilesData.ReadByAccount(authData.Model.Id);

        // Obtener header.
        contextManager.Delete(profile.Model.Id);



        return new()
        {
            Response = Responses.Success
        };

    }


    /// <summary>
    /// Asistente.
    /// </summary>
    /// <param name="request">Request.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpGet]
    public async Task<ReadAllResponse<Message>> Get([FromHeader] string token)
    {

        // Obtener datos de autenticación.
        var authData = await LIN.Access.Auth.Controllers.Authentication.Login(token);

        // Validar.
        if (authData.Response != Types.Responses.Responses.Success)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "No tienes autorización."
            };

        // Obtener perfil.
        var profile = await profilesData.ReadByAccount(authData.Model.Id);

        // Obtener header.
        Context context = contextManager.GetOrCreate(profile.Model);

        return new()
        {
            Response = Responses.Success,
            Models = context.Messages
        };

    }

}