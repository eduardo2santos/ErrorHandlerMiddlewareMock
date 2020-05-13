using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ErrorHandlerMiddlewareMock.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                // correct line
                //Summary = Summaries[rng.Next(Summaries.Length)]
                // wrong line setting an exception
                Summary = Summaries[9999]   
            })
            .ToArray();                       
        }

        [HttpGet]
        [Route("500error")]
        public Task<IActionResult> Error500()
        {

            throw new Exception("An error 500-Internal Server has occurred here.");
                        
        }

        [HttpGet]
        [Route("404error")]
        public Task<IActionResult> Error404()
        {

            throw new Exception("An error 404-Not Found has occurred here.");

        }

        [HttpGet]
        [Route("403error")]
        public Task<IActionResult> Error403()
        {

            throw new Exception("An error 403-Forbiden has occurred here.");

        }
    }
}
