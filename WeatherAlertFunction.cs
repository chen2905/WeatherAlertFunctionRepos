using System.Net;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Functions.Worker.Extensions.Timer;
using TimerTriggerAttribute = Microsoft.Azure.Functions.Worker.TimerTriggerAttribute;
using TimerInfo = Microsoft.Azure.Functions.Worker.TimerInfo;

namespace WeatherAlertFunction
{
    public  class WeatherAlertFunction
    {
        private static readonly HttpClient client = new HttpClient(); // Declare HttpClient for API requests

        // Ensure the method is public and static

        [Function("WeatherAlertFunction")]
        public static async Task Run(
           [TimerTrigger("0 22 * * *")] TimerInfo myTimer,
          
            FunctionContext context)
        {
            var log = context.GetLogger("WeatherAlertFunction"); // Get the logger from FunctionContext


            //log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            // Fetch weather data
            string apiKey = "d6feb9c16098c3ff8559c13df41ed855"; // Directly insert your API key here
            string city = "sydney"; // Replace with your city
            string weatherApiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&appid={apiKey}";

            try
            {
                var response = await client.GetStringAsync(weatherApiUrl);

                // Parse the weather data
                dynamic weatherData = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
                double currentTemp = weatherData.main.temp;

               log.LogInformation($"Current temperature in {city}: {currentTemp} C");

                // Threshold for the weather alert
                double thresholdTemp = 15; // Temperature threshold, can be adjusted

                if (currentTemp > thresholdTemp)
                {
                    log.LogWarning("Too hot!!111");

                }
                else
                {
                    log.LogInformation("Temperature is within the acceptable range.");
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Failed to fetch weather data: {ex.Message}");
            }
        }
    }
}
