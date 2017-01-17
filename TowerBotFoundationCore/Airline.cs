using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace TowerBotFoundationCore
{
    /// <summary>
    /// Information about worldwide airlines
    /// </summary>
    public class Airline
    {
        public string Country { get; set; }
        public string Name { get; set; }
        public string IATA { get; set; }

        static private IDictionary<string, IDictionary<string, object>> listAirlines;

        public static Airline GetAirlineByFlight(string flight)
        {
            string iata = (!String.IsNullOrEmpty(flight) && flight.Length >=4) ? flight.Substring(0, 3) : flight;
            if (listAirlines == null)
            {
                try
                {
                    StreamReader file = File.OpenText(System.IO.Directory.GetCurrentDirectory() + @"/Resources/airlines.json");

                    string jsonstring = file.ReadToEnd();

                    listAirlines = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, object>>>(jsonstring);

                }
                catch (Exception e)
                {
                    throw new ArgumentException(@"\Resources\airlines.jsonr");
                }

            }

            if (iata == null)
                iata = String.Empty;

            if (listAirlines.ContainsKey(iata))
            {
                var selectedAirline = listAirlines[iata];
                return new Airline()
                {
                    IATA = iata,
                    Country = selectedAirline["Country"].ToString(),
                    Name = selectedAirline["FullName"].ToString(),
                };
            }
            else
            {
                return new Airline()
                {
                    IATA = iata,
                    Country = String.Empty,
                    Name = iata,
                };

            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
