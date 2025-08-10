using LIN.OpenAI.Connector.Models;
using System.Text.Json.Nodes;

namespace LIN.Cloud.OpenAssistant.Services.Assistants;

public class OpenAIConnector
{


    public static List<FunctionModel> Tools = [
        new FunctionModel
        {
            Function = new FunctionDetails
            {
                Name = "weather",
                Description = "Obtener el clima de una ciudad",
                Parameters = new FunctionParameters
                {
                    Properties =
                    {
                       ["city"] = new PropertyField { Type = "string", Description = "Nombre de la ciudad para buscar el clima" }
                    },
                    Required = new[] { "city" }
                }
            }
        }
    ];


    public static Dictionary<string, Func<JsonObject, Task<string>>> Toolsl = [];

    static OpenAIConnector()
    {
        Toolsl.Add("weather", async (JsonObject parameters) =>
        {
            var city = parameters["city"]?.GetValue<string>();
            if (string.IsNullOrWhiteSpace(city))
                return "Ciudad no proporcionada.";
            // Aquí iría la lógica para obtener el clima real.
            await Task.Delay(500); // Simular una llamada asíncrona.
            return $"El clima en {city} es soleado con 25°C.";
        });
    }



}