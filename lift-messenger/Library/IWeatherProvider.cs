using lift_messenger.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace lift_messenger.Library
{
    public interface IWeatherProvider
    {
        WeatherData GetWeatherDataInCity(string city);

        bool IsBadWeather(string weather);
    }
}

