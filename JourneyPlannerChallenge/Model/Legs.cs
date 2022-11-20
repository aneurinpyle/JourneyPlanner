namespace JourneyPlannerClient.Model
{
    public class Legs
    {
        public Instruction Instruction { get; set; }
        public Point DeparturePoint { get; set; }
        public Point ArrivalPoint { get; set; }
        public Path Path { get; set; }
        public RouteOptions[] RouteOptions { get; set; }
        public Mode Mode { get; set; }
    }
}
