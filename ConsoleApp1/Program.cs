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




string apiKey = "AIzaSyDjfTO_wi4dwzmq1yHVVGwJW6ady9kV-LU";
string videoId = "MSJpsY9Smig"; // адрес видео

string videoApiUrl = $"https://www.googleapis.com/youtube/v3/videos?part=snippet%2Cstatistics&id={videoId}&key={apiKey}";

using (HttpClient client = new HttpClient())
{
    try
    {
        HttpResponseMessage videoResponse = await client.GetAsync(videoApiUrl);
        videoResponse.EnsureSuccessStatusCode();

        string videoResponseBody = await videoResponse.Content.ReadAsStringAsync();
        dynamic videoData = Newtonsoft.Json.JsonConvert.DeserializeObject(videoResponseBody);

        if (videoData.items.Count > 0)
        {
            string title = videoData.items[0].snippet.title;
            string channelTitle = videoData.items[0].snippet.channelTitle;
            int viewCount = videoData.items[0].statistics.viewCount;
            int likeCount = videoData.items[0].statistics.likeCount;
            string channelId = videoData.items[0].snippet.channelId;

            Console.WriteLine($"Информация о видео:");
            Console.WriteLine($"Название: {title}");
            Console.WriteLine($"Канал: {channelTitle}");
            Console.WriteLine($"Количество просмотров: {viewCount}");
            Console.WriteLine($"Количество лайков: {likeCount}");

            // Запрос информации о канале
            string channelApiUrl = $"https://www.googleapis.com/youtube/v3/channels?part=snippet%2Cstatistics&id={channelId}&key={apiKey}";
            HttpResponseMessage channelResponse = await client.GetAsync(channelApiUrl);
            channelResponse.EnsureSuccessStatusCode();

            string channelResponseBody = await channelResponse.Content.ReadAsStringAsync();
            dynamic channelData = Newtonsoft.Json.JsonConvert.DeserializeObject(channelResponseBody);

            if (channelData.items.Count > 0)
            {
                string channelName = channelData.items[0].snippet.title;
                string channelUsername = channelData.items[0].snippet.customUrl; // Никнейм, если указан
                string channelCreationDate = channelData.items[0].snippet.publishedAt;
                int subscriberCount = channelData.items[0].statistics.subscriberCount;
                int videoCount = channelData.items[0].statistics.videoCount;

                Console.WriteLine();
                Console.WriteLine($"Информация о канале:");
                Console.WriteLine($"Название канала: {channelName}");
                if (!string.IsNullOrEmpty(channelUsername))
                {
                    Console.WriteLine($"Никнейм канала: {channelUsername}");
                }
                Console.WriteLine($"Количество подписчиков: {subscriberCount}");
                Console.WriteLine($"Дата создания канала: {channelCreationDate}");
                Console.WriteLine($"Количество видео на канале: {videoCount}");
            }
            else
            {
                Console.WriteLine("Информация о канале не найдена.");
            }
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