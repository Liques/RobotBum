using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerBotFoundation;

namespace TowerBotLib.Filters
{
    class FilterLogAll : IFilter
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsTesting { get; set; }
        public Radar Radar { get; set; }


        public FilterLogAll()
        {
            Name = "LogAll";
            IsActive = true;
            IsTesting = false;

        }

        public List<AlertFilter> Analyser(object parameter)
        {
            List<AirplaneBasic> listAirplanes = (List<AirplaneBasic>)parameter;

            List<AlertFilter> listAlerts = new List<AlertFilter>();

            try
            {

                if (IsActive)
                {

                    // Lista de voos já conhecidos
                    var listAirplanesFiltered = listAirplanes;

                    string strPath = Environment.GetFolderPath(
                           System.Environment.SpecialFolder.MyDocuments) + "\\logs\\reg";

#if DEBUG
                    strPath += "\\debug";
#endif

                    bool exists = System.IO.Directory.Exists(strPath);


                    if (!exists)
                        System.IO.Directory.CreateDirectory(strPath);

                    foreach (AirplaneBasic airplane in listAirplanesFiltered)
                    {
                        foreach (var radar in airplane.Radars)
                        {
                            if (radar.Name != "BSB" || radar.Name != "BRA")
                                continue;

                            AlertFilter filterAlert = new AlertFilter(radar, Name, airplane, IconType.NoIcon, MessageType.Fixed);

                            //// Data fim dos alertas gerais de log, dando lugar aos regs
                            //if (DateTime.Now.Month == 4 && DateTime.Now.Year == 2015)
                            //{

                            //    filterAlert.AlertType = FilterAlertType.NoAlert;
                            //    filterAlert.Airplane = airplane;

                            //    filterAlert.Message = airplane.AircraftType + ";" + airplane.Registration + ";" + airplane.FlightName + ";" + ";" + airplane.State + ";" + airplane.From.IATA + ";" + airplane.To.IATA + ";" + airplane.Altitude + ";" + airplane.VerticalSpeed + ";" + airplane.Speed;
                            //    filterAlert.ID = "LogAll" + airplane.ID + airplane.State.ToString();
                            //    filterAlert.TimeToBeDeleted = DateTime.Now.AddHours(1);
                            //    filterAlert.Justify += ";" + airplane.StateJustify + ";" + HelperFilter.GetForwardLocationsPhrase(airplane, true);
                            //    filterAlert.Level = 0;
                            //    listAlerts.Add(filterAlert);
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
                ErrorManager.ThrowError(e, "Filter Log All DF");
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
