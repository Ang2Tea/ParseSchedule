using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using VPT_TelegramBot.Controller;

namespace VPT_TelegramBot.View
{
    public class BotMenu
    {
        public static async Task StartMenu(Chat chat)
        {
            if (Controller.BotTelegram.SecretKey == "") return;
            //
            // InlineKeyboardButton urlButton4 = new InlineKeyboardButton("Schedule");
            // InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(urlButton4);
            //


            Model.User user = new Model.User() { UserId = chat.Id.ToString(), FirstName = chat.FirstName, LastName = chat.LastName, UserName = chat.Username };
            HttpUser.UpdateUserInfoAtId(user, BotTelegram.SecretKey);

            // Send Message

            string message = $"Меню. {DateTime.Now.Date}\n\n" + 
                $"Выберите пункт из списка";

            var messageSend = await BotTelegram.bot.SendTextMessageAsync(chat, message, Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(BotButtons.GetMenuBtn(user)));

            user.MessageMenuId = messageSend.MessageId;
            HttpUser.UpdateUserInfoAtId(user, BotTelegram.SecretKey);
        }

        public static async Task MainMenu(Chat chat)
        {
            if (Controller.BotTelegram.SecretKey == "") return;

            InlineKeyboardButton urlButton4 = new InlineKeyboardButton("Schedule2");
            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(urlButton4);
            
            Model.User user = Controller.HttpUser.GetUserAtId(chat.Id.ToString(), Controller.BotTelegram.SecretKey);

            // Send Message
            //await bot.EditMessageTextAsync();
             await BotTelegram.bot.EditMessageTextAsync(chat, user.MessageMenuId,"Message", replyMarkup: keyboard);
        }

        public static void WeatherMenu()
        {

        }

        public static void SelectGroupMenu()
        {

        }

        public static void ScheduleMenu()
        {

        }
    }
}
