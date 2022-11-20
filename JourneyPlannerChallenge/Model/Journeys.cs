namespace JourneyPlannerClient.Model
{
    public class Journeys
    {
        public DateTime StartDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public Legs[] Legs { get; set; }
    }
}
