using System;
using System.Collections.Generic;
using TowerBotFoundationCore;
using System.Linq;

namespace TowerBotLibCore.Plugins
{
    public class PluginFilterAlerts : IPlugin
    {
        public bool IsActive { get; set; }
        public bool IsTesting { get; set; }
        public string Name { get; set; }
        public Radar Radar { get; set; }


        public PluginFilterAlerts()
        {
            Name = "PFA";
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
                    foreach (AirplaneBasic airplane in listAirplanes)
                    {

                        foreach (var radar in airplane.Radars)
                        {
                            if(airplane.State == AirplaneStatus.DataImcomplete)
                                continue;

                            if(!String.IsNullOrEmpty(airplane.SpecialDescription)){

                                MakeAlert(listAlerts,radar,airplane);
                                continue;
                            }

                            if(airplane.AircraftType.Type == AircraftModel.Helicopter && radar.ShowHelicopters){
                                MakeAlert(listAlerts,radar,airplane);
                                continue;
                            }

                            if(airplane.State != AirplaneStatus.Cruise && 
                            (radar.ShowAllFlightStartingWith.Any(a => airplane.FlightName.StartsWith(a, StringComparison.OrdinalIgnoreCase)) ||
                            radar.ShowAllModelsStartingWith.Any(a => airplane.AircraftType.ICAO.StartsWith(a, StringComparison.OrdinalIgnoreCase)))
                            )
                            {
                                MakeAlert(listAlerts,radar,airplane);
                                continue;
                            }

                            if(radar.AvoidAllFlightsStartingWith.Any(a => airplane.FlightName.StartsWith(a, StringComparison.OrdinalIgnoreCase)) ||
                               radar.AvoidAllModelsStartingWith.Any(a => airplane.AircraftType.ICAO.StartsWith(a, StringComparison.OrdinalIgnoreCase))
                            )
                            {
                                continue;
                            }

                            if(airplane.State != AirplaneStatus.Cruise && 
                                (radar.ShowApproximationLowWeightAirplanes && airplane.Weight == AirplaneWeight.Light ||
                                radar.ShowApproximationMediumWeightAirplanes && airplane.Weight == AirplaneWeight.Medium ||
                                radar.ShowApproximationHeavyWeightAirplanes && airplane.Weight == AirplaneWeight.Heavy)
                            ) {
                                MakeAlert(listAlerts,radar,airplane);
                                continue;
                            }

                            if(airplane.State != AirplaneStatus.Cruise && 
                                (!radar.ShowApproximationLowWeightAirplanes && airplane.Weight == AirplaneWeight.Light ||
                                !radar.ShowApproximationMediumWeightAirplanes && airplane.Weight == AirplaneWeight.Medium ||
                                !radar.ShowApproximationHeavyWeightAirplanes && airplane.Weight == AirplaneWeight.Heavy)
                            ) {
                                continue;
                            }

                            if(airplane.State == AirplaneStatus.Cruise && radar.ShowAllCruisesOnlyOnServer &&
                            airplane.From.ICAO != radar.MainAirportICAO && airplane.To.ICAO != radar.MainAirportICAO) {
                                MakeAlert(listAlerts,radar,airplane, PluginAlertType.Low);         
                                continue;                                                       
                            } else {
                                MakeAlert(listAlerts,radar,airplane, PluginAlertType.Low);         
                                continue;  
                            }

                        }

                    }
                }
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Plugin PFA");
            }

            return listAlerts;
        }



        private void MakeAlert(List<Alert> listAlerts, Radar radar, AirplaneBasic airplane, PluginAlertType alertType = PluginAlertType.High)
        {
            Alert pluginAlert = new Alert(radar, Name, airplane, IconType.NoIcon);
            pluginAlert.AlertType = alertType;
            pluginAlert.TimeToBeRemoved = DateTime.Now.AddHours(23);
            listAlerts.Add(pluginAlert);
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
