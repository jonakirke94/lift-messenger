using lift_messenger.API;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using lift_messenger.Library;
using lift_messenger.Settings;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Hangfire;
using Hangfire.MemoryStorage;

namespace lift_messenger
{
    class Program
    {
        public static IConfigurationRoot configuration;
        private static IWeatherProvider weatherService;
        private static IMessageProvider messageService;

        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            weatherService = (IWeatherProvider)serviceProvider.GetService(typeof(IWeatherProvider));
            messageService = (IMessageProvider)serviceProvider.GetService(typeof(IMessageProvider));

            GlobalConfiguration.Configuration.UseMemoryStorage();

            // Cron explanation: Every weekday at 07:00
            RecurringJob.AddOrUpdate(() => RunNotifier(), "0 7 * * 1-5");

            using (new BackgroundJobServer())
            {
                Console.WriteLine("Hangfire Server started");
                Console.ReadLine();
            }
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
            services.AddTransient<IMessageProvider, Messenger>();

        }

        public static void RunNotifier()
        {
            Console.WriteLine("Running notifier at .. " + DateTime.Now);
            var data = weatherService.GetWeatherDataInCity("Aalborg");
            var weather = data.weather[0].main;
            Console.WriteLine("The weather in Aalborg is..." + weather);

            if (weatherService.IsBadWeather(weather))
            {
                Console.WriteLine("It's raining so sending a notification ...");
                var generatedMessage = messageService.GenerateMessage();
                messageService.SendMessage(generatedMessage).Wait();
            }
        }
    }
}
