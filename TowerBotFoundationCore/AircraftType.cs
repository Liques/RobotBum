using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TowerBotFoundationCore
{
    public enum AircraftModel
    {
        NoModel = 0,
        AirplaneHeavy = 1,
        AirplaneMedium = 2,
        AirplaneLow = 3,
        Helicopter = 4
    }
    public class AircraftType
    {
        public string Name { get; set; }
        public string ICAO { get; set; }
        public bool IsValid { get; set; }
        public AircraftModel Type { get; set; }
        
        private AircraftType()
        {
          
        }

        public static implicit operator AircraftType(string icao)
        {
            return GetAircraftType(icao);
        }

        public static AircraftType GetAircraftType(string icao)
        {
            try
            {
                StreamReader file = File.OpenText(System.IO.Directory.GetCurrentDirectory() + @"/Resources/aircrafttypes.json");

                string jsonstring = file.ReadToEnd();

                var listNames = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, string>>>(jsonstring);

                string name = String.Empty;

                if (icao == null)
                    icao = string.Empty;

                var nameReg = listNames.Keys.Where(s => icao.StartsWith(s)).FirstOrDefault();
                nameReg = (String.IsNullOrEmpty(nameReg)) ? "" : nameReg;

                AircraftType aircraftType = new AircraftType();
                aircraftType.ICAO = icao;
                aircraftType.IsValid = false;

                if (listNames.ContainsKey(nameReg))
                {
                    aircraftType.Name = listNames[nameReg]["Name"];
                    aircraftType.Type = (AircraftModel)Enum.Parse(typeof(AircraftModel), listNames[nameReg]["Type"]);// listNames[nameReg]["Name"];
                    aircraftType.IsValid = true;

                    if (aircraftType.Type == AircraftModel.NoModel)
                        aircraftType.Type = AircraftModel.AirplaneLow;
                }
                else
                {
                    aircraftType.Name = aircraftType.ICAO;
                }

                return aircraftType;
            }
            catch (Exception e)
            {
                throw new ArgumentException(@"\Resources\aircrafttypes.jsonr");
            }

        }

        public override string ToString()
        {
            return this.Name;
        }

    }

    
}
