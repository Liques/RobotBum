using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerBotFoundation;

namespace TowerBotLib.Filters
{
    class FilterSobral : IFilter
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsTesting { get; set; }
        public Radar Radar { get; set; }


        public FilterSobral()
        {
            Name = "Sobral";
            IsActive = false;
            IsTesting = true;
        }

        public List<AlertFilter> Analyser(object parameter)
        {

            List<AirplaneBasic> listAirplanes = (List<AirplaneBasic>)parameter;
            for (int i = 0; i < listAirplanes.Count; i++)
            {
                listAirplanes[i].ForwardPlacesPhrase = HelperFilter.GetForwardLocationsPhrase(listAirplanes[i], false);
            }

            List<AlertFilter> listAlerts = new List<AlertFilter>();

            if (IsActive)
            {

                // Lista de voos já conhecidos
                var listAirplanesFiltered = listAirplanes.Where(s =>
                    s.ForwardPlacesPhrase.Contains("Sobradinho")
                    ).ToList();


                foreach (AirplaneBasic airplane in listAirplanesFiltered)
                {
                    AlertFilter filterAlert = new AlertFilter(this.Radar, Name, airplane, IconType.NoIcon);
                    filterAlert.Airplane = airplane;
                    string fromPlace = !String.IsNullOrEmpty(airplane.From.City) ? " vindo de " + airplane.From.City : "";
                    string toPlace = !String.IsNullOrEmpty(airplane.To.City) ? " com destino a " + airplane.To.City : "";
                    string fromPlaceShort = !String.IsNullOrEmpty(airplane.From.City) ? " de " + airplane.From.IATA : "";
                    string toPlaceShort = !String.IsNullOrEmpty(airplane.To.City) ? " para " + airplane.To.IATA : "";

                    filterAlert.Message = "Um " + airplane.AircraftType + " (" + airplane.Registration + " - " + airplane.FlightName + ")";

                    switch (airplane.State)
                    {
                        case AirplaneStatus.Cruise:
                            if (airplane.To.City != "Brasília")
                            {
                                filterAlert.Level = 0;
                                filterAlert.Message = "HIGH: " + filterAlert.Message + " está em cruzeiro " + HelperFilter.GetForwardLocationsPhrase(airplane, false) + fromPlaceShort + toPlaceShort;
                                filterAlert.Message += " está em cruzeiro " + HelperFilter.GetForwardLocationsPhrase(airplane, false) + fromPlaceShort + toPlaceShort;
                                filterAlert.Message += (airplane.FlightDistance > 0) ? ", numa viagem de " + airplane.FlightDistance.ToString("#") + " km." : ".";
                            }
                            break;

                        case AirplaneStatus.Landing:

                            filterAlert.Message += fromPlace + toPlace + " parece estar em aproximação" + HelperFilter.GetForwardLocationsPhrase(airplane, false) + ".";
                            filterAlert.Level = 1;

                            break;

                        case AirplaneStatus.TakingOff:
                            filterAlert.Level = 4;
                            filterAlert.Message += ", parece estar decolando de Brasília" + HelperFilter.GetForwardLocationsPhrase(airplane, true) + toPlace + "!";
                            break;

                    }

                    

                    if (IsTesting)
                    {
                        filterAlert.AlertType = FilterAlertType.Test;
                    }

                    if (airplane.State == AirplaneStatus.Landing || airplane.State == AirplaneStatus.TakingOff || airplane.State == AirplaneStatus.Cruise)
                    {
                        listAlerts.Add(filterAlert);
                    }


                }
            }
            return listAlerts;
        }

        /// <summary>
        /// Mostra qual é o nível de alerta a partir do modelo do avião
        /// </summary>
        /// <param name="airplane">Avião</param>
        /// <param name="isImportantToSee">Fazemos questão de ver?</param>
        /// <param name="isAcceptedCommonAirplanes">Aceitamos aviões comuns que não pertecem uma linha aerea e que não são brasileiros</param>
        /// <returns></returns>
        private FilterAlertType GetAlertByLevel(AirplaneBasic airplane, bool isImportantToSee, bool isAcceptedCommonAirplanes = false)
        {

            // Lista de aeronaves comuns que, se de empresas diferente, podem emitir alerta
            var listAircraftTypeCommon = HelperFilter.ListCommonAirplanes;

            // Lista de aeronaves que estão alerta HIGH
            var listAircraftTypeHighAlert = HelperFilter.ListWideAirplanes;

            // Lista de aeronaves que estão alerta SUPER HIGH
            var listAircraftTypeSuperHighAlert = HelperFilter.ListSuperHighAirplanes;

            FilterAlertType alertType = FilterAlertType.Low;

            bool isHighAlert = listAircraftTypeHighAlert.Where(s => airplane.AircraftType != null && airplane.AircraftType.ICAO.Contains(s)).Count() > 0;
            bool isSuperHighAlert = listAircraftTypeSuperHighAlert.Where(s => airplane.AircraftType != null && airplane.AircraftType.ICAO.Contains(s)).Count() > 0;
            bool isCommonAirplane = listAircraftTypeCommon.Where(s => airplane.AircraftType != null && airplane.AircraftType.ICAO.Contains(s)).Count() > 0;

            if (isSuperHighAlert)
                alertType = FilterAlertType.High;
            else if (isCommonAirplane && isAcceptedCommonAirplanes)
                alertType = FilterAlertType.High;
            else if (isHighAlert && isImportantToSee)
                alertType = FilterAlertType.High;
            else if (isHighAlert && !isImportantToSee)
                alertType = FilterAlertType.Medium;

            return alertType;
        }

        /// <summary>
        /// Filtra aviões específicos que já passaram pelo alerta.
        /// 
        /// </summary>
        /// <param name="airplane"></param>
        /// <returns></returns>
        private bool IsKnownAirplane(AirplaneBasic airplane)
        {
            bool isKnown = false;

            if (airplane.FlightName.ToLower().Contains("ptb")  // Filtra os PTB que vem de Barreiras e parecem estar alternando Brasília
                || airplane.Registration != null && airplane.Registration.Name.ToLower().Contains("pr-a") && airplane.AircraftType.ICAO.ToLower().Contains("B76")    // Filtra os TAM Cargo que esquecem de colocar o número do voo
                )
                isKnown = true;

            return isKnown;
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
