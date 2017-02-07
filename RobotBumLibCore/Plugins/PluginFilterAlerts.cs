using System;
using System.Collections.Generic;
using RobotBumFoundationCore;
using System.Linq;

namespace RobotBumLibCore.Plugins
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
                            // If the airplane does not have enough data...
                            if(airplane.State == AirplaneStatus.DataImcomplete)
                                continue;

                            // If is to show everyting...
                            if (radar.ShowEverything)
                            {
                                MakeAlert(listAlerts, radar, airplane);
                                continue;
                            }

                            // If the airplane is "special" on specialpaintings.json
                            if (!String.IsNullOrEmpty(airplane.SpecialDescription)){

                                MakeAlert(listAlerts,radar,airplane);
                                continue;
                            }

                            // if the airplane actually is a helicopter...
                            if(airplane.AircraftType.Type == AircraftModel.Helicopter && radar.ShowHelicopters){
                                MakeAlert(listAlerts,radar,airplane);
                                continue;
                            } else  if(airplane.AircraftType.Type == AircraftModel.Helicopter && !radar.ShowHelicopters){
                                continue;
                            }

                            // If the airplane has the required weight...
                            if(airplane.State != AirplaneStatus.Cruise && 
                                (radar.ShowAllApproximationLowWeightAirplanes && airplane.Weight == AirplaneWeight.Light ||
                                radar.ShowAllApproximationMediumWeightAirplanes && airplane.Weight == AirplaneWeight.Medium ||
                                radar.ShowAllApproximationHeavyWeightAirplanes && airplane.Weight == AirplaneWeight.Heavy)
                            ) {
                                MakeAlert(listAlerts,radar,airplane);
                                continue;
                            } else  if(airplane.State != AirplaneStatus.Cruise && 
                                (radar.AvoidAllApproximationLowWeightAirplanes && airplane.Weight == AirplaneWeight.Light ||
                                radar.AvoidAllApproximationMediumWeightAirplanes && airplane.Weight == AirplaneWeight.Medium ||
                                radar.AvoidAllApproximationHeavyWeightAirplanes && airplane.Weight == AirplaneWeight.Heavy)
                            ) {
                                continue;
                            }

                            // If the airplane has the required flight name...
                            if(airplane.State != AirplaneStatus.Cruise && 
                            (radar.ShowAllFlightStartingWith.Any(a => airplane.FlightName.StartsWith(a, StringComparison.OrdinalIgnoreCase)) ||
                            radar.ShowAllModelsStartingWith.Any(a => airplane.AircraftType.ICAO.StartsWith(a, StringComparison.OrdinalIgnoreCase)))
                            )
                            {
                                MakeAlert(listAlerts,radar,airplane);
                                continue;
                            }

                            // If the airplane's flight name is not wanted...
                            if(radar.AvoidAllFlightsStartingWith.Any(a => airplane.FlightName.StartsWith(a, StringComparison.OrdinalIgnoreCase)) ||
                               radar.AvoidAllModelsStartingWith.Any(a => airplane.AircraftType.ICAO.StartsWith(a, StringComparison.OrdinalIgnoreCase))
                            )
                            {
                                continue;
                            }

                            // If ShowAllCruisesOnlyOnServer is true...
                            if(airplane.State == AirplaneStatus.Cruise && radar.ShowAllCruisesHeavyWeight &&
                            airplane.Weight == AirplaneWeight.Heavy) {
                                MakeAlert(listAlerts,radar,airplane);         
                                continue;                                                       
                            } else if(airplane.State == AirplaneStatus.Cruise) {
                                MakeAlert(listAlerts,radar,airplane, PluginAlertType.Low); 
                                continue;   
                            }

                            // If this line is reached, the airplane is unknow. So we can make an alert for it!
                            MakeAlert(listAlerts,radar,airplane); 

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
