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
           
            radar.Plugins = new List<IPlugin>()
                {
                    new PluginUnknowAirplanes(false, false),
                    new PluginWide(),
                    new PluginRatification(false, false, false, false, true),
                };

                 radar.Plugins.ForEach(item => {
                item.Radar = radar;
            });



            listRadars.Add(radar);
        }
        
        public string HTMLServerFolder { get; set; }

        public int ApproximationMaxAltitude { get; set; }
        
        public bool ShowApproximationHeavyWeightAirplanes { get; set; }

        public bool ShowApproximationMediumWeightAirplanes { get; set; }

        public bool ShowApproximationLowWeightAirplanes { get; set; }


        [IgnoreDataMemberAttribute]
        public bool AvoidCommonTraffic { get; set; }

        public string TwitterConsumerKey { get; set; }
        public string TwitterConsumerSecret { get; set; }
        public string TwitterAccessToken { get; set; }
        public string TwitterAccessTokenSecret { get; set; }

        public Radar()
        {

            this.LastAirplaneListUpdate = new DateTime();
            CurrentAirplanes = new List<AirplaneBasic>();
            LastAirplanes = new List<AirplaneBasic>();
            ListRunways = new List<RunwayBasic>();
            Plugins = new List<IPlugin>();

            this.ShowApproximationHeavyWeightAirplanes = true;
            this.ShowApproximationMediumWeightAirplanes = true;
            this.ShowApproximationLowWeightAirplanes = true;
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
