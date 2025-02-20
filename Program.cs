using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using WordyBackend.Models;
using WordyBackend.Models.Game;
using WordyBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.env.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Services.AddEndpointsApiExplorer(); // Required for swagger
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
        Title = "Wordy API",
        Version = "v1",
        Description = "API for managing Worldle-like game sessions and gameplay.",
    });
});
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ??
                                 throw new InvalidOperationException();
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ??
                                     throw new InvalidOperationException();
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ??
                                                                throw new InvalidOperationException())),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();
Console.WriteLine(builder.Configuration["Authentication:Google:ClientId"]);

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseSession();
// app.UseAuthentication();
// app.UseAuthorization();


var gameSessionManager = new UserSessionManager();
var wordProvider = new WordProvider();


app.MapPost("/create-session", (HttpContext context) =>
    {
        var session = gameSessionManager.CreateSession("hello");
        var sessionId = session.Id;

        context.Session.SetString("sessionId", sessionId); // Store in session

        return Results.Ok(new { session = new UserSessionDTO(session) });
    })
    .WithName("CreateSession")
    .WithMetadata(new SwaggerOperationAttribute("Create a user session",
        "Creates a new user session and stores the session ID in the user's session."))
    .Produces<UserSessionDTO>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status500InternalServerError);
;


app.MapGet("/get-session", (HttpContext context) =>
    {
        var sessionId = context.Session.GetString("sessionId");
        var session = gameSessionManager.TryGetSession(sessionId ?? "");

        if (session == null)
            return Results.NotFound(new { error = "No active session" });

        return Results.Ok(new { status = "Session active", session = new UserSessionDTO(session) });
    })
    .WithName("GetSession")
    .WithMetadata(new SwaggerOperationAttribute("Get the current user session",
        "Retrieves the current user session based on the session ID stored in the user's session."))
    .Produces<UserSessionDTO>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);
;


app.MapPost("/create-game",
        (HttpContext context, [FromForm(Name = "word-length")] int wordLength = 5,
            [FromForm(Name = "max-attempts")] int maxAttempts = 5) =>
        {
            var sessionId = context.Session.GetString("sessionId");
            var session = gameSessionManager.TryGetSession(sessionId ?? "");

            if (session == null)
                return Results.NotFound(new { error = "No active session" });

            var gameInstance = new GameInstance(wordProvider.GetRandomWord(wordLength), maxAttempts);
            session.GameInstanceManager.AddGameInstance(gameInstance);

            return Results.Ok(new { gameInstance = new GameInstanceDTO(gameInstance) });
        })
    .WithName("CreateGame")
    .WithMetadata(new SwaggerOperationAttribute("Create a new game instance",
        "Creates a new game instance within the current game session."))
    .Produces<GameInstanceDTO>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .DisableAntiforgery();


app.MapPost("/guess", (HttpContext context, [FromForm] string guess) =>
    {
        var sessionId = context.Session.GetString("sessionId");
        var session = gameSessionManager.TryGetSession(sessionId ?? "");

        if (session == null)
            return Results.NotFound(new { error = "No active session" });

        var gameInstanceId = context.Request.Form["gameInstanceId"].ToString();
        // var guess = context.Request.Form["guess"].ToString();

        var gameInstance = session.GameInstanceManager.TryGetGameInstance(gameInstanceId);
        if (gameInstance == null)
            return Results.NotFound(new { error = "Game instance not found" });

        try
        {
            _ = gameInstance.Guess(guess);
        }
        catch (GameException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }

        return Results.Ok(new GameInstanceDTO(gameInstance));
    })
    .WithName("Guess")
    .WithMetadata(new SwaggerOperationAttribute("Make a guess in a game instance",
        "Makes a guess in the specified game instance."))
    .Produces<GameInstanceDTO>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status400BadRequest)
    .DisableAntiforgery();


app.Run();