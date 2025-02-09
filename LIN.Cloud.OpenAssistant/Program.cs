using Http.Extensions;
using LIN.Cloud.OpenAssistant.Persistence.Extensions;
using LIN.Cloud.OpenAssistant.Services;
using LIN.Access.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddLINHttp();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddSingleton<ContextManager, ContextManager>();
builder.Services.AddAuthenticationService();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseLINHttp();
app.UseDataBase();

// Limite.
app.UseRateTokenLimit(10, TimeSpan.FromMinutes(1));

LIN.Access.OpenIA.OpenIA.SetKey(builder.Configuration["OpenIA:Key"] ?? "");
LIN.Access.Gemini.Gemini.SetKey(builder.Configuration["Gemini:Key"] ?? "");

app.Run();