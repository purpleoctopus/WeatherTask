namespace WeatherTask.Models
{
    public class WeatherModel
    {
        public string City { get; set; }
        public string? DailyPrecipitation { get; set; }
        public string? NightlyPrecipitation { get; set; }
        public float MinTemperature { get; set; }
        public float MaxTemperature { get; set; }
    }
}
