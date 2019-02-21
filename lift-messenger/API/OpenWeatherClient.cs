using lift_messenger.Library;
using lift_messenger.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using lift_messenger.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace lift_messenger.API
{
    public class OpenWeatherClient : IWeatherProvider
    {
        private readonly RestClient client;
        private readonly OpenWeatherSettings _settings;

        public OpenWeatherClient(IOptions<OpenWeatherSettings> settings)
        {
            _settings = settings.Value;
            client = new RestClient(_settings.Url);
        }

        public WeatherData GetWeatherDataInCity(string cityName)
        {
            var request = new RestRequest("data/2.5/weather?", Method.GET);

            request.AddQueryParameter("q", cityName);
            request.AddQueryParameter("APPID", _settings.Key);

            try
            {
                return client.Execute<WeatherData>(request).Data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            return null;
        }
    }
}
