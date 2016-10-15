using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TowerBotFoundationCore
{
    /// <summary>
    /// Registration of an airplane
    /// </summary>
    public class AircraftRegistration
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public bool IsValid { get; set; }

        public AircraftRegistration(string registration)
        {
            try
            {
                this.Name = registration;
                this.Country = GetCountryRegistration(registration);
                this.IsValid = !String.IsNullOrEmpty(registration);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Aircraft Registration error");
            }
        }

        /// <summary>
        /// Gets the country owner of the registration (Example: An airplane registred as "PR-GGF" is from Brazil because it starts with "PR-"
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        private static string GetCountryRegistration(string registration)
        {
            StreamReader file = File.OpenText(Directory.GetCurrentDirectory() + @"\Resources\aircraftregistration.json");
            
            var listCountires = JsonConvert.DeserializeObject<IDictionary<string, string>>(file.ReadToEnd());

            string country = String.Empty;
            var countryReg = listCountires.Keys.Where(s => registration.StartsWith(s)).FirstOrDefault();
            countryReg = (String.IsNullOrEmpty(countryReg)) ? "" : countryReg;

            if (listCountires.ContainsKey(countryReg))
            {
                country = listCountires[countryReg];
            }

            return country;

        }

        public override string ToString()
        {
            return this.Name;
        }

    }

    
}
