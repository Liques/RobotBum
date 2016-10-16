using System;
using System.Collections.Generic;
using TowerBotFoundationCore;
using System.Linq;

namespace TowerBotLibCore.Plugins
{
    public class PluginWide : IPlugin
    {
        public bool IsActive { get; set; }
        public bool IsTesting { get; set; }
        public string Name { get; set; }
        public Radar Radar { get; set; }


        public PluginWide()
        {
            Name = "Wide";
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


                    List<AirplaneBasic> listAirplanesPlugined = null;

                    listAirplanesPlugined = listAirplanes.Where(s => s.AircraftType.Type == AircraftModel.AirplaneHeavy && (s.State == AirplaneStatus.TakingOff || s.State == AirplaneStatus.Landing || s.State == AirplaneStatus.Cruise)).ToList();

                    foreach (AirplaneBasic airplane in listAirplanesPlugined)
                    {

                        foreach (var radar in airplane.Radars)
                        {

                            Alert PluginAlert = new Alert(radar, Name, airplane, IconType.NoIcon);
                            string fromPlace = !String.IsNullOrEmpty(airplane.From.City) ? " vindo de " + airplane.From.City : "";
                            string toPlace = !String.IsNullOrEmpty(airplane.To.City) ? " com destino a " + airplane.To.City : "";
                            string fromPlaceShort = !String.IsNullOrEmpty(airplane.From.City) ? " de " + airplane.From.IATA : "";
                            string toPlaceShort = !String.IsNullOrEmpty(airplane.To.City) ? " para " + airplane.To.IATA : "";
                            PluginAlert.TimeToBeRemoved = DateTime.Now.AddHours(10);

                            PluginAlert.Message = "Um " + airplane.AircraftType.Name + " (" + airplane.FlightName + "/" + airplane.Registration + ")";

                            // Se o filtro está pegando os aviaoes locais...

                            switch (airplane.State)
                            {
                                case AirplaneStatus.Landing:
                                    PluginAlert.Icon = IconType.Landing;

                                    PluginAlert.Message += " parece que vai pousar em Brasília" + HelperPlugin.GetForwardLocationsPhrase(airplane, true) + fromPlace;
                                    PluginAlert.Level = 1;
                                    if (airplane.IsSpecial)
                                    {
                                        PluginAlert.AlertType = PluginAlertType.High;
                                        PluginAlert.Justify += ". IsSpecial.";
                                    }
                                    if ((radar.Name != "SAO" || radar.Name != "GRU") && airplane.Weight == AirplaneWeight.Heavy)
                                        PluginAlert.AlertType = PluginAlertType.High;
                                    else
                                        PluginAlert.AlertType = PluginAlertType.Medium;

                                    break;

                                case AirplaneStatus.TakingOff:
                                    PluginAlert.Icon = IconType.TakingOff;

                                    PluginAlert.Level = 3;
                                    PluginAlert.Message += " parece estar decolando nesse momento" + HelperPlugin.GetForwardLocationsPhrase(airplane, true) + toPlace;
                                    if (airplane.IsSpecial)
                                    {
                                        PluginAlert.AlertType = PluginAlertType.High;
                                        PluginAlert.Justify += ". IsSpecial.";
                                    }

                                    if ((radar.Name != "SAO" || radar.Name != "GRU") && airplane.Weight == AirplaneWeight.Heavy)
                                        PluginAlert.AlertType = PluginAlertType.High;
                                    else
                                        PluginAlert.AlertType = PluginAlertType.Medium;


                                    break;
                                case AirplaneStatus.Cruise:
                                    PluginAlert.Icon = IconType.Cruise;

                                    if ((radar.Name == "SAO" || radar.Name == "GRU") && airplane.Weight != AirplaneWeight.Heavy)
                                        continue;

                                        PluginAlert.Message += " está em cruzeiro" + HelperPlugin.GetForwardLocationsPhrase(airplane, true) + fromPlace;
                                    if (HelperPlugin.ListSuperHighAirplanes.Any(s => airplane.AircraftType.ICAO.StartsWith(s)))
                                    {
                                        PluginAlert.AlertType = PluginAlertType.High;
                                    }
                                    else
                                    {
                                        PluginAlert.AlertType = PluginAlertType.Medium;
                                    }
                                    if (airplane.IsSpecial)
                                    {
                                        PluginAlert.AlertType = PluginAlertType.High;
                                        PluginAlert.Justify += ". IsSpecial.";
                                    }
                                    break;
                                case AirplaneStatus.ParkingOrTaxing:
                                    PluginAlert.Icon = IconType.Taxing;

                                    string placeInAirport = HelperPlugin.GetOverLocation(airplane);
                                    placeInAirport = (!string.IsNullOrEmpty(placeInAirport)) ? " no " + placeInAirport : "";
                                    PluginAlert.Level = 2;
                                    PluginAlert.Message += " parece estar no aeroporto" + placeInAirport + fromPlace + toPlace;

                                    if (HelperPlugin.ListSuperHighAirplanes.Any(s => airplane.AircraftType.ICAO.StartsWith(s)))
                                    {
                                        PluginAlert.AlertType = PluginAlertType.High;
                                    }
                                    else
                                    {
                                        PluginAlert.AlertType = PluginAlertType.Medium;
                                    }

                                    break;

                            }

                            PluginAlert.Message += ".";

                            if (IsTesting)
                            {
                                PluginAlert.Message += " Nível:" + PluginAlert.AlertType;
                                PluginAlert.AlertType = PluginAlertType.Test;
                            }

                            if (PluginAlert.AlertType != PluginAlertType.NoData)
                            {
                                if (PluginAlert.Airplane.State == AirplaneStatus.Cruise || (PluginAlert.Airplane.State != AirplaneStatus.Cruise &&
                                    PluginAlert.Radar.IsWideAllowed))
                                    listAlerts.Add(PluginAlert);
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Plugin Wide");
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
