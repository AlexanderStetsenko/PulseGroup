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
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(Buttons.ButtonUSA, "country_usa"),
                InlineKeyboardButton.WithCallbackData(Buttons.ButtonEurope, "country_europe")
            }
        });

        _userSessions[chatId] = new CarCalculation { CurrentStep = "awaiting_country" };

        var message = $"{Messages.CalcTitle}\n\n" +
                     $"{Messages.CalcStep1}\n\n" +
                     $"{Messages.CalcWarningTitle}\n" +
                     $"{Messages.CalcWarningUSAEurope}";
        
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
                session.CurrentStep = "awaiting_delivery";

                // Get calculator to show available delivery options
                var calculator = _calculationService.GetCalculatorForCountry(session.Country!);
                
                var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(Buttons.ButtonDeliveryShip, "delivery_ship"),
                        InlineKeyboardButton.WithCallbackData(Buttons.ButtonDeliveryTrain, "delivery_train")
                    }
                });

                var msg = $"{Messages.CalcPriceSaved}\n\n{Messages.CalcStep3}";
                
                // Add country-specific delivery note
                var notes = calculator.GetCountrySpecificNotes();
                if (!string.IsNullOrEmpty(notes))
                {
                    msg += $"\n\n?? {notes}";
                }

                await MessageHelper.SendMessageSafeAsync(botClient, chatId, msg, cancellationToken, replyMarkup: keyboard);
            }
            else
            {
                await MessageHelper.SendMessageSafeAsync(botClient, chatId, Messages.InvalidPrice, cancellationToken);
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

            await MessageHelper.SendMessageSafeAsync(botClient, chatId, message, cancellationToken, ParseMode.Markdown);
        }
    }

    /// <summary>
    /// Handles delivery type selection
    /// </summary>
    public async Task HandleDeliverySelectionAsync(ITelegramBotClient botClient, long chatId, string deliveryType, CancellationToken cancellationToken)
    {
        if (_userSessions.ContainsKey(chatId))
        {
            _userSessions[chatId].DeliveryType = deliveryType;
            _userSessions[chatId].CurrentStep = "complete";

            var deliveryText = deliveryType == "ship" ? Messages.DeliveryShip : Messages.DeliveryTrain;
            var message = $"{string.Format(Messages.CalcDeliverySelected, deliveryText)}\n\n{Messages.CalcCalculating}";

            await MessageHelper.SendMessageSafeAsync(botClient, chatId, message, cancellationToken, ParseMode.Markdown);

            await Task.Delay(1000, cancellationToken);
            await CalculateFinalPriceAsync(botClient, chatId, cancellationToken);
        }
    }

    /// <summary>
    /// Calculates and displays final price using country-specific calculator
    /// </summary>
    private async Task CalculateFinalPriceAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var session = _userSessions[chatId];

        if (session.CarPrice == null || session.Country == null || session.DeliveryType == null)
        {
            await MessageHelper.SendMessageSafeAsync(botClient, chatId, Messages.ErrorDataIncomplete, cancellationToken);
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

        await MessageHelper.SendMessageSafeAsync(botClient, chatId, resultText, cancellationToken, ParseMode.Markdown);

        // Clear session
        _userSessions.Remove(chatId);
    }

    /// <summary>
    /// Shows example calculation
    /// </summary>
    public async Task ShowExampleAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var exampleText = _calculationService.GetExampleText();
        await MessageHelper.SendMessageSafeAsync(botClient, chatId, exampleText, cancellationToken, ParseMode.Markdown);
    }
}
