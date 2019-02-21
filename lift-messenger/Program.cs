using lift_messenger.API;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using lift_messenger.Library;
using lift_messenger.Settings;

namespace lift_messenger
{
    class Program
    {
        public static IConfigurationRoot configuration;

        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            var service = (IWeatherProvider)serviceProvider.GetService(typeof(IWeatherProvider));
            var data = service.GetWeatherDataInCity("Aalborg");
            var weather = data.weather[0].main;
            Console.WriteLine("The weather in Aalborg is..." + weather);
            Console.ReadKey();

            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            services.Configure<OpenWeatherSettings>(settings =>
            {
                settings.Key = configuration.GetSection("OpenWeather")["Key"];
                settings.Url = configuration.GetSection("OpenWeather")["Url"];
            });

            services.AddSingleton(configuration);
            services.AddTransient<IWeatherProvider, OpenWeatherClient>();
            services.AddTransient<IMessageService, Messenger>();

        }
    }
}
