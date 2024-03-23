
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

string apiKey = "a17bccc339c9b0489cfa485dd5d5c387";
string city = "Kazan";
string countryCode = "RU";

string apiUrl = $"http://api.openweathermap.org/data/2.5/forecast?q={city},{countryCode}&appid={apiKey}&units=metric";

using (HttpClient client = new HttpClient())
{
    try
    {
        HttpResponseMessage response = await client.GetAsync(apiUrl);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        dynamic forecastData = JsonConvert.DeserializeObject(responseBody);

        if (forecastData.list.Count > 0)
        {
            foreach (var forecast in forecastData.list)
            {
                long unixTime = forecast.dt;
                DateTime forecastTime = DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
                double temperature = forecast.main.temp;
                string description = forecast.weather[0].description;

                Console.WriteLine($"Прогноз на {forecastTime}:");
                Console.WriteLine($"Температура: {temperature} °C");
                Console.WriteLine($"Описание: {description}");
                Console.WriteLine();              
            }
        }
        else
        {
            Console.WriteLine("Прогноз не найден.");
        }
        Console.ReadKey();
    }

    catch (HttpRequestException e)
    {
        Console.WriteLine($"Ошибка при отправке запроса: {e.Message}");
    }
}