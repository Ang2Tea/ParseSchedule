using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace VPT_TelegramBot.View
{
    internal class BotButtons
    {
        internal static List<List<InlineKeyboardButton>> GetMenuBtn(Model.User user)
        {
            List<List<InlineKeyboardButton>> messageButtonsUser = new()
            {
                new List<InlineKeyboardButton> { new("☁ Погода") { CallbackData = "weather"} },
                new List<InlineKeyboardButton> { new("📁 Выбрать группу") { CallbackData = "selectGroup" } },
                new List<InlineKeyboardButton> { new("📝 Расписание") { CallbackData = "schedule" } }
            };

            List<List<InlineKeyboardButton>> messageButtonsTeacher = new()
            {
                new List<InlineKeyboardButton> { new("☁ Погода") { CallbackData = "weather"} },
                new List<InlineKeyboardButton> { new("📝 Расписание") { CallbackData = "scheduleTeacher" } },
                new List<InlineKeyboardButton> { new("✉ Отправить рассылку") { CallbackData = "news" } },
                new List<InlineKeyboardButton> { new("⏱ Получить количесвто часов") { CallbackData = "getScheduleTime" } },
            };

            List<List<InlineKeyboardButton>> messageButtonsAdmin = new()
            {
                new List<InlineKeyboardButton> { new("☁ Погода") { CallbackData = "weather"} },
                new List<InlineKeyboardButton> { new("📁 Выбрать группу") { CallbackData = "selectGroup" } },
                new List<InlineKeyboardButton> { new("📝 Расписание") { CallbackData = "schedule" } },
                new List<InlineKeyboardButton> { new("📝 Расписание преподователей") { CallbackData = "selectTeacher" } },
                new List<InlineKeyboardButton> { new("✉ Отправить рассылку") { CallbackData = "news" } },

            };

            switch (user.Role)
            {
                case "Student":
                    return messageButtonsUser;
                    break;
                case "Teacher":
                    return messageButtonsUser;
                    break;
                case "Admin":
                    return messageButtonsUser;
                    break;
                default:
                    return new List<List<InlineKeyboardButton>>()
                    {
                        new List<InlineKeyboardButton>()
                        {
                            new("В меню") { CallbackData = "menu"}
                        }
                    };
                    break;
            }
        }



        internal static List<List<InlineKeyboardButton>> GetScheduleBtn(Model.User user)
        {
            List<List<InlineKeyboardButton>> messageButtonsUser = new()
            {
                new List<InlineKeyboardButton> { new("☁ Меню") { CallbackData = "menu"} }
            };


            switch (user.Role)
            {
                case "Student":
                    return messageButtonsUser;
                    break;
                case "Teacher":
                    return messageButtonsUser;
                    break;
                case "Admin":
                    return messageButtonsUser;
                    break;
                default:
                    return new List<List<InlineKeyboardButton>>()
                    {
                        new List<InlineKeyboardButton>()
                        {
                            new("В меню") { CallbackData = "menu"}
                        }
                    };
                    break;
            }
        }
    }
}
