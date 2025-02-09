using LIN.Cloud.OpenAssistant.Persistence.Data;
using LIN.Cloud.OpenAssistant.Services.Context;
using LIN.Types.Cloud.OpenAssistant.Models;

namespace LIN.Cloud.OpenAssistant.Services.Responders;

public interface IIAResponder
{
    Task<string?> Reply(string system, string token, string currentApp, UserContext context, Profiles profiles);
}