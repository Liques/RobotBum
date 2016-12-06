using System;
using System.Collections.Generic;
using System.Linq;
using TowerBotFoundation;

namespace TowerBotLibrary.Plugins
{
    public class PluginGoodEvening : IPlugin
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsTesting { get; set; }
        public Radar Radar { get; set; }

        public PluginGoodEvening()
        {
            Name = "GoodEve";
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

                    if (Alert.ListOfAlerts == null)
                        return listAlerts;
                    
                    var listOfActiveRadar = Alert.ListOfAlerts.Where(w => w.Radar != null).Select(s => s.Radar.Name).Distinct().ToList();
                    var listOfAlerts24Hours = Alert.ListOfAlerts.Where(w => w.TimeCreated > DateTime.Now.AddDays(-1)).ToList();

                    foreach (var radra in listOfActiveRadar)
                    {
                        try
                        {
                            if (radra == "BRA")
                                continue;

                            var radar = Radar.GetRadar(radra);

                            var listOfAlertByRadar = listOfAlerts24Hours.Where(w => w.Radar.Name == radra).ToList();

                            Alert alert = new Alert(radar, "gdAnun", "", IconType.GoodNightAnnoucement);
                            alert.ID += DateTime.Now.ToString("yyMdHm");
                            alert.CustomMessage = FirstPhrase(radar) + " "
                                                + SecondPhrase(radar) + " "
                                                + ThridPhrase(radar) + " "
                                                + TakeoffsPhrase(radar, listOfAlertByRadar, false) + " "
                                                + LandingsPhrase(radar, listOfAlertByRadar) + " ";

                           
                            alert.CustomMessage += OrbitPhrase(radar, listOfAlertByRadar) + " "
                                                + AdsbPhrase(radar) + " "
                                                + WeatherTomorrowPhrase(radar) + " "
                                                + FinalPhrase(radar);

                            alert.AlertType = PluginAlertType.Test;

                            listAlerts.Add(alert);
                        }
                        catch (Exception e)
                        {
                            string x = "";
                        }
                    }

                }
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Plugin Good Evening");
            }
            return listAlerts;
        }

        private string FirstPhrase(Radar radar)
        {
            string radarName = radar.Description.Replace(" - ", "|").Split('|').First();

            var listPhares = new List<string>()
            {
                "Boa noite!","Olá, booa noite!","Opa, boa noite!","Fala pessoal, boa noite!",
                string.Format("Olá, boa noite {0}!",radarName),
                string.Format("Boa noite {0}!",radarName),
                string.Format("E aí {0}, boa noite!",radarName),
                string.Format("Eu + {0} = S2! Boa noite!",radarName),
            };

            return RandomListPhrases(listPhares);
        }

        private string FinalPhrase(Radar radar)
        {
            string radarName = radar.Description.Replace(" - ", "|").Split('|').First();

            var listPhares = new List<string>()
            {
                "Bom, é isso aí, volto amanhã,",
                "Por hoje é só,",
                "Bom por hoje foi esse o resumo,",
                "Ok galera",
                "Ok pessoal",
                "Ok pessoal",
                "Certo,",
                string.Format("Beleza {0}",radarName),
                string.Format("Bom {0},",radarName),
            };

            var start = RandomListPhrases(listPhares);
            listPhares = new List<string>()
            {
                " boa noite!",
                " boa noite, até amanhã!",
                " até amanhã!",
                " tchau!",
                " tchau, até amanhã!",
                " tchau, volto amanhã!",
                " tchau, volto amanhã!",
                " valeu, até amanhã!",
                " falows, até amanhã!",
            };
            return start + RandomListPhrases(listPhares);

        }


        private string AdsbPhrase(Radar radar)
        {
            if (radar.IsModeSEnabled || radar.Name == "SWUZ" || radar.Name == "SBMT")
            {
                return String.Empty;
            }

            string radarName = radar.Description.Replace(" - ", "|").Split('|').First();

            var listPhares = new List<string>()
            {
                "","","","","","","","","","","","","","","","","","","","","","","","","","",
                "",
                "",
                "Lembrando que aqui em especial eu só detecto aviões com ADS-B.",
                "Lembrando, como sempre, que aqui em especial eu só detecto aviões com ADS-B.",
                "Lembrando eu só detecto aviões com ADS-B, enquanto uma boa alma não me fornecer um radar local.",
                "Lembrando que enquanto uma boa alma não me fornecer um radar local, eu só detecto aviões com ADS-B.",
            };

            return RandomListPhrases(listPhares);
        }

        private string SecondPhrase(Radar radar)
        {
            string radarName = radar.Description.Replace(" - ", "|").Split('|').First();
            var listPhares = new List<string>();

            listPhares.Add(" Como foi o dia de vocês?");
            listPhares.Add(" Tudo bem com vocês?");
            listPhares.Add(" Tudo bem com vocês?");
            listPhares.Add(" Tudo bem com vocês?");
            listPhares.Add(" Tudo bem com vocês?");
            listPhares.Add(" Tudo bem com vocês?");
            listPhares.Add(" Tudo bem com vocês?");
            listPhares.Add(" Tudo bem com vocês?");
            listPhares.Add(" Tudo bem com vocês?");
            listPhares.Add(" Tudo bem com vocês?");
            listPhares.Add(" Tudo bem com vocês?");
            listPhares.Add(" Espero que seu dia tenha sido ótimo!");
            listPhares.Add(" Passei o dia aqui vendo os aviões!");
            listPhares.Add(" Aqui estou eu, como todo dia vendo os aviões!");
            listPhares.Add(" Hoje me deu saudades de spottear, bora combinar um dia?");
            listPhares.Add(" Hoje eu queria contar uma piada, mas sou ruim nisso!");
            listPhares.Add(" Avião é legal né? Ô invenção boa!");
            listPhares.Add(" Tempo que não voo... quero jump!");
            listPhares.Add(" Sabiam que eu sou piloto? Er... brincadeira!");
            listPhares.Add(" Sabiam que a Nasa quem me fez? Er... brincadeira! Sou brazuca!");
            listPhares.Add(" Sabiam que eu nasci em Brasília? A comunidade de spotting da cidade que me fez!");
            listPhares.Add(" Qual boa de hoje?");
            listPhares.Add(" Qual boa de hoje?");



            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                listPhares.Add(" Conseguiu sobreviver a segunda-feira?");
                listPhares.Add(" Venceu a segunda-feira?");
                listPhares.Add(" Começou bem a semana?");
                listPhares.Add(" Que esse semana comece bem para todos nós!");
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                listPhares.Add(" Hoje é seeeexta!");
                listPhares.Add(" Já curtindo a sexta?");
                listPhares.Add(" Já curtindo a sexta? Vou ficar em casa mesmo, kkk.");
                listPhares.Add(" Sexta! Eba!");
                listPhares.Add(" Sexta! Sexta! Sexta!");
                listPhares.Add(" TGIF! TGIF! TGIF!");
                listPhares.Add(" Qual boa dessa sexta?");

            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                listPhares.Add(" Como foi o domingo de vocês?");
            }

            var weather = AirportWeather.GetWeather(radar.MainAirport.ICAO);


            if (!string.IsNullOrEmpty(weather.Metar))
            {
                if (weather.Temperature > 35)
                {
                    listPhares.Add(" Meu deus, que calor terrível é esse?");
                    listPhares.Add(" Meu deus, vou morrer com esse calor!");
                    listPhares.Add(" Gente, não tow aguentando o calor não!");
                    listPhares.Add(" Vou derreter com esse calor!");
                    listPhares.Add(" Eh normal essa calor? Pelo amor!");
                    listPhares.Add(" Socorro! Que calor é esse?");
                    listPhares.Add(" Também desejo boa sorte com esse calor terrível!");

                }
                else if (weather.Temperature > 28)
                {
                    listPhares.Add(" Que calor esse?");
                    listPhares.Add(" Tow derretendo aqui!");
                    listPhares.Add(" Esse calor é normal?");
                    listPhares.Add(" Tow com ventilador aqui.");
                    listPhares.Add(" Quero ar concionado!");
                    listPhares.Add(" Meu ventilador quebrou hoje. :-(");
                    listPhares.Add(" Daqui a pouco vou tomar sorvete, tah quente.");
                    listPhares.Add(" Queria tomar café, mas tah muito quente!");
                    listPhares.Add(" Noites quentes me faz ter vontade de spottear!");
                    listPhares.Add(" Calorzinho tah ótimo para dar um passeio.");
                    listPhares.Add(" Tomei banho de novo, para ver se passa o calor!");
                    listPhares.Add(" Calorzinho esse sô!");
                    listPhares.Add(" Tem piscina aí? Calor ta forte.");
                    listPhares.Add(" Tow aqui tomando um sorvete.");
                    listPhares.Add(" Tow aqui tomando um guaraná bem gelado.");
                    listPhares.Add(" Também desejo boa sorte com esse calor!");


                }
                else if (weather.Temperature > 15)
                {
                    listPhares.Add(" Climinha bom, fresco!");
                    listPhares.Add(" Gosto do clima como está.");
                    listPhares.Add(" Bom esse clima!");

                }
                else if (weather.Temperature > 8)
                {
                    listPhares.Add(" Que frio bom!");
                    listPhares.Add(" Que friozinho bom!");
                    listPhares.Add(" Tow aqui tomando um chocolate quente.");
                    listPhares.Add(" Tow aqui tomando um cafezinho bem quente.");
                    listPhares.Add(" Friozinho ta bom, mas eu queria um pouco mais quente.");
                    listPhares.Add(" Tow aqui, embrulhadinho, só vendo o radar.");
                    listPhares.Add(" Tow aqui, de boa na cama, só vendo o radar.");
                    listPhares.Add(" Também desejo que curta muito esse friozinho!");


                }
                else if (weather.Temperature > 3)
                {
                    listPhares.Add(" Gente, que frio é esse?");
                    listPhares.Add(" Meu deus, que gelo por aqui!");
                    listPhares.Add(" Socorro, sou muito friento!");
                    listPhares.Add(" Preciso de mais roupas para aguentar o frio!");
                    listPhares.Add(" To morrendo, muito frio!");
                    listPhares.Add(" Também boa sorte com esse frio!");



                }
                else if (weather.Temperature != 0 && weather.Temperature <= 3)
                {
                    listPhares.Add(" Que geloooo!");
                    listPhares.Add(" Ta nevando é?");
                    listPhares.Add(" Que geeelo, literalmente!");
                    listPhares.Add(" Também boa sorte com esse frio terrível!");

                }

                if (weather.Sky == SkyType.Clear)
                {
                    listPhares.Add(" Tow aqui fora vendo os aviões em cruzeiro.");
                    listPhares.Add(" Noite bonita hoje hein?");
                    listPhares.Add(" Que céu legal o de hoje!");
                    listPhares.Add(" Céu hoje ta limpinho!");
                    listPhares.Add(" Aliás, tenha uma ótima noite CAVOK!");
                    listPhares.Add(" Céu legal o de hoje. Aliás, tah tendo lua?");
                }
                else if (weather.Sky == SkyType.Overcast || weather.Sky == SkyType.VeryCloudy)
                {
                    listPhares.Add(" Não gosto do céu fechado como tah agora.");
                    listPhares.Add(" Que tanta nuvem é essa no céu agora?");
                    listPhares.Add(" Agora podia ter menos nuvens, eu queria ver mais aviões...");
                    listPhares.Add(" Que tanta nuvem é essa hoje?");
                    listPhares.Add(" Saudade de ver estrelas...");
                    listPhares.Add(" Saudade de um céu CAVOK...");
                }

                switch (weather.WeatherType)
                {
                    case WeatherType.Rain:
                    case WeatherType.NeighborRain:
                    case WeatherType.WeakRain:
                        listPhares.Add(" O que você está achando da chuvinha?");
                        listPhares.Add(" Tah tendo uma chuvinha, mas nada demais.");
                        listPhares.Add(" Dormir com barulho de chuva é muito bom!");
                        listPhares.Add(" Uma chuva vez ou outra não mata ninguém.");
                        listPhares.Add(" Está chovendo em alguns locais daqui.");
                        listPhares.Add(" Tah tendo uma chuvinha, aqui está chovendo.");
                        break;
                    case WeatherType.StrongRain:
                    case WeatherType.VeryStrongRain:
                        listPhares = new List<string>();
                        listPhares.Add(" Que chuva forte é essa agora? Vou ficar atento.");
                        listPhares.Add(" Tomara que a chuva forte não cause problemas... Vou ficar atento.");
                        listPhares.Add(" Nesse momento está chovendo, vou ficar atento ao que está acontecendo.");
                        listPhares.Add(" Tenho medo de chuva forte, igual agora... vou ficar atento.");

                        break;
                    case WeatherType.Fog:
                    case WeatherType.Hail:
                    case WeatherType.Haze:
                        listPhares = new List<string>();
                        listPhares.Add(" A neblina no aeroporto está me preocupando.");
                        listPhares.Add(" Está tendo neblina nesse momento no aerporto.");
                        listPhares.Add(" Neblima + aeroporto não combinam... Está tendo agora no aero.");

                        break;
                }
            }


            return RandomListPhrases(listPhares);
        }

        private string ThridPhrase(Radar radar)
        {
            string radarName = radar.Description.Replace(" - ", "|").Split('|').First();

            var listPhares = new List<string>()
            {
                "Ok,",
                "Ok então,",
                "Ok gente,",
                "Ok galera,",
                "Bom,",
                "Bom pessoal,",
                "Bom galera,",
                "Isso aí,",
                "Legal,",
                "Top,",
                "Certo,",
                "Certo pessoal,",
                "Certo galera,",
            };

            var start = RandomListPhrases(listPhares);

            listPhares = new List<string>()
            {
                "","","","","","","","","","","","","","","","","","","","","","","",
                " chega de lero lero,",
                " hora de trabalhar,",
                " deixa eu pegar os papéis aqui,",
                " hoje tow com pressa,",
                " ainda vou jantar,",
                " ainda vou jogar,",
                " ainda vou estudar,",
            };

            if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)
            {
                listPhares.Add(" hoje ainda vou ver futebol,");
                listPhares.Add(" hoje ainda tem jogo,");
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                listPhares.Add(" ainda vou sair hoje,");
                listPhares.Add(" ainda vou tomar uma já já,");
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                listPhares.Add(" ainda vou descansar,");
                listPhares.Add(" ainda vou dormir mais cedo hoje,");
            }

            var middle = RandomListPhrases(listPhares);

            listPhares = new List<string>()
            {
                " vamos ao resumo do dia.",
                " vamos resumir o dia.",
                " vamos ao resumo diário.",
                " vamos ao resumo de aviões de hoje.",
                " vamos aos reportes de hoje.",
                " vamos aos movimentos de hoje.",
                " vamos ao Jornal do Bum!",
                " vamos ao Jornal do Aeroporto!",
            };


            var end = RandomListPhrases(listPhares);


            var phrase = start + middle + end;


            listPhares = new List<string>()
            {
                "","","","","","","","","","","","",
                phrase
            };
            return RandomListPhrases(listPhares);


        }

        private string GetAirplaneName(AirplaneBasic airplane)
        {
            if (!airplane.Registration.IsValid)
                return null;

            string name = "";

            name = airplane.Registration.Name;

            if (airplane.AircraftType.IsValid)
            {
                name = name + string.Format("/{0}", airplane.AircraftType.Name);
            }

            return name;
        }

        private string TakeoffsPhrase(Radar radar, List<Alert> listOfAlerts, bool forceRadarName)
        {
            string radarName = radar.Description.Replace(" - ", "|").Split('|').First();
            var listPhares = new List<string>();

            var takeoff = listOfAlerts.Count(a => a.Icon == IconType.TakingOff);
            var landing = listOfAlerts.Count(a => a.Icon == IconType.Landing);

            listPhares = new List<string>()
                {

                    "Por aqui hoje",
                    "Nas últimas 24 horas",
                    string.Format("Por {0} hoje",radarName),
                    string.Format("Por {0} nas últimas 24 horas",radarName),

                };

            var start = RandomListPhrases(listPhares);

            if (forceRadarName)
                start = listPhares.Last();

            if (takeoff == 0)
            {
                listPhares = new List<string>()
                {
                    " não teve nenhuma decolagem",
                    " não decolou nenhum avião",
                };

                return start + RandomListPhrases(listPhares);
            }

            string takeoffPhare = string.Empty;


            if (takeoff == 1)
            {
                listPhares = new List<string>()
                {
                    string.Format(" houve uma decolagem"),
                    string.Format(" teve uma decolagem"),
                };

                takeoffPhare = RandomListPhrases(listPhares);

            }
            else if (takeoff > 1)
            {
                listPhares = new List<string>()
                {
                    string.Format(" houve {0} decolagens",takeoff),
                    string.Format(" teve {0} decolagens",takeoff),
                };

                takeoffPhare = RandomListPhrases(listPhares);
            }

            string airplanes = string.Empty;

            var listOfAirplanes = listOfAlerts.Where(a => a.Icon == IconType.TakingOff && a.Airplane != null).Select(s => s.Airplane).ToList();

            if (takeoff != 0 && takeoff <= 3)
            {
                for (int i = 0; i < listOfAirplanes.Count; i++)
                {
                    var airplane = listOfAirplanes[i];

                    if (i == 0)
                        airplanes = GetAirplaneName(airplane);
                    else if (i == listOfAirplanes.Count - 1)
                        airplanes += " e " + GetAirplaneName(airplane);
                    else
                        airplanes += ", " + GetAirplaneName(airplane);

                }
            }

            if (!string.IsNullOrEmpty(airplanes))
                airplanes = string.Format("({0})", airplanes);

            return start + takeoffPhare + airplanes;
        }

        private string LandingsPhrase(Radar radar, List<Alert> listOfAlerts)
        {
            string radarName = radar.Description.Replace(" - ", "|").Split('|').First();
            var listPhares = new List<string>();

            var takeoff = listOfAlerts.Count(a => a.Icon == IconType.TakingOff);
            var landing = listOfAlerts.Count(a => a.Icon == IconType.Landing);

            string start = " e";

            if (landing == 0)
            {
                listPhares = new List<string>()
                {
                    " não teve nenhum pouso.",
                    " não pousou nenhum avião.",
                };

                return start + RandomListPhrases(listPhares);
            }

            string landingPhare = string.Empty;

            if (landing == 1)
            {
                listPhares = new List<string>()
                {
                    string.Format(" houve um pouso"),
                    string.Format(" teve um pouso"),
                };

                landingPhare = RandomListPhrases(listPhares);

            }
            else if (landing > 0)
            {
                listPhares = new List<string>()
                {
                    string.Format(" houve {0} pousos",landing),
                    string.Format(" teve {0} pousos",landing),
                };

                landingPhare = RandomListPhrases(listPhares);

            }



            string airplanes = string.Empty;

            var listOfAirplanes = listOfAlerts.Where(a => a.Icon == IconType.Landing && a.Airplane != null).Select(s => s.Airplane).ToList();

            if (landing != 0 && landing <= 3)
            {
                bool isTheSame = true;

                if (takeoff == landing)
                {
                    var listOfAirplanesTakeoff = listOfAlerts.Where(a => a.Icon == IconType.TakingOff && a.Airplane != null).Select(s => s.Airplane).ToList();

                    if (listOfAirplanesTakeoff.Count == 0)
                        isTheSame = false;
                    else {

                        foreach (var airplaneLanding in listOfAirplanes)
                        {
                            if (!listOfAirplanesTakeoff.Any(a => a.ID == airplaneLanding.ID))
                            {
                                isTheSame = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    isTheSame = false;
                }

                if (!isTheSame)
                {
                    for (int i = 0; i < listOfAirplanes.Count; i++)
                    {
                        var airplane = listOfAirplanes[i];

                        if (i == 0)
                            airplanes = GetAirplaneName(airplane);
                        else if (i == listOfAirplanes.Count - 1)
                            airplanes += " e " + GetAirplaneName(airplane);
                        else
                            airplanes += ", " + GetAirplaneName(airplane);

                    }
                }
                else if (landing == 0)
                {
                    airplanes = "mesmo avião";
                }
                else if (landing > 0)
                {
                    airplanes = "mesmos aviões";
                }
            }

            if (!string.IsNullOrEmpty(airplanes))
                airplanes = string.Format("({0})", airplanes);

            return start + landingPhare + airplanes + ".";
        }

        private string OrbitPhrase(Radar radar, List<Alert> listOfAlerts)
        {
            string radarName = radar.Description.Replace(" - ", "|").Split('|').First();
            var listPhares = new List<string>();

            var orbits = listOfAlerts.Count(a => a.Icon == IconType.Orbit);

            listPhares = new List<string>()
                {
                    "Registrei também",
                    "Registrei",
                    "Detectei",
                    "Detectei também",
                    "Houve também",
                };

            var start = RandomListPhrases(listPhares);

            if (orbits == 0)
            {
                return string.Empty;
            }
            else if (orbits == 1)
            {
                listPhares = new List<string>()
                {
                    " uma órbita.",
                    " um avião orbitando.",
                };

                return start + RandomListPhrases(listPhares);
            }
            else
            {

                listPhares = new List<string>()
                {
                    string.Format("{0} órbitas.", orbits),
                };

                return start + RandomListPhrases(listPhares);

            }

        }

        private string WeatherTomorrowPhrase(Radar radar)
        {
            string radarName = radar.Description.Replace(" - ", "|").Split('|').First();

            var listPhares = new List<string>()
            {
                "A previsão para as próximas 24 horas é",
                "A previsão até amanhã é",
            };

            var start = RandomListPhrases(listPhares); ;


            var weatherTomorrow = AirportWeather.GetWeather(radar.MainAirport.ICAO);

            if (weatherTomorrow.ListFutureWeather != null)
            {
                listPhares = new List<string>();

                if (weatherTomorrow.ListFutureWeather.Any(w => w.Temperature > 35))
                {
                    var temp = weatherTomorrow.ListFutureWeather.Where(w => w.Temperature > 35).First();
                    listPhares.Add(string.Format(" de calor , muito calor ({0} graus a partir das {1} horas)", temp.Temperature, temp.DateBegin.ToString("HH")));
                    listPhares.Add(string.Format(" de esperar muito, mas muito calor ({0} graus a partir das {1} horas)", temp.Temperature, temp.DateBegin.ToString("HH")));
                    listPhares.Add(string.Format(" de muito calor ({0} graus a partir das {1} horas)", temp.Temperature, temp.DateBegin.ToString("HH")));


                }
                else if (weatherTomorrow.ListFutureWeather.Any(w => w.Temperature > 28))
                {
                    var temp = weatherTomorrow.ListFutureWeather.Where(w => w.Temperature > 28).First();

                    listPhares.Add(string.Format(" de enfrentar calor ({0} graus a partir das {1} horas)", temp.Temperature, temp.DateBegin.ToString("HH")));
                    listPhares.Add(string.Format(" de calor ({0} graus a partir das {1} horas)", temp.Temperature, temp.DateBegin.ToString("HH")));

                }
                else if (weatherTomorrow.ListFutureWeather.Any(w => w.Temperature > 15))
                {
                    var temp = weatherTomorrow.ListFutureWeather.Where(w => w.Temperature > 15).First();
                    listPhares.Add(string.Format(" de esperar uma temperatura agradavel ({0} graus a partir das {1} horas)", temp.Temperature, temp.DateBegin.ToString("HH")));
                    listPhares.Add(string.Format(" de clima fresco ({0} graus a partir das {1} horas)", temp.Temperature, temp.DateBegin.ToString("HH")));
                    listPhares.Add(string.Format(" de temperatura amena ({0} graus a partir das {1} horas)", temp.Temperature, temp.DateBegin.ToString("HH")));


                }
                else if (weatherTomorrow.ListFutureWeather.Any(w => w.Temperature > 8))
                {
                    var temp = weatherTomorrow.ListFutureWeather.Where(w => w.Temperature > 8).First();
                    listPhares.Add(string.Format(" de esperar frio ({0} graus a partir das {1} horas)", temp.Temperature, temp.DateBegin.ToString("HH")));
                    listPhares.Add(string.Format(" de previsão de frio ({0} graus a partir das {1} horas)", temp.Temperature, temp.DateBegin.ToString("HH")));
                }
                else if (weatherTomorrow.ListFutureWeather.Any(w => w.Temperature > 3))
                {
                    var temp = weatherTomorrow.ListFutureWeather.Where(w => w.Temperature > 3).First();
                    listPhares.Add(string.Format(" de esperar muito frio ({0} graus a partir das {1} horas)", temp.Temperature, temp.DateBegin.ToString("HH")));
                    listPhares.Add(string.Format(" de previsão de muito frio ({0} graus a partir das {1} horas)", temp.Temperature, temp.DateBegin.ToString("HH")));

                }

                var temperature = RandomListPhrases(listPhares); ;

                listPhares = new List<string>();


                if (weatherTomorrow.ListFutureWeather.Any(w => w.Sky == SkyType.Clear))
                {
                    listPhares.Add(" de céu sem nuvens");
                }
                else if (weatherTomorrow.ListFutureWeather.Any(w => w.Sky == SkyType.Overcast || w.Sky == SkyType.VeryCloudy))
                {
                    listPhares.Add(" de céu encoberto");
                }

                var sky = RandomListPhrases(listPhares);

                string time = ".";

                listPhares = new List<string>();

                if (weatherTomorrow.ListFutureWeather.Any(w => w.WeatherType == WeatherType.Rain ||
                                                            w.WeatherType == WeatherType.NeighborRain ||
                                                            w.WeatherType == WeatherType.WeakRain))
                {
                    listPhares.Add(" de chuva");

                    time = WeatherTimeFinalWords(weatherTomorrow.ListFutureWeather.Where(w => w.WeatherType == WeatherType.Rain ||
                                                            w.WeatherType == WeatherType.NeighborRain ||
                                                            w.WeatherType == WeatherType.WeakRain).ToList());
                }
                else if (weatherTomorrow.ListFutureWeather.Any(w => w.WeatherType == WeatherType.StrongRain ||
                                                          w.WeatherType == WeatherType.VeryStrongRain))
                {
                    listPhares.Add(" de chuva forte");
                    listPhares.Add(" de muita chuva");
                    listPhares.Add(" de também previsão de chuva forte");
                    listPhares.Add(" de também previsão de muita chuva");

                    time = WeatherTimeFinalWords(weatherTomorrow.ListFutureWeather.Where(w => w.WeatherType == WeatherType.StrongRain ||
                                                          w.WeatherType == WeatherType.VeryStrongRain).ToList());
                }
                else if (weatherTomorrow.ListFutureWeather.Any(w => w.WeatherType == WeatherType.Fog ||
                                                          w.WeatherType == WeatherType.Hail ||
                                                          w.WeatherType == WeatherType.Haze))
                {
                    listPhares.Add(" de neblina");

                    time = WeatherTimeFinalWords(weatherTomorrow.ListFutureWeather.Where(w => w.WeatherType == WeatherType.Fog ||
                                                          w.WeatherType == WeatherType.Hail ||
                                                          w.WeatherType == WeatherType.Haze).ToList());

                }
                else
                {
                    listPhares.Add(" de dia sem chuvas");
                    listPhares.Add(" de dia sem chuvas");

                }

                var finalWeather = RandomListPhrases(listPhares);

                if (!String.IsNullOrEmpty(temperature) || !String.IsNullOrEmpty(sky))
                {
                    finalWeather = " e " + finalWeather;
                }
                else if (!String.IsNullOrEmpty(temperature) && !String.IsNullOrEmpty(sky))
                {
                    sky = " e " + sky;
                }


                return start + temperature + sky + finalWeather + time;

            }

            return string.Empty;


        }

        private string WeatherTimeFinalWords(List<AirportWeather> listWeather)
        {
            if (listWeather == null)
                return ".";
            else if (listWeather.Count == 0)
                return ".";

            string word = string.Empty;

            double hoursOfEvent = 24;
            AirportWeather weatherWithLessHourEvent = null;

            foreach (var weather in listWeather)
            {
                var hoursOfThisEvent = (weather.DateEnd - weather.DateBegin).TotalHours;
                if (hoursOfThisEvent < hoursOfEvent)
                {
                    hoursOfEvent = hoursOfThisEvent;
                    weatherWithLessHourEvent = weather;
                }
            }

            if (weatherWithLessHourEvent == null)
                weatherWithLessHourEvent = listWeather.First();

            var listPhares = new List<string>();

            if (weatherWithLessHourEvent.DateBegin.Hour < 5)
            {
                listPhares.Add(" pela madrugada.");
                listPhares.Add(" na madrugada.");
            }
            else if (weatherWithLessHourEvent.DateBegin.Hour < 8)
            {
                listPhares.Add(" ao amanhecer.");
                listPhares.Add(" ao nascer do sol.");
                listPhares.Add(" ao iniciar.");
            }
            else if (weatherWithLessHourEvent.DateBegin.Hour < 11)
            {
                listPhares.Add(" pela manhã.");
                listPhares.Add(" de manhã.");
            }
            else if (weatherWithLessHourEvent.DateBegin.Hour < 13)
            {
                listPhares.Add(" meio dia.");
                listPhares.Add(" a partir do meio dia.");
            }
            else if (weatherWithLessHourEvent.DateBegin.Hour < 17)
            {
                listPhares.Add(" pela tarde.");
                listPhares.Add(" a tarde.");
            }
            else if (weatherWithLessHourEvent.DateBegin.Hour < 19)
            {
                listPhares.Add(" no fim da tarde.");
                listPhares.Add(" no por do sol.");
                listPhares.Add(" ao terminar a tarde.");
            }
            else if (weatherWithLessHourEvent.DateBegin.Hour < 23 && weatherWithLessHourEvent.DateBegin.Day != DateTime.Now.Day)
            {
                listPhares.Add(" a noite.");
                listPhares.Add(" pela noite.");
            }
            else if (weatherWithLessHourEvent.DateBegin.Hour < 23 && weatherWithLessHourEvent.DateBegin.Day != DateTime.Now.Day)
            {
                listPhares.Add(" agora a noite.");
            }


            return RandomListPhrases(listPhares);


        }


        public string RandomListPhrases(string lstPhrases)
        {
            return RandomListPhrases(lstPhrases.Split(';').ToList());
        }

        public string RandomListPhrases(List<string> lstPhrases)
        {
            int seed = 0;
            int dateUtcNow = Convert.ToInt32(DateTime.UtcNow.ToString("ddMMyyyyy"));
            int timeUtcNow = Convert.ToInt32(DateTime.UtcNow.ToString("HHmmss"));
            if (seed == 0)
                seed = dateUtcNow + timeUtcNow;
            Random random = new Random(seed);
            double nextDouble = random.NextDouble();
            int sorteNumber = Convert.ToInt32(Math.Round(nextDouble * lstPhrases.Count));
            // por conta de bug nao encontrado, existe o if no código abaixo
            if (sorteNumber >= lstPhrases.Count - 1)
                return lstPhrases.LastOrDefault();
            else if (sorteNumber < 0)
                return lstPhrases.FirstOrDefault();
            else
                return lstPhrases[sorteNumber];
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
