using Http.Extensions;
using LIN.Access.Auth;
using LIN.Cloud.OpenAssistant.Persistence.Extensions;
using LIN.Cloud.OpenAssistant.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLINHttp();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddSingleton<ContextManager, ContextManager>();
builder.Services.AddAuthenticationService();
builder.Services.AddOpenIAConnector(builder.Configuration);

var app = builder.Build();

app.UseLINHttp(useGateway: true);
app.UseAuthorization();
app.MapControllers();
app.UseDataBase();

// Limite.
app.UseRateTokenLimit(10, TimeSpan.FromMinutes(1));

app.Run();