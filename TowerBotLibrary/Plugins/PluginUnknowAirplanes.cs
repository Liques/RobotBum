using System;
using System.Collections.Generic;
using TowerBotFoundation;
using System.Linq;

namespace TowerBotLibrary.Plugins
{
    public class PluginUnknowAirplanes : IPlugin
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsTesting { get; set; }
        public Radar Radar { get; set; }

        bool ShowGeneralAviation = false;
        bool ShowBrazilianCompanies = false;

        public PluginUnknowAirplanes(bool showBrazilianCompanies, bool showGeneralAviation)
        {
            Name = "UnknowAir";
            IsActive = true;
            IsTesting = false;

            ShowBrazilianCompanies = showBrazilianCompanies;
            ShowGeneralAviation = showGeneralAviation;

        }

        public List<Alert> Analyser(object parameter)
        {
            List<AirplaneBasic> listAirplanes = (List<AirplaneBasic>)parameter;

            List<Alert> listAlerts = new List<Alert>();

            try
            {
                if (IsActive)
                {
                    // Necessário essa lista de Wide, pq o Lambda não estava funcionando direito junto com os outros filtros.
                    var listAirplanesPlugined = listAirplanes.Where(s =>
                        s.AircraftType.Type != AircraftModel.AirplaneHeavy && // Para não entrar em conflito com o filtro Wide
                        s.AircraftType.Type != AircraftModel.NoModel &&
                        s.AircraftType.IsValid == true
                        ).ToList();

                    foreach (AirplaneBasic airplane in listAirplanesPlugined)
                    {
                        foreach (var radar in airplane.Radars)
                        {


                            var radarName = radar.Name;

                            if (radar.AvoidCommonTraffic)
                            {

                                if (airplane.AircraftType.Type == AircraftModel.Helicopter ||
                                    airplane.AircraftType.Type == AircraftModel.NoModel ||
                                    airplane.AircraftType.Type != AircraftModel.AirplaneLow)
                                    continue;

                                if ((airplane.FlightName.StartsWith("TAM") || airplane.FlightName.StartsWith("JJ") ||
                           airplane.FlightName.StartsWith("GLO") || airplane.FlightName.StartsWith("G3") || airplane.FlightName.StartsWith("GOL") ||
                           airplane.FlightName.StartsWith("AZU") || airplane.FlightName.StartsWith("AD") ||
                           airplane.FlightName.StartsWith("DAL") || airplane.FlightName.StartsWith("DL") ||
                           airplane.FlightName.StartsWith("ONE") || airplane.FlightName.StartsWith("O6") ||
                           airplane.FlightName.StartsWith("PTB") ||
                           airplane.FlightName.StartsWith("FAB") || airplane.Registration.Name.StartsWith("FAB")) &&
                           !airplane.IsSpecial// Sempre trazer os de pinturas especias)
                                    )
                                    continue;

                            }


                            bool inApproximation = HelperPlugin.IsAirplaneInApproximation(airplane, Radar);
                            bool isLightAirplaneBelowTransitionLimit = airplane.Altitude <= 5000;

                            Alert PluginAlert = new Alert(radar, Name, airplane, IconType.NoIcon);
                            PluginAlert.Airplane = airplane;
                            string fromPlace = !String.IsNullOrEmpty(airplane.From.City) ? " vindo de " + airplane.From.City : "";
                            string toPlace = !String.IsNullOrEmpty(airplane.To.City) ? " com destino a " + airplane.To.City : "";
                            string fromPlaceShort = !String.IsNullOrEmpty(airplane.From.City) ? " de " + airplane.From.IATA : "";
                            string toPlaceShort = !String.IsNullOrEmpty(airplane.To.City) ? " para " + airplane.To.IATA : "";

                            PluginAlert.Message = "Um " + airplane.AircraftType + " (" + airplane.Registration + " - " + airplane.FlightName + ")";

                            switch (airplane.State)
                            {
                                case AirplaneStatus.Cruise:
                                    PluginAlert.Icon = IconType.Cruise;


                                    if ((airplane.FlightName.StartsWith("TAM") || airplane.FlightName.StartsWith("JJ") ||
                               airplane.FlightName.StartsWith("GLO") || airplane.FlightName.StartsWith("G3") || airplane.FlightName.StartsWith("GOL") ||
                               airplane.FlightName.StartsWith("AZU") || airplane.FlightName.StartsWith("AD") ||
                               airplane.FlightName.StartsWith("DAL") || airplane.FlightName.StartsWith("DL") ||
                               airplane.FlightName.StartsWith("ONE") || airplane.FlightName.StartsWith("O6") ||
                               airplane.FlightName.StartsWith("PTB") ||
                               airplane.FlightName.StartsWith("FAB") || airplane.Registration.Name.StartsWith("FAB")) &&
                               !airplane.IsSpecial// Sempre trazer os de pinturas especias)
                                        )
                                        continue;

                                    if (airplane.To.IATA != "CWB")
                                    {
                                        PluginAlert.Level = 0;
                                        PluginAlert.Message += " está em cruzeiro " + HelperPlugin.GetForwardLocationsPhrase(airplane, false) + fromPlaceShort + toPlaceShort;
                                        PluginAlert.Message += (airplane.FlightDistance > 0) ? ", numa viagem de " + airplane.FlightDistance.ToString("#") + " km." : ".";
                                        PluginAlert.AlertType = HelperPlugin.GetAlertByLevel(airplane, radar, isLightAirplaneBelowTransitionLimit, false, inApproximation, true);

                                        listAlerts.Add(PluginAlert);
                                    }
                                    break;

                                case AirplaneStatus.Landing:

                                    if (radar.AltitudeOfTolerence <= airplane.Altitude || airplane.Weight == AirplaneWeight.Medium && radar.IsMediusNotAllowed)
                                        continue;

                                    PluginAlert.Icon = IconType.Landing;

                                    //    if (string.IsNullOrEmpty(airplane.To.City) && !IsKnownAirplane(airplane))
                                    //    {
                                    //        PluginAlert.Message += fromPlace + toPlace + ", parece que vai pousar em Curitiba!";
                                    //        PluginAlert.Justify = airplane.StateJustify;
                                    //        PluginAlert.AlertType = GetAlertByLevel(airplane, true, false);
                                    //    }

                                    //    else 

                                    PluginAlert.Message += fromPlace + toPlace + " parece estar em aproximação" + HelperPlugin.GetForwardLocationsPhrase(airplane, false);
                                    PluginAlert.AlertType = HelperPlugin.GetAlertByLevel(airplane, radar, isLightAirplaneBelowTransitionLimit, false, inApproximation, true);
                                    PluginAlert.Justify += ". !IsKnownAirplane.";


                                    // Se for para ter aviação geral...
                                    if (ShowGeneralAviation && airplane.AircraftType.Type == AircraftModel.AirplaneLow)
                                    {
                                        PluginAlert.Message += fromPlace + toPlace + " parece estar em aproximação" + HelperPlugin.GetForwardLocationsPhrase(airplane, false);
                                        PluginAlert.AlertType = PluginAlertType.High;
                                        PluginAlert.Justify += ". Aviação geral.";
                                    }

                                    if (airplane.Registration.IsValid && airplane.Registration.Country != "Brasil")
                                    {
                                        if (!String.IsNullOrEmpty(airplane.Registration.Country))
                                            PluginAlert.Message += ". País de origem: " + airplane.Registration.Country;

                                        string[] listKnownCountries = new string[] { "Brasil", "EUA", "Inglaterra", "Canadá", "Uruguai", "Bolivia", "Argentina", "Chile", "Espanha", "Portugal", "França", "Panama", "Colômbia", "Países Baixos", "México", "Reino Unido", "Coreia do Sul" };
                                        bool isKnownCountry = listKnownCountries.Where(s => s == airplane.Registration.Country).Count() > 0;

                                        if (!isKnownCountry)
                                        {
                                            PluginAlert.AlertType = PluginAlertType.High;
                                            PluginAlert.Justify += ". É de um país não comum voar por aqui.";
                                        }

                                    }

                                    if (airplane.IsSpecial)
                                    {
                                        PluginAlert.AlertType = PluginAlertType.High;
                                        PluginAlert.Justify += ". IsSpecial.";
                                    }

                                    PluginAlert.Level = 1;
                                    listAlerts.Add(PluginAlert);

                                    break;

                                case AirplaneStatus.TakingOff:

                                    if (radar.AltitudeOfTolerence <= airplane.Altitude || airplane.Weight == AirplaneWeight.Medium && radar.IsMediusNotAllowed)
                                        continue;

                                    PluginAlert.Icon = IconType.TakingOff;

                                    PluginAlert.Level = 4;
                                    PluginAlert.Message += ", parece estar decolando de Curitiba" + HelperPlugin.GetForwardLocationsPhrase(airplane, true) + toPlace + "!";

                                    PluginAlert.AlertType = HelperPlugin.GetAlertByLevel(airplane, radar, isLightAirplaneBelowTransitionLimit, false, inApproximation, true);

                                    // Se for para ter aviação geral...
                                    if (ShowGeneralAviation && airplane.AircraftType.Type == AircraftModel.AirplaneLow && airplane.Altitude <= 6100)
                                    {
                                        PluginAlert.AlertType = PluginAlertType.High;
                                    }
                                    if (airplane.IsSpecial)
                                    {
                                        PluginAlert.AlertType = PluginAlertType.High;
                                        PluginAlert.Justify += ". IsSpecial.";
                                    }

                                    listAlerts.Add(PluginAlert);
                                    break;
                                case AirplaneStatus.ParkingOrTaxing:
                                    PluginAlert.Icon = IconType.Taxing;

                                    string placeInAirport = HelperPlugin.GetOverLocation(airplane);
                                    placeInAirport = (!string.IsNullOrEmpty(placeInAirport)) ? " no " + placeInAirport : "";

                                    bool isBrazilianComercailAirplane = airplane.Registration.IsValid && airplane.Registration.Country == "Brasil";
                                    bool isPlaceInteresting = !String.IsNullOrEmpty(placeInAirport) && !placeInAirport.ToLower().Contains("runway") && !placeInAirport.ToLower().Contains("píer");

                                    if (!isBrazilianComercailAirplane || isPlaceInteresting)
                                    {
                                        PluginAlert.Level = 3;
                                        PluginAlert.Message += " parece estar no aeroporto" + placeInAirport + toPlace + ".";
                                        PluginAlert.AlertType = HelperPlugin.GetAlertByLevel(airplane, radar, isLightAirplaneBelowTransitionLimit, false, inApproximation, true);
                                        listAlerts.Add(PluginAlert);
                                    }
                                    break;

                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Plugin Unknow Airplanes DF");
            }
            return listAlerts;
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
