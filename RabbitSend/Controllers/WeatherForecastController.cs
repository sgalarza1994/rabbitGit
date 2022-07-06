using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitSend.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitSend.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            AmqpService amqpService)
        {
            _logger = logger;
            AmqpService = amqpService;
        }

        public AmqpService AmqpService { get; }

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> Get()
        {



            var rng = new Random();
            var response = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            AmqpService.PublishMessage(response);
            return response;
        }
    }
}
