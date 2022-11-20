using JourneyPlannerClient.Model;

namespace JourneyPlannerClient.Service
{
    public interface IJourneyPlannerService
    {
        Task<IEnumerable<Journeys>> GetJourneyFromApiAsync(string startStation, string destinationStation, string? viaStation, string? excludingStation);
        Task<string> GetStopPointFromApiAsync(string stationName, bool icsCode);
        Task ReturnJourneyInformation(string startStation, string destinationStation, string? viaStation, string? excludingStation);
    }
}
