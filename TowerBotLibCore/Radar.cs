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
        public bool IsTwitterEnabled { get { return 
            !String.IsNullOrEmpty(this.TwitterAccessToken) &&
            !String.IsNullOrEmpty(this.TwitterAccessTokenSecret) &&
            !String.IsNullOrEmpty(this.TwitterConsumerKey) &&
            !String.IsNullOrEmpty(this.TwitterConsumerSecret)
        ; 
        } }

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
        
        [IgnoreDataMemberAttribute]
        public bool ShowAllApproximationHeavyWeightAirplanes { get; set; }

        [IgnoreDataMemberAttribute]
        public bool ShowAllApproximationMediumWeightAirplanes { get; set; }

        [IgnoreDataMemberAttribute]
        public bool ShowAllApproximationLowWeightAirplanes { get; set; }


         [IgnoreDataMemberAttribute]
        public bool AvoidAllApproximationHeavyWeightAirplanes { get; set; }

        [IgnoreDataMemberAttribute]
        public bool AvoidAllApproximationMediumWeightAirplanes { get; set; }

        [IgnoreDataMemberAttribute]
        public bool AvoidAllApproximationLowWeightAirplanes { get; set; }


        [IgnoreDataMemberAttribute]
        public List<string> AvoidAllFlightsStartingWith { get; set; }
        [IgnoreDataMemberAttribute]
        public List<string> ShowAllFlightStartingWith { get; set; }
        [IgnoreDataMemberAttribute]
        public List<string> AvoidAllModelsStartingWith { get; set; }
        [IgnoreDataMemberAttribute]
        public List<string> ShowAllModelsStartingWith { get; set; }
        [IgnoreDataMemberAttribute]
        public bool ShowHelicopters { get; set; }
        [IgnoreDataMemberAttribute]
        public bool ShowAllCruisesOnlyOnServer { get; set; }
        [IgnoreDataMemberAttribute]
        public string TwitterConsumerKey { get; set; }
        [IgnoreDataMemberAttribute]
        public string TwitterConsumerSecret { get; set; }
        [IgnoreDataMemberAttribute]
        public string TwitterAccessToken { get; set; }
        [IgnoreDataMemberAttribute]
        public string TwitterAccessTokenSecret { get; set; }

        public Radar()
        {

            this.LastAirplaneListUpdate = new DateTime();
            CurrentAirplanes = new List<AirplaneBasic>();
            LastAirplanes = new List<AirplaneBasic>();
            ListRunways = new List<RunwayBasic>();
            Plugins = new List<IPlugin>();

            this.ShowHelicopters = true;
            
            AvoidAllFlightsStartingWith = new List<string>();
            ShowAllFlightStartingWith = new List<string>();
            AvoidAllModelsStartingWith = new List<string>();
            ShowAllModelsStartingWith = new List<string>();

            this.Plugins = new List<IPlugin>()
                {
                    //new PluginUnknowAirplanes(false, false),
                    //new PluginWide(),
                    new PluginRatification(false, false, false, false, true),
                    new PluginFilterAlerts(),
                };

            this.Plugins.ForEach(item => {
                item.Radar = this;
            });

        }

        public static implicit operator Radar(string radarName)
        {
            return GetRadar(radarName);
        }

        public static explicit operator string(Radar radar)
        {
            return radar.Name;
        }

        public static Radar GetRadar(string radarName)
        {

            var radar = ListRadars.Where(s => s.Name.ToLower() == radarName.ToLower()).FirstOrDefault();

            return radar;
        }
        
    }
}
