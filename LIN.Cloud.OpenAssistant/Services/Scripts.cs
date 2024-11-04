using Azure;
using LIN.Access.OpenIA.Models;
using LIN.Cloud.OpenAssistant.Persistence.Data;
using LIN.Types.Cloud.OpenAssistant.Models;
using LIN.Types.Responses;
using Newtonsoft.Json;
using SILF.Script;
using SILF.Script.Elements;
using SILF.Script.Elements.Functions;
using SILF.Script.Interfaces;
using SILF.Script.Objects;
using SILF.Script.Runtime;

namespace LIN.Cloud.OpenAssistant.Services;

public class Scripts
{


    internal class BridgeFunction : IFunction
    {


        public Tipo? Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Parameter> Parameters { get; set; } = new();

        SILF.Script.Elements.Context IFunction.Context { get; set; }

        private Func<List<SILF.Script.Elements.ParameterValue>, FuncContext> Action;

        public BridgeFunction(Func<List<SILF.Script.Elements.ParameterValue>, FuncContext> action)
        {
            this.Action = action;
        }

        public FuncContext Run(Instance instance, List<SILF.Script.Elements.ParameterValue> values, ObjectContext @object)
        {
            return Action.Invoke(values);
        }

    }






    /// <summary>
    /// Construye las funciones
    /// </summary>
    public static List<IFunction> Build(string token, int profile, string prompt, string app, Profiles profiles, Context context)
    {

        // Acciones.
        List<IFunction> Actions = [];

        // Función require.
        FuncContext require(List<ParameterValue> values)
        {

            // Obtener la aplicación de contexto.
            string? application = values.LastOrDefault(t => t.Name == "value")?.Objeto.GetValue().ToString();

            // Variables.
            string url = string.Empty;
            bool getMethods = application == app;

            // Según la app.
            switch (application)
            {
                // Contacts.
                case "contacts":
                    url = "https://api.contacts.linplatform.com/emma";
                    break;
                // Allo.
                case "allo":
                    url = "https://api.communication.linplatform.com/emma";
                    break;
                // Inventory.
                case "inventory":
                    url = "https://api.inventory.linplatform.com/emma";
                    break;
                // Calendar.
                case "calendar":
                    url = "https://api.calendar.linplatform.com/emma";
                    break;
                // Calendar.
                case "notes":
                    url = "https://api.notes.linplatform.com/emma";
                    break;
            }

            // Emma API.
            var responseTask = ToEmma(url, token, getMethods);
            responseTask.Wait();

            // Obtener la respuesta.
            var appResponse = responseTask.Result.Model.ToString() ?? "";

            // Builder IA.
            Access.OpenIA.IAModelBuilder iaBuilder = new()
            {
                Schema = context.Schema
            };

            // Agregar mensajes.
            iaBuilder.Load([Message.FromSystem(appResponse), .. context.Messages.Take(5), Message.FromUser(prompt)]);

            // Esperar respuesta.
            var reply = iaBuilder.Reply();
            reply.Wait();

            return new FuncContext()
            {
                WaitType = new("string"),
                IsReturning = true,
                Value = new SILFClassObject()
                {
                    Tipo = new("string"),
                    Value = reply.Result.Content,
                }
            };
        }

        // Función require.
        FuncContext force(List<ParameterValue> values)
        {

            // Obtener la aplicación de contexto.
            string? type = values.LastOrDefault(t => t.Name == "type")?.Objeto.GetValue().ToString();
            string? value = values.LastOrDefault(t => t.Name == "value")?.Objeto.GetValue().ToString();

            switch (type)
            {
                case "alias":
                    {
                        _ = profiles.Update(new()
                        {
                            Id = profile,
                            Alias = value,
                        });
                        break;
                    }
                case "ciudad":
                    {
                        _ = profiles.Update(new()
                        {
                            Id = profile,
                            City = value,
                        });
                        break;
                    }
            }
            return new FuncContext()
            {
                IsReturning = true
            };
        }

        // Acciones.
        Actions = [new BridgeFunction(require)
        {
            Name = "require",
            Parameters = [
                new Parameter("value", new("string"))
                ],
            Type = new("string"),
        },
        new BridgeFunction(force)
        {
            Name = "force",
            Parameters = [
                new Parameter("type", new("string")),
                new Parameter("value", new("string"))
                ]
        }];

        return Actions;

    }


    /// <summary>
    /// Obtiene las conversaciones asociadas a un perfil
    /// </summary>
    /// <param name="token">Token de acceso</param>
    public static async Task<ReadOneResponse<object>> ToEmma(string url, string token, bool includeMethods)
    {

        // Crear HttpClient
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("tokenAuth", token);
        httpClient.DefaultRequestHeaders.Add("includeMethods", includeMethods.ToString().ToLower());

        try
        {
            // Envía la solicitud
            var response = await httpClient.GetAsync(url);

            // Lee la respuesta del servidor
            var responseContent = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<ReadOneResponse<object>>(responseContent);

            return obj ?? new();

        }
        catch (Exception e)
        {
            Console.WriteLine($"Error al hacer la solicitud GET: {e.Message}");
        }


        return new();





    }

}