using Microsoft.Extensions.Options;
using System.Text.Json;
using WeatherTask.Models;

namespace WeatherTask.Services
{
    public class WeatherService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _apiKey = configuration.GetSection("Keys:AccuWeatherAPI").Value;
            _baseUrl = configuration.GetSection("APIUrls:AccuWeatherAPI").Value;
            _httpClient = httpClient;
        }

        private async Task<string> GetLocationKeyAsync(string cityName)
        {
            var locationUrl = $"{_baseUrl}/locations/v1/cities/search?apikey={_apiKey}&q={cityName}";

            try
            {
                var response = await _httpClient.GetStringAsync(locationUrl);
                var location = JsonDocument.Parse(response).RootElement[0].GetProperty("Key").GetString();

                return location;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<WeatherModel> GetCurrentWeatherAsync(string cityName)
        {
            var locationKey = await GetLocationKeyAsync(cityName);

            if (locationKey == null) return null;

            var weatherUrl = $"{_baseUrl}/forecasts/v1/daily/1day/{locationKey}?apikey={_apiKey}&metric=true";
            var response = await _httpClient.GetStringAsync(weatherUrl);
            var weatherData = JsonDocument.Parse(response);

            var hasDailyPrecipitation = weatherData.RootElement.GetProperty("DailyForecasts")[0].GetProperty("Day")
                .GetProperty("HasPrecipitation").GetBoolean();
            var hasNightlyPrecipitation = weatherData.RootElement.GetProperty("DailyForecasts")[0].GetProperty("Night")
                .GetProperty("HasPrecipitation").GetBoolean();

            return new WeatherModel
            {
                City = cityName,
                MinTemperature = (float)weatherData.RootElement.GetProperty("DailyForecasts")[0].GetProperty("Temperature")
                    .GetProperty("Minimum").GetProperty("Value").GetDouble(),
                MaxTemperature = (float)weatherData.RootElement.GetProperty("DailyForecasts")[0].GetProperty("Temperature")
                    .GetProperty("Maximum").GetProperty("Value").GetDouble(),
                DailyPrecipitation = hasDailyPrecipitation ? weatherData.RootElement.GetProperty("DailyForecasts")[0]
                    .GetProperty("Day").GetProperty("PrecipitationIntensity").GetString() + " " + 
                    weatherData.RootElement.GetProperty("DailyForecasts")[0]
                    .GetProperty("Day").GetProperty("PrecipitationType").GetString() : null,
                NightlyPrecipitation = hasNightlyPrecipitation ? weatherData.RootElement.GetProperty("DailyForecasts")[0]
                    .GetProperty("Night").GetProperty("PrecipitationIntensity").GetString() + " " +
                    weatherData.RootElement.GetProperty("DailyForecasts")[0]
                    .GetProperty("Night").GetProperty("PrecipitationType").GetString() : null
            };
        }
    }
}
