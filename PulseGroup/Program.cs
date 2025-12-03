using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using PulseGroup.Configuration;
using PulseGroup.Models;
using PulseGroup.Services;
using PulseGroup.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// ============================================================================
// CONSOLE ENCODING SETUP (Important for Russian text!)
// ============================================================================
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// ============================================================================
// WEB APPLICATION BUILDER (For Render Health Check)
// ============================================================================
var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on the port provided by Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(int.Parse(port));
});

// Add HttpClient for self-ping
builder.Services.AddHttpClient();

var app = builder.Build();

// Health check endpoint for Render
app.MapGet("/", () => Results.Ok(new { status = "healthy", bot = "PulseGroup", version = BotConfig.BotVersion }));
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

// ============================================================================
// BOT INITIALIZATION
// ============================================================================
Console.WriteLine("?? PulseGroup Bot Starting...");
Console.WriteLine($"?? Version: {BotConfig.BotVersion}");
Console.WriteLine($"?? HTTP Server listening on port: {port}");
Console.WriteLine($"?? Bot Token: {BotConfig.BotToken.Substring(0, 10)}..."); // Show only first 10 chars
Console.WriteLine();

// Create bot client
var botClient = BotInitializer.CreateBotClient();

using CancellationTokenSource cts = new();

// Test connection
Console.WriteLine("?? Testing connection to Telegram API...");
if (!await BotInitializer.TestConnectionAsync(botClient, cts.Token))
{
    Console.WriteLine("? Failed to connect. Exiting...");
    Console.WriteLine();
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
    return;
}

Console.WriteLine("? Bot connection successful!");
Console.WriteLine();

// ============================================================================
// INITIALIZE SERVICES AND HANDLERS
// ============================================================================
Console.WriteLine("?? Initializing configuration...");

// Session dictionaries
var userSessions = new Dictionary<long, CarCalculation>();
var adminSessions = new Dictionary<long, AdminSession>();

// Services
var configurationService = new ConfigurationService();
Console.WriteLine($"?? Config file path: {configurationService.GetConfigFilePath()}");
Console.WriteLine($"?? Stats file path: {configurationService.GetStatsFilePath()}");
Console.WriteLine();

var pricingConfig = configurationService.LoadConfiguration();
Console.WriteLine($"? Configuration loaded successfully");
Console.WriteLine($"   ImportPreparation: ${pricingConfig.ImportPreparation}");
Console.WriteLine($"   LandSeaDelivery: ${pricingConfig.LandSeaDelivery}");
Console.WriteLine($"   Broker: ${pricingConfig.Broker}");
Console.WriteLine($"   TransportFromPort: ${pricingConfig.TransportFromPort}");
Console.WriteLine($"   CustomsPercent: {pricingConfig.CustomsPercent * 100}%");
Console.WriteLine($"   ImportServices: ${pricingConfig.ImportServices}");
Console.WriteLine();

var statisticsService = new StatisticsService(configurationService);
var calculationService = new CalculationService(pricingConfig);

// Handlers
var calculationHandler = new CalculationHandler(userSessions, calculationService, statisticsService);
var adminHandler = new AdminHandler(adminSessions, pricingConfig, calculationService, statisticsService, userSessions, configurationService);
var adminCallbackHandler = new AdminCallbackHandler(adminSessions, pricingConfig, calculationService, statisticsService, userSessions, adminHandler, configurationService);
var commandHandler = new CommandHandler(userSessions, adminSessions, calculationHandler, adminHandler);
var callbackQueryHandler = new CallbackQueryHandler(userSessions, adminSessions, calculationHandler, adminCallbackHandler);
var updateHandler = new UpdateHandler(userSessions, adminSessions, commandHandler, calculationHandler, adminHandler, callbackQueryHandler);

Console.WriteLine("? All handlers initialized");
Console.WriteLine();

// ============================================================================
// START RECEIVING UPDATES
// ============================================================================
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // Receive all update types
};

botClient.StartReceiving(
    updateHandler: updateHandler.HandleUpdateAsync,
    errorHandler: ErrorHandler.HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

Console.WriteLine($"? Listening for messages...");
Console.WriteLine($"Bot is running. Press Ctrl+C to stop.");


// ============================================================================
// SELF-PING MECHANISM (Keep Render free tier alive)
// ============================================================================
var renderUrl = Environment.GetEnvironmentVariable("RENDER_EXTERNAL_URL");
if (!string.IsNullOrEmpty(renderUrl))
{
    var httpClientFactory = app.Services.GetRequiredService<IHttpClientFactory>();
    _ = Task.Run(async () =>
    {
        var httpClient = httpClientFactory.CreateClient();
        Console.WriteLine($"?? Self-ping enabled for {renderUrl}");
        
        while (!cts.Token.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(10), cts.Token);
                var response = await httpClient.GetAsync($"{renderUrl}/health", cts.Token);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"?? Self-ping successful at {DateTime.UtcNow:HH:mm:ss}");
                }
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"?? Self-ping failed: {ex.Message}");
            }
        }
    }, cts.Token);
}

// Handle shutdown gracefully
Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

// Start the web server
var webServerTask = app.RunAsync(cts.Token);

// Wait indefinitely until cancellation is requested
try
{
    await Task.WhenAny(
        Task.Delay(-1, cts.Token),
        webServerTask
    );
}
catch (TaskCanceledException)
{
    // Expected when cts.Cancel() is called
}

// ============================================================================
// SHUTDOWN
// ============================================================================
Console.WriteLine("\n?? Bot stopped.");
