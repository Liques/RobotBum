using TowerBotLibrary.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerBotFoundation;

namespace TowerBotLibrary
{
    public static class PluginsManager
    {
        public static List<AirplaneBasic> ListOldAirplanesDF { get; set; }
        private static List<Airport> listMainAirports = new List<Airport>();
        public static List<Airport> ListMainAirports { get { return listMainAirports; } }

        public static DateTime TimeNext { get; set; }
        public static TimeSpan Period { get; set; }
        public static List<Alert> listOldAlerts { get; set; }

        private static List<Radar> listRadars { get; set; }

        static PluginsManager()
        {

            listMainAirports = Airport.ListAirports.Where(s => s.Value["ICAO"].ToString().StartsWith("SB")).Select(s => Airport.GetAirportByIata(s.Key)).ToList();
            listMainAirports.Add(Airport.GetAirportByIata("SWUZ"));

            ListOldAirplanesDF = new List<AirplaneBasic>();
            Period = new TimeSpan(0, 0, 15);
            listOldAlerts = new List<Alert>();

            listRadars = new List<Radar>()
            {
                Radar.GetRadar("BRA"),
                Radar.GetRadar("BSB"),
            };

            foreach (var radar in listRadars)
            {
                foreach (var Plugin in radar.Plugins)
                {
                    Plugin.Radar = radar;
                }
            }
        }

        public static List<Alert> GetAlerts(bool updateAll)
        {
            List<Alert> listAlerts = new List<Alert>();

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

                        var listToDelete = new List<Alert>();


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

                                if (Alert.ListOfRecentAlerts != null && Alert.ListOfRecentAlerts.Any(a => a.ID == item.ID))
                                {
                                    listToDelete.Add(item);
                                    continue;
                                }

                                if (Alert.ListOfRecentAlerts != null && Alert.ListOfRecentAlerts.Where(w => w.TimeCreated > DateTime.Now.AddMinutes(-15) && w.TimeCreated < DateTime.Now.AddMinutes(-2)).Any(a => a.AirplaneID == item.AirplaneID && item.Icon == a.Icon))
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

        private static List<Alert> Run(Radar radar, object parameter = null)
        {

            List<Alert> listAlerts = new List<Alert>();

            for (int i = 0; i < radar.Plugins.Count; i++)
            {
                listAlerts.AddRange(radar.Plugins[i].Analyser(parameter));
            }

            // Verify if there are some alert older then it's time to be removed
            if (listAlerts.Count > 0)
            {
                List<Alert> list = listOldAlerts.Where(s => s.TimeToBeRemoved <= DateTime.Now).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    listOldAlerts.Remove(list[i]);
                }
            }

            // Verify if there is any alert equal.
            List<Alert> listAlertLessThenOneHour = listOldAlerts;
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
            listOldAlerts = new List<Alert>();
        }

        public static void AccessPluginCommandLine()
        {
            Console.WriteLine("Qual filtro você deseja acessar?\n");

            for (int i = 0; i < listRadars.Count; i++)
            {
                for (int j = 0; j < listRadars[i].Plugins.Count; j++)
                {
                    Console.WriteLine("-{0} (ativo:{1}, em teste:{2})", listRadars[i].Plugins[j].Name, listRadars[i].Plugins[j].IsActive, listRadars[i].Plugins[j].IsTesting);
                }
            }
            string comando = Console.ReadLine();
            IPlugin selectedPlugin = null;
            for (int i = 0; i < listRadars[i].Plugins.Count; i++)
            {
                for (int j = 0; j < listRadars[i].Plugins.Count; j++)
                {
                    if (listRadars[i].Plugins[j].Name.ToLower().StartsWith(comando.ToLower()))
                    {
                        selectedPlugin = listRadars[i].Plugins[j];
                        selectedPlugin.CommandLine();
                        break;
                    }

                }
            }

            if (selectedPlugin == null)
            {
                Console.WriteLine("Filtro não encontrado.");
            }

        }

    }
}
