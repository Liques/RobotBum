using System;
using System.Collections.Generic;
using RobotBumFoundationCore;

namespace RobotBumLibCore.Plugins
{
    public class PluginRatification : IPlugin
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsTesting { get; set; }
        public Radar Radar { get; set; }

        bool AnalyseRunwayLowAircraft = false;
        bool AnalyseRunwayHeavyAircraft = false;
        bool AnalyseChartLowAircraft = false;
        bool AnalyseChartHeavyAircraft = false;
        bool AnalyseOrbit = false;

        public PluginRatification(bool analyseRunwayLowAircraft, bool analyseRunwayHeavyAircraft, bool analyseChartLowAircraft, bool analyseChartHeavyAircraft, bool analyseOrbit)
        {
            Name = "Ratification";
            IsActive = false;
            IsTesting = false;

            AnalyseRunwayLowAircraft = analyseRunwayLowAircraft;
            AnalyseRunwayHeavyAircraft = analyseRunwayHeavyAircraft;
            AnalyseChartLowAircraft = analyseChartLowAircraft;
            AnalyseChartHeavyAircraft = analyseChartHeavyAircraft;
            AnalyseOrbit = analyseOrbit;
        }

        public List<Alert> Analyser(object parameter)
        {
            List<AirplaneBasic> listAirplanes = (List<AirplaneBasic>)parameter;

            List<Alert> listAlerts = new List<Alert>();

            try
            {
                if (IsActive)
                {

                    // TODO em modo testes, todos os aviaos vao passar. EXCLUIR ESSA LINHA ABAIXO e tirar os PluginAlertType.Test!
                    var listAirplanesPlugined = listAirplanes;


                    foreach (AirplaneBasic airplane in listAirplanesPlugined)
                    {
                        if (airplane.PreviousAirplane != null)
                        {
                            foreach (var radar in airplane.Radars)
                            {
                                if (airplane.State == AirplaneStatus.TakingOff || airplane.State == AirplaneStatus.Landing || airplane.State == AirplaneStatus.DataImcomplete)
                                {
                                    
                                    #region Orbit detection
                                    if (AnalyseOrbit && airplane.IsOrbiting)
                                    {

                                        Alert PluginAlert = new Alert(radar, Name, airplane, IconType.Orbit, MessageType.General, RatificationType.Orbit);
                                        if (airplane.LastAlertType != PluginAlertType.High && airplane.LastAlertType != PluginAlertType.Medium && airplane.LastAlertType != PluginAlertType.Low)
                                            PluginAlert.AlertType = PluginAlertType.Test;
                                        
                                        PluginAlert.AlertType = PluginAlertType.High;

                                        listAlerts.Add(PluginAlert);

                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Plugin " + this.Name);
            }

            if (listAlerts.Count > 0)
            {
                string breake = "";
            }

            return listAlerts;
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
