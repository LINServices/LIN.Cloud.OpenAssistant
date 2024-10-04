namespace LIN.Types.Cloud.OpenAssistant.Api;

public class AssistantRequest
{
    public string Prompt { get; set; }
    public string App { get; set; }
    public string Methods { get; set; }
    public string AdditionalTopics { get; set; }
}