using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using WeatherTask.Models;
using WeatherTask.Services;

namespace WeatherTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WeatherService _weatherService;

        public HomeController(ILogger<HomeController> logger, WeatherService weatherService)
        {
            _logger = logger;
            _weatherService = weatherService;
        }

        public IActionResult Index(WeatherModel model)
        {
            if(model == null)
            {
                return View(new WeatherModel());
            }
            return View(new WeatherModel());
        }

        public async Task<IActionResult> GetWeather(string City)
        {
            try
            {
                var weather = await _weatherService.GetCurrentWeatherAsync(City);

                if (weather == null)
                {
                    return RedirectToAction("Error", new { statusCode = 404, Msg = "City is not found" });
                }

                return View("Index", weather);
            }
            catch(HttpRequestException ex) when ((int)ex.StatusCode == 503)
            {
                return RedirectToAction("Error", new
                {
                    statusCode = (int)ex.StatusCode,
                    Msg = "Looks like AccuWeather API token expired."
                });
            }
            catch (HttpRequestException ex)
            {
                return RedirectToAction("Error", new { statusCode = (int)ex.StatusCode });
            }
        }

        public IActionResult Error(int statusCode) {
            return View(new ErrorModel() { StatusCode = statusCode});
        }
    }
}
