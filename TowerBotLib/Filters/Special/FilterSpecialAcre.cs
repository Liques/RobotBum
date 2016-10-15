using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerBotFoundation;

namespace TowerBotLib.Filters
{
    class FilterSpecialAcre : IFilter
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsTesting { get; set; }
        public Radar Radar { get; set; }


        public FilterSpecialAcre()
        {
            Name = "SpecialSam";
            IsActive = false;
            IsTesting = false;
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
                    s.From.City.Contains("Rio Branco") || s.To.City.Contains("Rio Branco")
                    ).ToList();


                foreach (AirplaneBasic airplane in listAirplanesFiltered)
                {
                    AlertFilter filterAlert = new AlertFilter(this.Radar, Name, airplane,IconType.NoIcon);
                    filterAlert.Airplane = airplane;
                    string fromPlace = !String.IsNullOrEmpty(airplane.From.City) ? " vindo de " + airplane.From.City : "";
                    string toPlace = !String.IsNullOrEmpty(airplane.To.City) ? " com destino a " + airplane.To.City : "";
                    string fromPlaceShort = !String.IsNullOrEmpty(airplane.From.City) ? " de " + airplane.From.IATA : "";
                    string toPlaceShort = !String.IsNullOrEmpty(airplane.To.City) ? " para " + airplane.To.IATA : "";

                    filterAlert.Message = "<h3>O @liques ta no " + airplane.FlightName + "(" + airplane.Registration + ")";

                    switch (airplane.State)
                    {
                        case AirplaneStatus.Landing:
                            if (DateTime.Now.Date == new DateTime(2015, 3, 30).Date && (airplane.FlightName == "TAM3585" || airplane.FlightName == "JJ3585"))
                            {
                                filterAlert.Message += fromPlace + toPlace + " parece estar em aproximação" + HelperFilter.GetForwardLocationsPhrase(airplane, false) + ".";
                                filterAlert.Level = 1;
                                filterAlert.AlertType = FilterAlertType.Medium;
                            }
                            break;

                        case AirplaneStatus.TakingOff:
                            if (DateTime.Now.Date == new DateTime(2015, 3, 19).Date && (airplane.FlightName == "TAM3584" || airplane.FlightName == "JJ3584"))
                            {
                                filterAlert.Level = 4;
                                filterAlert.Message += ", que vai para Rio Branco, acabou de decolar de Brasília" + HelperFilter.GetForwardLocationsPhrase(airplane, true) + ".";
                                filterAlert.AlertType = FilterAlertType.Medium;
                            }
                            break;

                        case AirplaneStatus.ParkingOrTaxing:
                            if (DateTime.Now.Date == new DateTime(2015, 3, 19).Date && (airplane.FlightName == "TAM3584" || airplane.FlightName == "JJ3584"))
                            {
                                string placeInAirport = HelperFilter.GetOverLocation(airplane);
                                placeInAirport = (!string.IsNullOrEmpty(placeInAirport)) ? " no " + placeInAirport : "";

                                filterAlert.Level = 3;
                                filterAlert.Message += " parece estar no aeroporto" + placeInAirport + toPlace + ".";
                                filterAlert.AlertType = FilterAlertType.Medium;
                                listAlerts.Add(filterAlert);
                            }
                            break;

                    }

                    filterAlert.Message += "</h3>";

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
