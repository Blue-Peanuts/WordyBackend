using WordyBackend.Models;
using WordyBackend.Models.Game;
using WordyBackend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

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

    return Results.Json(new { gameInstance });
});

app.Run();