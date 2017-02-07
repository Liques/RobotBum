using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RobotBumFoundationCore;
using RobotBumLibCore.Plugins;
using System.Text.RegularExpressions;

namespace RobotBumLibCore
{
    public class MessageMaker
    {

        static List<string> listOfArticles = new List<string>();
        static List<string> listOfIndefinitiveArticles = new List<string>();
        static string notIdentifiedMessage = String.Empty;
        static List<string> listOfFromMessages = new List<string>();
        static List<string> listOfToMessages = new List<string>();
        static List<string> listOfToMessagesTakingOffPhraseStart = new List<string>();
        static List<string> listOfToMessagesTakingOffPhraseEnd = new List<string>();
        static List<string> listOfToMessagesCruisePhraseStart = new List<string>();
        static List<string> listOfToMessagesCruisePhraseEnd = new List<string>();

        static List<string> listOfToMessagesLandingPhraseStart = new List<string>();
        static List<string> listOfToMessagesLandingPhraseEnd = new List<string>();

        static List<string> listOfToMessagesTaxingPhraseStart = new List<string>();
        static List<string> listOfToMessagesTaxingPhraseEnd = new List<string>();
        static List<string> listOfToMessagesOrbit = new List<string>();


        public string Message { get; set; }
        public Radar Radar { get; set; }
        private AirplaneBasic Airplane;

        string airplaneRegistrationOrModel = String.Empty; // nome de apresentação do avião de acordo com os dados
        string article = String.Empty;
        string indefinitiveArticle = String.Empty;
        string phraseBeginningWithAirplaneRegistration = String.Empty; // Com a primeira letra maiuscula
        string phraseEndingWithAirplaneRegistration = String.Empty; // Com a primeira letra minuscula
        string fromPlace = String.Empty; // Frase 'De...'
        string toPlace = String.Empty; // Frase 'Para...'
        string airplaneTypeLongPhrase = String.Empty; // 'Com um Boeing 7...
        string formatedAltitude = String.Empty;

        public static string CustomLanguageCode = String.Empty;

        static MessageMaker()
        {
            LoadMessages();
        }

        public static void LoadMessages()
        {

            string messageLanguageFolder = MultiOSFileSupport.ResourcesFolder + MultiOSFileSupport.Splitter + "messages" + MultiOSFileSupport.Splitter;
            string messageLanguageFileName = CultureInfo.CurrentCulture.Name + ".json";

            if (String.IsNullOrEmpty(CustomLanguageCode))
            {

                if (!File.Exists(messageLanguageFolder + messageLanguageFileName))
                {
                    messageLanguageFileName = CultureInfo.CurrentCulture.TwoLetterISOLanguageName + ".json";
                    if (!File.Exists(messageLanguageFolder + messageLanguageFileName))
                    {
                        messageLanguageFileName = "en.json";
                        Console.WriteLine("Messages in {0} was not found... But you can make your own translation! :-)", CultureInfo.CurrentCulture.EnglishName);
                    }
                }
            }
            else
            {

                if (String.IsNullOrEmpty(CustomLanguageCode))
                    return;
                messageLanguageFileName = CustomLanguageCode + ".json";
            }

            StreamReader file = File.OpenText(messageLanguageFolder + messageLanguageFileName);

            string jsonstring = file.ReadToEnd();//file.ReadToEnd();
            try
            {
                var listNames = JsonConvert.DeserializeObject<IDictionary<string, List<string>>>(jsonstring);

                listOfArticles = listNames["articles"];
                listOfIndefinitiveArticles = listNames["indefinitiveArticle"];
                listOfFromMessages = listNames["from"];
                listOfToMessages = listNames["to"];
                listOfToMessagesTakingOffPhraseStart = listNames["TakingOffPhraseStart"];
                listOfToMessagesTakingOffPhraseEnd = listNames["TakingOffPhraseEnd"];
                listOfToMessagesCruisePhraseStart = listNames["CruisePhraseStart"];
                listOfToMessagesCruisePhraseEnd = listNames["CruisePhraseEnd"];
                listOfToMessagesLandingPhraseStart = listNames["LandingPhraseStart"];
                listOfToMessagesLandingPhraseEnd = listNames["LandingPhraseEnd"];
                listOfToMessagesTaxingPhraseStart = listNames["TaxingPhraseStart"];
                listOfToMessagesTaxingPhraseEnd = listNames["TaxingPhraseEnd"];
                listOfToMessagesOrbit = listNames["Orbit"];

                notIdentifiedMessage = RandomListPhrases(listNames["notIdentifiedAirplane"]);
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Error loading data from the file messages.json");
            }
        }

        public MessageMaker(AirplaneBasic airplane, Radar radar, RatificationType ratificationType = RatificationType.NoRatification)
        {
            MakeCurrentMessage(airplane, radar, ratificationType);
        }

        public MessageMaker(AirplaneBasic airplane, Radar radar, int seed, RatificationType ratificationType = RatificationType.NoRatification)
        {
            MakeCurrentMessage(airplane, radar, ratificationType, seed);
        }

        private void MakeCurrentMessage(AirplaneBasic airplane, Radar radar, RatificationType ratificationType = RatificationType.NoRatification, int seed = 0)
        {
            try
            {
                this.Airplane = airplane;
                this.Message = String.Empty;
                this.Radar = radar;

                this.article = RandomListPhrases(listOfArticles);
                this.indefinitiveArticle = RandomListPhrases(listOfIndefinitiveArticles);

                #region Setting the airplane name and/or model

                if (airplane.AircraftType.IsValid || !String.IsNullOrEmpty(airplane.AircraftType.Name))
                {
                    airplaneTypeLongPhrase = String.Format(", {0}", airplane.AircraftType.Name);
                }

                if (!String.IsNullOrEmpty(this.Airplane.SpecialDescription))
                {
                    if (this.Airplane.IsSpecial)
                        airplaneRegistrationOrModel = String.Format("{0}, {1} {2}", airplane.SpecialDescription, this.article.ToLower(), this.Airplane.Registration.Name);
                    else
                        airplaneRegistrationOrModel = String.Format("{0}, ({1})", airplane.SpecialDescription, this.Airplane.Registration.Name);

                    if (!String.IsNullOrEmpty(this.Airplane.FlightName))
                        airplaneRegistrationOrModel += String.Format(", {0}", this.Airplane.FlightName);

                    article = String.Empty;
                }
                else if (this.Airplane.Registration.IsValid && this.Airplane.AircraftType.Type == AircraftModel.AirplaneHeavy)
                {
                    if (!String.IsNullOrEmpty(this.Airplane.FlightName))
                        airplaneRegistrationOrModel = String.Format("{0}, {1} ({2})", airplane.AircraftType.Name, this.Airplane.FlightName, this.Airplane.Registration.Name);
                    //airplaneRegistrationOrModel = airplane.AircraftType.Name + ", o " + this.Airplane.FlightName + " (" + this.Airplane.Registration.Name + ")";
                    else
                        airplaneRegistrationOrModel = String.Format("{0}, ({1})", airplane.AircraftType.Name, this.Airplane.Registration.Name);
                    //airplaneRegistrationOrModel = airplane.AircraftType.Name + " (" + this.Airplane.Registration.Name + ")";

                    airplaneTypeLongPhrase = String.Empty;

                    if (ratificationType == RatificationType.NoRatification)
                        article = this.indefinitiveArticle;

                }
                else if (!string.IsNullOrEmpty(this.Airplane.FlightName) && this.Airplane.Registration.IsValid && this.Airplane.AircraftType.Type == AircraftModel.Helicopter)
                {
                    airplaneRegistrationOrModel = String.Format("{0}, {1} {2} ({3})", airplane.AircraftType.Name, this.article.ToLower(), this.Airplane.FlightName, this.Airplane.Registration.Name);
                    if (ratificationType == RatificationType.NoRatification)
                        article = this.indefinitiveArticle;
                }
                else if (!string.IsNullOrEmpty(this.Airplane.FlightName) && this.Airplane.Registration.IsValid)
                {
                    airplaneRegistrationOrModel = String.Format("{0} ({1})", this.Airplane.FlightName, this.Airplane.Registration.Name);
                }
                else if (!string.IsNullOrEmpty(this.Airplane.Registration.Name))
                {
                    airplaneRegistrationOrModel = this.Airplane.Registration.Name;
                }
                else if (this.Airplane.AircraftType.IsValid)
                {
                    airplaneRegistrationOrModel = this.Airplane.AircraftType.Name;
                    article = this.indefinitiveArticle;
                    airplaneTypeLongPhrase = String.Empty;
                }
                else
                {
                    airplaneRegistrationOrModel = String.Format("{0} (Hex: {1})", notIdentifiedMessage, this.Airplane.ID);
                    article = String.Empty;
                }

                if (!String.IsNullOrEmpty(article))
                {
                    phraseBeginningWithAirplaneRegistration = article + " " + airplaneRegistrationOrModel;
                    phraseEndingWithAirplaneRegistration = article.ToLower() + " " + airplaneRegistrationOrModel;
                }
                else
                {
                    phraseBeginningWithAirplaneRegistration = airplaneRegistrationOrModel;
                    phraseEndingWithAirplaneRegistration = airplaneRegistrationOrModel[0].ToString().ToLower() + airplaneRegistrationOrModel.Substring(1);
                }

                if (ratificationType == RatificationType.NoRatification)
                {
                    #region This code was commented for a while, the system is not giving enough support to these features
                    // if (!this.Airplane.IsKnowCountry)
                    // {
                    //     phraseBeginningWithAirplaneRegistration += " - " + this.Airplane.Registration.Country + " - ";
                    //     phraseEndingWithAirplaneRegistration += "  - " + this.Airplane.Registration.Country + " - ";
                    // }

                    // if (this.Airplane.FollowingChart != null)
                    // {
                    //     phraseBeginningWithAirplaneRegistration += ", seguindo " + this.Airplane.FollowingChart.ChartType + " " + this.Airplane.FollowingChart.Name + ",";
                    //     phraseEndingWithAirplaneRegistration += ", seguindo " + this.Airplane.FollowingChart.ChartType + " " + this.Airplane.FollowingChart.Name + ",";
                    // }

                    // if (!String.IsNullOrEmpty(this.Airplane.RunwayName) && (this.Airplane.State == AirplaneStatus.Landing || this.Airplane.State == AirplaneStatus.TakingOff))
                    // {
                    //     phraseBeginningWithAirplaneRegistration += ", pista " + this.Airplane.RunwayName + ",";
                    //     phraseEndingWithAirplaneRegistration += ", pista " + this.Airplane.RunwayName + ",";
                    // }
                    #endregion

                    #endregion

                    #region Setting what is the route of the airplane
                    if (airplane.From.City != airplane.To.City)
                    {
                        string fromPlace = !String.IsNullOrEmpty(airplane.From.City) ? String.Format(" {0} {1}", RandomListPhrases(listOfFromMessages), airplane.From.City) : String.Empty;
                        string toPlace = !String.IsNullOrEmpty(airplane.To.City) ? String.Format(" {0} {1}", RandomListPhrases(listOfToMessages), airplane.From.City) : String.Empty;
                    }
                    #endregion

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

                        Message += airplaneTypeLongPhrase;

                        if (airplane.State == AirplaneStatus.Cruise || airplane.State == AirplaneStatus.Landing)
                        {
                            if (Message.Length <= 100)
                            {
                                Message += fromPlace;
                            }
                        }
                        if (airplane.State == AirplaneStatus.Cruise || airplane.State == AirplaneStatus.TakingOff)
                        {
                            if (Message.Length <= 100)
                            {
                                Message += toPlace;
                            }
                        }

                        if (Message.Length <= 110)
                        {
                            Message += RobotBumLibCore.Plugins.HelperPlugin.GetForwardLocationsPhrase(this.Airplane, true, 2);
                        }
                        else
                        {
                            Message += RobotBumLibCore.Plugins.HelperPlugin.GetForwardLocationsPhrase(this.Airplane, true);
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


                if (Message.Length <= 125 && (airplane.State == AirplaneStatus.Landing || airplane.State == AirplaneStatus.TakingOff) && radar.MainAirport != null)
                {
                    Message += " #Airport" + radar.MainAirport.ICAO;
                }

                if (Message.Length <= 130)
                    Message += " #RobotBum";

                Message = Regex.Replace(Message, @"\s+", " ");

            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Message Maker " + e.Message);

            }
        }

        private string GetCruisePhrase()
        {
            return GetFormatedPhrase(listOfToMessagesCruisePhraseStart, listOfToMessagesCruisePhraseEnd);
        }

        private string GetTakingOffPhrase()
        {
            return GetFormatedPhrase(listOfToMessagesTakingOffPhraseStart, listOfToMessagesTakingOffPhraseEnd);
        }


        private string GetLandingPhrase()
        {
            return GetFormatedPhrase(listOfToMessagesLandingPhraseStart, listOfToMessagesLandingPhraseEnd);

        }

        private string GetParkingTaxiPhrase()
        {
            return GetFormatedPhrase(listOfToMessagesTakingOffPhraseStart, listOfToMessagesTakingOffPhraseEnd);
        }

        private string GetFormatedPhrase(List<string> listOfPhrasesStarting, List<string> listOfPhrasesEnding)
        {
            string startingPhrase = RandomListPhrases(listOfPhrasesStarting);
            string endingPhrase = RandomListPhrases(listOfPhrasesEnding);
            return RandomListPhrases(new List<string>() {
               String.Format("{0} {1}",phraseBeginningWithAirplaneRegistration, endingPhrase),
                String.Format("{0} {1}",startingPhrase, phraseEndingWithAirplaneRegistration),
            });
        }

        private string GetChartPhrase()
        {
            // This version is not supporting charts well yet.
            return String.Empty;
        }

        private string GetRunwayPhrase()
        {
            // This version is not supporting runways well yet.
            return String.Empty;

        }

        private string GetOrbitPhrase()
        {
            return airplaneRegistrationOrModel + " " + RandomListPhrases(listOfToMessagesOrbit);
        }

        private static string RandomListPhrases(List<string> lstPhrases, int seed = 0)
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
    }
}
