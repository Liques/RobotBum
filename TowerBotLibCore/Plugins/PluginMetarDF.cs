using System;
using System.Collections.Generic;
using TowerBotFoundationCore;

namespace TowerBotLibCore.Plugins
{
    class PluginMetarDF : IPlugin
    {
        public bool IsActive { get; set; }
        public bool IsTesting { get; set; }
        public string Name { get; set; }
        DateTime emergencyEndTime = new DateTime();
        public Radar Radar { get; set; }

        public PluginMetarDF()
        {
            Name = "PluginMetarDF";
            IsActive = true;
            IsTesting = false;
        }
        public List<Alert> Analyser(object parameter)
        {           
            List<Alert> listAlerts = new List<Alert>();

            try
            {
                if (IsActive)
                {

                    AirportWeather brasiliaWeather = AirportWeather.GetWeather(Airport.GetAirportByIata("BSB").ICAO);
                    string message = String.Empty;


                    // Mudança de direção de vento
                    if (brasiliaWeather.WindDirection >= 20 && brasiliaWeather.WindDirection < 200)
                    {
                        message = "As pistas em operação são agora as pistas 11L e 11R. " + GetWeatherConditionMessage(brasiliaWeather.WeatherType, brasiliaWeather.Sky, brasiliaWeather.Metar);
                        Alert PluginAlert = new Alert(this.Radar, this.Name, "Rwy11", IconType.AirportWeather);
                        PluginAlert.Message = message;
                        PluginAlert.Group = "RunwayChange";
                        PluginAlert.TimeToBeRemoved = DateTime.Now.AddYears(1);
                        PluginAlert.AlertType = PluginAlertType.High;
                        listAlerts.Add(PluginAlert);

                    }
                    else if ((brasiliaWeather.WindDirection > 200 && brasiliaWeather.WindDirection < 360 || brasiliaWeather.WindDirection > 20 && brasiliaWeather.WindDirection < 0))
                    {
                        message = "As pistas em operação são agora as pistas  29L e 29R. " + GetWeatherConditionMessage(brasiliaWeather.WeatherType, brasiliaWeather.Sky, brasiliaWeather.Metar);
                        Alert PluginAlert = new Alert(this.Radar, this.Name, "Rwy29", IconType.AirportWeather);
                        PluginAlert.Message = message;
                        PluginAlert.Group = "RunwayChange";
                        PluginAlert.TimeToBeRemoved = DateTime.Now.AddYears(1);
                        PluginAlert.AlertType = PluginAlertType.High;
                        listAlerts.Add(PluginAlert);

                    }

                    // Mostrando a visibilidade
                    if (brasiliaWeather.Visibility > -1 && brasiliaWeather.Visibility < 210 && !String.IsNullOrEmpty(brasiliaWeather.Metar))
                    {
                        message = "Talvez o aeroporto está fechado, visibilidade muito baixa. (" + brasiliaWeather.Metar + ")";
                        Alert PluginAlert = new Alert(this.Radar, this.Name, "VisibilityCAT3", IconType.AirportWeather);
                        PluginAlert.Message = message;
                        PluginAlert.TimeToBeRemoved = DateTime.Now.AddHours(3);
                        PluginAlert.AlertType = PluginAlertType.High;
                        listAlerts.Add(PluginAlert);

                    }
                    else if (brasiliaWeather.Visibility > -1 && brasiliaWeather.Visibility < 370 && !String.IsNullOrEmpty(brasiliaWeather.Metar))
                    {
                        message = "Baixa visibilidade, Brasília parece estar operando na categoria CAT II. (" + brasiliaWeather.Metar + ")";

                        Alert PluginAlert = new Alert(this.Radar, this.Name, "VisibilityCAT2", IconType.AirportWeather);
                        PluginAlert.Message = message;
                        PluginAlert.TimeToBeRemoved = DateTime.Now.AddHours(3);
                        PluginAlert.AlertType = PluginAlertType.High;
                        listAlerts.Add(PluginAlert);

                    }
                    else if (brasiliaWeather.Visibility > -1 && brasiliaWeather.Visibility < 370 && !String.IsNullOrEmpty(brasiliaWeather.Metar))
                    {
                        message = "Pouca visibilidade, Brasília parece estar operando na categoria CAT I. (" + brasiliaWeather.Metar + ")";

                        Alert PluginAlert = new Alert(this.Radar, this.Name, "VisibilityCAT1", IconType.AirportWeather);
                        PluginAlert.Message = message;
                        PluginAlert.TimeToBeRemoved = DateTime.Now.AddHours(3);
                        PluginAlert.AlertType = PluginAlertType.High;
                        listAlerts.Add(PluginAlert);
                    }

                    // condições do tempo
                    Alert PluginAlertWeatherConditition = new Alert(this.Radar, this.Name, "VisibilityCAT1", IconType.AirportWeather);
                    PluginAlertWeatherConditition.Group = "PluginAlertWeatherConditition";
                    PluginAlertWeatherConditition.TimeToBeRemoved = DateTime.Now.AddYears(1);

                    switch (brasiliaWeather.WeatherType)
                    {
                        case WeatherType.Tornado:
                        case WeatherType.VolcanicAsh:
                        case WeatherType.Duststorm:
                        case WeatherType.Smoke:
                        case WeatherType.Snow:
                        case WeatherType.Poeira:
                        case WeatherType.Fog:
                        case WeatherType.Hail:
                        case WeatherType.VeryStrongRain:
                            PluginAlertWeatherConditition.ID = this.Name + brasiliaWeather.WeatherType;
                            PluginAlertWeatherConditition.Message = GetWeatherConditionMessage(brasiliaWeather.WeatherType, brasiliaWeather.Sky, brasiliaWeather.Metar);
                            SetEmergencyTime();
                            break;
                        case WeatherType.Haze:
                        case WeatherType.Rain:
                        case WeatherType.WeakRain:
                        case WeatherType.StrongRain:
                        case WeatherType.NeighborRain:
                        case WeatherType.Drizzle:
                            PluginAlertWeatherConditition.ID = this.Name + brasiliaWeather.WeatherType;
                            PluginAlertWeatherConditition.Message = GetWeatherConditionMessage(brasiliaWeather.WeatherType, brasiliaWeather.Sky, brasiliaWeather.Metar);
                            break;
                        default:

                            switch (brasiliaWeather.Sky)
                            {
                                case SkyType.Clear:
                                case SkyType.FewClouds:
                                case SkyType.Overcast:
                                case SkyType.SomeCloud:
                                case SkyType.VeryCloudy:
                                    PluginAlertWeatherConditition.ID = this.Name + brasiliaWeather.Sky;
                                    PluginAlertWeatherConditition.Message = GetWeatherConditionMessage(brasiliaWeather.WeatherType, brasiliaWeather.Sky, brasiliaWeather.Metar);
                                    break;
                            }

                            break;



                    }



                    bool isEmergency = emergencyEndTime > DateTime.Now;

                    PluginAlertWeatherConditition.AlertType = isEmergency ? PluginAlertType.High : PluginAlertType.Low;

                    if (!String.IsNullOrEmpty(PluginAlertWeatherConditition.Message))
                        listAlerts.Add(PluginAlertWeatherConditition);


                    if (IsTesting)
                    {
                        for (int i = 0; i < listAlerts.Count; i++)
                        {
                            listAlerts[i].Message += " Nível:" + listAlerts[i].AlertType;
                            listAlerts[i].AlertType = PluginAlertType.Test;
                        }

                    }
                    //if (isFirstTime)
                    //{
                    //    for (int i = 0; i < listAlerts.Count; i++)
                    //    {
                    //        listAlerts[i].AlertType = PluginAlertType.Low;
                    //    }

                    //    isFirstTime = false;

                    //}





                }
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Plugin METAR");
            }

            return listAlerts;
        }


        private string GetWeatherConditionMessage(WeatherType weatherType, SkyType Sky, string Metar)
        {
            string message = String.Empty;

            if (weatherType == WeatherType.Tornado)
            {
                message = "ALERTA! Está se formando um tornado na capital! (" + Metar + ")";
            }
            else if (weatherType == WeatherType.VolcanicAsh)
            {
                message = "O metar está falando que tem fumaça vulcanica passando por aqui (" + Metar + ")";
            }
            else if (weatherType == WeatherType.Duststorm)
            {
                message = "Está tendo tempestadade de areia em Brasília (" + Metar + ")";
            }
            else if (weatherType == WeatherType.Smoke)
            {
                message = "Está tendo muita fumaça perto do aeroporto (" + Metar + ")";
            }
            else if (weatherType == WeatherType.Snow)
            {
                message = "Neve? Tem errado na minha programação... só pode. Registro de neve em Brasília. (" + Metar + ")";
            }
            else if (weatherType == WeatherType.Poeira)
            {
                message = "O aeroporto de Brasília está registrando ter muita poeira (" + Metar + ")";
            }
            else if (weatherType == WeatherType.Poeira)
            {
                message = "O aeroporto de Brasília está registrando ter muita poeira (" + Metar + ")";
            }
            else if (weatherType == WeatherType.Fog)
            {
                message = "O aeroporto de Brasília está com neblina (" + Metar + ")";
            }
            else if (weatherType == WeatherType.Haze)
            {
                message = "Névoa seca está tomando de conta capital (" + Metar + ")";
            }
            else if (weatherType == WeatherType.Hail)
            {
                message = "Chuva com granizo no aeroporto! (" + Metar + ")";
            }
            else if (weatherType == WeatherType.WeakRain)
            {
                message = "Uma chuva fraca está atingindo no aeroporto. (" + Metar + ")";
            }
            else if (weatherType == WeatherType.StrongRain)
            {
                message = "Está chovendo no aeroporto (" + Metar + ")";
            }
            else if (weatherType == WeatherType.VeryStrongRain)
            {
                message = "Chuva muito forte está atingindo no aeroporto! (" + Metar + ")";

            }
            else if (weatherType == WeatherType.Rain)
            {
                message = "O aeroporto está enfrentando chuvas moderadas. (" + Metar + ")";
            }
            else if (weatherType == WeatherType.NeighborRain)
            {
                message = "Tem chuva na vizinhança do aeroporto. (" + Metar + ")";
            }
            else if (weatherType == WeatherType.Drizzle)
            {
                message = "Está chuviscando no aeroporto. (" + Metar + ")";
            }
            else if (Sky == SkyType.Overcast)
            {
                message = "Está nublado na capital. (" + Metar + ")";
            }
            else if (Sky == SkyType.VeryCloudy)
            {
                message = "Há muita nuvem no céu da capital. (" + Metar + ")";
            }
            else if (Sky == SkyType.SomeCloud)
            {
                message = "Há algumas nuvens no céu da capital. (" + Metar + ")";
            }
            else if (Sky == SkyType.FewClouds)
            {
                message = "Há poucas nuvens no céu da capital. (" + Metar + ")";
            }
            else if (Sky == SkyType.Clear)
            {
                message = "Céu aberto na capital federal! (" + Metar + ")";
            }

            return message;
        }


        private void SetEmergencyTime()
        {
            if (emergencyEndTime < DateTime.Now)
            {
                emergencyEndTime = DateTime.Now.AddHours(1);
            }
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
