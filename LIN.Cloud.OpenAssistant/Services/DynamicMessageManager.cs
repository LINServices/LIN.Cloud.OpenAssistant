using LIN.Types.Cloud.OpenAssistant.Models;

namespace LIN.Cloud.OpenAssistant.Services;

public class DynamicMessageManager
{

    /// <summary>
    /// Modelo.
    /// </summary>
    private static string? Model { get; set; }


    /// <summary>
    /// Obtener datos del modelo.
    /// </summary>
    private static string GetModel()
    {
        // Armar datos de usuario.
        Model ??= File.ReadAllText("wwwroot/data.ia");
        return Model;
    }


    /// <summary>
    /// Obtener datos de usuario.
    /// </summary>
    /// <param name="profile">Perfil.</param>
    private static string GetDataUser(ProfileModel profile)
    {
        // Armar datos de usuario.
        string userData = $"""Eres Emma, el asistente personal del usuario '{profile.Alias}'""";
        return userData;

    }


    /// <summary>
    /// Obtener locación.
    /// </summary>
    /// <param name="profile">Perfil.</param>
    private static string GetLocation(ProfileModel profile)
    {
        // Armar datos de usuario.
        DateTime now = DateTime.Now;
        string locationData = $"""Eres una experta en dar la hora y la fecha, La fecha actual es {now:dddd MMMM yyyy} y la hora actual es {now:HH:mm}, y el usuario esta en la ciudad de {profile.City}""";
        return locationData;
    }


    /// <summary>
    /// Obtener header.
    /// </summary>
    /// <param name="profile">Perfil.</param>
    public static string GetHeader(ProfileModel profile)
    {
        return $"{GetDataUser(profile)}\n{GetModel()}\n{GetLocation(profile)}";
    }

}