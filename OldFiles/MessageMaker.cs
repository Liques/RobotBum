using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerBotFoundationCore;
using TowerBotLibCore.Plugins;

namespace TowerBotLibCore
{
    public class MessageMaker
    {

        enum Place
        {
            Miami,
            Orlando,
            Portugal,
            France,
            Guarulhos,
            Congonhas,
            RioDeJaneiro,
            UnitedStates,
        }


        public enum MesssageCategory
        {
            NoIcon = 0,
            Cruise = 1,
            Landing = 2,
            TakingOff = 3,
            Taxing = 4,
            Orbit = 5,
            Chart = 6,
            AirportWeather = 7,
            Runway = 8,
            TouchAndGo = 9,
            FinalRunway = 8,
        }

        public enum MessageFormat
        {
            Normal,
            OnlyAircraftModel,
            SpecialDescrition,
            AircraftModelFirst,
            FlightAndModel,
            AirplaneUndetified,
        }

        public string Message { get; set; }
        public Radar Radar { get; set; }
        private AirplaneBasic Airplane;

        bool isSuperHighAlert;

        string fullName = String.Empty; // nome de apresentação do avião de acordo com os dados
        string shortName = String.Empty; // nome de apresentação do avião de acordo com os dados, mas o menor possivel
        string fullNamePreposition = String.Empty; // Preposição que vai ligar a frase
        string fullNameWithPrepositionStarting = String.Empty; // Com a primeira letra maiuscula
        string fullNameWithPrepositionMiddle = String.Empty; // Com a primeira letra minuscula
        string shortNameWithPrepositionStarting = String.Empty; // Com a primeira letra maiuscula
        string shortNameWithPrepositionMiddle = String.Empty; // Com a primeira letra minuscula
        string fromPlace = String.Empty; // Frase 'De...'
        string toPlace = String.Empty; // Frase 'Para...'
        string fromPlaceShort = String.Empty; // Frase curta 'De...'
        string toPlaceShort = String.Empty; // Frase curta 'para...'
        string airplaneTypeLongPhrase = String.Empty; // 'Com um Boeing 7...
        string airplaneTypeShortPhrase = String.Empty; // 'Com um B7...
        bool mustBeShort = false;
        int seed = 0;

        string formatedAltitude = String.Empty;

        public MessageMaker(AirplaneBasic airplane, Radar radar, RatificationType ratificationType = RatificationType.NoRatification)
        {
            MakeCurrentMessage(airplane, radar, ratificationType);
        }

        public MessageMaker(AirplaneBasic airplane, Radar radar, int seed, RatificationType ratificationType = RatificationType.NoRatification)
        {
            this.seed = seed;
            MakeCurrentMessage(airplane, radar, ratificationType);
        }

        private void MakeCurrentMessage(AirplaneBasic airplane, Radar radar, RatificationType ratificationType = RatificationType.NoRatification)
        {
            try
            {
                this.Airplane = airplane;
                this.Message = String.Empty;
                this.Radar = radar;

                isSuperHighAlert = HelperPlugin.ListSuperHighAirplanes.Where(s => this.Airplane.AircraftType != null && this.Airplane.AircraftType.ICAO.Contains(s)).Count() > 0;

                formatedAltitude = (this.Airplane.Altitude / 1000).ToString("##.##0ft");

                #region Pegando o nome completo do avião e modelo

                if (airplane.AircraftType.IsValid || !String.IsNullOrEmpty(airplane.AircraftType.Name))
                {
                    var listAirplaneTypeLongPhrase = " modelo " + airplane.AircraftType.Name + ";" +
                                                          ", um " + airplane.AircraftType.Name + ";" +
                                                          ", " + airplane.AircraftType.Name;

                    airplaneTypeLongPhrase = RandomListPhrases(listAirplaneTypeLongPhrase);
                    airplaneTypeShortPhrase = ", " + airplane.AircraftType.ICAO;
                }

                if (!String.IsNullOrEmpty(this.Airplane.SpecialDescription))
                {
                    if (this.Airplane.IsSpecial)
                        fullName = airplane.SpecialDescription + ", o " + this.Airplane.Registration.Name;
                    else
                        fullName = airplane.SpecialDescription + " (" + this.Airplane.Registration.Name + ")";

                    if (!String.IsNullOrEmpty(this.Airplane.FlightName))
                        fullName += ", " + this.Airplane.FlightName;

                    shortName = this.Airplane.Registration.Name;

                    fullNamePreposition = "";
                }
                else if (this.Airplane.Registration.IsValid && this.Airplane.AircraftType.Type == AircraftModel.AirplaneHeavy)
                {
                    if (!String.IsNullOrEmpty(this.Airplane.FlightName))
                        fullName = airplane.AircraftType.Name + ", o " + this.Airplane.FlightName + " (" + this.Airplane.Registration.Name + ")";
                    else
                        fullName = airplane.AircraftType.Name + " (" + this.Airplane.Registration.Name + ")";


                    shortName = this.Airplane.FlightName;
                    airplaneTypeLongPhrase = "";
                    airplaneTypeShortPhrase = "";

                    if (ratificationType == RatificationType.NoRatification)
                        fullNamePreposition = "Um";
                    else
                        fullNamePreposition = "O";

                }
                else if (!string.IsNullOrEmpty(this.Airplane.FlightName) && this.Airplane.Registration.IsValid && this.Airplane.AircraftType.Type == AircraftModel.Helicopter)
                {
                    fullName = airplane.AircraftType.Name + ", o " + this.Airplane.FlightName + " (" + this.Airplane.Registration.Name + ")";
                    shortName = this.Airplane.FlightName;
                    if (ratificationType == RatificationType.NoRatification)
                        fullNamePreposition = "Um";
                    else
                        fullNamePreposition = "O";
                }
                else if (!string.IsNullOrEmpty(this.Airplane.FlightName) && this.Airplane.Registration.IsValid)
                {
                    fullName = this.Airplane.FlightName + " (" + this.Airplane.Registration.Name + ")";
                    shortName = this.Airplane.FlightName;
                    fullNamePreposition = "O";
                }
                else if (!string.IsNullOrEmpty(this.Airplane.FlightName) && this.Airplane.Registration.IsValid)
                {
                    fullName = this.Airplane.FlightName + " (" + this.Airplane.Registration.Name + ")";
                    shortName = this.Airplane.FlightName;
                    fullNamePreposition = "O";
                }
                else if (!string.IsNullOrEmpty(this.Airplane.Registration.Name))
                {
                    fullName = this.Airplane.Registration.Name;
                    shortName = this.Airplane.Registration.Name;
                    fullNamePreposition = "O";
                }
                else if (this.Airplane.AircraftType.IsValid)
                {
                    fullName = this.Airplane.AircraftType.Name;
                    shortName = this.Airplane.AircraftType.Name;
                    fullNamePreposition = "Um";

                    // Zerando os modelos das maquinas pq já foi estraio.
                    airplaneTypeLongPhrase = String.Empty;
                    airplaneTypeShortPhrase = String.Empty;
                }
                else
                {
                    fullName = "avião não identificado (Hex: " + airplane.ID + ")";
                    shortName = "avião não identificado";
                    fullNamePreposition = "Um";
                }

                if (!String.IsNullOrEmpty(fullNamePreposition))
                {
                    fullNameWithPrepositionStarting = fullNamePreposition + " " + fullName;
                    fullNameWithPrepositionMiddle = fullNamePreposition.ToLower() + " " + fullName;
                    shortNameWithPrepositionStarting = fullNamePreposition + " " + shortName;
                    shortNameWithPrepositionMiddle = fullNamePreposition.ToLower() + " " + shortName;
                }
                else
                {
                    fullNameWithPrepositionStarting = fullName;
                    fullNameWithPrepositionMiddle = fullName[0].ToString().ToLower() + fullName.Substring(1);
                    shortNameWithPrepositionStarting = fullNameWithPrepositionStarting;
                    shortNameWithPrepositionMiddle = fullNameWithPrepositionMiddle;
                }

                if (ratificationType == RatificationType.NoRatification)
                {
                    if (!this.Airplane.IsKnowCountry)
                    {
                        fullNameWithPrepositionStarting += " - " + this.Airplane.Registration.Country + " - ";
                        fullNameWithPrepositionMiddle += "  - " + this.Airplane.Registration.Country + " - ";
                    }

                    if (this.Airplane.FollowingChart != null)
                    {
                        fullNameWithPrepositionStarting += ", seguindo " + this.Airplane.FollowingChart.ChartType + " " + this.Airplane.FollowingChart.Name + ",";
                        fullNameWithPrepositionMiddle += ", seguindo " + this.Airplane.FollowingChart.ChartType + " " + this.Airplane.FollowingChart.Name + ",";
                    }

                    if (!String.IsNullOrEmpty(this.Airplane.RunwayName) && (this.Airplane.State == AirplaneStatus.Landing || this.Airplane.State == AirplaneStatus.TakingOff))
                    {
                        fullNameWithPrepositionStarting += ", pista " + this.Airplane.RunwayName + ",";
                        fullNameWithPrepositionMiddle += ", pista " + this.Airplane.RunwayName + ",";
                    }


                    #endregion

                    #region Criando a parte de From and To
                    string fromPlace = !String.IsNullOrEmpty(airplane.From.City) ? " de " + airplane.From.City : "";
                    string toPlace = !String.IsNullOrEmpty(airplane.To.City) ? " para " + airplane.To.City : "";
                    string fromPlaceShort = !String.IsNullOrEmpty(airplane.From.City) ? " de " + airplane.From.IATA : "";
                    string toPlaceShort = !String.IsNullOrEmpty(airplane.To.City) ? " para " + airplane.To.IATA : "";
                    #endregion

                    if (fullNameWithPrepositionStarting.Length >= 100 || fullNameWithPrepositionMiddle.Length >= 100)
                        mustBeShort = true;

                    if (this.Airplane != null)
                    {
                        if (airplane.State == AirplaneStatus.TakingOff)
                        {
                            Message = GetTakingOffPhrase();

                        }
                        else if (airplane.State == AirplaneStatus.Landing)
                        {
                            Message = GetLandingPhrase();

                        }
                        else if (airplane.State == AirplaneStatus.Cruise)
                        {
                            Message = GetCruisePhrase();
                        }
                        else if (airplane.State == AirplaneStatus.ParkingOrTaxing)
                        {
                            Message = GetParkingTaxiPhrase();
                        }


                        if (Message.Length <= 50)
                        {
                            Message += airplaneTypeLongPhrase;
                        }
                        else
                        {
                            Message += airplaneTypeShortPhrase;
                        }

                        if (airplane.State == AirplaneStatus.Cruise || airplane.State == AirplaneStatus.Landing)
                        {
                            if (Message.Length <= 70)
                            {
                                Message += fromPlace;
                            }
                            else
                            {
                                Message += fromPlaceShort;
                            }
                        }
                        if (airplane.State == AirplaneStatus.Cruise || airplane.State == AirplaneStatus.TakingOff)
                        {
                            if (Message.Length <= 70)
                            {
                                Message += toPlace;
                            }
                            else
                            {
                                Message += toPlaceShort;
                            }
                        }

                        if (airplane.State == AirplaneStatus.ParkingOrTaxing)
                        {
                            string overLocation = HelperPlugin.GetOverLocation(airplane);
                            if (!String.IsNullOrEmpty(overLocation))
                                Message += " no " + overLocation;
                        }
                        else if (Message.Length <= 110)
                        {
                            Message += TowerBotLibCore.Plugins.HelperPlugin.GetForwardLocationsPhrase(this.Airplane, true, 2);
                        }
                        else
                        {
                            Message += TowerBotLibCore.Plugins.HelperPlugin.GetForwardLocationsPhrase(this.Airplane, true);
                        }

                    }
                    Message += ".";

                }
                else if (ratificationType == RatificationType.Chart)
                {
                    this.Message += GetChartPhrase();
                }
                else if (ratificationType == RatificationType.FinalRunway)
                {
                    this.Message += GetRunwayPhrase();
                }
                else if (ratificationType == RatificationType.Orbit)
                {
                    this.Message += GetOrbitPhrase();
                }


                if (Message.Length <= 125 && (airplane.State == AirplaneStatus.Landing || airplane.State == AirplaneStatus.TakingOff))
                {
                    if (this.Radar.Name == "BSB")
                        Message += " #AeroportoBSB";
                    else if (this.Radar.Name == "CWB")
                        Message += " #AeroportoCWB";
                    else if (this.Radar.Name == "GRU")
                        Message += " #AeroportoGRU";
                }

                if (Message.Length <= 131)
                    Message += " #Avião";

                if (Message.Length <= 130)
                    Message += " #RobôBum";

            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Message Maker");

            }
        }

        private void SetAlertaAirplaneMessage()
        {

        }

        private string GetCruisePhrase()
        {
            string phrases =
                fullNameWithPrepositionStarting + " está em cruzeiro" + ";" +
                fullNameWithPrepositionStarting + " está passando por aqui" + ";" +
                fullNameWithPrepositionStarting + " está cruzando a região" + ";" +
                fullNameWithPrepositionStarting + " está voando pela região" + ";" +
                fullNameWithPrepositionStarting + " está voando pela região de " + this.Radar.Description.Replace(" - ", "|").Split('|').First() + " " + ";" +
                fullNameWithPrepositionStarting + " está voando por aqui" + ";" +
                "Está em cruzeiro " + fullNameWithPrepositionMiddle + ";" +

                "Está passando por aqui " + fullNameWithPrepositionMiddle + ";" +
                "Está cruzando a região " + fullNameWithPrepositionMiddle + ";" +
                "Está voando pela região " + fullNameWithPrepositionMiddle + ";" +
                "Está voando pela região de " + this.Radar.Description.Replace(" - ", "|").Split('|').First() + " " + fullNameWithPrepositionMiddle + ";" +
                "Está voando por " + this.Radar.Description.Replace(" - ", "|").Split('|').First() + " " + fullNameWithPrepositionMiddle + ";" +

                //"Está voando por aqui " + fullNameWithPrepositionMiddle + ";" +
                "Na altitude de " + formatedAltitude + " está em cruzeiro " + fullNameWithPrepositionMiddle + ";" +

            "Voando a " + this.Airplane.Speed + " kts está cruzando " + fullNameWithPrepositionMiddle;





            if (this.Radar.Name == "BSB")
            {
                phrases += ";" +
                    "Tah com muita pressa passando por aqui " + fullNameWithPrepositionMiddle + ";" +
                "Nem vai parar aqui para tomar um chá " + fullNameWithPrepositionMiddle + ";" +
                "Nem vai parar aqui para tomar um cafezinho " + fullNameWithPrepositionMiddle + ";" +
                "E aeh 'brodi', alterna aí! Está em cruzeiro " + fullNameWithPrepositionMiddle + ";" +
                "Vei, alterna aí! Está em cruzeiro " + fullNameWithPrepositionMiddle + ";" +
                "Tah pousando... ops, ta em cruzeiro " + fullNameWithPrepositionMiddle + ";" +
                "Voando, voando, voando está em cruzeiro " + fullNameWithPrepositionMiddle + ";" +
                fullNameWithPrepositionStarting + " está passando pela capital" + ";" +
                fullNameWithPrepositionStarting + " está passando pelo DF" + ";" +
                fullNameWithPrepositionStarting + " está voando pelo DF" + ";" +
                "Está passando pela capital " + fullNameWithPrepositionMiddle + ";" +
                "Está passando pelo DF " + fullNameWithPrepositionMiddle + ";" +
                "Está voando pelo DF " + fullNameWithPrepositionMiddle + ";" +
                "Está passando pela capital " + fullNameWithPrepositionMiddle + ";" +
                "Na vertical de Brasília está em cruzeiro " + fullNameWithPrepositionMiddle + ";" +

                "Vendo o DF lá de cima está " + fullNameWithPrepositionMiddle + ";" +
                "Dando um 'High Five' em Brasília está " + fullNameWithPrepositionMiddle + ";" +
                "Dando uma passada rápida pelos céus do DF está " + fullNameWithPrepositionMiddle;

                if (isSuperHighAlert)
                {
                    phrases += ";" +
                    "Alterna! Alterna! Está passando por aqui " + fullNameWithPrepositionMiddle + ";" +
                    "Opa!!! Está passando por aqui " + fullNameWithPrepositionMiddle;
                }
            }
            else if (this.Radar.Name == "CWB")
            {
                phrases += ";" +
                fullNameWithPrepositionStarting + " está passando pelo Paraná" + ";" +
                fullNameWithPrepositionStarting + " está voando pelo Paraná" + ";" +
                fullNameWithPrepositionStarting + " está passando por Curitiba" + ";" +
                fullNameWithPrepositionStarting + " está voando por Curitiba" + ";" +
                "Está passando pelo Paraná " + fullNameWithPrepositionMiddle + ";" +
                "Está voando pelo Paraná " + fullNameWithPrepositionMiddle + ";" +
                "Está passando por Curitiba " + fullNameWithPrepositionMiddle + ";" +
                "Está voando por Curitiba " + fullNameWithPrepositionMiddle + ";" +
                "Na vertical de Curitiba está em cruzeiro " + fullNameWithPrepositionMiddle + ";" +
                "Vendo o Paraná lá de cima está " + fullNameWithPrepositionMiddle + ";" +
                "Vendo Curitiba lá de cima está " + fullNameWithPrepositionMiddle + ";" +
                "Dando um 'High Five' em Curitiba está " + fullNameWithPrepositionMiddle + ";" +
                "Dando uma passada rápida pelos céus do Paraná está " + fullNameWithPrepositionMiddle + ";" +
                "Dando uma passada rápida pelos céus do Curitiba está " + fullNameWithPrepositionMiddle;
            }


            if (IsFromOrToPlace(this.Airplane, Place.UnitedStates))
            {
                phrases += ";" +
                fullNameWithPrepositionStarting + " está em cruzeiro ao Tio Sam" + ";" +
                "Levando gente para fazer compras está em cruzeiro " + fullNameWithPrepositionMiddle + ";" +
                "Levando muambeiros está em cruzeiro " + fullNameWithPrepositionMiddle + ";" +
                "Léts trai óur inglêchi? Está em cruzeiro " + fullNameWithPrepositionMiddle + ";" +
                "Indo ao Tio Sam está em cruzeiro " + fullNameWithPrepositionMiddle + ";" +
                "Levando um monte de turista com dólar está em cruzeiro " + fullNameWithPrepositionMiddle + ";" +
                "Indo levar turistas aos EUA está em cruzeiro " + fullNameWithPrepositionMiddle + ";" +
                "Indo levar turistas aos EUA está em cruzeiro " + fullNameWithPrepositionMiddle + ";" +
                fullNameWithPrepositionStarting + " está em cruzeiro vindo do Tio Sam" + ";" +
                "Cheio de carga dos turistas está o " + fullNameWithPrepositionMiddle + ";" +
                "Cheio de  muambeiro está em cruzeiro " + fullNameWithPrepositionMiddle + ";" +
                "Trazendo centenas de iPads está em cruzeiro " + fullNameWithPrepositionMiddle + ";" +
                "Vindo do Tio Sam em cruzeiro está " + fullNameWithPrepositionMiddle + ";" +
                "Trazendo lembrancinhas dos EUA está em cruzeiro " + fullNameWithPrepositionMiddle + ";" +
                "Com muita gente com medo da Receita, em cruzeiro está " + fullNameWithPrepositionMiddle + ";" +
                "Com turistas de roupas de 'grife', está em cruzeiro " + fullNameWithPrepositionMiddle;
            }



            if (mustBeShort)
            {
                phrases =
                 fullNameWithPrepositionStarting + " está em cruzeiro" + ";" +
                 "Está em cruzeiro " + fullNameWithPrepositionMiddle;
            }



            return RandomListPhrases(phrases);

        }

        private string GetTakingOffPhrase()
        {
            string phrases =
                 fullNameWithPrepositionStarting + " está decolando" + ";" +
                 fullNameWithPrepositionStarting + " está decolando de " + this.Radar.Description.Replace(" - ", "|").Split('|').First() + " " + ";" +
                fullNameWithPrepositionStarting + " está parece estar decolando" + ";" +
                fullNameWithPrepositionStarting + " está decolando do aeroporto" + ";" +

                fullNameWithPrepositionStarting + " está saindo do aeroporto" + ";" +
                fullNameWithPrepositionStarting + " está indo embora da região" + ";" +
                "Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Está decolando de " + this.Radar.Description.Replace(" - ", "|").Split('|').First() + " " + fullNameWithPrepositionMiddle + ";" +
                "Parece estar decolando " + fullNameWithPrepositionMiddle + ";" +
                "Está decolando do aeroporto " + fullNameWithPrepositionMiddle + ";" +
                "Está decolando do aeroporto " + fullNameWithPrepositionMiddle + ";" +
                "Está indo embora da região " + fullNameWithPrepositionMiddle + ";" +
                "Já na altitude de " + formatedAltitude + " está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Já está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Subindo rapidamente, decolando está " + fullNameWithPrepositionMiddle + ";" +
                "Bem rapidinho está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Começando a missão, está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Hora de trabalhar! Está decolando " + fullNameWithPrepositionMiddle;// + ";" +
                                                                                     //"Levando o povo da conexão, está decolando " + fullNameWithPrepositionMiddle;


            if (this.Radar.Name == "BSB")
            {
                phrases += ";" +
                fullNameWithPrepositionStarting + " está decolando de Brasília" + ";" +
                fullNameWithPrepositionStarting + " está indo embora do DF" + ";" +
                "Está decolando de Brasília " + fullNameWithPrepositionMiddle + ";" +
                "Está indo embora do DF " + fullNameWithPrepositionMiddle + ";" +
                "Já na altitude de " + formatedAltitude + " está decolando de Brasília " + fullNameWithPrepositionMiddle;
            }
            else if (this.Radar.Name == "CWB")
            {
                phrases += ";" +
                fullNameWithPrepositionStarting + " está decolando de Curitiba" + ";" +
                "Está decolando de Curitiba " + fullNameWithPrepositionMiddle + ";" +
                "Está indo embora de Curitiba " + fullNameWithPrepositionMiddle + ";" +
                "Está indo embora daqui do Sul " + fullNameWithPrepositionMiddle + ";" +
                "Já na altitude de " + formatedAltitude + " está decolando de Curitiba " + fullNameWithPrepositionMiddle;
            }

            if (IsFromOrToPlace(this.Airplane, Place.UnitedStates))
            {
                phrases += ";" +
                fullNameWithPrepositionStarting + " está decolando ao Tio Sam" + ";" +
                //"Com muitas malas vazias para encher de compras, está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Com muitas malas só com um travesseiro para encher de compras, está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Levando muambeiros está decolando do aero " + fullNameWithPrepositionMiddle + ";" +
                "Indo ao Tio Sam " + fullNameWithPrepositionMiddle + ";" +
                "Decolando com um monte de turista bonado está " + fullNameWithPrepositionMiddle + ";" +
                "Tchau pobres! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Podia ter um voo para NY...  Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Podia voltar o voo para Atlanta...  Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Já pensou BSB - LAX?  Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Com passageiros que pagaram caro pelo visto, está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Com passageiros que estão com medo de serem barrados, está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Levando turistas aos EUA " + fullNameWithPrepositionMiddle;
            }
            else if (IsFromOrToPlace(this.Airplane, Place.Miami))
            {
                phrases += ";" +
                fullNameWithPrepositionStarting + " está indo para Miami" + ";" +
                fullNameWithPrepositionStarting + " está indo para Flórida" + ";" +
                fullNameWithPrepositionStarting + " está indo para os Outlets" + ";" +
                "Vindo da Meca das compras, está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Vindo de Miami " + fullNameWithPrepositionMiddle + ";" +
                "Vindo dos Outlets " + fullNameWithPrepositionMiddle + ";" +
                "Todo mundo quer pra Miami! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Lá pousa A380! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Me Leva! Quero conhecer Miami Beach! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Vindo da Flórida " + fullNameWithPrepositionMiddle + ";";
            }
            else if (IsFromOrToPlace(this.Airplane, Place.Orlando))
            {
                phrases += ";" +
                fullNameWithPrepositionStarting + " está chegando de Orlando" + ";" +
                fullNameWithPrepositionStarting + " está chegando da Flórida" + ";" +
                fullNameWithPrepositionStarting + " está chegando da Disney" + ";" +
                "Vindo da Disney " + fullNameWithPrepositionMiddle + ";" +
                "Franceses não estão decolando o " + fullNameWithPrepositionMiddle + ";" +
                "Viva o Pateta! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Viva o Pato Donald! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Viva o Tio Patinhas! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Viva a Margida! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Viva ao Gastão! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Viva ao Woddy (Toy Story)! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Viva ao Buzz (Toy Story)! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Viva ao Hugo, Zezinho e Luizinho! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Viva a Elsa (Frozen)! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Indo sentir cheiro da baunilha da Disney está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Com passageiros que vão de gritar nas montanhas russas está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Com passageiros que vão cantar 'Let It Go' está decolando " + fullNameWithPrepositionMiddle + ";" +
                "'Let it go, Let it goooooo....' Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Pensa num avião lotado de muleque? Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Pensa num avião lotado de criança? Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Indo tirar fotos dos fogos da Disney está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Viva o Mickey Mouse! " + fullNameWithPrepositionMiddle + ";";
            }
            else if (IsFromOrToPlace(this.Airplane, Place.Portugal))
            {
                phrases += ";" +
                fullNameWithPrepositionStarting + " está decolando para Portugal" + ";" +
                "Ora pois! Está decolando para Portugal " + fullNameWithPrepositionMiddle + ";" +
                "Portugueses estão decolando " + fullNameWithPrepositionMiddle + ";" +
                "Levando brasileiros a portugal, decolando está " + fullNameWithPrepositionMiddle + ";" +
                "Indo para a terra de Cabral, está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Com passageiros que tem medo de falar inglês, está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Com passageiros que vão pra Lisboa só pra dizer que foi, está decolando " + fullNameWithPrepositionMiddle + ";" +
                "TAP, te amamos! Está decolando " + fullNameWithPrepositionMiddle + ";" +

                "Boa viagem para quem fizer conexão em Lisboa e ficar por lá mermo! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Boa viagem para quem fizer conexão em Lisboa em direção a Dubai! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Boa viagem para quem fizer conexão em Lisboa em direção a Paris! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Boa viagem para quem fizer conexão em Lisboa em direção a Cingapura! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Boa viagem para quem fizer conexão em Lisboa em direção na Síria! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Boa viagem para quem fizer conexão em Lisboa em direção a Porto! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Boa viagem para quem fizer conexão em Lisboa em direção a qualquer lugar! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Indo atravessar o Atlântico, está decolando " + fullNameWithPrepositionMiddle;
            }
            else if (IsFromOrToPlace(this.Airplane, Place.France))
            {
                phrases += ";" +
                fullNameWithPrepositionStarting + " está decolando para França" + ";" +
                "Com várias miniaturas da Torre Eiffel, está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Franceses estão decolando " + fullNameWithPrepositionMiddle + ";" +
                "Levando brasileiros para França, decolando está " + fullNameWithPrepositionMiddle + ";" +
                "Au Revoir! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Com muuuuitos casais a bordo, está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Com poucos solteiros a bordo, está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Miga, que chiiiquêêêê!!! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Com alguns passageiros que só vão tomar banho quando voltarem, está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Com alguns passageiros que só vão fazer conexão em Paris, está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Boa viagem para quem fizer conexão em Paris e ficar por lá mermo! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Boa viagem para quem fizer conexão em Paris em direção a Pequim (北京)! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Boa viagem para quem fizer conexão em Paris em direção a Shanguai (上海)! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Boa viagem para quem fizer conexão em Paris em direção a Hong Kong! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Boa viagem para quem fizer conexão em Paris em direção a Tokyo (東京)! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Boa viagem para quem fizer conexão em Paris em direção ao Iraque! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Boa viagem para quem fizer conexão em Paris em direção a qualquer lugar! Está decolando " + fullNameWithPrepositionMiddle + ";" +
                "Indo atravessar o Atlântico, está decolando " + fullNameWithPrepositionMiddle;
            }

            if (mustBeShort)
            {
                phrases =
                 fullNameWithPrepositionStarting + " está decolando" + ";" +
                 "Está decolando " + fullNameWithPrepositionMiddle;
            }

            List<string> lstPhrases = phrases.Split(';').ToList();
            string phrase = RandomListPhrases(lstPhrases);

            bool isSuperHighAlert = HelperPlugin.ListSuperHighAirplanes.Where(s => this.Airplane.AircraftType != null && this.Airplane.AircraftType.ICAO.Contains(s)).Count() > 0;

            if (isSuperHighAlert)
                phrase = "ATENÇÃO: " + phrase;

            return phrase;


        }


        private string GetLandingPhrase()
        {
            string phrases =
                 fullNameWithPrepositionStarting + " está em aproximação" + ";" +
                "Está em aproximação " + fullNameWithPrepositionMiddle + ";" +
                fullNameWithPrepositionStarting + " está na aproximação da região" + ";" +
                "Está na aproximação da região " + fullNameWithPrepositionMiddle + ";" +

             fullNameWithPrepositionStarting + " está pousando" + ";" +
             fullNameWithPrepositionStarting + " está pousando em " + this.Radar.Description.Replace(" - ", "|").Split('|').First() + " " + ";" +
            fullNameWithPrepositionStarting + " está parece estar pousando" + ";" +
            fullNameWithPrepositionStarting + " está descendo para o aeroporto" + ";" +
            fullNameWithPrepositionStarting + " está chegando ao aeroporto" + ";" +
            fullNameWithPrepositionStarting + " está na aproximação da região" + ";" +
            fullNameWithPrepositionStarting + " vai pousar já já" + ";" +
            fullNameWithPrepositionStarting + " vai pousar daqui a pouco" + ";" +
            fullNameWithPrepositionStarting + " vai pousar logo mais" + ";" +
            fullNameWithPrepositionStarting + " vai pousar em breve" + ";" +
            fullNameWithPrepositionStarting + " vai pousar em alguns minutos" + ";" +
            fullNameWithPrepositionStarting + " vai pousar no aeroporto" + ";" +
            fullNameWithPrepositionStarting + " vai chegando no aeroporto" + ";" +
            fullNameWithPrepositionStarting + " está perto de pousar no aeroporto" + ";" +
            "Está em pousando " + fullNameWithPrepositionMiddle + ";" +
            "Está em pousando em " + this.Radar.Description.Replace(" - ", "|").Split('|').First() + " " + fullNameWithPrepositionMiddle + ";" +
            "Parece estar pousando " + fullNameWithPrepositionMiddle + ";" +
            "Está descendo para o aeroporto " + fullNameWithPrepositionMiddle + ";" +
            "Está chegando ao aeroporto " + fullNameWithPrepositionMiddle + ";" +
            "Vai pousar já já " + fullNameWithPrepositionMiddle + ";" +
            "Vai pousar daqui a pouco " + fullNameWithPrepositionMiddle + ";" +
            "Vai pousar logo mais " + fullNameWithPrepositionMiddle + ";" +
            "Vai pousar em breve " + fullNameWithPrepositionMiddle + ";" +
            "Vai pousar em alguns minutos " + fullNameWithPrepositionMiddle + ";" +
            "Vai pousar no aeroporto " + fullNameWithPrepositionMiddle + ";" +
            "Vai chegando no aeroporto " + fullNameWithPrepositionMiddle + ";" +

            "Na altitude de " + formatedAltitude + " está pousando " + fullNameWithPrepositionMiddle + ";" +
            "Já em aproximação está chegando " + fullNameWithPrepositionMiddle + ";" +
            "Descendo aos poucos está pousando " + fullNameWithPrepositionMiddle + ";" +
            "Bem rapidinho está pousando " + fullNameWithPrepositionMiddle + ";" +
            "Depois de um bom voo, está pousando " + fullNameWithPrepositionMiddle + ";" +
            "Cumprindo a missão, está pousando " + fullNameWithPrepositionMiddle + ";" +
            "Chegando a hora de descansar! Está pousando " + fullNameWithPrepositionMiddle + ";" +
            "Trazendo gente para conexão, está pousando " + fullNameWithPrepositionMiddle;

            if (this.Radar.Name == "BSB")
            {
                phrases += ";" +
                fullNameWithPrepositionStarting + " está descendo em Brasília" + ";" +
                fullNameWithPrepositionStarting + " está na aproximação do DF" + ";" +
                "Está descendo em Brasília " + fullNameWithPrepositionMiddle + ";" +
                "Na altitude de " + formatedAltitude + " está chegando em Brasília " + fullNameWithPrepositionMiddle + ";" +
                "Está na aproximação do DF " + fullNameWithPrepositionMiddle;
            }
            else if (this.Radar.Name == "CWB")
            {
                phrases += ";" +
                fullNameWithPrepositionStarting + " está descendo em Curitiba" + ";" +
                fullNameWithPrepositionStarting + " está na aproximação de Curitiba" + ";" +
                "Está descendo em Curitiba " + fullNameWithPrepositionMiddle + ";" +
                "Na altitude de " + formatedAltitude + " está chegando em Curitiba " + fullNameWithPrepositionMiddle + ";" +
                "Está na aproximação de Curitiba " + fullNameWithPrepositionMiddle;
            }

            if (IsFromOrToPlace(this.Airplane, Place.UnitedStates))
            {
                phrases += ";" +
                fullNameWithPrepositionStarting + " está em chegando do Tio Sam" + ";" +
                "Trazendo um monte de compras, está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Trazendo muambeiros está chegando ao aero " + fullNameWithPrepositionMiddle + ";" +
                "Depois de praticar o 'ingrêis', está chegando a Brasília " + fullNameWithPrepositionMiddle + ";" +
                "Chegando do Tio Sam " + fullNameWithPrepositionMiddle + ";" +
                "Chegando com um monte de turista zerado está " + fullNameWithPrepositionMiddle + ";" +

                "Com novos pobres está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com turistas zeraaaaados está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Podia chegar um voo de Nashville!  Está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Podia ter um voo vindo de São Francisco...  Está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Já pensou BSB - LAX?  Está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com passageiros que não sabem quando vão voltar, está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com passageiros já de pé pra descer, está pousando " + fullNameWithPrepositionMiddle + ";" +
                "There is a flight landing right now from USA called " + fullNameWithPrepositionMiddle + ";" +
                "Pousando com turistas dos EUA " + fullNameWithPrepositionMiddle;
            }
            else if (IsFromOrToPlace(this.Airplane, Place.Miami))
            {
                phrases += ";" +
                fullNameWithPrepositionStarting + " está chegando de Miami" + ";" +
                fullNameWithPrepositionStarting + " está chegando da Flórida" + ";" +
                fullNameWithPrepositionStarting + " está chegando dos Outlets" + ";" +
                "Vindo da Meca das compras, está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Vindo de Miami está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Vindo dos Outlets está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Vindo da Flórida está pousando " + fullNameWithPrepositionMiddle + ";";
            }
            else if (IsFromOrToPlace(this.Airplane, Place.Orlando))
            {
                phrases += ";" +
                fullNameWithPrepositionStarting + " está chegando de Orlando" + ";" +
                fullNameWithPrepositionStarting + " está chegando da Flórida" + ";" +
                fullNameWithPrepositionStarting + " está chegando da Disney" + ";" +
                "Vindo da Disney está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Franceses não estão pousando o " + fullNameWithPrepositionMiddle + ";" +
                "Vindo de Miami está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Vindo da Flórida está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Viva o Pateta! Está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Viva o Pato Donald! Está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Viva o Tio Patinhas! Está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Viva a Margida! Está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Viva ao Gastão! Está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Viva ao Woddy (Toy Story)! Está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Viva ao Buzz (Toy Story)! Está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Viva ao Hugo, Zezinho e Luizinho! Está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Viva a Elsa (Frozen)! Está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com saudade do cheiro da baunilha da Disney está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com passageiros rocos de gritar nas montanhas russas está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com passageiros rocos de cantar 'Let It Go' está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Pensa num avião lotado de muleque? Está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Pensa num avião lotado de criança? Está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com fotos dos fogos da Disney está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Viva o Mickey Mouse! Está pousando " + fullNameWithPrepositionMiddle + ";";
            }
            else if (IsFromOrToPlace(this.Airplane, Place.Portugal))
            {
                phrases += ";" +
                fullNameWithPrepositionStarting + " está chegando de Portugal" + ";" +
                "Ora pois! Está chegando de Portugal " + fullNameWithPrepositionMiddle + ";" +
                "Manuel da padaria está nesse voo! Está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Portugueses estão pousando " + fullNameWithPrepositionMiddle + ";" +
                "Trazendo brasileiros de portugal, pousando está " + fullNameWithPrepositionMiddle + ";" +
                "Trazendo lembrancinhas de Portugal, está chegando " + fullNameWithPrepositionMiddle + ";" +
                "Depois de atravessar o Atlântico, está pousando " + fullNameWithPrepositionMiddle;
            }
            else if (IsFromOrToPlace(this.Airplane, Place.France))
            {
                phrases += ";" +
                fullNameWithPrepositionStarting + " está chegando da França" + ";" +
                "Com várias miniaturas da Torre Eiffel, está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Franceses estão pousando " + fullNameWithPrepositionMiddle + ";" +
                "Trazendo brasileiros da França, pousando está " + fullNameWithPrepositionMiddle + ";" +
                "Bienvenue à Brasília! Está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Será que alguém trouxe baguete? Está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com alguns passageiros que tomaram pouco banho, está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com poucos passageiros solteiros, está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Ah 'parri'! está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com passageiros que estão voltando da lua de mel (e que não conseguiram first) está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com passageiros que estão voltando da lua de mel (conseguiram first) está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com passageiros que estão voltando de uma viagem romântica está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com passageiros que estão voltando que quebraram o pau em Paris está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com passageiros bonados que casaram em Paris está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com o avião sujinho está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com passageiros que passageiros que vão falar de Paris por meses, está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com passageiros que fizeram conexão e lamentam não ter conhecido Paris, está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com passageiros 'perfumados' por que tomaram pouco banho, está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com passageiros que vão ficar babacas por alguns meses falando da viagem, está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com alguns passageiros dizendo 'Ai amiga, chegamos na pobreza' está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com alguns passageiros dizendo 'Ai amiga, ui!' está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Com poucos Euros a bordo está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Depois de atravessar o Atlântico, está pousando " + fullNameWithPrepositionMiddle;
            }

            if (!String.IsNullOrEmpty(this.Airplane.From.Country) && this.Airplane.From.Country != "Brasil")
            {
                phrases += ";" +
                fullNameWithPrepositionStarting + " está pousando, vindo de fora" + ";" +
                fullNameWithPrepositionStarting + " está pousando, vindo de outro país" + ";" +
                fullNameWithPrepositionStarting + " está pousando depois de um longo voo" + ";" +
                fullNameWithPrepositionStarting + " está pousando depois de várias horas" + ";" +
                "Com gente tremendo da Receita Federal, está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Vindo de fora do Brasil, está chegando " + fullNameWithPrepositionMiddle + ";" +
                "Concluindo um longo voo, está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Terminando um longo voo, está pousando " + fullNameWithPrepositionMiddle + ";" +
                "Depois de vááárias horas, está pousando " + fullNameWithPrepositionMiddle;
            }

            if (mustBeShort)
            {
                phrases =
                 fullNameWithPrepositionStarting + " está pousando" + ";" +
                 "Está pousando " + fullNameWithPrepositionMiddle;
            }

            List<string> lstPhrases = phrases.Split(';').ToList();
            string phrase = RandomListPhrases(lstPhrases);


            if (isSuperHighAlert)
                phrase = "ATENÇÃO: " + phrase;

            return phrase;


        }

        private string GetParkingTaxiPhrase()
        {
            string phrases =
                 fullNameWithPrepositionStarting + " parece estar no aeroporto" + ";" +
                 fullNameWithPrepositionStarting + " está no aeroporto" + ";" +
                 fullNameWithPrepositionStarting + " está em solo" + ";" +
                 "Está no aeroporto " + fullNameWithPrepositionMiddle + ";" +
                 "Parece estar no aeroporto " + fullNameWithPrepositionMiddle + ";" +
                "Está em solo " + fullNameWithPrepositionMiddle;

            List<string> lstPhrases = phrases.Split(';').ToList();
            string phrase = RandomListPhrases(lstPhrases);

            bool isSuperHighAlert = HelperPlugin.ListSuperHighAirplanes.Where(s => this.Airplane.AircraftType != null && this.Airplane.AircraftType.ICAO.Contains(s)).Count() > 0;

            if (isSuperHighAlert)
                phrase = "ATENÇÃO: " + phrase;

            return phrase;


        }

        private string GetModeSPhrase()
        {
            string phrases = String.Empty;
            if (this.Airplane.Altitude > 17000 && this.Airplane.Altitude < 28000 && (this.Airplane.Weight == AirplaneWeight.Heavy || this.Airplane.Weight == AirplaneWeight.Medium))
            {
                phrases =
                     fullNameWithPrepositionStarting + " está em aproximação (sem ADS-B) na altitude de " + formatedAltitude + " ;" +
                     "Está em aproximação (sem ADS-B) na altitude de " + formatedAltitude + " " + fullNameWithPrepositionMiddle + " ";
            }
            else if (this.Airplane.Altitude < 12000 && (this.Airplane.Weight == AirplaneWeight.Heavy || this.Airplane.Weight == AirplaneWeight.Medium))
            {
                phrases =
                    fullNameWithPrepositionStarting + " está decolando (sem ADS-B) na altitude de " + formatedAltitude + " ;" +
                    "Está decolando (sem ADS-B) na altitude de " + formatedAltitude + " " + fullNameWithPrepositionMiddle + " ";
            }
            else
            {
                double altitudeRemainder = this.Airplane.Altitude % 1000;
                if (altitudeRemainder <= 0.25 && altitudeRemainder >= -0.25)
                {
                    phrases =
                 fullNameWithPrepositionStarting + " está em cruzeiro (sem ADS-B) na altitude de " + formatedAltitude + " ;" +
                 "Está em cruzeiro (sem ADS-B) na altitude de " + formatedAltitude + " " + fullNameWithPrepositionMiddle + " ";
                }
                else
                {
                    phrases =
                     fullNameWithPrepositionStarting + " está na região (sem ADS-B) na altitude de " + formatedAltitude + " ;" +
                     "Está na região (sem ADS-B) na altitude de " + formatedAltitude + " " + fullNameWithPrepositionMiddle + " ";
                }
            }

            List<string> lstPhrases = phrases.Split(';').ToList();
            string phrase = RandomListPhrases(lstPhrases);

            bool isSuperHighAlert = HelperPlugin.ListSuperHighAirplanes.Where(s => this.Airplane.AircraftType != null && this.Airplane.AircraftType.ICAO.Contains(s)).Count() > 0;

            if (isSuperHighAlert && HelperPlugin.IsAirplaneInApproximation(this.Airplane, this.Radar))
                phrase = "ATENÇÃO: " + phrase;

            return phrase;


        }

        private string GetChartPhrase()
        {
            string chartPhrase = this.Airplane.FollowingChart.ChartType.ToString().ToUpper() + " " + this.Airplane.FollowingChart.Name;

            string shortChartPhrase = (this.Airplane.FollowingChart.ChartType == Map.ChartType.SID) ? "o " + chartPhrase : "a " + chartPhrase;

            string phrases =
                 shortNameWithPrepositionStarting + " está seguindo " + shortChartPhrase + ";" +
                // shortNameWithPrepositionStarting + " está seguindo a carta " + chartPhrase + ";" +
                //shortNameWithPrepositionStarting + " está seguindo o procedimento " + chartPhrase + ";" +
                //"Está seguindo o procedimento " + chartPhrase  + " " + shortNameWithPrepositionMiddle + ";" +
                //"Está seguindo a carta " + chartPhrase  + " " + shortNameWithPrepositionMiddle + ";" +
                "Está seguindo " + shortChartPhrase + " " + shortNameWithPrepositionMiddle;

            List<string> lstPhrases = phrases.Split(';').ToList();
            string phrase = RandomListPhrases(lstPhrases);

            if (phrase.Length <= 110)
            {
                phrase += TowerBotLibCore.Plugins.HelperPlugin.GetForwardLocationsPhrase(this.Airplane, true, 2);
            }

            phrase += ". #Chart ";

            if (this.Airplane.FollowingChart.ChartType == Map.ChartType.SID)
            {
                phrase += RandomListPhrases("#flwsBoaViagem;#VolteSempre;#Partiu;#Adios".Split(';').ToList());

            }
            else if (this.Airplane.FollowingChart.ChartType == Map.ChartType.Star)
            {
                phrase += RandomListPhrases("#jahChegaChegando;#BemVindo;#queroVerNaApp;#top;#IFR;#JaJaPousa;#pertoDePousar;#aproximação;#seguindoStar;#seguindoCertinho;#queriaEhTahLah".Split(';').ToList());

            }

            return phrase;
        }

        private string GetRunwayPhrase()
        {


            string phrases = string.Empty;

            if (this.Airplane.State == AirplaneStatus.Landing)
                phrases += shortNameWithPrepositionStarting + " está na final " + this.Airplane.RunwayName + ";" +
                shortNameWithPrepositionStarting + " está na final da pista " + this.Airplane.RunwayName + ";" +
                shortNameWithPrepositionStarting + " está pousando na final " + this.Airplane.RunwayName + ";" +
                 shortNameWithPrepositionStarting + " está pousando na runway " + this.Airplane.RunwayName + ";" +
                 shortNameWithPrepositionStarting + " está descendo na final " + this.Airplane.RunwayName + ";" +
                 shortNameWithPrepositionStarting + " já está baixado e travado na final " + this.Airplane.RunwayName + ";" +
                "Está na final " + this.Airplane.RunwayName + " " + shortNameWithPrepositionMiddle + ";" +
                "Está na final da pista " + this.Airplane.RunwayName + " " + shortNameWithPrepositionMiddle + ";" +
                "Está na final da runway " + this.Airplane.RunwayName + " " + shortNameWithPrepositionMiddle + ";" +
                "Está pousando na final " + this.Airplane.RunwayName + " " + shortNameWithPrepositionMiddle + ";" +
                "Está descendo na final " + this.Airplane.RunwayName + " " + shortNameWithPrepositionMiddle + ";" +
                "Já está baixado e travado na final " + this.Airplane.RunwayName + " " + shortNameWithPrepositionMiddle;
            else if (this.Airplane.State == AirplaneStatus.TakingOff)
                phrases += shortNameWithPrepositionStarting + " decolou da pista " + this.Airplane.RunwayName + ";" +
                "Decolou da pista " + this.Airplane.RunwayName + " " + shortNameWithPrepositionMiddle + " ";
            else if (this.Airplane.State == AirplaneStatus.ParkingOrTaxing)
                phrases += shortNameWithPrepositionStarting + " já está na " + this.Airplane.RunwayName + ";" +
                "Já está na " + this.Airplane.RunwayName + " " + shortNameWithPrepositionMiddle + " ";

            string phrase = RandomListPhrases(phrases);

            phrase += ". #runway ";

            return phrase;
        }

        private string GetOrbitPhrase()
        {
            string phrases = fullNameWithPrepositionStarting + " está orbitando;" +
                "Está orbitando " + fullNameWithPrepositionMiddle;

            List<string> lstPhrases = phrases.Split(';').ToList();
            string phrase = RandomListPhrases(lstPhrases);

            return phrase;
        }

        public string RandomListPhrases(string lstPhrases)
        {
            return RandomListPhrases(lstPhrases.Split(';').ToList());
        }

        public string RandomListPhrases(List<string> lstPhrases)
        {
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



        private bool IsFromOrToPlace(Airplane airplane, Place place)
        {
            switch (place)
            {
                case Place.Miami:
                    if (airplane.FlightName.EndsWith("8042") ||
                        airplane.FlightName.EndsWith("8043") ||
                        airplane.FlightName.EndsWith("AAL213", StringComparison.CurrentCultureIgnoreCase) ||
                        airplane.FlightName.EndsWith("AAL214", StringComparison.CurrentCultureIgnoreCase))
                        return true;
                    break;
                case Place.Orlando:
                    if (airplane.FlightName.EndsWith("8048") ||
                        airplane.FlightName.EndsWith("8049") ||
                        airplane.FlightName.EndsWith("L587", StringComparison.CurrentCultureIgnoreCase) ||
                        airplane.FlightName.EndsWith("L586", StringComparison.CurrentCultureIgnoreCase))
                        return true;
                    break;
                case Place.France:
                    if (airplane.FlightName.EndsWith("R515", StringComparison.CurrentCultureIgnoreCase) ||
                        airplane.FlightName.EndsWith("R520", StringComparison.CurrentCultureIgnoreCase))
                        return true;
                    break;
                case Place.Portugal:
                    if (airplane.FlightName.StartsWith("TAP", StringComparison.CurrentCultureIgnoreCase))
                        return true;
                    break;
                case Place.UnitedStates:
                    return IsFromOrToPlace(airplane, Place.Miami) || IsFromOrToPlace(airplane, Place.Orlando);
                    break;
            }

            return false;
        }


        private string MakeMessage(AirplaneBasic airplane, Radar radar, MesssageCategory category)
        {
            string newMessage = string.Empty;

            try
            {

                MessageFormat messageFormat = MessageFormat.Normal;

                newMessage = String.Empty;

                isSuperHighAlert = HelperPlugin.ListSuperHighAirplanes.Where(s => airplane.AircraftType != null && airplane.AircraftType.ICAO.Contains(s)).Count() > 0;

                formatedAltitude = (airplane.Altitude / 1000).ToString("##.##0ft");

                #region Pegando o nome completo do avião e modelo

                if (airplane.AircraftType.IsValid || !String.IsNullOrEmpty(airplane.AircraftType.Name))
                {
                    messageFormat = MessageFormat.OnlyAircraftModel;

                    var listAirplaneTypeLongPhrase = " modelo " + airplane.AircraftType.Name + ";" +
                                                          ", um " + airplane.AircraftType.Name + ";" +
                                                          ", " + airplane.AircraftType.Name;

                    airplaneTypeLongPhrase = RandomListPhrases(listAirplaneTypeLongPhrase);
                    airplaneTypeShortPhrase = ", " + airplane.AircraftType.ICAO;
                }

                if (!String.IsNullOrEmpty(airplane.SpecialDescription))
                {
                    messageFormat = MessageFormat.SpecialDescrition;

                    if (airplane.IsSpecial)
                        fullName = airplane.SpecialDescription + ", o " + airplane.Registration.Name;
                    else
                        fullName = airplane.SpecialDescription + " (" + airplane.Registration.Name + ")";

                    if (!String.IsNullOrEmpty(airplane.FlightName))
                        fullName += ", " + airplane.FlightName;

                    shortName = airplane.Registration.Name;

                    fullNamePreposition = "";
                }
                else if (airplane.Registration.IsValid && airplane.AircraftType.Type == AircraftModel.AirplaneHeavy)
                {
                    messageFormat = MessageFormat.AircraftModelFirst;

                    if (!String.IsNullOrEmpty(airplane.FlightName))
                        fullName = airplane.AircraftType.Name + ", o " + airplane.FlightName + " (" + airplane.Registration.Name + ")";
                    else
                        fullName = airplane.AircraftType.Name + " (" + airplane.Registration.Name + ")";


                    shortName = airplane.FlightName;
                    airplaneTypeLongPhrase = "";
                    airplaneTypeShortPhrase = "";

                    fullNamePreposition = "O";

                }
                else if (!string.IsNullOrEmpty(airplane.FlightName) && airplane.Registration.IsValid && airplane.AircraftType.Type == AircraftModel.Helicopter)
                {
                    messageFormat = MessageFormat.AircraftModelFirst;

                    fullName = airplane.AircraftType.Name + ", o " + airplane.FlightName + " (" + airplane.Registration.Name + ")";
                    shortName = airplane.FlightName;

                    fullNamePreposition = "Um";

                }
                else if (!string.IsNullOrEmpty(airplane.FlightName) && airplane.Registration.IsValid)
                {
                    messageFormat = MessageFormat.FlightAndModel;

                    fullName = airplane.FlightName + " (" + airplane.Registration.Name + ")";
                    shortName = airplane.FlightName;
                    fullNamePreposition = "O";
                }
                else if (!string.IsNullOrEmpty(airplane.Registration.Name))
                {
                    messageFormat = MessageFormat.Normal;

                    fullName = airplane.Registration.Name;
                    shortName = airplane.Registration.Name;
                    fullNamePreposition = "O";
                }
                else if (airplane.AircraftType.IsValid)
                {
                    messageFormat = MessageFormat.OnlyAircraftModel;

                    fullName = airplane.AircraftType.Name;
                    shortName = airplane.AircraftType.Name;
                    fullNamePreposition = "Um";

                    airplaneTypeLongPhrase = String.Empty;
                    airplaneTypeShortPhrase = String.Empty;
                }
                else
                {
                    messageFormat = MessageFormat.AirplaneUndetified;

                    fullName = "avião não identificado (Hex: " + airplane.ID + ")";
                    shortName = "avião não identificado";
                    fullNamePreposition = "Um";
                }

                // legacy all if
                if (!String.IsNullOrEmpty(fullNamePreposition))
                {
                    fullNameWithPrepositionStarting = fullNamePreposition + " " + fullName;
                    fullNameWithPrepositionMiddle = fullNamePreposition.ToLower() + " " + fullName;
                    shortNameWithPrepositionStarting = fullNamePreposition + " " + shortName;
                    shortNameWithPrepositionMiddle = fullNamePreposition.ToLower() + " " + shortName;
                }
                else
                {
                    fullNameWithPrepositionStarting = fullName;
                    fullNameWithPrepositionMiddle = fullName[0].ToString().ToLower() + fullName.Substring(1);
                    shortNameWithPrepositionStarting = fullNameWithPrepositionStarting;
                    shortNameWithPrepositionMiddle = fullNameWithPrepositionMiddle;
                }

                switch (category)
                {
                    case MesssageCategory.Cruise:
                    case MesssageCategory.TakingOff:
                    case MesssageCategory.Landing:
                    case MesssageCategory.Taxing:


                        if (!airplane.IsKnowCountry)
                        {
                            fullNameWithPrepositionStarting += " - " + airplane.Registration.Country + " - ";
                            fullNameWithPrepositionMiddle += "  - " + airplane.Registration.Country + " - ";
                        }

                        if (airplane.FollowingChart != null)
                        {
                            fullNameWithPrepositionStarting += ", seguindo " + airplane.FollowingChart.ChartType + " " + airplane.FollowingChart.Name + ",";
                            fullNameWithPrepositionMiddle += ", seguindo " + airplane.FollowingChart.ChartType + " " + airplane.FollowingChart.Name + ",";
                        }

                        if (!String.IsNullOrEmpty(airplane.RunwayName) && (airplane.State == AirplaneStatus.Landing || airplane.State == AirplaneStatus.TakingOff))
                        {
                            fullNameWithPrepositionStarting += ", pista " + airplane.RunwayName + ",";
                            fullNameWithPrepositionMiddle += ", pista " + airplane.RunwayName + ",";
                        }


                        #endregion

                        #region Criando a parte de From and To
                        string fromPlace = !String.IsNullOrEmpty(airplane.From.City) ? " de " + airplane.From.City : "";
                        string toPlace = !String.IsNullOrEmpty(airplane.To.City) ? " para " + airplane.To.City : "";
                        string fromPlaceShort = !String.IsNullOrEmpty(airplane.From.City) ? " de " + airplane.From.IATA : "";
                        string toPlaceShort = !String.IsNullOrEmpty(airplane.To.City) ? " para " + airplane.To.IATA : "";
                        #endregion

                        if (fullNameWithPrepositionStarting.Length >= 100 || fullNameWithPrepositionMiddle.Length >= 100)
                            mustBeShort = true;

                        if (airplane != null)
                        {
                            if (airplane.State == AirplaneStatus.TakingOff)
                            {
                                newMessage = GetTakingOffPhrase();

                            }
                            else if (airplane.State == AirplaneStatus.Landing)
                            {
                                newMessage = GetLandingPhrase();

                            }
                            else if (airplane.State == AirplaneStatus.Cruise)
                            {
                                newMessage = GetCruisePhrase();
                            }
                            else if (airplane.State == AirplaneStatus.ParkingOrTaxing)
                            {
                                newMessage = GetParkingTaxiPhrase();
                            }


                            if (newMessage.Length <= 50)
                            {
                                newMessage += airplaneTypeLongPhrase;
                            }
                            else
                            {
                                newMessage += airplaneTypeShortPhrase;
                            }

                            if (airplane.State == AirplaneStatus.Cruise || airplane.State == AirplaneStatus.Landing)
                            {
                                newMessage += fromPlaceShort;

                            }
                            if (airplane.State == AirplaneStatus.Cruise || airplane.State == AirplaneStatus.TakingOff)
                            {
                                newMessage += toPlaceShort;
                            }

                            if (airplane.State == AirplaneStatus.ParkingOrTaxing)
                            {
                                string overLocation = HelperPlugin.GetOverLocation(airplane);
                                if (!String.IsNullOrEmpty(overLocation))
                                    newMessage += " no " + overLocation;
                            }
                            else if (newMessage.Length <= 110)
                            {
                                newMessage += TowerBotLibCore.Plugins.HelperPlugin.GetForwardLocationsPhrase(airplane, true, 2);
                            }
                            else
                            {
                                newMessage += TowerBotLibCore.Plugins.HelperPlugin.GetForwardLocationsPhrase(airplane, true);
                            }

                        }

                        newMessage += ".";

                        break;
                    case MesssageCategory.Chart:
                        newMessage += GetChartPhrase();
                        break;
                    case MesssageCategory.FinalRunway:
                        newMessage += GetRunwayPhrase();
                        break;
                    case MesssageCategory.Orbit:
                        newMessage += GetOrbitPhrase();
                        break;

                }

            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "newMessage Maker");

            }

            return newMessage;
        }


    }
}
