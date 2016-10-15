using TowerBotLibCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerBotFoundationCore;

namespace TowerBotLibCore
{
    public static class FiltersManager
    {
        public static List<AirplaneBasic> ListOldAirplanesDF { get; set; }
        private static List<Airport> listMainAirports = new List<Airport>();
        public static List<Airport> ListMainAirports { get { return listMainAirports; } }

        public static DateTime TimeNext { get; set; }
        public static TimeSpan Period { get; set; }
        public static List<AlertFilter> listOldAlerts { get; set; }

        private static List<Radar> listRadars { get; set; }

        static FiltersManager()
        {

            listMainAirports = Airport.ListAirports.Where(s => s.Value["ICAO"].ToString().StartsWith("SB")).Select(s => Airport.GetAirportByIata(s.Key)).ToList();
            listMainAirports.Add(Airport.GetAirportByIata("SWUZ"));

            ListOldAirplanesDF = new List<AirplaneBasic>();
            Period = new TimeSpan(0, 0, 15);
            listOldAlerts = new List<AlertFilter>();

            listRadars = new List<Radar>()
            {
                Radar.GetRadar("BRA"),
                Radar.GetRadar("BSB"),
            };

            foreach (var radar in listRadars)
            {
                foreach (var filter in radar.Filters)
                {
                    filter.Radar = radar;
                }
            }
        }

        public static List<AlertFilter> GetAlerts(bool updateAll)
        {
            List<AlertFilter> listAlerts = new List<AlertFilter>();

            if (TimeNext == null || TimeNext <= DateTime.Now || updateAll)
            {

                for (int i = 0; i < listRadars.Count; i++)
                {
                    var radar = listRadars[i];

                    List<AirplaneBasic> listAirplanes = null;
                    if (radar != null)
                    {

                        listAirplanes = AirplanesData.GetAirplanes(radar).Result;
                        var newAlerts = Run(radar, listAirplanes);

                        var listToDelete = new List<AlertFilter>();


                        if (radar.Name == "BRA")
                        {
                            var listOfAirports = new List<Airport>();

                            Airport airport = null;

                            if (newAlerts.Count > 0)
                            {
                                listOfAirports = Airport.ListAirports.Where(s => s.Value["ICAO"].ToString().StartsWith("SB")).Select(s => Airport.GetAirportByIata(s.Key)).ToList();
                            }


                            foreach (var item in newAlerts)
                            {

                                if (AlertFilter.ListOfRecentAlerts != null && AlertFilter.ListOfRecentAlerts.Any(a => a.ID == item.ID))
                                {
                                    listToDelete.Add(item);
                                    continue;
                                }

                                if (AlertFilter.ListOfRecentAlerts != null && AlertFilter.ListOfRecentAlerts.Where(w => w.TimeCreated > DateTime.Now.AddMinutes(-15) && w.TimeCreated < DateTime.Now.AddMinutes(-2)).Any(a => a.AirplaneID == item.AirplaneID && item.Icon == a.Icon))
                                {
                                    listToDelete.Add(item);
                                    continue;
                                }

                            }
                        }

                        listToDelete.ForEach(item => newAlerts.Remove(item));

                        listAlerts.AddRange(newAlerts);

                    }


                    ListOldAirplanesDF = listAirplanes;

                }

                TimeNext = DateTime.Now + Period;

            }

            return listAlerts;


        }

        private static List<AlertFilter> Run(Radar radar, object parameter = null)
        {

            List<AlertFilter> listAlerts = new List<AlertFilter>();

            for (int i = 0; i < radar.Filters.Count; i++)
            {
                listAlerts.AddRange(radar.Filters[i].Analyser(parameter));
            }

            // Verify if there are some alert older then it's time to be removed
            if (listAlerts.Count > 0)
            {
                List<AlertFilter> list = listOldAlerts.Where(s => s.TimeToBeRemoved <= DateTime.Now).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    listOldAlerts.Remove(list[i]);
                }
            }

            // Verify if there is any alert equal.
            List<AlertFilter> listAlertLessThenOneHour = listOldAlerts;
            if (radar.Name == "BRA")
                listAlertLessThenOneHour = listOldAlerts.Where(s => s.TimeCreated > DateTime.Now.AddHours(-1)).ToList();

            for (int i = 0; i < listAlertLessThenOneHour.Count; i++)
            {
                var alertEqual = listAlerts.Where(s => s.ID == listAlertLessThenOneHour[i].ID && s.AlertType == listAlertLessThenOneHour[i].AlertType).ToList().LastOrDefault();
                if (alertEqual != null)
                {
                    if (alertEqual.Level <= listAlertLessThenOneHour[i].Level)
                    {
                        listAlerts.Remove(alertEqual);
                    }
                }

                // Verify if there are some alert from the same group
                if (!String.IsNullOrEmpty(listAlertLessThenOneHour[i].Group))
                {
                    var alertSameGroup = listAlerts.Where(s => s.ID != listAlertLessThenOneHour[i].ID && s.Group == listAlertLessThenOneHour[i].Group).ToList();
                    for (int j = 0; j < alertSameGroup.Count; j++)
                    {
                        listAlertLessThenOneHour[i].Group = String.Empty;
                        listAlertLessThenOneHour[i].TimeToBeRemoved = listAlertLessThenOneHour[i].TimeCreated.AddDays(1);
                        listAlertLessThenOneHour[i].ID += listAlertLessThenOneHour[i].TimeCreated.ToString("ddMMyyyyhhmm");

                    }
                }

            }

            listOldAlerts.AddRange(listAlerts);

            return listAlerts;

        }

        public static void RefreshAll()
        {
            TimeNext = new DateTime(1988, 4, 1);
            listOldAlerts = new List<AlertFilter>();
        }

        public static void AccessFilterCommandLine()
        {
            Console.WriteLine("Qual filtro você deseja acessar?\n");

            for (int i = 0; i < listRadars.Count; i++)
            {
                for (int j = 0; j < listRadars[i].Filters.Count; j++)
                {
                    Console.WriteLine("-{0} (ativo:{1}, em teste:{2})", listRadars[i].Filters[j].Name, listRadars[i].Filters[j].IsActive, listRadars[i].Filters[j].IsTesting);
                }
            }
            string comando = Console.ReadLine();
            IFilter selectedFilter = null;
            for (int i = 0; i < listRadars[i].Filters.Count; i++)
            {
                for (int j = 0; j < listRadars[i].Filters.Count; j++)
                {
                    if (listRadars[i].Filters[j].Name.ToLower().StartsWith(comando.ToLower()))
                    {
                        selectedFilter = listRadars[i].Filters[j];
                        selectedFilter.CommandLine();
                        break;
                    }

                }
            }

            if (selectedFilter == null)
            {
                Console.WriteLine("Filtro não encontrado.");
            }

        }

    }
}
