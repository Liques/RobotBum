using System;
using System.Collections.Generic;
using TowerBotFoundationCore;
using System.Linq;

namespace TowerBotLibCore.Filters
{
    public class FilterWide : IFilter
    {
        public bool IsActive { get; set; }
        public bool IsTesting { get; set; }
        public string Name { get; set; }
        public Radar Radar { get; set; }


        public FilterWide()
        {
            Name = "Wide";
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


                    List<AirplaneBasic> listAirplanesFiltered = null;

                    listAirplanesFiltered = listAirplanes.Where(s => s.AircraftType.Type == AircraftModel.AirplaneHeavy && (s.State == AirplaneStatus.TakingOff || s.State == AirplaneStatus.Landing || s.State == AirplaneStatus.Cruise)).ToList();

                    foreach (AirplaneBasic airplane in listAirplanesFiltered)
                    {

                        foreach (var radar in airplane.Radars)
                        {

                            AlertFilter filterAlert = new AlertFilter(radar, Name, airplane, IconType.NoIcon);
                            string fromPlace = !String.IsNullOrEmpty(airplane.From.City) ? " vindo de " + airplane.From.City : "";
                            string toPlace = !String.IsNullOrEmpty(airplane.To.City) ? " com destino a " + airplane.To.City : "";
                            string fromPlaceShort = !String.IsNullOrEmpty(airplane.From.City) ? " de " + airplane.From.IATA : "";
                            string toPlaceShort = !String.IsNullOrEmpty(airplane.To.City) ? " para " + airplane.To.IATA : "";
                            filterAlert.TimeToBeRemoved = DateTime.Now.AddHours(10);

                            filterAlert.Message = "Um " + airplane.AircraftType.Name + " (" + airplane.FlightName + "/" + airplane.Registration + ")";

                            // Se o filtro está pegando os aviaoes locais...

                            switch (airplane.State)
                            {
                                case AirplaneStatus.Landing:
                                    filterAlert.Icon = IconType.Landing;

                                    filterAlert.Message += " parece que vai pousar em Brasília" + HelperFilter.GetForwardLocationsPhrase(airplane, true) + fromPlace;
                                    filterAlert.Level = 1;
                                    if (airplane.IsSpecial)
                                    {
                                        filterAlert.AlertType = FilterAlertType.High;
                                        filterAlert.Justify += ". IsSpecial.";
                                    }
                                    if ((radar.Name != "SAO" || radar.Name != "GRU") && airplane.Weight == AirplaneWeight.Heavy)
                                        filterAlert.AlertType = FilterAlertType.High;
                                    else
                                        filterAlert.AlertType = FilterAlertType.Medium;

                                    break;

                                case AirplaneStatus.TakingOff:
                                    filterAlert.Icon = IconType.TakingOff;

                                    filterAlert.Level = 3;
                                    filterAlert.Message += " parece estar decolando nesse momento" + HelperFilter.GetForwardLocationsPhrase(airplane, true) + toPlace;
                                    if (airplane.IsSpecial)
                                    {
                                        filterAlert.AlertType = FilterAlertType.High;
                                        filterAlert.Justify += ". IsSpecial.";
                                    }

                                    if ((radar.Name != "SAO" || radar.Name != "GRU") && airplane.Weight == AirplaneWeight.Heavy)
                                        filterAlert.AlertType = FilterAlertType.High;
                                    else
                                        filterAlert.AlertType = FilterAlertType.Medium;


                                    break;
                                case AirplaneStatus.Cruise:
                                    filterAlert.Icon = IconType.Cruise;

                                    if ((radar.Name == "SAO" || radar.Name == "GRU") && airplane.Weight != AirplaneWeight.Heavy)
                                        continue;

                                        filterAlert.Message += " está em cruzeiro" + HelperFilter.GetForwardLocationsPhrase(airplane, true) + fromPlace;
                                    if (HelperFilter.ListSuperHighAirplanes.Any(s => airplane.AircraftType.ICAO.StartsWith(s)))
                                    {
                                        filterAlert.AlertType = FilterAlertType.High;
                                    }
                                    else
                                    {
                                        filterAlert.AlertType = FilterAlertType.Medium;
                                    }
                                    if (airplane.IsSpecial)
                                    {
                                        filterAlert.AlertType = FilterAlertType.High;
                                        filterAlert.Justify += ". IsSpecial.";
                                    }
                                    break;
                                case AirplaneStatus.ParkingOrTaxing:
                                    filterAlert.Icon = IconType.Taxing;

                                    string placeInAirport = HelperFilter.GetOverLocation(airplane);
                                    placeInAirport = (!string.IsNullOrEmpty(placeInAirport)) ? " no " + placeInAirport : "";
                                    filterAlert.Level = 2;
                                    filterAlert.Message += " parece estar no aeroporto" + placeInAirport + fromPlace + toPlace;

                                    if (HelperFilter.ListSuperHighAirplanes.Any(s => airplane.AircraftType.ICAO.StartsWith(s)))
                                    {
                                        filterAlert.AlertType = FilterAlertType.High;
                                    }
                                    else
                                    {
                                        filterAlert.AlertType = FilterAlertType.Medium;
                                    }

                                    break;

                            }

                            filterAlert.Message += ".";

                            if (IsTesting)
                            {
                                filterAlert.Message += " Nível:" + filterAlert.AlertType;
                                filterAlert.AlertType = FilterAlertType.Test;
                            }

                            if (filterAlert.AlertType != FilterAlertType.NoData)
                            {
                                if (filterAlert.Airplane.State == AirplaneStatus.Cruise || (filterAlert.Airplane.State != AirplaneStatus.Cruise &&
                                    filterAlert.Radar.IsWideAllowed))
                                    listAlerts.Add(filterAlert);
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Filter Wide");
            }

            return listAlerts;
        }




        public void CommandLine()
        {
            Console.WriteLine("---------------\nFiltro 747 de Teste\n\n+Filtro ativo:" + this.IsActive + "\n+Filtro em teste:" + this.IsTesting + "\n\n---------------\n-test on\\off\n-disable\n-enable\n");
            string comando = Console.ReadLine();
            if (comando.Contains("test on"))
            {
                IsTesting = true;
                Console.WriteLine("Filtros colocados em teste");
            }
            else if (comando.Contains("test off"))
            {
                IsTesting = false;
                Console.WriteLine("Filtros retirados de teste");
            }
            else if (comando == "enable")
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
