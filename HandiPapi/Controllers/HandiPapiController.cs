using Microsoft.AspNetCore.Mvc;

namespace HandiPapi.Controllers
{
    // [ApiController]
    [Route("[controller]")]
    public class HandiPapiController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<HandiPapiController> _logger;

        public HandiPapiController(ILogger<HandiPapiController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetHandiPapi")]
        public IEnumerable<HandiPapi> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new HandiPapi
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}