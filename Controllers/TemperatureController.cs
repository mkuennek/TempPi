using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TempPi.Configuration;

namespace TempPi
{
    [Route("api/{controller}")]
    public class TemperatureController : ControllerBase
    {
        private readonly IOptions<SensorMappings> sensorMappingsConfig;

        private readonly Regex temperatureRegex = new Regex(@"t=(?<temp>\d+)", RegexOptions.Compiled);

        public TemperatureController(IOptions<SensorMappings> sensorMappingsConfig)
        {
            this.sensorMappingsConfig = sensorMappingsConfig;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetTemperatureForSensor(string name)
        {
            var mapping = sensorMappingsConfig.Value.Mappings.FirstOrDefault(mapping => mapping.Name == name);
            if (mapping == default)
            {
                return NotFound();
            }

            var path = mapping.Path;
            var fileContent = await System.IO.File.ReadAllTextAsync(path);

            var match = temperatureRegex.Match(fileContent);
            if (!match.Success)
            {
                return StatusCode(500);
            }

            var tempString = match.Groups["temp"].Value;
            if (!double.TryParse(tempString, out double tempDouble))
            {
                return StatusCode(500);
            }

            tempDouble /= 1000d;

            return new OkObjectResult(tempDouble);
        }
    }
}