using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using PulseGroup.Models;
using PulseGroup.Services;
using static PulseGroup.Handlers.Localization;

namespace PulseGroup.Handlers;

/// <summary>
/// Handler for user calculation interactions
/// </summary>
public class CalculationHandler
{
    private readonly Dictionary<long, CarCalculation> _userSessions;
    private readonly CalculationService _calculationService;
    private readonly StatisticsService _statisticsService;

    public CalculationHandler(
        Dictionary<long, CarCalculation> userSessions,
        CalculationService calculationService,
        StatisticsService statisticsService)
    {
        _userSessions = userSessions;
        _calculationService = calculationService;
        _statisticsService = statisticsService;
    }

    /// <summary>
    /// Starts a new calculation session
    /// </summary>
    public async Task StartCalculationAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(Buttons.ButtonChina, "country_china"),
            }
            // USA and Europe buttons hidden - only China is available
            // new[]
            // {
            //     InlineKeyboardButton.WithCallbackData(Buttons.ButtonUSA, "country_usa"),
            //     InlineKeyboardButton.WithCallbackData(Buttons.ButtonEurope, "country_europe")
            // }
        });

        _userSessions[chatId] = new CarCalculation { CurrentStep = "awaiting_country" };

        var message = $"{Messages.CalcTitle}\n\n" +
                     $"{Messages.CalcStep1}";
                     // Removed warning about USA/Europe since they're not shown
        
        await MessageHelper.SendMessageSafeAsync(botClient, chatId, message, cancellationToken, ParseMode.Markdown, keyboard);
    }

    /// <summary>
    /// Handles calculation step input
    /// </summary>
    public async Task HandleCalculationStepAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var session = _userSessions[chatId];

        if (session.CurrentStep == "awaiting_price")
        {
            if (decimal.TryParse(message.Text, out decimal price) && price > 0)
            {
                session.CarPrice = price;
                session.DeliveryType = "train"; // Always use train delivery
                session.CurrentStep = "complete";

                var msg = $"{Messages.CalcPriceSaved}\n\n{Messages.CalcCalculating}";
                await MessageHelper.SendMessageSafeAsync(botClient, chatId, msg, cancellationToken);

                await Task.Delay(1000, cancellationToken);
                await CalculateFinalPriceAsync(botClient, chatId, cancellationToken);
            }
            else
            {
                await MessageHelper.SendMessageSafeAsync(botClient, chatId, Messages.InvalidPrice, cancellationToken, includeMainMenuButton: true);
            }
        }
    }

    /// <summary>
    /// Handles country selection
    /// </summary>
    public async Task HandleCountrySelectionAsync(ITelegramBotClient botClient, long chatId, string country, CancellationToken cancellationToken)
    {
        if (_userSessions.ContainsKey(chatId))
        {
            _userSessions[chatId].Country = country;
            _userSessions[chatId].CurrentStep = "awaiting_price";

            var calculator = _calculationService.GetCalculatorForCountry(country);
            var message = $"{string.Format(Messages.CalcCountrySelected, calculator.CountryName)}\n\n";
            
            // Show warning for USA and Europe calculators
            if (country == "usa" || country == "europe")
            {
                message += $"{Messages.CalcWarningCountryInDevelopment}\n\n";
            }
            
            message += $"{Messages.CalcStep2}\n\n" +
                      $"{Messages.CalcPriceExample}";

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(Buttons.ButtonMainMenu, "main_menu")
                }
            });

            await MessageHelper.SendMessageSafeAsync(botClient, chatId, message, cancellationToken, ParseMode.Markdown, keyboard);
        }
    }

    /// <summary>
    /// Handles delivery type selection - No longer used, kept for compatibility
    /// </summary>
    public async Task HandleDeliverySelectionAsync(ITelegramBotClient botClient, long chatId, string deliveryType, CancellationToken cancellationToken)
    {
        // This method is kept for backward compatibility but is no longer used
        // Delivery type is now automatically set to "train" when price is entered
        await Task.CompletedTask;
    }

    /// <summary>
    /// Calculates and displays final price using country-specific calculator
    /// </summary>
    private async Task CalculateFinalPriceAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var session = _userSessions[chatId];

        if (session.CarPrice == null || session.Country == null || session.DeliveryType == null)
        {
            await MessageHelper.SendMessageSafeAsync(botClient, chatId, Messages.ErrorDataIncomplete, cancellationToken, includeMainMenuButton: true);
            _userSessions.Remove(chatId);
            return;
        }

        decimal carPrice = session.CarPrice.Value;
        
        // Use country-specific calculator
        decimal total = _calculationService.CalculateTotalPrice(carPrice, session.DeliveryType, session.Country);

        // Update statistics
        _statisticsService.RecordCalculation(total);

        // Get formatted result text with country-specific breakdown
        var resultText = _calculationService.GetCalculationResultText(session, carPrice, total);

        // Create keyboard with "New Calculation" and "Main Menu" buttons
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(Buttons.ButtonNewCalculation, "new_calculation"),
                InlineKeyboardButton.WithCallbackData(Buttons.ButtonMainMenu, "main_menu")
            }
        });

        await MessageHelper.SendMessageSafeAsync(botClient, chatId, resultText, cancellationToken, ParseMode.Markdown, keyboard);

        // Clear session
        _userSessions.Remove(chatId);
    }

    /// <summary>
    /// Shows example calculation
    /// </summary>
    public async Task ShowExampleAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var exampleText = _calculationService.GetExampleText();
        
        // Create keyboard with "Calculate" and "Main Menu" buttons
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(Buttons.ButtonNewCalculation, "new_calculation"),
                InlineKeyboardButton.WithCallbackData(Buttons.ButtonMainMenu, "main_menu")
            }
        });

        await MessageHelper.SendMessageSafeAsync(botClient, chatId, exampleText, cancellationToken, ParseMode.Markdown, keyboard);
    }
}
