using System;
using System.Collections.Generic;
using TowerBotFoundationCore;

namespace TowerBotLibCore.Filters
{
    class FilterMetarCWB : IFilter
    {
        public bool IsActive { get; set; }
        public bool IsTesting { get; set; }
        public string Name { get; set; }
        DateTime emergencyEndTime = new DateTime();
        public Radar Radar { get; set; }

        public FilterMetarCWB()
        {
            Name = "FilterMetarCWB";
            IsActive = true;
            IsTesting = false;
        }
        public List<AlertFilter> Analyser(object parameter)
        {
            List<AlertFilter> listAlerts = new List<AlertFilter>();

            try
            {
                if (IsActive)
                {

                    AirportWeather curitibaWeather = AirportWeather.GetWeather(Airport.GetAirportByIata("CWB").ICAO);
                    string message = String.Empty;


                    // Mudança de direção de vento
                    if (curitibaWeather.WindDirection >= 240 && curitibaWeather.WindDirection < 60)
                    {
                        message = "A pista em operação agora é a 33. " + GetWeatherConditionMessage(curitibaWeather.WeatherType, curitibaWeather.Sky, curitibaWeather.Metar);
                        AlertFilter filterAlert = new AlertFilter(this.Radar, this.Name, "Rwy11",IconType.AirportWeather);
                        filterAlert.Message = message;
                        filterAlert.Group = "RunwayChange";
                        filterAlert.TimeToBeDeleted = DateTime.Now.AddYears(1);
                        filterAlert.AlertType = FilterAlertType.High;
                        listAlerts.Add(filterAlert);

                    }
                    else if ((curitibaWeather.WindDirection > 60 && curitibaWeather.WindDirection < 240))
                    {
                        message = "A pista em operação agora é a 15. " + GetWeatherConditionMessage(curitibaWeather.WeatherType, curitibaWeather.Sky, curitibaWeather.Metar);
                        AlertFilter filterAlert = new AlertFilter(this.Radar, this.Name, "Rwy29", IconType.AirportWeather);
                        filterAlert.Message = message;
                        filterAlert.Group = "RunwayChange";
                        filterAlert.TimeToBeDeleted = DateTime.Now.AddYears(1);
                        filterAlert.AlertType = FilterAlertType.High;
                        listAlerts.Add(filterAlert);

                    }

                    // Mostrando a visibilidade
                    if (curitibaWeather.Visibility > -1 && curitibaWeather.Visibility < 210)
                    {
                        message = "Talvez o aeroporto está fechado, visibilidade muito baixa. (" + curitibaWeather.Metar + ")";
                        AlertFilter filterAlert = new AlertFilter(this.Radar, this.Name, "VisibilityCAT3", IconType.AirportWeather);
                        filterAlert.Message = message;
                        filterAlert.TimeToBeDeleted = DateTime.Now.AddHours(3);
                        filterAlert.AlertType = FilterAlertType.High;
                        listAlerts.Add(filterAlert);

                    }
                    else if (curitibaWeather.Visibility > -1 && curitibaWeather.Visibility < 370)
                    {
                        message = "Baixa visibilidade, Brasília parece estar operando na categoria CAT II. (" + curitibaWeather.Metar + ")";

                        AlertFilter filterAlert = new AlertFilter(this.Radar, this.Name, "VisibilityCAT2", IconType.AirportWeather);
                        filterAlert.Message = message;
                        filterAlert.TimeToBeDeleted = DateTime.Now.AddHours(3);
                        filterAlert.AlertType = FilterAlertType.High;
                        listAlerts.Add(filterAlert);

                    }
                    else if (curitibaWeather.Visibility > -1 && curitibaWeather.Visibility < 370)
                    {
                        message = "Pouca visibilidade, Brasília parece estar operando na categoria CAT I. (" + curitibaWeather.Metar + ")";

                        AlertFilter filterAlert = new AlertFilter(this.Radar, this.Name, "VisibilityCAT1", IconType.AirportWeather);
                        filterAlert.Message = message;
                        filterAlert.TimeToBeDeleted = DateTime.Now.AddHours(3);
                        filterAlert.AlertType = FilterAlertType.High;
                        listAlerts.Add(filterAlert);
                    }

                    // condições do tempo
                    AlertFilter filterAlertWeatherConditition = new AlertFilter(this.Radar, this.Name, "VisibilityCAT1", IconType.AirportWeather);
                    filterAlertWeatherConditition.Group = "filterAlertWeatherConditition";
                    filterAlertWeatherConditition.TimeToBeDeleted = DateTime.Now.AddYears(1);

                    switch (curitibaWeather.WeatherType)
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
                            filterAlertWeatherConditition.ID = this.Name + curitibaWeather.WeatherType;
                            filterAlertWeatherConditition.Message = GetWeatherConditionMessage(curitibaWeather.WeatherType, curitibaWeather.Sky, curitibaWeather.Metar);
                            SetEmergencyTime();
                            break;
                        case WeatherType.Haze:
                        case WeatherType.Rain:
                        case WeatherType.WeakRain:
                        case WeatherType.StrongRain:
                        case WeatherType.NeighborRain:
                        case WeatherType.Drizzle:
                            filterAlertWeatherConditition.ID = this.Name + curitibaWeather.WeatherType;
                            filterAlertWeatherConditition.Message = GetWeatherConditionMessage(curitibaWeather.WeatherType, curitibaWeather.Sky, curitibaWeather.Metar);
                            break;
                        default:

                            switch (curitibaWeather.Sky)
                            {
                                case SkyType.Clear:
                                case SkyType.FewClouds:
                                case SkyType.Overcast:
                                case SkyType.SomeCloud:
                                case SkyType.VeryCloudy:
                                    filterAlertWeatherConditition.ID = this.Name + curitibaWeather.Sky;
                                    filterAlertWeatherConditition.Message = GetWeatherConditionMessage(curitibaWeather.WeatherType, curitibaWeather.Sky, curitibaWeather.Metar);
                                    break;
                            }

                            break;



                    }



                    bool isEmergency = emergencyEndTime > DateTime.Now;

                    filterAlertWeatherConditition.AlertType = isEmergency ? FilterAlertType.High : FilterAlertType.Low;

                    if (!String.IsNullOrEmpty(filterAlertWeatherConditition.Message))
                        listAlerts.Add(filterAlertWeatherConditition);


                    if (IsTesting)
                    {
                        for (int i = 0; i < listAlerts.Count; i++)
                        {
                            listAlerts[i].Message += " Nível:" + listAlerts[i].AlertType;
                            listAlerts[i].AlertType = FilterAlertType.Test;
                        }

                    }
                    //if (isFirstTime)
                    //{
                    //    for (int i = 0; i < listAlerts.Count; i++)
                    //    {
                    //        listAlerts[i].AlertType = FilterAlertType.Low;
                    //    }

                    //    isFirstTime = false;

                    //}





                }
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Filter METAR");
            }

            return listAlerts;
        }


        private string GetWeatherConditionMessage(WeatherType weatherType, SkyType Sky, string Metar)
        {
            string message = String.Empty;

            if (weatherType == WeatherType.Tornado)
            {
                message = "ALERTA! Parece estar se formando um tornado! (" + Metar + ")";
            }
            else if (weatherType == WeatherType.VolcanicAsh)
            {
                message = "O metar está falando que tem fumaça vulcanica passando por aqui (" + Metar + ")";
            }
            else if (weatherType == WeatherType.Duststorm)
            {
                message = "Está tendo tempestadade de areia (" + Metar + ")";
            }
            else if (weatherType == WeatherType.Smoke)
            {
                message = "Está tendo muita fumaça perto do aeroporto (" + Metar + ")";
            }
            else if (weatherType == WeatherType.Snow)
            {
                message = "Neve? Tem errado na minha programação... Confere aí. (" + Metar + ")";
            }
            else if (weatherType == WeatherType.Poeira)
            {
                message = "O aeroporto está registrando ter muita poeira (" + Metar + ")";
            }
            else if (weatherType == WeatherType.Poeira)
            {
                message = "O aeroporto está registrando ter muita poeira (" + Metar + ")";
            }
            else if (weatherType == WeatherType.Fog)
            {
                message = "O aeroporto está com neblina (" + Metar + ")";
            }
            else if (weatherType == WeatherType.Haze)
            {
                message = "Névoa seca está tomando de conta do aeroporto (" + Metar + ")";
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
                message = "Está nublado na região. (" + Metar + ")";
            }
            else if (Sky == SkyType.VeryCloudy)
            {
                message = "Há muita nuvem no céu da região. (" + Metar + ")";
            }
            else if (Sky == SkyType.SomeCloud)
            {
                message = "Há algumas nuvens no céu da cidade. (" + Metar + ")";
            }
            else if (Sky == SkyType.FewClouds)
            {
                message = "Há poucas nuvens no céu da cidade. (" + Metar + ")";
            }
            else if (Sky == SkyType.Clear)
            {
                message = "Céu aberto! (" + Metar + ")";
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
