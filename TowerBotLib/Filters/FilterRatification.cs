using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerBotFoundation;

namespace TowerBotLib.Filters
{
    public class FilterRatification : IFilter
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

        public FilterRatification(bool analyseRunwayLowAircraft, bool analyseRunwayHeavyAircraft, bool analyseChartLowAircraft, bool analyseChartHeavyAircraft, bool analyseOrbit)
        {
            Name = "Ratification";
            IsActive = true;
            IsTesting = false;

            AnalyseRunwayLowAircraft = analyseRunwayLowAircraft;
            AnalyseRunwayHeavyAircraft = analyseRunwayHeavyAircraft;
            AnalyseChartLowAircraft = analyseChartLowAircraft;
            AnalyseChartHeavyAircraft = analyseChartHeavyAircraft;
            AnalyseOrbit = analyseOrbit;
        }

        public List<AlertFilter> Analyser(object parameter)
        {
            List<AirplaneBasic> listAirplanes = (List<AirplaneBasic>)parameter;

            List<AlertFilter> listAlerts = new List<AlertFilter>();

            try
            {
                if (IsActive)
                {

                    // TODO em modo testes, todos os aviaos vao passar. EXCLUIR ESSA LINHA ABAIXO e tirar os FilterAlertType.Test!
                    var listAirplanesFiltered = listAirplanes;


                    foreach (AirplaneBasic airplane in listAirplanesFiltered)
                    {
                        if (airplane.PreviousAirplane != null)
                        {
                            foreach (var radar in airplane.Radars)
                            {
                                if (airplane.State == AirplaneStatus.TakingOff || airplane.State == AirplaneStatus.Landing || airplane.State == AirplaneStatus.DataImcomplete)
                                {
                                    if (airplane.FollowingChart != null && airplane.PreviousAirplane.FollowingChart == null
                                        && (radar.Name == "BSB" || radar.Name == "CWB"))
                                    {
                                        AlertFilter filterAlert = new AlertFilter(radar, Name, airplane, IconType.Chart, MessageType.General, RatificationType.Chart);
                                        filterAlert.Airplane = airplane;
                                        filterAlert.Justify += ". Foi encontrado uma nova carta." + filterAlert.AlertType.ToString();
                                        filterAlert.AlertType = airplane.PreviousAirplane.LastAlertType;

                                        if (AnalyseRunwayHeavyAircraft && airplane.Weight == AirplaneWeight.Heavy || AnalyseRunwayLowAircraft)
                                        {
                                            listAlerts.Add(filterAlert);
                                        }

                                    }

                                    if (String.IsNullOrEmpty(airplane.PreviousAirplane.RunwayName) && !String.IsNullOrEmpty(airplane.RunwayName)
                                        && (radar.Name == "BSB" || radar.Name == "CWB" || radar.Name == "GRU" || radar.Name == "CGH"))
                                    {
                                        AlertFilter filterAlert = new AlertFilter(radar, Name, airplane, IconType.Runway, MessageType.General, RatificationType.FinalRunway);
                                        filterAlert.Airplane = airplane;
                                        filterAlert.Justify += ". Foi detectado runway." + filterAlert.AlertType.ToString();
                                        filterAlert.AlertType = airplane.PreviousAirplane.LastAlertType;

                                        if (AnalyseRunwayHeavyAircraft && airplane.Weight == AirplaneWeight.Heavy || AnalyseRunwayLowAircraft)
                                        {
                                            listAlerts.Add(filterAlert);
                                        }

                                    }

                                    #region Detecção de órbitas
                                    if (AnalyseOrbit && airplane.IsOrbiting)
                                    {

                                        AlertFilter filterAlert = new AlertFilter(radar, Name, airplane, IconType.Orbit, MessageType.General, RatificationType.Orbit);
                                        filterAlert.Justify += ". Foi detectado orbita.";
                                        if (airplane.LastAlertType != FilterAlertType.High && airplane.LastAlertType != FilterAlertType.Medium && airplane.LastAlertType != FilterAlertType.Low)
                                            filterAlert.AlertType = FilterAlertType.Test;
                                        else
                                            filterAlert.AlertType = FilterAlertType.Test;// airplane.LastAlertType;

                                        filterAlert.AlertType = FilterAlertType.High;

                                        listAlerts.Add(filterAlert);

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
                ErrorManager.ThrowError(e, "Filter " + this.Name);
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
