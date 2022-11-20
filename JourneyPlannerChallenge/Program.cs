using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JourneyPlannerClient.Model;
using JourneyPlannerClient.Service;

namespace JourneyPlannerClient
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        static async Task Main()
        {
            RegisterServices();

            var clientService = _serviceProvider.GetService<IJourneyPlannerService>();

            Console.WriteLine("TfL Journey Planner Client\r");
            Console.WriteLine("--------------------------\n");

            Console.WriteLine("Please enter a start and a destination station for a list of stations and lines that the train will go through to reach the required destination. It will also display any connections that will need to be made.");
            Console.WriteLine("Note that only a central subset of London Underground stations may be entered at this time.\n");
            Console.Write("From station: ");
            var fromStation = Console.ReadLine();

            if (string.IsNullOrEmpty(fromStation))
                throw new ArgumentException($"Error: The input station cannot be null. Please check the full list of supported Underground stations and try again.");

            var fromStationId = await clientService.GetStopPointFromApiAsync(fromStation, true);

            Console.Write("\nTo station: ");
            var toStation = Console.ReadLine();

            if (string.IsNullOrEmpty(toStation))
                throw new ArgumentException($"Error: The input station cannot be null. Please check the full list of supported Underground stations and try again.");

            var toStationId = await clientService.GetStopPointFromApiAsync(toStation, true);

            Console.Write("\nVia (optional): ");
            var viaStation = Console.ReadLine();

            var viaStationId = string.IsNullOrEmpty(viaStation) ? null : await clientService.GetStopPointFromApiAsync(viaStation, true);

            Console.Write("\nExcluding (optional): ");
            var excludingStation = Console.ReadLine();

            var excludingStationId = string.IsNullOrEmpty(excludingStation) ? null : await clientService.GetStopPointFromApiAsync(excludingStation, false);

            Console.WriteLine("--------------------------\n");

            await clientService.ReturnJourneyInformation(fromStationId, toStationId, viaStationId, excludingStationId);

            DisposeServices();
        }
        private static void RegisterServices()
        {
            var services = new ServiceCollection();
            services.AddScoped<IApiService, ApiService>();
            services.AddScoped<IJourneyPlannerService, JourneyPlannerService>();
            services.AddHttpClient();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var appSettings = new ApplicationConfiguration();
            configuration.Bind("ApplicationSettings", appSettings);
            services.AddSingleton(appSettings);

            _serviceProvider = services.BuildServiceProvider();
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}