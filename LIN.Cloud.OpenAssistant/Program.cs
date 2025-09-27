using Http.Extensions;
using LIN.Cloud.OpenAssistant.Persistence.Extensions;
using LIN.Cloud.OpenAssistant.Services;
using LIN.Access.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLINHttp();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddSingleton<ContextManager, ContextManager>();
builder.Services.AddAuthenticationService();

var app = builder.Build();

app.UseLINHttp(useGateway: true);
app.UseAuthorization();
app.MapControllers();
app.UseDataBase();

// Limite.
app.UseRateTokenLimit(10, TimeSpan.FromMinutes(1));

LIN.Access.OpenIA.OpenIA.SetKey(builder.Configuration["OpenIA:Key"] ?? "");
LIN.Access.Gemini.Gemini.SetKey(builder.Configuration["Gemini:Key"] ?? "");

LIN.OpenAI.Connector.Client.ApiKey = builder.Configuration["OpenIA:Key"] ?? "";

app.Run();