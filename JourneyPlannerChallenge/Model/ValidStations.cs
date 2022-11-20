using System.ComponentModel;

namespace JourneyPlannerClient.Model
{
    public enum ValidStations
    {
        // using ICS Id to verify valid stations

        [Description("Bank")]
        Bank = 1000013,
        [Description("Blackfriars")]
        Blackfriars = 1000023,
        [Description("Cannon Street")]
        CannonStreet = 1000040,
        [Description("Chancery Lane")]
        ChanceryLane = 1000044,
        [Description("Charing Cross")]
        CharingCross = 1000045,
        [Description("Covent Garden")]
        CoventGarden = 1000056,
        [Description("Embankment")]
        Embankment = 1000075,
        [Description("Holborn")]
        Holborn = 1000112,
        [Description("Leicester Square")]
        LeicesterSquare = 1000135,
        [Description("Mansion House")]
        MansionHouse = 1000143,
        [Description("Monument")]
        Monument = 1000148,
        [Description("St. Paul's")]
        StPauls = 1000225,
        [Description("Temple")]
        Temple = 1000231,
        [Description("Tottenham Court Road")]
        TottenhamCourtRoad = 1000235,
    }
}
