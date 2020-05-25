using System;
using System.IO;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;

namespace Valerias_Test_Bot
{
    class Program
    {
        private static ITelegramBotClient botClient;

        public static List<string> Messages = new List<string>() { "", "", "" };

        public static List<string> ClearMessages(List<string> Messages)
        {
            Messages.Clear();

            Messages.Add("");

            Messages.Add("");

            Messages.Add("");

            return Messages;
        }

        public static void Main(string[] args)
        {
            botClient = new TelegramBotClient("") { Timeout = TimeSpan.FromSeconds(1) };

            botClient.OnMessage += Bot_OnMessage;

            botClient.StartReceiving();

            Console.ReadKey();

            botClient.StopReceiving();
        }

        public static string Get(string A)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(A);

            request.Method = "GET";

            WebResponse response = request.GetResponse();

            Stream s = response.GetResponseStream();

            StreamReader reader = new StreamReader(s);

            string answer = reader.ReadToEnd();

            response.Close();

            return answer;
        }

        public static ReplyKeyboardMarkup keyboard()
        {
            var replyKeyboard = new ReplyKeyboardMarkup(
               new[]

               {
                    new[] { new KeyboardButton("/latest"),new KeyboardButton("/byday") },

                    new[] { new KeyboardButton("/period"),new KeyboardButton("/cinfo") },

                    new[] { new KeyboardButton("/convert"),new KeyboardButton("/change") },

                    new[] { new KeyboardButton("/help"), },

               });
            replyKeyboard.ResizeKeyboard = true;

            return replyKeyboard;
        }

        public async static void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var text = e.Message.Text;

            var Id = e.Message.Chat.Id;

            try
            {
                if (text == "/byday" || text == "/change" || text == "/cinfo" || text == "/convert" || text == "/period" || text == "/start" || text == "/help" || text == "/latest")
                {
                    switch (text)
                    {
                        case "/start":
                            {
                                await botClient.SendTextMessageAsync(Id, $"{e.Message.Chat.FirstName}, бот приветствует вас!\nЧто бы получить дополнительную информацию выберите /help\nЧто бы Вы хотели сделать?", replyMarkup: keyboard());

                                break;
                            }
                        case "/help":
                            {
                                await botClient.SendTextMessageAsync(Id, $"Список доступных функций: \n/latest - текущая информация о валютах;\n/byday - информация о валютах за любой выбраный вами день (!с 01.01.2010);\n/period - информация о продаже валют за любой период (!с 2017-01-01);\n/cinfo - общая информация о валюте;\n/convert - конвертер валют по их текущему курсу;\n/change - изменение цены валюты по последним обновлениям;\n---------------------\nЧто бы воспользоваться коммандами кликайте на названия комманд в этом сообщении, воспользуйтесь подсказками с картинки, воспользуйтесь клавиатурой или вписывайте через '/'.\n---------------------\nСписок доступных валют: EUR, USD,RUB.\nВсе данные отображаются по отношению к UAH.\n---------------------\nРазработчик: @n30_kl");

                                break;
                            }
                        case "/latest":
                            {
                                await botClient.SendTextMessageAsync(Id, $"Данные о продаже валют за {e.Message.Date.ToShortDateString()}\n\n" + Get($"https://currencyapibyn30kl.azurewebsites.net/Byday/date={e.Message.Date.ToShortDateString()}"));

                                break;
                            }
                        case "/byday":
                            {
                                Messages[0] = text;

                                await botClient.SendTextMessageAsync(Id, $"Введите дату (!Не раньше 01.01.2010) в формате ДД.ММ.ГГГГ за которую хотите получить данные. Например: {e.Message.Date.ToShortDateString()}");

                                break;
                            }
                        case "/period":
                            {
                                Messages[0] = text;

                                await botClient.SendTextMessageAsync(Id, "Введите дату с 2017 года в формате ГГГГ-ММ-ДД, от которой хотите получить данные, например: 2020-04-15");

                                break;

                            }
                        case "/cinfo":
                            {
                                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, $"Введите валюту в короткой форме, например: USD");

                                Messages[0] = text;

                                break;
                            }
                        case "/convert":
                            {
                                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, $"Введите валюту в короткой форме которую хотите конвертировать, например: USD");

                                Messages[0] = text;

                                break;
                            }
                        case "/change":
                            {
                                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, $"Введите валюту в короткой форме изменения о которой вы хотите узнать, например: USD");

                                Messages[0] = text;

                                break;
                            }
                    }
                }
                else
                {

                    try
                    {
                        if (Messages[0] == "/byday")
                        {
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, $"Данные за {text}:\n\n" + Get($"https://currencyapibyn30kl.azurewebsites.net/Byday/date={text}"));

                            await botClient.SendTextMessageAsync(Id, "Пожалуйста, выберите функцию бота:", replyMarkup: keyboard());

                            ClearMessages(Messages);
                        }
                        if (Messages[0] == "/change")
                        {
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, Get($"https://currencyapibyn30kl.azurewebsites.net/Change/symbol={text}"));

                            await botClient.SendTextMessageAsync(Id, "Пожалуйста, выберите функцию бота:", replyMarkup: keyboard());

                            ClearMessages(Messages);
                        }
                        if (Messages[0] == "/cinfo")
                        {
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, Get($"https://currencyapibyn30kl.azurewebsites.net/Cinfo/symbol={text}"));

                            await botClient.SendTextMessageAsync(Id, "Пожалуйста, выберите функцию бота:", replyMarkup: keyboard());

                            ClearMessages(Messages);
                        }
                        if (Messages[0] == "/convert")
                        {
                            if (Messages[1] != "" && Messages[2] != "")
                            {
                                await botClient.SendTextMessageAsync(e.Message.Chat.Id, Get($"https://currencyapibyn30kl.azurewebsites.net/Convert/pair1={Messages[1]}/pair2={Messages[2]}/amount={text}"));

                                await botClient.SendTextMessageAsync(Id, "Пожалуйста, выберите функцию бота:", replyMarkup: keyboard());

                                ClearMessages(Messages);
                            }
                            else
                            if (Messages[1] != "")
                            {
                                Messages[2] = text;

                                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, $"Введите суму, которую хотите конвертировать, например: 200");
                            }
                            else
                            if (text != "/convert")
                            {
                                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, $"Введите валюту в короткой форме в которую хотите конвертировать, например: EUR");

                                Messages[1] = text;
                            }
                        }
                        if (Messages[0] == "/period")
                        {
                            if (Messages[1] != "" && Messages[2] != "")
                            {
                                await botClient.SendTextMessageAsync(e.Message.Chat.Id, $"Данные за период с {Messages[1]} по {Messages[2]}:\n\n" + Get($"https://currencyapibyn30kl.azurewebsites.net/Period/symbol={text.ToUpper()}/from={Messages[1]}/to={Messages[2]}"));

                                await botClient.SendTextMessageAsync(Id, "Пожалуйста, выберите функцию бота:", replyMarkup: keyboard());

                                ClearMessages(Messages);
                            }
                            else
                            if (Messages[1] != "")
                            {
                                Messages[2] = text;

                                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, "Введите валюту в короткой форме, например: USD");
                            }
                            else
                            if (text != "/period")
                            {
                                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, $"Введите дату с 2017 года в формате ГГГГ-ММ-ДД, до которой хотите получить данные, например: 2020-04-19");

                                Messages[1] = text;
                            }
                        }
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(chatId: e.Message.Chat, "Ой, я не могу обработать ваш запрос(\nПожалуйста, проверьте данные и поробуйте еще раз или выберите другую функцию.", replyMarkup: keyboard());
                    }
                }

            }
            catch
            {
                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, "Упс! Кажется что то пошло не так!) \nПожалуйста, проверьте данные и поробуйте еще раз или выберите другую функцию.", replyMarkup: keyboard());
            }
        }
    }
}
