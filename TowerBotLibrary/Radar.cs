using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TowerBotFoundation;
using TowerBotLibrary.Plugins;

namespace TowerBotLibrary
{
    [Serializable]
    public class Radar
    {

        public string Name { get; set; }
        [IgnoreDataMemberAttribute]
        public string Description { get; set; }
        [IgnoreDataMemberAttribute]
        public string EndpointUrl { get; set; }
      
        [IgnoreDataMemberAttribute]
        public int Port { get; set; }
        [IgnoreDataMemberAttribute]
        public List<AirplaneBasic> CurrentAirplanes { get; set; }
        [IgnoreDataMemberAttribute]
        public List<Plugins.IPlugin> Plugins { get; set; }
        [IgnoreDataMemberAttribute]
        public List<AirplaneBasic> LastAirplanes { get; set; }
        [IgnoreDataMemberAttribute]
        public List<RunwayBasic> ListRunways { get; set; }
        [IgnoreDataMemberAttribute]

        public double LongitudeX { get; set; }
        [IgnoreDataMemberAttribute]

        public double LatitudeX { get; set; }
        [IgnoreDataMemberAttribute]

        public double LongitudeY { get; set; }
        [IgnoreDataMemberAttribute]

        public double LatitudeY { get; set; }
        [IgnoreDataMemberAttribute]

        public Radar RadarParent { get; set; }
        [IgnoreDataMemberAttribute]
        public DateTime LastAirplaneListUpdate { get; set; }
        [IgnoreDataMemberAttribute]

        public bool ModeSAllowed { get; set; }

        [IgnoreDataMemberAttribute]
        public Airport MainAirport { get; set; }


        [IgnoreDataMemberAttribute]
        public bool IsModeSEnabled { get; set; }

        [IgnoreDataMemberAttribute]

        private static List<Radar> listRadars { get; set; }
        [IgnoreDataMemberAttribute]
        public static List<Radar> ListRadars
        {
            get
            {
                if (listRadars == null)
                    LoadRadars();

                return listRadars;
            }
        }
        
        [IgnoreDataMemberAttribute]
        public bool HasTwitter { get; set; }

        [IgnoreDataMemberAttribute]
        public int AltitudeOfTolerence
        {
            get
            {
                if (this.Name == "BSB" ||
                            this.Name == "SAO" ||
                            this.Name == "RIO" ||
                            this.Name == "VCP" ||
                            this.Name == "BHZ" ||
                            this.Name == "SSA" ||
                            this.Name == "POA" ||
                            this.Name == "CWB" ||
                            this.Name == "FOR" ||
                            this.Name == "REC")
                    return 28000;
                else {
                    return this.MainAirport != null ? this.MainAirport.Altitude + 5000 : 5000;
                }
            }
        }


        [IgnoreDataMemberAttribute]
        public bool IsWideAllowed
        {
            get
            {
                if (this.Name == "BRA" ||
                              this.Name == "BSB" ||
                              this.Name == "SAO" ||
                              this.Name == "GRU" ||
                              this.Name == "MAO" ||
                              this.Name == "RIO" ||
                              this.Name == "VCP" ||
                              this.Name == "CWB" ||
                              this.Name == "FOR" ||
                              this.Name == "BHZ" ||
                              this.Name == "PNZ" ||
                              this.Name == "POA")
                    return true;
                else
                    return false;
            }
        }

        [IgnoreDataMemberAttribute]
        public bool IsMediusNotAllowed
        {
            get
            {
                if (this.Name == "SWUZ" ||
                              this.Name == "SBMT" ||

                            this.Name == "BFH"
                              )
                    return true;
                else
                    return false;
            }
        }


        [IgnoreDataMemberAttribute]
        public bool AvoidCommonTraffic
        {
            get
            {
                if (this.Name == "BSB" ||
                            this.Name == "SAO" ||
                            this.Name == "RIO" ||
                            this.Name == "GRU" ||
                            this.Name == "CGH" ||
                            this.Name == "SDU" ||
                            this.Name == "GIG" ||
                            this.Name == "GRU" ||
                            this.Name == "VCP" ||
                            this.Name == "BHZ" ||
                            this.Name == "SSA" ||
                            this.Name == "POA" ||
                            this.Name == "CWB" ||
                            this.Name == "MAO" ||
                            this.Name == "REC" ||

                            this.Name == "BFH" ||
                            this.Name == "SWUZ" ||
                              this.Name == "SBMT")
                    return true;
                else
                    return false;
            }
        }

        public static Radar GetAnyRadar()
        {
            return Radar.ListRadars[new Random().Next(Radar.ListRadars.Count)];
        }

        private Radar()
        {
            this.LastAirplaneListUpdate = new DateTime();
            CurrentAirplanes = new List<AirplaneBasic>();
            LastAirplanes = new List<AirplaneBasic>();
            this.ModeSAllowed = true;
            ListRunways = new List<RunwayBasic>();
            Plugins = new List<IPlugin>();
            //CurrentAlerts = new List<Alert>();
        }

        private static void LoadRadars()
        {
            listRadars = new List<Radar>();

            listRadars.Add(new Radar()
            {
                Name = "BRA",
                Description = "Brasil afora",
                EndpointUrl = "http://162.243.32.213:8088/json",
                Plugins = new List<IPlugin>()
                {
                    new PluginLogAll(),
                    new PluginUnknowAirplanes(false, false),
                    new PluginWide(),
                    new PluginRatification(false, false, false, false, true)
                },
            });
            listRadars.Add(new Radar()
            {
                Name = "BSB",
                Description = "Brasília - DF",
                MainAirport = Airport.GetAirportByIata("BSB"),
                HasTwitter = true,
                IsModeSEnabled = true,                
                EndpointUrl = "http://bsbradar.ddns.net:8081/json",
                LongitudeX = -48.336099,
                LatitudeX = -15.364184,
                LongitudeY = -47.256692,
                LatitudeY = -16.194103,
                RadarParent = Radar.GetRadar("BRA"),
                Plugins = new List<IPlugin>()
                {
                    new PluginLogAll(),
                    new PluginUnknowAirplanes(false, false),
                    new PluginWide(),
                    new PluginBackingOrGo(),
                    new PluginRatification(false, false, false, false, true),
                    new PluginMetarDF(),
                    new PluginAlertAll(),
                },
                ListRunways = new List<RunwayBasic>() {
                    new RunwayBasic()
                    {
                        NameSideOne = "11L",
                        NameSideTwo = "29R",
                        LatitudeSideOne = -15.861333,
                        LongitudeSideOne = -47.930333,
                        LatitudeSideTwo = -15.86,
                        LongitudeSideTwo = -47.898167,
                    },
                    new RunwayBasic()
                    {
                        NameSideOne = "11R",
                        NameSideTwo = "29L",
                        LatitudeSideOne = -15.879167,
                        LongitudeSideOne = -47.942,
                        LatitudeSideTwo = -15.8765,
                        LongitudeSideTwo = -47.9085,
                    }
                }

            });
            listRadars.Add(new Radar()
            {
                Name = "CWB",
                Description = "Curitiba - PR",
                EndpointUrl = "http://tmapicuritiba.no-ip.org:8088/json",
                LongitudeX = -49.617468,
                LatitudeX = -25.160087,
                LongitudeY = -48.826141,
                LatitudeY = -25.758437,
                RadarParent = Radar.GetRadar("BRA"),
                MainAirport = Airport.GetAirportByIata("CWB"),
                ListRunways = new List<RunwayBasic>() {
                    new RunwayBasic()
                    {
                        NameSideOne = "SBCT 15",
                        NameSideTwo = "SBCT 33",
                        LatitudeSideOne = -25.521722,
                        LongitudeSideOne = -49.183325,
                        LatitudeSideTwo = -25.536495,
                        LongitudeSideTwo = -49.166800,
                    },
                    new RunwayBasic()
                    {
                        NameSideOne = "SBCT 11",
                        NameSideTwo = "SBCT 29",
                        LatitudeSideOne = -25.528277,
                        LongitudeSideOne = -49.180183,
                        LatitudeSideTwo = -25.528770,
                        LongitudeSideTwo = -49.162260,
                    },
                    new RunwayBasic()
                    {
                        NameSideOne = "Bacacheri 18",
                        NameSideTwo = "Bacacheri 36",
                        LatitudeSideOne = -25.399246,
                        LongitudeSideOne = -49.234622,
                        LatitudeSideTwo = -25.410835,
                        LongitudeSideTwo = -49.229407,
                    }
                }
            });
            listRadars.Add(new Radar()
            {
                Name = "SAO",
                Description = "Grande São Paulo - SP",
                EndpointUrl = "",
                LongitudeX = -46.971893,
                LatitudeX = -23.371846,
                LongitudeY = -46.130631,
                LatitudeY = -23.834366,
                RadarParent = Radar.GetRadar("BRA"),
                MainAirport = Airport.GetAirportByIata("GRU"),
                ListRunways = new List<RunwayBasic>() {
                    new RunwayBasic()
                    {
                        NameSideOne = "GRU 09R",
                        NameSideTwo = "GRU 27L",
                        LatitudeSideOne = -23.438959,
                        LongitudeSideOne = -46.487564,
                        LatitudeSideTwo = -23.431038,
                        LongitudeSideTwo = -46.458305,
                    },
                    new RunwayBasic()
                    {
                        NameSideOne = "GRU 09L",
                        NameSideTwo = "GRU 27R",
                        LatitudeSideOne = -23.434273,
                        LongitudeSideOne = -46.483348,
                        LatitudeSideTwo = -23.424750,
                        LongitudeSideTwo = -46.448064,
                    },
                    new RunwayBasic()
                    {
                        NameSideOne = "CGH 17R",
                        NameSideTwo = "CGH 35L",
                        LatitudeSideOne = -23.619963,
                        LongitudeSideOne = -46.661109,
                        LatitudeSideTwo = -23.634805,
                        LongitudeSideTwo = -46.650825,
                    }
                }
            });

            listRadars.Add(new Radar()
            {
                Name = "RIO",
                Description = "Rio de Janeiro - RJ",
                EndpointUrl = "",
                LongitudeX = -43.86997,
                LatitudeX = -22.596812,
                LongitudeY = -42.618204,
                LatitudeY = -23.221517,
                RadarParent = Radar.GetRadar("BRA"),

            });
            

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
                    RadarParent = Radar.GetRadar("BRA"),
                    MainAirport = airport,

                };
                ListRadars.Add(radar);

            }

            return radar;
        }
        
    }
}
