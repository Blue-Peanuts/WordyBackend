using Microsoft.OpenApi.Models;
using WordyBackend.Models;
using WordyBackend.Models.Game;
using WordyBackend.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer(); // Required for swagger
builder.Services.AddSwaggerGen();
builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.AppDomain.CurrentDomain.FriendlyName}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My Game API",
        Version = "v1",
        Description = "API for managing game sessions and gameplay.",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your.email@example.com",
            Url = new Uri("https://example.com"),
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession();


var gameSessionManager = new UserSessionManager();
var wordProvider = new WordProvider();


app.MapPost("/create-session", (HttpContext context) =>
{
    var session = gameSessionManager.CreateSession("hello");
    var sessionId = session.Id;

    context.Session.SetString("sessionId", sessionId); // Store in session

    return Results.Json(new { session = new UserSessionDTO(session) });
});


app.MapGet("/get-session", (HttpContext context) =>
{
    var sessionId = context.Session.GetString("sessionId");
    var session = gameSessionManager.TryGetSession(sessionId ?? "");
    
    if (session == null)
        return Results.Json(new { error = "No active session" });

    return Results.Json(new { status = "Session active", session = new UserSessionDTO(session) });
});


app.MapPost("/create-game", (HttpContext context) =>
{
    var sessionId = context.Session.GetString("sessionId");
    var session = gameSessionManager.TryGetSession(sessionId ?? "");
    
    if (session == null)
        return Results.Json(new { error = "No active session" });

    var gameInstance = new GameInstance(wordProvider.GetRandomWord(), 5);
    session.GameInstanceManager.AddGameInstance(gameInstance);

    return Results.Json(new { gameInstance = new GameInstanceDTO(gameInstance) });
});


app.MapPost("/guess", (HttpContext context) =>
{
    var sessionId = context.Session.GetString("sessionId");
    var session = gameSessionManager.TryGetSession(sessionId ?? "");
    
    if (session == null)
        return Results.Json(new { error = "No active session" });

    var gameInstanceId = context.Request.Form["gameInstanceId"].ToString();
    var guess = context.Request.Form["guess"].ToString();

    var gameInstance = session.GameInstanceManager.TryGetGameInstance(gameInstanceId);
    if (gameInstance == null)
        return Results.Json(new { error = "Game instance not found" });

    try
    {
        _ = gameInstance.Guess(guess);   
    }
    catch (GameException ex)
    {
        return Results.Json(new { error = ex.Message });
    }
    return Results.Json(new GameInstanceDTO(gameInstance));
});


app.Run();