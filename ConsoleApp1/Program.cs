using System;
using System.Net.Http;
using System.Threading.Tasks;

string apiKey = "AIzaSyDjfTO_wi4dwzmq1yHVVGwJW6ady9kV-LU";
string videoId = "tWYsfOSY9vY"; //адрес видео

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