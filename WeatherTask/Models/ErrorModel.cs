using Microsoft.AspNetCore.Mvc;

namespace WeatherTask.Models
{
    public class ErrorModel
    {
        public int StatusCode { get; set; }
        public string Msg { get; set; }
    }
}
