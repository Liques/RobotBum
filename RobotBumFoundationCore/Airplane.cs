using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace RobotBumFoundationCore
{
    /// <summary>
    /// Status of action of an airplane
    /// </summary>
    public enum AirplaneStatus
    {
        DataImcomplete,
        ParkingOrTaxing,
        TakingOff,
        Landing,
        Cruise,
    }

    /// <summary>
    /// Type of airplane based on its weight
    /// </summary>
    public enum AirplaneWeight
    {
        NotSet,
        Heavy,
        Medium,
        Light
    }


    /// <summary>
    /// The main object to refers to a real airplane. It have the main properties of an airplane.
    /// </summary>
    public class Airplane
    {
        public string ID { get; set; }
        public AircraftRegistration Registration { get; set; }
        public AircraftType AircraftType { get; set; }
        public double Altitude { get; set; }
        public double VerticalSpeed { get; set; }
        public double Speed { get; set; }
        public double Direction { get; set; }
        public double DirectionChange { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string FlightName { get; set; }
        public Airline Airline { get; set; }
        public AirplaneStatus State { get; set; }
        public double FlightDistance { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime DateExpiration { get; set; }
        public AirplaneWeight Weight { get; set; }
        public bool IsSpecial { get; set; }
        public string SpecialDescription { get; set; }
        public string RunwayName { get; set; }

        public Airport From { get; set; }
        public Airport To { get; set; }
        public bool IsKnowCountry { get; set; }
        public bool IsOrbiting { get; set; }
        public bool IsTouchAndGo { get; set; }


        public string ForwardPlacesPhrase { get; set; }


        private static Dictionary<string, string> ListSpecialPainitngs = new Dictionary<string, string>();
        
        public override string ToString()
        {
            return this.FlightName + " _ " + this.Registration;
        }
        
        /// <summary>
        /// Get information of an airplane only using its hexcode
        /// </summary>
        protected class HexCodeAirplane
        {
            public bool IsValid { get; set; }
            public string HexCode { get; set; }
            public AircraftRegistration Registration { get; set; }
            public AircraftType AircraftType { get; set; }
            public String Description { get; set; }

            static List<HexCodeAirplane> ListHexCodes = new List<HexCodeAirplane>();

            public HexCodeAirplane(string _hexcode)
            {
                hexCodesString = File.OpenText(MultiOSFileSupport.ResourcesFolder + "knownairplanes.json").ReadToEnd();

                if (ListHexCodes.Count <= 0)
                {
                    var listNames = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, string>>>(hexCodesString);

                    if (listNames.ContainsKey(_hexcode))
                    {
                        var data = listNames[_hexcode];
                        this.HexCode = _hexcode;
                        this.Registration = new AircraftRegistration(data["reg"]);
                        this.AircraftType = (AircraftType)(data["model"]);

                        if(data.ContainsKey("desc"))
                            this.Description = data["desc"];


                        IsValid = true;
                    }
                }
            }

            string hexCodesString = String.Empty;
        }




    }


}
