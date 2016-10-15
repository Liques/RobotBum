using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerBotFoundation;

namespace TowerBotLib.Filters
{
    class FilterMetarDF : IFilter
    {
        public bool IsActive { get; set; }
        public bool IsTesting { get; set; }
        public string Name { get; set; }
        DateTime emergencyEndTime = new DateTime();
        public Radar Radar { get; set; }

        public FilterMetarDF()
        {
            Name = "FilterMetarDF";
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

                    AirportWeather brasiliaWeather = AirportWeather.GetWeather(Airport.GetAirportByIata("BSB"));
                    string message = String.Empty;


                    // Mudança de direção de vento
                    if (brasiliaWeather.WindDirection >= 20 && brasiliaWeather.WindDirection < 200)
                    {
                        message = "As pistas em operação são agora as pistas 11L e 11R. " + GetWeatherConditionMessage(brasiliaWeather.WeatherType, brasiliaWeather.Sky, brasiliaWeather.Metar);
                        AlertFilter filterAlert = new AlertFilter(this.Radar, this.Name, "Rwy11", IconType.AirportWeather);
                        filterAlert.Message = message;
                        filterAlert.Group = "RunwayChange";
                        filterAlert.TimeToBeDeleted = DateTime.Now.AddYears(1);
                        filterAlert.AlertType = FilterAlertType.High;
                        listAlerts.Add(filterAlert);

                    }
                    else if ((brasiliaWeather.WindDirection > 200 && brasiliaWeather.WindDirection < 360 || brasiliaWeather.WindDirection > 20 && brasiliaWeather.WindDirection < 0))
                    {
                        message = "As pistas em operação são agora as pistas  29L e 29R. " + GetWeatherConditionMessage(brasiliaWeather.WeatherType, brasiliaWeather.Sky, brasiliaWeather.Metar);
                        AlertFilter filterAlert = new AlertFilter(this.Radar, this.Name, "Rwy29", IconType.AirportWeather);
                        filterAlert.Message = message;
                        filterAlert.Group = "RunwayChange";
                        filterAlert.TimeToBeDeleted = DateTime.Now.AddYears(1);
                        filterAlert.AlertType = FilterAlertType.High;
                        listAlerts.Add(filterAlert);

                    }

                    // Mostrando a visibilidade
                    if (brasiliaWeather.Visibility > -1 && brasiliaWeather.Visibility < 210)
                    {
                        message = "Talvez o aeroporto está fechado, visibilidade muito baixa. (" + brasiliaWeather.Metar + ")";
                        AlertFilter filterAlert = new AlertFilter(this.Radar, this.Name, "VisibilityCAT3", IconType.AirportWeather);
                        filterAlert.Message = message;
                        filterAlert.TimeToBeDeleted = DateTime.Now.AddHours(3);
                        filterAlert.AlertType = FilterAlertType.High;
                        listAlerts.Add(filterAlert);

                    }
                    else if (brasiliaWeather.Visibility > -1 && brasiliaWeather.Visibility < 370)
                    {
                        message = "Baixa visibilidade, Brasília parece estar operando na categoria CAT II. (" + brasiliaWeather.Metar + ")";

                        AlertFilter filterAlert = new AlertFilter(this.Radar, this.Name, "VisibilityCAT2", IconType.AirportWeather);
                        filterAlert.Message = message;
                        filterAlert.TimeToBeDeleted = DateTime.Now.AddHours(3);
                        filterAlert.AlertType = FilterAlertType.High;
                        listAlerts.Add(filterAlert);

                    }
                    else if (brasiliaWeather.Visibility > -1 && brasiliaWeather.Visibility < 370)
                    {
                        message = "Pouca visibilidade, Brasília parece estar operando na categoria CAT I. (" + brasiliaWeather.Metar + ")";

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
                            filterAlertWeatherConditition.ID = this.Name + brasiliaWeather.WeatherType;
                            filterAlertWeatherConditition.Message = GetWeatherConditionMessage(brasiliaWeather.WeatherType, brasiliaWeather.Sky, brasiliaWeather.Metar);
                            SetEmergencyTime();
                            break;
                        case WeatherType.Haze:
                        case WeatherType.Rain:
                        case WeatherType.WeakRain:
                        case WeatherType.StrongRain:
                        case WeatherType.NeighborRain:
                        case WeatherType.Drizzle:
                            filterAlertWeatherConditition.ID = this.Name + brasiliaWeather.WeatherType;
                            filterAlertWeatherConditition.Message = GetWeatherConditionMessage(brasiliaWeather.WeatherType, brasiliaWeather.Sky, brasiliaWeather.Metar);
                            break;
                        default:

                            switch (brasiliaWeather.Sky)
                            {
                                case SkyType.Clear:
                                case SkyType.FewClouds:
                                case SkyType.Overcast:
                                case SkyType.SomeCloud:
                                case SkyType.VeryCloudy:
                                    filterAlertWeatherConditition.ID = this.Name + brasiliaWeather.Sky;
                                    filterAlertWeatherConditition.Message = GetWeatherConditionMessage(brasiliaWeather.WeatherType, brasiliaWeather.Sky, brasiliaWeather.Metar);
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
