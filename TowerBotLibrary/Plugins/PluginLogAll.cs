using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerBotFoundation;

namespace TowerBotLibrary.Plugins
{
    class PluginLogAll : IPlugin
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsTesting { get; set; }
        public Radar Radar { get; set; }


        public PluginLogAll()
        {
            Name = "LogAll";
            IsActive = true;
            IsTesting = false;

        }

        public List<Alert> Analyser(object parameter)
        {
            List<AirplaneBasic> listAirplanes = (List<AirplaneBasic>)parameter;

            List<Alert> listAlerts = new List<Alert>();

            try
            {

                if (IsActive)
                {

                    // Lista de voos já conhecidos
                    var listAirplanesPlugined = listAirplanes;

                    string strPath = System.IO.Directory.GetCurrentDirectory() + "\\logs\\reg";

#if DEBUG
                    strPath += "\\debug";
#endif

                    bool exists = System.IO.Directory.Exists(strPath);


                    if (!exists)
                        System.IO.Directory.CreateDirectory(strPath);

                    foreach (AirplaneBasic airplane in listAirplanesPlugined)
                    {
                        foreach (var radar in airplane.Radars)
                        {
                            if (radar.Name != "BSB" || radar.Name != "BRA")
                                continue;

                            Alert PluginAlert = new Alert(radar, Name, airplane, IconType.NoIcon, MessageType.Fixed);

                            //// Data fim dos alertas gerais de log, dando lugar aos regs
                            //if (DateTime.Now.Month == 4 && DateTime.Now.Year == 2015)
                            //{

                            //    PluginAlert.AlertType = PluginAlertType.NoAlert;
                            //    PluginAlert.Airplane = airplane;

                            //    PluginAlert.Message = airplane.AircraftType + ";" + airplane.Registration + ";" + airplane.FlightName + ";" + ";" + airplane.State + ";" + airplane.From.IATA + ";" + airplane.To.IATA + ";" + airplane.Altitude + ";" + airplane.VerticalSpeed + ";" + airplane.Speed;
                            //    PluginAlert.ID = "LogAll" + airplane.ID + airplane.State.ToString();
                            //    PluginAlert.TimeToBeDeleted = DateTime.Now.AddHours(1);
                            //    PluginAlert.Justify += ";" + airplane.StateJustify + ";" + HelperPlugin.GetForwardLocationsPhrase(airplane, true);
                            //    PluginAlert.Level = 0;
                            //    listAlerts.Add(PluginAlert);
                            //}

                            string stateString = "N";
                            switch (airplane.State)
                            {
                                case AirplaneStatus.Cruise:
                                    stateString = "C";
                                    break;
                                case AirplaneStatus.Landing:
                                    stateString = "L";
                                    break;
                                case AirplaneStatus.ParkingOrTaxing:
                                    stateString = "P";
                                    break;
                                case AirplaneStatus.TakingOff:
                                    stateString = "T";
                                    break;
                            }

                            // Date;ID;Flight;Registration;Model;FromPlace;ToPlace;Latitude;Longitude;Speed;VerticalSpeed;Direction;IsWide;State;Radar;Star;Runway;IsOrbit;Altitude
                            string regMsg = airplane.ID + ";" + airplane.FlightName + ";" + airplane.Registration + ";" + airplane.AircraftType.ICAO + ";" + airplane.From.IATA + ";" + airplane.To.IATA + ";" + airplane.Latitude + ";" + airplane.Longitude + ";" + airplane.Speed + ";" + airplane.VerticalSpeed + ";" + airplane.Direction + ";" + airplane.Weight + ";" + stateString + ";" + this.Radar.Name + ";" + (airplane.FollowingChart != null ? airplane.FollowingChart.ToString() : "") + ";" + airplane.RunwayName + ";" + (airplane.IsOrbiting ? "S" : "N") + ";" + airplane.Altitude;

                            using (StreamWriter w = File.AppendText(strPath + "\\" + this.Radar.Name + "-reg_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt"))
                            {
                                w.Write("{0}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                                w.WriteLine(";{0};", regMsg);
                            }
                        }
                    }




                }
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Plugin Log All DF");
            }

            return listAlerts;
        }




        public void CommandLine()
        {
            Console.WriteLine("---------------\nFiltro de fazer log de todos aviões do DF\n\n+Filtro ativo:" + this.IsActive + "\n\n---------------\n-disable\n-enable\n");
            string comando = Console.ReadLine();
            if (comando == "enable")
            {
                IsActive = true;
                Console.WriteLine("Ok");
            }
            else if (comando == "disable")
            {
                IsActive = false;
                Console.WriteLine("Ok");
            }
            else
            {
                Console.WriteLine("Comando do filtro não reconhecido.");
            }
        }
    }
}
