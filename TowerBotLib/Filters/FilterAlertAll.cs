using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerBotFoundation;

namespace TowerBotLib.Filters
{
    public class FilterAlertAll : IFilter
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsTesting { get; set; }
        public Radar Radar { get; set; }

        public FilterAlertAll()
        {
            Name = "Ax";
            IsActive = false;
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

                    var listOfRecentAlerts = AlertFilter.ListOfRecentAlerts != null ? AlertFilter.ListOfRecentAlerts.Where(w => w.TimeCreated > DateTime.Now.AddMinutes(-30) && w.Radar.Name == this.Radar.Name).ToList() : new List<AlertFilter>();

                    foreach (AirplaneBasic airplane in listAirplanes)
                    {
                        foreach (var radar in airplane.Radars)
                        {
                            AlertFilter alert = new AlertFilter(radar, Name, airplane, IconType.NoIcon);
                            alert.ID += DateTime.Now.ToString("yyMdHm");
                            alert.CustomMessage = String.Empty;
                            alert.Airplane = airplane;
                            alert.AlertType = FilterAlertType.NoAlert;

                            if (airplane.IsOrbiting)
                            {
                                alert.Icon = IconType.Orbit;
                                alert.Message = string.Format("{0} {1}", airplane.Latitude, airplane.Longitude);

                                if (!IsDuplicated(listOfRecentAlerts, alert) && !(airplane.Weight == AirplaneWeight.Medium && radar.IsMediusNotAllowed))
                                    listAlerts.Add(alert);
                            }

                            if (airplane.IsTouchAndGo)
                            {
                                alert.Icon = IconType.TouchAndGo;
                                alert.Message = airplane.FlightName;

                                if (!IsDuplicated(listOfRecentAlerts, alert))
                                    listAlerts.Add(alert);
                            }

                            if (airplane.FollowingChart != null)
                            {
                                alert.Icon = IconType.Chart;
                                alert.Message = string.Format("{0}", airplane.FollowingChart.Name);

                                if (!IsDuplicated(listOfRecentAlerts, alert))
                                    listAlerts.Add(alert);
                            }

                            if (!string.IsNullOrEmpty(airplane.RunwayName))
                            {
                                alert.Icon = IconType.Runway;
                                alert.Message = string.Format("{0} {1}", airplane.RunwayName, airplane.State);

                                if (!IsDuplicated(listOfRecentAlerts, alert))
                                    listAlerts.Add(alert);
                            }

                            if (listOfRecentAlerts.Any(a => a.Icon == IconType.AirportWeather && a.AlertType != FilterAlertType.NoAlert))
                            {
                                alert.Icon = IconType.AirportWeather;
                                alert.Message = listOfRecentAlerts.Where(a => a.Icon == IconType.AirportWeather).First().Message;

                                if (!IsDuplicated(listOfRecentAlerts, alert))
                                    listAlerts.Add(alert);
                            }

                            if (radar.IsWideAllowed && (airplane.State == AirplaneStatus.Landing || airplane.State == AirplaneStatus.Landing))
                                continue;

                            switch (airplane.State)
                            {
                                case AirplaneStatus.Cruise:
                                    alert.Icon = IconType.Cruise;
                                    alert.Message = AirplaneToString(airplane);

                                    break;

                                case AirplaneStatus.Landing:
                                    alert.Icon = IconType.Landing;
                                    alert.Message = AirplaneToString(airplane);

                                    break;

                                case AirplaneStatus.TakingOff:
                                    alert.Icon = IconType.TakingOff;
                                    alert.Message = AirplaneToString(airplane);

                                    break;
                                case AirplaneStatus.ParkingOrTaxing:
                                    alert.Icon = IconType.Taxing;
                                    alert.Message = AirplaneToString(airplane);

                                    break;
                                default:
                                    continue;
                            }


                            if (!IsDuplicated(listOfRecentAlerts, alert))
                                listAlerts.Add(alert);


                        }

                    }
                }
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Filter AlertAll");
            }
            return listAlerts;
        }

        private string AirplaneToString(AirplaneBasic airplane)
        {
            return airplane.ID + "|" + airplane.Registration.Name + "|" + airplane.AircraftType.ICAO + "|" + ((int)airplane.Weight);
        }

        private bool IsDuplicated(List<AlertFilter> listOfRecentAlerts, AlertFilter alert)
        {
            return (listOfRecentAlerts.Any(a => a.AlertType == FilterAlertType.NoAlert &&
                         a.AirplaneID == alert.Airplane.ID &&
                         a.Icon == alert.Icon
                        ));
        }

        /// <summary>
        /// Filtra aviões específicos que já passaram pelo alerta.
        /// 
        /// </summary>
        /// <param name="airplane"></param>
        /// <returns></returns>
        private bool IsKnownAirplane(AirplaneBasic airplane)
        {
            //bool isKnown = false;

            //if (airplane.FlightName.ToLower().Contains("ptb") || airplane.FlightName.ToLower().Contains("azu") && airplane.To.IATA.ToUpper() == "BRA"  // Filtra os PTB ou AZU que vem de Barreiras e parecem estar alternando Brasília
            //    || airplane.Registration != null && airplane.Registration.Name.ToLower().Contains("pr-a") && airplane.AircraftType.ICAO.ToLower().Contains("B76")    // Filtra os TAM Cargo que esquecem de colocar o número do voo
            //    )
            //    isKnown = true;

            return true;// isKnown;
        }

        public void CommandLine()
        {
            Console.WriteLine("---------------\nFiltro de avião desconhecidos que passam por Brasília\n\n+Filtro ativo:" + this.IsActive + "\n\n---------------\n-disable\n-enable\n");
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
