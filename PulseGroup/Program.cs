using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using PulseGroup.Configuration;
using PulseGroup.Models;
using PulseGroup.Services;
using PulseGroup.Handlers;

// ============================================================================
// CONSOLE ENCODING SETUP (Important for Russian text!)
// ============================================================================
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// ============================================================================
// BOT INITIALIZATION
// ============================================================================
Console.WriteLine("?? PulseGroup Bot Starting...");
Console.WriteLine($"?? Version: {BotConfig.BotVersion}");
Console.WriteLine();

// Create bot client
var botClient = BotInitializer.CreateBotClient();

using CancellationTokenSource cts = new();

// Test connection
if (!await BotInitializer.TestConnectionAsync(botClient, cts.Token))
{
    Console.WriteLine("Failed to connect. Exiting...");
    return;
}

// ============================================================================
// INITIALIZE SERVICES AND HANDLERS
// ============================================================================
// Session dictionaries
var userSessions = new Dictionary<long, CarCalculation>();
var adminSessions = new Dictionary<long, AdminSession>();

// Services
var configurationService = new ConfigurationService();
var pricingConfig = configurationService.LoadConfiguration();
var statisticsService = new StatisticsService(configurationService);
var calculationService = new CalculationService(pricingConfig);

// Handlers
var calculationHandler = new CalculationHandler(userSessions, calculationService, statisticsService);
var adminHandler = new AdminHandler(adminSessions, pricingConfig, calculationService, statisticsService, userSessions, configurationService);
var adminCallbackHandler = new AdminCallbackHandler(adminSessions, pricingConfig, calculationService, statisticsService, userSessions, adminHandler, configurationService);
var commandHandler = new CommandHandler(userSessions, adminSessions, calculationHandler, adminHandler);
var callbackQueryHandler = new CallbackQueryHandler(userSessions, adminSessions, calculationHandler, adminCallbackHandler);
var updateHandler = new UpdateHandler(userSessions, adminSessions, commandHandler, calculationHandler, adminHandler, callbackQueryHandler);

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

// Handle shutdown gracefully
Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

// Wait indefinitely until cancellation is requested
try
{
    await Task.Delay(-1, cts.Token);
}
catch (TaskCanceledException)
{
    // Expected when cts.Cancel() is called
}

// ============================================================================
// SHUTDOWN
// ============================================================================
Console.WriteLine("\n?? Bot stopped.");
