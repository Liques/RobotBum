using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TowerBotFoundationCore;
using TowerBotLibCore.Plugins;

namespace TowerBotLibCore
{
    [Serializable]
    public class Radar
    {

        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string EndpointUrl { get; set; }
      
        public int Port { get; set; }
        [IgnoreDataMemberAttribute]
        public List<AirplaneBasic> CurrentAirplanes { get; set; }
        [IgnoreDataMemberAttribute]
        public List<Plugins.IPlugin> Plugins { get; set; }
        [IgnoreDataMemberAttribute]
        public List<AirplaneBasic> LastAirplanes { get; set; }
       
        public List<RunwayBasic> ListRunways { get; set; }
       
        public double LongitudeX { get; set; }
        
        public double LatitudeX { get; set; }
       
        public double LongitudeY { get; set; }
        
        public double LatitudeY { get; set; }

        public string RadarParentName { get; set; }

        [IgnoreDataMemberAttribute]
        public Radar RadarParent { get { return (Radar)RadarParentName; } }

        [IgnoreDataMemberAttribute]
        public DateTime LastAirplaneListUpdate { get; set; }

        [IgnoreDataMemberAttribute]
        public bool IsModeSAllowed { get { return this.RadarParent == null; } }

        [IgnoreDataMemberAttribute]
        public Airport MainAirport { get { return Airport.GetAirportByICAO(MainAirportICAO); } }

        public string MainAirportICAO { get; set; }

        [IgnoreDataMemberAttribute]
        private static List<Radar> listRadars = new List<Radar>();

        [IgnoreDataMemberAttribute]
        public static List<Radar> ListRadars
        {
            get
            {
                return listRadars;
            } 
        }

        public static void AddRadar(Radar radar) {
            listRadars.Add(radar);
        }
        
        public string HTMLServerFolder { get; set; }

        public int ApproximationMaxAltitude { get; set; }
        
        public bool ShowApproximationHeavyWeightAirplanes { get; set; }

        public bool ShowApproximationMediumWeightAirplanes { get; set; }

        public bool ShowApproximationLowWeightAirplanes { get; set; }


        [IgnoreDataMemberAttribute]
        public bool AvoidCommonTraffic { get; set; }
        
        public Radar()
        {
            this.LastAirplaneListUpdate = new DateTime();
            CurrentAirplanes = new List<AirplaneBasic>();
            LastAirplanes = new List<AirplaneBasic>();
            ListRunways = new List<RunwayBasic>();
            Plugins = new List<IPlugin>();
        }

        public static implicit operator Radar(string radarName)
        {
            return GetRadar(radarName);
        }

        public static explicit operator string(Radar radar)
        {
            return radar.Name;
        }


        public static List<Radar> GetRadarList(string radarName)
        {
            var radars = new List<Radar>();

            if (radarName == "GRU")
            {
                var gru = GetRadar("GRU");
                gru.ListRunways = new List<RunwayBasic>() {
                    new RunwayBasic()
                    {
                        NameSideOne = "09R",
                        NameSideTwo = "27L",
                        LatitudeSideOne = -23.438959,
                        LongitudeSideOne = -46.487564,
                        LatitudeSideTwo = -23.431038,
                        LongitudeSideTwo = -46.458305,
                    },
                    new RunwayBasic()
                    {
                        NameSideOne = "09L",
                        NameSideTwo = "27R",
                        LatitudeSideOne = -23.434273,
                        LongitudeSideOne = -46.483348,
                        LatitudeSideTwo = -23.424750,
                        LongitudeSideTwo = -46.448064,
                    }
                };
                radars.Add(gru);

            }

            if (radarName == "CGH")
            {
                var cgh = GetRadar("CGH");
                cgh.ListRunways = new List<RunwayBasic>() {
                    new RunwayBasic()
                    {
                        NameSideOne = "17R",
                        NameSideTwo = "35L",
                        LatitudeSideOne = -23.619963,
                        LongitudeSideOne = -46.661109,
                        LatitudeSideTwo = -23.634805,
                        LongitudeSideTwo = -46.650825,
                    }
                };
                radars.Add(cgh);

            }

            radars.Add(GetRadar("SAO"));


            if (radarName == "GRU" || radarName == "CGH" || radarName == "BGC" || radarName == "SSZ" || radarName == "SBMT")
                radars.Add(GetRadar("SAO"));

            if (radarName == "SDU" || radarName == "CIG" || radarName == "STU")
                radars.Add(GetRadar("RIO"));

            if (radarName == "CNF" || radarName == "PLU")
                radars.Add(GetRadar("BHZ"));

            if (radarName == "BFH")
                radars.Add(GetRadar("CWB"));

            if (radarName == "JJG")
                radars.Add(GetRadar("CCM"));

            radars.Add(GetRadar(radarName));
            return radars;
        }

        public static Radar GetRadar(string radarName)
        {

            var radar = ListRadars.Where(s => s.Name.ToLower() == radarName.ToLower()).FirstOrDefault();

            if (radar == null)
            {
                var airport = Airport.GetAirportByIata(radarName);

                if (airport == null)
                    return GetRadar("BRA");

                double radius = 0.2;

                if (radarName == "SWUZ" || radarName == "MAE" || radarName == "GRU" || radarName == "CGH" || radarName == "GIG" || radarName == "SDU" || radarName == "SBMT")
                {
                    radius = 0.04;
                }

                radar = new Radar()
                {
                    Name = radarName.ToUpper(),
                    Description = airport.City,
                    EndpointUrl = "",
                    LongitudeX = airport.Longitude - radius,
                    LatitudeX = airport.Latitude - radius,
                    LongitudeY = airport.Longitude + radius,
                    LatitudeY = airport.Latitude - radius,
                    RadarParentName = "BRA",
                    MainAirportICAO = airport.ICAO,

                };
                ListRadars.Add(radar);

            }

            return radar;
        }
        
    }
}
