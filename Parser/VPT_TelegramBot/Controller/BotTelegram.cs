using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;

namespace VPT_TelegramBot.Controller
{
    public class BotTelegram
    {
        internal static ITelegramBotClient bot;
        internal string Token { get; set; }
        internal static string SecretKey { get; set; } = "";

        public BotTelegram(string key, string token)
        {
            SecretKey = key;
            this.Token = token;
        }

        

        public bool Start()
        {
            if (Token == "") return false;
            bot = new TelegramBotClient(Token);

            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            return true;
        }


        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия

            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text.ToLower() == "/start")
                {
                    
                    
                    await View.BotMenu.StartMenu(message.Chat);
                    return;
                }
                await botClient.SendTextMessageAsync(message.Chat, "Привет-привет!!");
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
    }
}
