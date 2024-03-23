using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace YourNamespace
{
    class Program
    {
        static ITelegramBotClient _botClient;

        static ReceiverOptions _receiverOptions;

        async Task Main(string[] args)
        {
            string apiKey = "a17bccc339c9b0489cfa485dd5d5c387";
            string videoId = "dQw4w9WgXcQ"; //адрес видео

            string apiUrl = $"https://www.googleapis.com/youtube/v3/videos?part=snippet%2Cstatistics&id={videoId}&key={apiKey}";

            _botClient = new TelegramBotClient("YOUR_TELEGRAM_BOT_TOKEN");
            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                },

                ThrowPendingUpdates = true,
            };

            using var cts = new CancellationTokenSource();

            _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token);

            var me = await _botClient.GetMeAsync();
            Console.WriteLine($"{me.FirstName} запущен!");

            await Task.Delay(-1);
        }

        static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Обязательно ставим блок try-catch, чтобы наш бот не "падал" в случае каких-либо ошибок
            try
            {
                // Сразу же ставим конструкцию switch, чтобы обрабатывать приходящие Update
                switch (update.Type)
                {
                    case UpdateType.Message:
                        {
                            var message = update.Message;
                            var user = message.From;
                            var chat = message.Chat;

                            Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                            switch (message.Text)
                            {
                                case "/start":
                                    {
                                        var replyKeyboard = new ReplyKeyboardMarkup(
                                            new List<KeyboardButton[]>()
                                            {
                                                new KeyboardButton[]
                                                {
                                                    new KeyboardButton("пака"),
                                                    new KeyboardButton("привет"),
                                                }
                                            })
                                        {
                                            ResizeKeyboard = true,
                                        };
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Выбери клавиатуру:\n" +
                                            "/inline\n" +
                                            "/reply\n",
                                            replyMarkup: replyKeyboard);

                                        break;
                                    }
                                case "/reply":
                                    {
                                        var replyKeyboard = new ReplyKeyboardMarkup(
                                            new List<KeyboardButton[]>()
                                            {
                                                new KeyboardButton[]
                                                {
                                                    new KeyboardButton("пака"),
                                                    new KeyboardButton("привет"),
                                                }
                                            })
                                        {
                                            ResizeKeyboard = true,
                                        };
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Это reply клавиатура!",
                                            replyMarkup: replyKeyboard);

                                        break;
                                    }
                                default:
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Используй только текст!");
                                        break;
                                    }
                            }

                            break;
                        }
                }

                // После обработки сообщения выводим информацию о видео
                string apiKey = "a17bccc339c9b0489cfa485dd5d5c387";
                string videoId = "dQw4w9WgXcQ"; // адрес видео

                string apiUrl = $"https://www.googleapis.com/youtube/v3/videos?part=snippet%2Cstatistics&id={videoId}&key={apiKey}";

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(apiUrl);
                        response.EnsureSuccessStatusCode();

                        string responseBody = await response.Content.ReadAsStringAsync();
                        dynamic videoData = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);

                        if (videoData.items.Count > 0)
                        {
                            string title = videoData.items[0].snippet.title;
                            string channelTitle = videoData.items[0].snippet.channelTitle;
                            int viewCount = videoData.items[0].statistics.viewCount;
                            int likeCount = videoData.items[0].statistics.likeCount;

                            Console.WriteLine($"Информация о видео:");
                            Console.WriteLine($"Название: {title}");
                            Console.WriteLine($"Канал: {channelTitle}");
                            Console.WriteLine($"Количество просмотров: {viewCount}");
                            Console.WriteLine($"Количество лайков: {likeCount}");
                        }
                        else
                        {
                            Console.WriteLine("Видео не найдено.");
                        }
                    }
                    catch (HttpRequestException e)
                    {
                        Console.WriteLine($"Ошибка при отправке запроса: {e.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => error.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}