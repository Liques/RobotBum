using System;
using System.Collections.Generic;
using TowerBotFoundationCore;
using System.Linq;

namespace TowerBotLibCore.Plugins
{
    // This class was not refectored, it is still having messages in Portuguese and, for a while, it's useless.
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
            ListPlacesGPS.Add(new Place {
                Name = "Sobradinho",
                LatitudeX = -15.627886,
                LongitudeX = -47.849836,
                LatitudeY = -15.673334,
                LongitudeY = -47.718972,
                Preposicao = "via"
            });
            ListPlacesGPS.Add(new Place
            {
                Name = "Plano Piloto",
                LatitudeX = -15.727922,
                LongitudeX = -47.934709,
                LatitudeY = -15.862051,
                LongitudeY = -47.776942,
                Preposicao = "pelo"
            });
            ListPlacesGPS.Add(new Place
            {
                Name = "Planaltina",
                LatitudeX = -15.594438,
                LongitudeX = -47.703498,
                LatitudeY = -15.678287,
                LongitudeY = -47.608233,
                Preposicao = "em"
            });
            ListPlacesGPS.Add(new Place
            {
                Name = "Ceilândia",
                LatitudeX = -15.789215,
                LongitudeX = -48.16899,
                LatitudeY = -15.884792,
                LongitudeY = -48.078958,
                Preposicao = "por"
            });
            ListPlacesGPS.Add(new Place
            {
                Name = "Taguatinga",
                LatitudeX = -15.79677,
                LongitudeX = -48.073006,
                LatitudeY = -15.877033,
                LongitudeY = -47.982245,
                Preposicao = "em"
            });
            ListPlacesGPS.Add(new Place
            {
                Name = "Águas Lindas",
                LatitudeX = -15.71214,
                LongitudeX = -48.32655,
                LatitudeY = -15.805314,
                LongitudeY = -48.235023,
                Preposicao = "por"
            });
            ListPlacesGPS.Add(new Place
            {
                Name = "Formosa",
                LatitudeX = -15.514172,
                LongitudeX = -47.377017,
                LatitudeY = -15.603774,
                LongitudeY = -47.25887,
                Preposicao = "em"
            });
            ListPlacesGPS.Add(new Place
            {
                Name = "Brasilinha",
                LatitudeX = -15.422749,
                LongitudeX = -47.651442,
                LatitudeY = -15.500408,
                LongitudeY = -47.569695,
                Preposicao = "por"
            });
            ListPlacesGPS.Add(new Place
            {
                Name = "São Sebastião",
                LatitudeX = -15.887909,
                LongitudeX = -47.797477,
                LatitudeY = -15.918150,
                LongitudeY = -47.746865,
                Preposicao = "por"
            });
            ListPlacesGPS.Add(new Place
            {
                Name = "Gama",
                LatitudeX = -15.997741,
                LongitudeX = -48.096109,
                LatitudeY = -16.046101,
                LongitudeY = -48.041297,
                Preposicao = "pelo"
            });

            ListPlacesAirport = new List<Place>();
            //ListPlacesAirport.Add(new Place
            //{
            //    Name = "taxiway/runway da 11L/29R",
            //    LatitudeX = -15.861345,
            //    LongitudeX = -47.928958,
            //    LatitudeY = -15.864729,
            //    LongitudeY = -47.895994
            //});
            //ListPlacesAirport.Add(new Place
            //{
            //    Name = "taxiway/runway da 11R/29L",
            //    LatitudeX = -15.87796,
            //    LongitudeX = -47.940352,
            //    LatitudeY = -15.880669,
            //    LongitudeY = -47.908061
            //});
            //ListPlacesAirport.Add(new Place
            //{
            //    Name = "Píer Norte",
            //    LatitudeX = -15.866753,
            //    LongitudeX = -47.921585,
            //    LatitudeY = -15.868673,
            //    LongitudeY = -47.917435
            //}); 
            //ListPlacesAirport.Add(new Place
            //{
            //    Name = "Píer da Tam (Sul)",
            //    LatitudeX = -15.871293,
            //    LongitudeX = -47.920388,
            //    LatitudeY = -15.872768,
            //    LongitudeY = -47.917225
            //});
            //ListPlacesAirport.Add(new Place
            //{
            //    Name = "Terminal 2",
            //    LatitudeX = -15.868314,
            //    LongitudeX = -47.932421,
            //    LatitudeY = -15.869462,
            //    LongitudeY = -47.929125
            //});
            //ListPlacesAirport.Add(new Place
            //{
            //    Name = "Setor de Hangares",
            //    LatitudeX = -15.866883,
            //    LongitudeX = -47.935997,
            //    LatitudeY = -15.867534,
            //    LongitudeY = -47.925671
            //});
            //ListPlacesAirport.Add(new Place
            //{
            //    Name = "Pátio Militar",
            //    LatitudeX = -15.864980,
            //    LongitudeX = -47.912157,
            //    LatitudeY = -15.867360,
            //    LongitudeY = -47.903930
            //});
            //ListPlacesAirport.Add(new Place
            //{
            //    Name = "Pátio de Cargas",
            //    LatitudeX = -15.868248,
            //    LongitudeX = -47.929150,
            //    LatitudeY = -15.869534,
            //    LongitudeY = -47.925483
            //});

            // CWB

            ListPlacesGPS.Add(new Place
            {
                Name = "Colombo",
                LatitudeX = -25.274900,
                LongitudeX = -49.252191,
                LatitudeY = -25.299343,
                LongitudeY = -49.201480,
                Preposicao = "por"
            });

            ListPlacesGPS.Add(new Place
            {
                Name = "Paranaguá",
                LatitudeX = -25.444053,
                LongitudeX = -48.61853,
                LatitudeY = -25.630267,
                LongitudeY = -48.427707,
                Preposicao = "por"
            });


            ListPlacesGPS.Add(new Place
            {
                Name = "Fazenda Grande",
                LatitudeX = -25.626838,
                LongitudeX = -49.352260,
                LatitudeY = -25.683625,
                LongitudeY = -49.281536,
                Preposicao = "por"
            });

            ListPlacesGPS.Add(new Place
            {
                Name = "Araucária",
                LatitudeX = -25.558229,
                LongitudeX = -49.449134,
                LatitudeY = -25.622551,
                LongitudeY = -49.367712,
                Preposicao = "pela"
            });

            ListPlacesGPS.Add(new Place
            {
                Name = "Itapoá",
                LatitudeX = -25.978133,
                LongitudeX = -48.652318,
                LatitudeY = -26.100472,
                LongitudeY = -48.559864,
                Preposicao = "por"
            });

            ListPlacesGPS.Add(new Place
            {
                Name = "centro",
                LatitudeX = -25.401515,
                LongitudeX = -49.318501,
                LatitudeY = -25.472238,
                LongitudeY = -49.201855,
                Preposicao = "pelo"
            });



        }

        public static List<Place> GetForwardLocations(AirplaneBasic airplane, bool isPassNear)
        {
            double lat = airplane.Latitude;// -16.052121;
            double lon = airplane.Longitude;// -47.767362;
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
            double lat = airplane.Latitude;// -16.052121;
            double lon = airplane.Longitude;// -47.767362;
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

        /// <summary>
        /// Mostra qual é o nível de alerta a partir do modelo do avião
        /// </summary>
        /// <param name="airplane">Avião</param>
        /// <param name="isImportantToSee">Fazemos questão de ver?</param>
        /// <param name="isAcceptedCommonAirplanes">Aceitamos aviões comuns que não pertecem uma linha aerea e que não são brasileiros</param>
        /// <returns></returns>
        public static PluginAlertType GetAlertByLevel(AirplaneBasic airplane, bool isImportantToSee, bool isAcceptedCommonAirplanes = false)
        {

            // Lista de aeronaves comuns que, se de empresas diferente, podem emitir alerta
            var listAircraftTypeCommon = HelperPlugin.ListCommonAirplanes;

            // Lista de aeronaves que estão alerta HIGH
            var listAircraftTypeHighAlert = HelperPlugin.ListWideAirplanes;

            // Lista de aeronaves que estão alerta SUPER HIGH
            var listAircraftTypeSuperHighAlert = HelperPlugin.ListSuperHighAirplanes;

            PluginAlertType alertType = PluginAlertType.Low;

            bool isHighAlert = listAircraftTypeHighAlert.Where(s => airplane.AircraftType != null && airplane.AircraftType.ICAO.Contains(s)).Count() > 0;
            bool isSuperHighAlert = listAircraftTypeSuperHighAlert.Where(s => airplane.AircraftType != null && airplane.AircraftType.ICAO.Contains(s)).Count() > 0;
            bool isCommonAirplane = listAircraftTypeCommon.Where(s => airplane.AircraftType != null && airplane.AircraftType.ICAO.Contains(s)).Count() > 0;

            if (isSuperHighAlert)
                alertType = PluginAlertType.High;
            else if (isCommonAirplane && isAcceptedCommonAirplanes)
                alertType = PluginAlertType.High;
            else if (isHighAlert && isImportantToSee)
                alertType = PluginAlertType.High;
            else if (isHighAlert && !isImportantToSee)
                alertType = PluginAlertType.Medium;
            else if (isAcceptedCommonAirplanes)
                alertType = PluginAlertType.High;

            return alertType;
        }

        public static PluginAlertType GetAlertByLevel(AirplaneBasic airplane, Radar radar, bool lightWeight, bool mediumWeight, bool heavyWeight, bool superHeavysAndRare)
        {

            // Lista de aeronaves que estão alerta HIGH
            var listAircraftTypeHighAlert = HelperPlugin.ListWideAirplanes;

            // Lista de aeronaves que estão alerta SUPER HIGH
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

            if(!radar.AvoidCommonTraffic && (airplane.State == AirplaneStatus.Landing || airplane.State == AirplaneStatus.TakingOff))
                alertType = PluginAlertType.High;


            return alertType;
        }

        public static bool IsAirplaneInApproximation(AirplaneBasic airplane, Radar radar)
        {
            return airplane.Altitude <= 28000;
        }
    }
}
