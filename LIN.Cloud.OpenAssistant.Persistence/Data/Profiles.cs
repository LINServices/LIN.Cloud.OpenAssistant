using LIN.Cloud.OpenAssistant.Persistence.Context;
using LIN.Types.Cloud.OpenAssistant.Models;
using Microsoft.EntityFrameworkCore;

namespace LIN.Cloud.OpenAssistant.Persistence.Data;

public class Profiles(DataContext context)
{

    /// <summary>
    /// Crear perfil.
    /// </summary>
    /// <param name="profile">Service model</param>
    public async Task<CreateResponse> Create(ProfileModel profile)
    {
        try
        {
            await context.Profiles.AddAsync(profile);
            context.SaveChanges();
            return new(Responses.Success, profile.Id);
        }
        catch (Exception)
        {
        }
        return new(Responses.Undefined);
    }


    /// <summary>
    /// Obtener perfil por medio de la cuenta.
    /// </summary>
    /// <param name="account">Id de la cuenta.</param>
    public async Task<ReadOneResponse<ProfileModel>> ReadByAccount(int account)
    {
        try
        {

            var profile = await (from p in context.Profiles
                                 where p.AccountId == account
                                 select p).FirstOrDefaultAsync();

            return profile is null ? new(Responses.NotExistProfile) : new(Responses.Success, profile);
        }
        catch (Exception)
        {
        }
        return new(Responses.Undefined);
    }


    /// <summary>
    /// Actualizar perfil.  
    /// </summary>
    /// <param name="profile">Modelo.</param>
    public async Task<ResponseBase> Update(ProfileModel profile)
    {
        try
        {

            int update = await (from p in context.Profiles
                                where p.Id == profile.Id
                                select p).ExecuteUpdateAsync(t => t.SetProperty(p => p.City, p => profile.City ?? p.City).SetProperty(p => p.Alias, p => profile.Alias ?? p.Alias));

            return update <= 0 ? new(Responses.NotExistProfile) : new(Responses.Success);
        }
        catch (Exception)
        {
        }
        return new(Responses.Undefined);
    }

}