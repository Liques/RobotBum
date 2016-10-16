using System;
using System.Collections.Generic;
using System.Linq;
using TowerBotFoundationCore;

namespace TowerBotLibCore.Plugins
{
    class PluginBackingOrGo : IPlugin
    {
        public bool IsActive { get; set; }
        public bool IsTesting { get; set; }
        public string Name { get; set; }
        public Radar Radar { get; set; }

        public PluginBackingOrGo()
        {
            Name = "Backing";
            IsActive = false;
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
                    var listAirplanesPlugined = listAirplanes.Where(s => s.From.IATA.Contains("BSB") && !String.IsNullOrEmpty(s.From.IATA) && s.State == AirplaneStatus.Landing ||
                                                                    s.To.IATA.Contains("BSB") && !String.IsNullOrEmpty(s.To.IATA) && s.State == AirplaneStatus.TakingOff
                    ).ToList();

                    // TODO para testes foi feito isso, mas engloba td
                    listAirplanesPlugined = listAirplanes;

                    foreach (AirplaneBasic airplane in listAirplanesPlugined)
                    {
                        string toPlace = (airplane.To.City != null) ? " que ia para " + airplane.To.City : "";
                        string fromPlace = !String.IsNullOrEmpty(airplane.From.City) ? " vindo de " + airplane.From.City : "";

                        #region Arremetida
                        int i = 0;
                        AirplaneBasic currentAiplane = airplane;

                        int numberOfAnalysis = 12;

                        int takeOffNumber = 0;
                        int landingNumber = 0;
                        bool finalRunway = false;

                        AirplaneBasic currentAirplane = airplane;
                        bool orbit = false;
                        string runway = string.Empty;

                        while (true)
                        {
                            if (!String.IsNullOrEmpty(currentAirplane.RunwayName))
                                runway = currentAirplane.RunwayName;
                            if (airplane.IsOrbiting)
                                orbit = true;

                            if (currentAirplane.PreviousAirplane != null)
                            {
                                currentAirplane = currentAirplane.PreviousAirplane;
                                continue;
                            }
                            break;

                        }

                        if (orbit && !string.IsNullOrEmpty(runway))
                        {
                            airplane.IsTouchAndGo = true;
                            Alert PluginAlert = new Alert(this.Radar, Name, airplane, IconType.TouchAndGo, MessageType.Fixed);
                            PluginAlert.Message = "O voo " + airplane.FlightName + " (" + airplane.Registration + ")";

                            PluginAlert.Message += ", parece que teve que arremeter da pista " + runway + ".";
                            PluginAlert.Level = 1;
                            PluginAlert.AlertType = PluginAlertType.High;
                            listAlerts.Add(PluginAlert);
                        }
                        #endregion


                    }
                }
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Plugin Backing or Go");
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
