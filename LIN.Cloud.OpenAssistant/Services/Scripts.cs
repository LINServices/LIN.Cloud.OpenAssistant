using LIN.Access.OpenIA.Models;
using LIN.Cloud.OpenAssistant.Persistence.Data;
using LIN.Cloud.OpenAssistant.Services.Context;
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