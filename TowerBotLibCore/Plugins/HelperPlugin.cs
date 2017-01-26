using System;
using System.Collections.Generic;
using TowerBotFoundationCore;
using System.Linq;

namespace TowerBotLibCore.Plugins
{
    // This class was not refectored, it is still having messages in Portuguese and, for a while, most of the of this class is useless.
    public static class HelperPlugin
    {
        public static string[] ListSuperHighAirplanes { get; set; }
        public static string[] ListWideAirplanes { get; set; }
        public static string[] ListCommonAirplanes { get; set; }
        private static List<Place> ListPlacesGPS { get; set; }
        private static List<Place> ListPlacesAirport { get; set; }

        public class Place
        {
            public string Name { get; set; }
            public double LongitudeX { get; set; }
            public double LatitudeX { get; set; }
            public double LongitudeY { get; set; }
            public double LatitudeY { get; set; }
            public string Preposicao { get; set; }

            public override string ToString()
            {
                return this.Name;
            }
        }

        static HelperPlugin()
        {
            ListCommonAirplanes = new string[] { 
                "A31", // Airbus a31x
                "A32", // Airbus a32x
                "B73", // Boeing 73x
                "E17", // Embraer 17x
                "E19", // Embraer 19x
                "AT", // Todos ATRs
            };

            ListWideAirplanes = new string[] { 
                "A1", // Antonov 1x
                "A2", // Antonov 224, // Antonov 1x
                "AN", // Qualquer Antonov
                "A3ST", // Beluga
                "A38", // Airbus a380
                "A34", // Airbus a340
                "A33", // Airbus a330
                "A35", // Airbus a350
                "A30", // Airbus a300
                "B72", // Boeing 727
                "B74", // Boeing 747
                "B75", // Boeing 757
                "B77", // Boeing 777
                "B76", // Boeing 767
                "B78", // Boeing 787
                "DC", // Qualquer antigo DC
                "IL", // Qualquer Ilyushin
                "MD", // Qualquer MD
                "T1", // Tupolev 1x
                "T2", // Tupolev 2x
                "VC", // Airforce one
            };

            ListSuperHighAirplanes = new string[] { 
                "AN", // Antonov 224, // Antonov 1x
                "A2", // Antonov 224, // Antonov 1x
                "A30", // Antonov 224, // Antonov 1x
                "A3ST", // Beluga
                "A38", // Airbus a380
                "A35", // Airbus a350
                "B78", // Boeing 787
                "B74", // Boeing 747
                "DC", // Qualquer antigo DC
                "MD", // Qualquer MD
                "VC", // Airforce one

            };

            ListPlacesGPS = new List<Place>();

            // If you want to detect local places (example: neighborhoods), just insert here item to the ListPlacesGPS.

        }

        public static List<Place> GetForwardLocations(AirplaneBasic airplane, bool isPassNear)
        {
            double lat = airplane.Latitude;
            double lon = airplane.Longitude;
            double direction = airplane.Direction * Math.PI / 180;

            double vlat = (Math.Cos(direction) * 0.1);
            double vlon = (Math.Sin(direction) * 0.1);

            List<Place> locationsFound = new List<Place>();

            int distanceToCover = 100;
            distanceToCover = (isPassNear) ? 35 : distanceToCover;

            for (int i = 0; i < 100; i++)
            {
                double currentLat = lat + (vlat * i * 0.08);
                double currentLon = lon + (vlon * i * 0.08);

                List<Place> listPlacesOver = ListPlacesGPS.Where(s => s.LatitudeX > currentLat && s.LatitudeY < currentLat && s.LongitudeX < currentLon && s.LongitudeY > currentLon).ToList();

                if (listPlacesOver.Count > 0)
                {
                    bool hasFound = locationsFound.Where(s => s.Name == listPlacesOver.FirstOrDefault().Name).Count() > 0;
                    if (!hasFound)
                    {
                        locationsFound.Add(listPlacesOver.FirstOrDefault());
                    }
                }

            }

            return locationsFound;

        }

        public static string GetForwardLocationsPhrase(AirplaneBasic airplane, bool isPassNear, int maxLocations = 50)
        {
            try
            {
                string final = String.Empty;
                List<Place> locationsFound = GetForwardLocations(airplane, isPassNear);
                List<Place> locationsFoundPluginred = new List<Place>();

                // The "For" below was needed because a bug.
                maxLocations = (maxLocations > locationsFound.Count) ? locationsFound.Count : maxLocations;
                for (int i = 0; i < maxLocations; i++)
                {
                    locationsFoundPluginred.Add(locationsFound[i]);
                }

                for (int i = 0; i < locationsFoundPluginred.Count; i++)
                {
                    if (i == 0)
                    {
                        final += ", vai passar ";
                        if (!String.IsNullOrEmpty(locationsFoundPluginred[i].Preposicao))
                            final += locationsFoundPluginred[i].Preposicao;
                        else
                            final += "por";

                        final += " ";

                    }

                    final += locationsFoundPluginred[i];

                    if (locationsFoundPluginred.Count - 2 == i)
                        final += " e ";
                    else if (locationsFoundPluginred.Count - 3 >= i)
                        final += ", ";


                }

                return final;
            }
            catch (Exception e)
            {
                new ArgumentException("Problema ao faser a frase de forwardinLocations. Msg:" + e.Message);

                throw e;

            }

        }

        public static string GetOverLocation(AirplaneBasic airplane)
        {
            double lat = airplane.Latitude;
            double lon = airplane.Longitude;
            double direction = airplane.Direction * Math.PI / 180;

            List<string> locationsFound = new List<string>();

            double currentLat = lat;
            double currentLon = lon;

            if (airplane.State == AirplaneStatus.ParkingOrTaxing)
            {
                var listPlacesOverAirport = ListPlacesAirport.Where(s => s.LatitudeX > currentLat && s.LatitudeY < currentLat && s.LongitudeX < currentLon && s.LongitudeY > currentLon).ToList();

                if (listPlacesOverAirport.Count > 0)
                {
                    bool hasFound = locationsFound.Where(s => s == listPlacesOverAirport.FirstOrDefault().Name).Count() > 0;
                    if (!hasFound)
                    {
                        locationsFound.Add(listPlacesOverAirport.FirstOrDefault().Name);
                    }
                }
            }
            else
            {

                var listPlacesOver = ListPlacesGPS.Where(s => s.LatitudeX > currentLat && s.LatitudeY < currentLat && s.LongitudeX < currentLon && s.LongitudeY > currentLon).ToList();

                if (listPlacesOver.Count > 0)
                {
                    bool hasFound = locationsFound.Where(s => s == listPlacesOver.FirstOrDefault().Name).Count() > 0;
                    if (!hasFound)
                    {
                        locationsFound.Add(listPlacesOver.FirstOrDefault().Name);
                    }
                }
            }

            return locationsFound.FirstOrDefault();

        }

        public static PluginAlertType GetAlertByLevel(AirplaneBasic airplane, Radar radar, bool lightWeight, bool mediumWeight, bool heavyWeight, bool superHeavysAndRare)
        {

            var listAircraftTypeHighAlert = HelperPlugin.ListWideAirplanes;

            var listAircraftTypeSuperHighAlert = HelperPlugin.ListSuperHighAirplanes;

            PluginAlertType alertType = PluginAlertType.Low;

            bool isSuperHighAlert = listAircraftTypeSuperHighAlert.Where(s => airplane.AircraftType != null && airplane.AircraftType.ICAO.Contains(s)).Count() > 0
                                    && superHeavysAndRare;

            if (isSuperHighAlert)
                alertType = PluginAlertType.High;
            else if (heavyWeight && airplane.Weight == AirplaneWeight.Heavy ||
                        mediumWeight && airplane.Weight == AirplaneWeight.Medium ||
                        lightWeight && airplane.Weight == AirplaneWeight.Light)
                alertType = PluginAlertType.High;
            else if (!heavyWeight && airplane.Weight == AirplaneWeight.Heavy)
                alertType = PluginAlertType.Medium;

            return alertType;
        }

        public static bool IsAirplaneInApproximation(AirplaneBasic airplane, Radar radar)
        {
            return airplane.Altitude <= 28000;
        }
    }
}
