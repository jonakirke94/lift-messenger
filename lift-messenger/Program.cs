using lift_messenger.API;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using lift_messenger.Library;
using lift_messenger.Settings;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

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

            if (weather == "Rain")
            {
                var msg = (IMessageService)serviceProvider.GetService(typeof(IMessageService));
                var generatedMsg = msg.GenerateMessage();
                msg.SendMessage(generatedMsg).Wait();
            }

            // prevent program from halting        
            Console.ReadKey();
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

            services.Configure<AWSSettings>(settings =>
            {
                settings.Key = configuration.GetSection("AWS")["Key"];
                settings.SecretKey = configuration.GetSection("AWS")["SecretKey"];
                settings.Phone = configuration.GetSection("AWS")["Phone"];
            });

            services.AddSingleton(configuration);
            services.AddTransient<IWeatherProvider, OpenWeatherClient>();
            services.AddTransient<IMessageService, Messenger>();

        }
    }
}
