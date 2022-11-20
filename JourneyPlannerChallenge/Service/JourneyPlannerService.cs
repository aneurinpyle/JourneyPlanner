using JourneyPlannerClient.Common;
using JourneyPlannerClient.Model;
using Newtonsoft.Json;
using System.Net;

namespace JourneyPlannerClient.Service
{
    public class JourneyPlannerService : IJourneyPlannerService
    {
        private readonly ApplicationConfiguration _appConfig;
        private readonly IApiService _tflJourneyPlannerApiService;

        public JourneyPlannerService(IApiService tflJourneyPlannerApiService, ApplicationConfiguration appConfig)
        {
            _appConfig = appConfig;
            _tflJourneyPlannerApiService = tflJourneyPlannerApiService;
        }

        public async Task ReturnJourneyInformation(string startStation, string destinationStation, string viaStation, string excludingStation)
        {
            try
            {
                var journeys = await GetJourneyFromApiAsync(startStation, destinationStation, viaStation, excludingStation);

                if (!journeys.Any())
                {
                    Console.WriteLine($"Your journey from {startStation} to {destinationStation}");
                    if (!string.IsNullOrEmpty(viaStation)) Console.WriteLine($"via {viaStation}");
                    if (!string.IsNullOrEmpty(excludingStation)) Console.WriteLine($"excluding {excludingStation}");
                    Console.WriteLine($"returned 0 results. Please try again with a different set of stations\n");
                    Environment.ExitCode = 0;
                    return;
                }

                Console.WriteLine($"Your journey from {startStation} to {destinationStation}");
                if (!string.IsNullOrEmpty(viaStation)) Console.WriteLine($"via {viaStation}");
                if (!string.IsNullOrEmpty(excludingStation)) Console.WriteLine($"excluding {excludingStation}");
                Console.WriteLine($"\n");

                foreach (var journey in journeys)
                {
                    Console.WriteLine($"Departing {startStation} at {journey.StartDateTime.TimeOfDay}");
                    foreach (var leg in journey.Legs)
                    {
                        Console.WriteLine($"\nFrom {leg.DeparturePoint.CommonName}, take the {leg.Instruction.Summary}\n");
                        Console.WriteLine($"Calling at:");
                        foreach (var stopPoint in leg.Path.StopPoints)
                        {
                            Console.WriteLine($"-{stopPoint.Name}");
                        }
                    }
                    Console.WriteLine($"\nArriving at {destinationStation} at {journey.ArrivalDateTime.TimeOfDay}\r");
                    Console.WriteLine($"--------------------------\n\n");

                }
                Environment.ExitCode = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.ExitCode = 1;
            }
        }

        public async Task<IEnumerable<Journeys>> GetJourneyFromApiAsync(string startStation, string destinationStation, string? viaStation, string? excludingStation)
        {
            var urlToUse = $"{_appConfig.TflJourneyPlannerApiUrl}/{startStation}/to/{destinationStation}";
            var queryParams = new Dictionary<string, string>
                {
                    {Constants.AppKey, _appConfig.TflApiKey},
                    {Constants.Modes, Constants.Tube},
                };

            if (!string.IsNullOrEmpty(viaStation)) queryParams.Add(Constants.Via, viaStation);

            var response = await _tflJourneyPlannerApiService.GetAsync(urlToUse, queryParams);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var journeyPlannerResponse = JsonConvert.DeserializeObject<JourneyPlannerApiResponse>(content);

                // For some reason, the query 'Mode' does not filter out the walking option when using 'tube'... so filtered out here instead.
                var tubeJourneys = journeyPlannerResponse.Journeys.Where(d => d.Legs.Any(c => c.Mode.Id.Contains(Constants.Tube))).ToList();

                if (!string.IsNullOrEmpty(excludingStation))
                {
                    tubeJourneys = tubeJourneys.Where(a => !a.Legs.Any(c => c.Path.StopPoints.Any(c => c.Id.Equals(excludingStation)))).ToList();
                }
                return tubeJourneys;
            }

            if (response.StatusCode == HttpStatusCode.TooManyRequests) // The API appears to return '429 - TooManyRequests' when the AppKey is invalid. Should return '401 Unauthorized'.
                throw new Exception("Error: Too many API requests and/or App Key supplied is invalid and may have expired and/or the API URL is invalid. Please check the status of your App Key and the API URL on the TfL API Developer Portal.");
            else
            {
                throw new Exception($"Error: API request failed. Reason: {response.ReasonPhrase}. ErrorCode: {(int)response.StatusCode}.");
            }
        }

        public async Task<string> GetStopPointFromApiAsync(string stationName)
        {
            var urlToUse = $"{_appConfig.TflStopPointApiUrl}";
            var queryParams = new Dictionary<string, string>
                {
                    {Constants.AppKey, _appConfig.TflApiKey},
                    {Constants.Query, stationName},
                    {Constants.Modes, Constants.Tube},
                };

            var response = await _tflJourneyPlannerApiService.GetAsync(urlToUse, queryParams);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var stopPointSearchResponse = JsonConvert.DeserializeObject<StopPointSearchApiResponse>(content);

                if (!stopPointSearchResponse.Matches.Any())
                    throw new ArgumentException($"Error: {stationName} is not a valid Underground station. Please check the full list of supported Underground stations and try again.");

                var firstResult = stopPointSearchResponse.Matches.FirstOrDefault();

                if (!Enum.IsDefined((ValidStations)firstResult.IcsId))
                    throw new ArgumentException($"Error: {stationName} is not part of this Journey Planner application yet. Please check the full list of supported Underground stations and try again.");

                return firstResult.Id;
            }

            if (response.StatusCode == HttpStatusCode.TooManyRequests) // The API appears to return '429 - TooManyRequests' when the AppKey is invalid. Should return '401 Unauthorized'.
                throw new Exception("Error: Too many API requests and/or App Key supplied is invalid and may have expired and/or the API URL is invalid. Please check the status of your App Key and the API URL on the TfL API Developer Portal.");
            else
            {
                throw new Exception($"Error: API request failed. Reason: {response.ReasonPhrase}. ErrorCode: {(int)response.StatusCode}.");
            }
        }
    }
}
