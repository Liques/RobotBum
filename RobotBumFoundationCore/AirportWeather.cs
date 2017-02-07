using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RobotBumFoundationCore
{
    /// <summary>
    /// Type of Weather, if it's a rain, fog and so on.
    /// </summary>
    public enum WeatherType
    {
        NoInformation,
        Drizzle,
        Rain,
        WeakRain,
        StrongRain,
        VeryStrongRain,
        NeighborRain,
        Snow,
        Hail,
        Fog,
        Smoke,
        VolcanicAsh,
        Sand,
        Haze,
        Poeira,
        Duststorm,
        Tornado
    }

    /// <summary>
    /// Density of cloudness in the sky
    /// </summary>
    public enum SkyType
    {
        NoInformation,
        Clear,
        FewClouds,
        SomeCloud,
        VeryCloudy,
        Overcast
    }

    /// <summary>
    /// This enum refers to the Metar type of change of climate.
    /// </summary>
    public enum WeatherChangeType
    {
        NoInformation,
        Tempo,
        BECMG
    }

    /// <summary>
    /// This object gets the weather information of any airport available. It gets the current weather of an airport (METAR) and its predictions (TAF).
    /// </summary>
    public class AirportWeather
    {
        public static List<AirportWeather> CurrentWeatherColleciton
        {
            get
            {
                return listRecentWeather;
            }
        }
        private static List<AirportWeather> listRecentWeather { get; set; }
        public static bool ForceOnlyLoadSingleAirports { get; set; }
        private static DateTime loadAirportSBNextTime = DateTime.Now;

        public List<AirportWeather> ListFutureWeather { get; set; }

        public double WindSpeed { get; set; }
        public double WindDirection { get; set; }
        public double Visibility { get; set; }
        public string ICAO { get; set; }
        public double Temperature { get; set; }
        public SkyType Sky { get; set; }
        DateTime DateExperition { get; set; }
        public string Metar { get; set; }
        public string TAF { get; set; }
        public WeatherType WeatherType { get; set; }
        protected bool IsVFR { get; set; }
        protected bool IsIFR { get; set; }
        public WeatherChangeType ChangeType { get; set; }
        public int Probability { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }

        internal DateTime LastMetarAttempet { get; set; }
        internal DateTime LastTAFAttempet { get; set; }

        // We are using as weather API the weather service from aviationweath.gov.
        private static string urlTAF = "https://aviationweather.gov/adds/dataserver_current/httpparam?dataSource=tafs&requestType=retrieve&format=xml&hoursBeforeNow=2&stationString=";
        private static string urlMETAR = "https://www.aviationweather.gov/adds/dataserver_current/httpparam?dataSource=metars&requestType=retrieve&format=xml&hoursBeforeNow=2&stationString=";

        private AirportWeather()
        {
            Probability = -1;
            WindDirection = -1;
            WindSpeed = -1;
            Temperature = -1000;
            LastMetarAttempet = new DateTime();
            LastTAFAttempet = new DateTime();
        }

        /// <summary>
        /// Get the current weather of an airport (METAR).
        /// </summary>
        /// <param name="airport"></param>
        /// <returns></returns>
        public static AirportWeather GetWeather(string ICAO)
        {
            return GetWeatherMetar(ICAO, null, null, true);
        }

        /// <summary>
        /// Get the current weather of an airport (METAR) and its predicitions (TAF).
        /// </summary>
        /// <param name="airport"></param>
        /// <returns></returns>
        public List<AirportWeather> GetFutureWeather(DateTime dateToAnalyse)
        {
            List<AirportWeather> listOfAirportsFutureWithPresent = new List<AirportWeather>();
            if (this.ListFutureWeather != null)
                listOfAirportsFutureWithPresent.AddRange(this.ListFutureWeather);
            listOfAirportsFutureWithPresent.Add(this);

            return listOfAirportsFutureWithPresent.Where(s => dateToAnalyse >= s.DateBegin && dateToAnalyse <= s.DateEnd).ToList();

        }

        /// <summary>
        /// Next time to update the weather (hour + 5 minuts)
        /// </summary>
        /// <returns></returns>
        private static DateTime NextTimeToLoadUrl()
        {
            return DateTime.Now.AddMinutes(65 - DateTime.Now.Minute);
        }

        /// <summary>
        /// Load the weather data from webservice
        /// </summary>
        /// <param name="listOfICAOs">List of ICAOs</param>
        /// <param name="loadFutureWeather">If is to load predictions (TAF) or not.</param>
        public static void PreLoadAirportsWeather(List<string> listOfICAOs, bool loadFutureWeather)
        {
            var listOfUrls = new List<string>();

            string urlFormating = String.Empty;

            for (int i = 0; i < listOfICAOs.Count; i++)
            {
                urlFormating += listOfICAOs[i] + " ";

                if (i % 40 == 0 || i == listOfICAOs.Count - 1)
                {
                    listOfUrls.Add(urlFormating);
                    urlFormating = String.Empty;
                }
            }

            HttpClient httpClient = new HttpClient();

            for (int i = 0; i < listOfUrls.Count; i++)
            {
                var responseMetar = httpClient.GetStringAsync(urlMETAR + listOfUrls[i]).Result;
                string responseTAF = String.Empty;

                XDocument allSBAirports = XDocument.Parse(responseMetar);
                var listOfAirports = listOfUrls[i].Split(' ').Where(s => !String.IsNullOrEmpty(s)).ToList();

                if (loadFutureWeather)
                    responseTAF = httpClient.GetStringAsync(urlTAF + String.Join(" ", listOfAirports)).Result;

                string icao = "";

                try
                {
                    foreach (var s in listOfAirports)
                    {

                        icao = s;

                        GetWeatherMetar(s, responseMetar, responseTAF, loadFutureWeather);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

            }

            loadAirportSBNextTime = NextTimeToLoadUrl();
        }

        /// <summary>
        /// Get the current weather or/and its predictions but also gets the cached version.
        /// </summary>
        /// <param name="airport">Airport object</param>
        /// <param name="xmlMetarResult">XML result from webservice (METAR)</param>
        /// <param name="xmTAFResult">XML result from webservice (TAF)</param>
        /// <param name="loadFutureWeather">If is to load predictions</param>
        /// <returns></returns>
        private static AirportWeather GetWeatherMetar(string ICAO, string xmlMetarResult, string xmTAFResult, bool loadFutureWeather)
        {
            try
            {
                if (String.IsNullOrEmpty(ICAO))
                    throw new ArgumentException("You must to provide a valid ICAO code. For more information about what is ICAO code, see https://en.wikipedia.org/wiki/International_Civil_Aviation_Organization_airport_code.");

                if (listRecentWeather == null)
                    listRecentWeather = new List<AirportWeather>();

                List<AirportWeather> listExpired = listRecentWeather.Where(s => s.DateExperition <= DateTime.Now).ToList();
                for (int i = 0; i < listExpired.Count; i++)
                {
                    listRecentWeather.Remove(listExpired[i]);
                }

                AirportWeather airportWeather = listRecentWeather.Where(s => s.ICAO == ICAO).FirstOrDefault();

                if (airportWeather != null)
                {
                    // If the user wants an airport with TAF, the airport data is cached and it tries to load the TAF, it blocks to try to load the TAF again                    
                    if (loadFutureWeather && airportWeather.ListFutureWeather == null && airportWeather.LastTAFAttempet < DateTime.Now)
                    {
                        airportWeather.ListFutureWeather = GetWeatherTAF(ICAO);

                        // If there is no TAF data yet, it tries to load in one hour again
                        if (airportWeather.ListFutureWeather == null)
                            airportWeather.LastTAFAttempet = DateTime.Now.AddHours(1);
                    }
                    return airportWeather;
                }

                // If the airport data is not cached, so let's get the data!
                airportWeather = new AirportWeather();
                airportWeather.ICAO = ICAO;
                airportWeather.DateExperition = NextTimeToLoadUrl();
                airportWeather.DateBegin = DateTime.Now;
                airportWeather.DateEnd = airportWeather.DateExperition;

                Dictionary<string, object> listWeatherLines = null;

                if (String.IsNullOrEmpty(xmlMetarResult))
                    listWeatherLines = LoadMetarAPI(ICAO, loadFutureWeather).Result;
                else
                    listWeatherLines = MetarToDictionary(ICAO, xmlMetarResult).Result;

                // If there is no data from airport...
                if (listWeatherLines == null)
                {
                    // If there is not TAF data yet, try again in 6 hours
                    airportWeather.DateExperition = DateTime.Now.AddHours(6);
                    listRecentWeather.Add(airportWeather);
                    return airportWeather;
                }

                if (loadFutureWeather && !String.IsNullOrEmpty(xmTAFResult))
                {
                    airportWeather.ListFutureWeather = GetWeatherTAF(ICAO, xmTAFResult);

                    // If there is not TAF data yet, try again in one hours
                    if (airportWeather.ListFutureWeather == null)
                        airportWeather.LastTAFAttempet = DateTime.Now.AddHours(1);
                }

                string MetarRaw = listWeatherLines["RawMetar"].ToString();

                if (listWeatherLines.ContainsKey("WindSpeed"))
                {
                    airportWeather.WindDirection = Convert.ToDouble(listWeatherLines["WindDirection"]);
                    airportWeather.WindSpeed = Convert.ToDouble(listWeatherLines["WindSpeed"]);
                }

                if (!String.IsNullOrEmpty(MetarRaw))
                {
                    airportWeather.Metar = MetarRaw;

                    double visibilityNumber = -1;

                    if (listWeatherLines.ContainsKey("Visibility"))
                    {
                        string visibilityString = listWeatherLines["Visibility"].ToString();
                        bool isInMiles = true;

                        double.TryParse(visibilityString, out visibilityNumber);
                        visibilityNumber *= 1000;
                        visibilityNumber = (isInMiles) ? visibilityNumber * 1.6 : visibilityNumber;
                    }
                    airportWeather.Visibility = visibilityNumber;

                    IEnumerable<dynamic> skyConditions = (IEnumerable<dynamic>)listWeatherLines["SkyConditions"];

                    if (skyConditions.Any(s => s.SkyCover == "CAVOK") || skyConditions.Any(s => s.SkyCover == "NSC"))
                        airportWeather.Sky = SkyType.Clear;
                    else if (skyConditions.Any(s => s.SkyCover == "OVC"))
                        airportWeather.Sky = SkyType.Overcast;
                    else if (skyConditions.Any(s => s.SkyCover == "BKN"))
                        airportWeather.Sky = SkyType.VeryCloudy;
                    else if (skyConditions.Any(s => s.SkyCover == "SCT"))
                        airportWeather.Sky = SkyType.SomeCloud;
                    else if (skyConditions.Any(s => s.SkyCover == "FEW"))
                        airportWeather.Sky = SkyType.FewClouds;

                }
                if (listWeatherLines.ContainsKey("Temperature"))
                {
                    airportWeather.Temperature = Convert.ToDouble(listWeatherLines["Temperature"]);
                }

                if (listWeatherLines.ContainsKey("FlightCategory"))
                {

                    if (listWeatherLines["FlightCategory"].ToString().Contains("VFR"))
                    {
                        airportWeather.IsIFR = true;
                        airportWeather.IsVFR = true;
                    }
                    else if (listWeatherLines["FlightCategory"].ToString() == "IFR")
                    {
                        airportWeather.IsIFR = true;
                    }

                }
                else
                {
                    airportWeather.IsIFR = true;
                    airportWeather.IsVFR = true;
                }

                if (listWeatherLines.ContainsKey("AdditionalInfo"))
                {
                    string additionalInfo = listWeatherLines["AdditionalInfo"].ToString();

                    airportWeather.Metar = (!String.IsNullOrEmpty(airportWeather.Metar)) ? airportWeather.Metar : "";

                    airportWeather.WeatherType = GetWeatherType(additionalInfo);

                    airportWeather.ChangeType = WeatherChangeType.Tempo;
                    airportWeather.Probability = 100;
                }

                listRecentWeather.Add(airportWeather);



                return airportWeather;

            } catch (Exception e)
            {
                throw e;
            }

        }

        private static List<AirportWeather> GetWeatherTAF(string ICAO, string xmlTafResult = null)
        {
            List<AirportWeather> listFutureLocalWeather = new List<AirportWeather>();

            if (String.IsNullOrEmpty(ICAO))
                throw new ArgumentException("You must to provide a valid ICAO code. For more information about what is ICAO code, see https://en.wikipedia.org/wiki/International_Civil_Aviation_Organization_airport_code.");
            
            var airportWeatherStandart = new AirportWeather();
            airportWeatherStandart.ICAO = ICAO;


            Dictionary<string, object> listNodeWeatherLines = null;

            try
            {
                if (String.IsNullOrEmpty(xmlTafResult))
                    listNodeWeatherLines = LoadTafAPI(ICAO).Result;
                else
                {
                    listNodeWeatherLines = TafToDictionary(ICAO, xmlTafResult).Result;
                }
            }
            catch (Exception e)
            {
                new ArgumentException("Problema no load dos dados");
            }

            if (listNodeWeatherLines == null)
                return null;

            string TAFRaw = listNodeWeatherLines["RawTAF"].ToString();

            var listForecasts = (List<object>)listNodeWeatherLines["Forecasts"];

            foreach (var forecastRawLinesRaw in listForecasts)
            {
                var forecastRawLines = (Dictionary<string, object>)forecastRawLinesRaw;

                var airportWeather = new AirportWeather();
                airportWeather.ICAO = airportWeatherStandart.ICAO;


                if (forecastRawLines.ContainsKey("TimeStart"))
                    airportWeather.DateBegin = DateTime.Parse(forecastRawLines["TimeStart"].ToString());
                if (forecastRawLines.ContainsKey("TimeEnd"))
                    airportWeather.DateEnd = DateTime.Parse(forecastRawLines["TimeEnd"].ToString());

                if (forecastRawLines.ContainsKey("Probality"))
                    airportWeather.Probability = Convert.ToInt32(forecastRawLines["Probality"].ToString());

                if (forecastRawLines.ContainsKey("WindSpeed"))
                {
                    airportWeather.WindDirection = Convert.ToDouble(forecastRawLines["WindDirection"]);
                    airportWeather.WindSpeed = Convert.ToDouble(forecastRawLines["WindSpeed"]);
                }

                if (forecastRawLines.ContainsKey("ChangeIndicator"))
                {
                    if (forecastRawLines["ChangeIndicator"].ToString() == "TEMPO")
                        airportWeather.ChangeType = WeatherChangeType.Tempo;
                }
                //

                airportWeather.TAF = TAFRaw;

                // Setando o Visibility
                double visibilityNumber = -1;

                string visibilityString = String.Empty;
                if (forecastRawLines.ContainsKey("Visibility"))
                    visibilityString = forecastRawLines["Visibility"].ToString();

                bool isInMiles = true;

                double.TryParse(visibilityString, out visibilityNumber);
                visibilityNumber *= 1000;
                visibilityNumber = (isInMiles) ? visibilityNumber * 1.6 : visibilityNumber;
                airportWeather.Visibility = visibilityNumber;

                if (forecastRawLines.ContainsKey("SkyConditions"))
                {
                    IEnumerable<dynamic> skyConditions = (IEnumerable<dynamic>)forecastRawLines["SkyConditions"];

                    if (skyConditions.Any(s => s.SkyCover == "CAVOK") || skyConditions.Any(s => s.SkyCover == "NSC"))
                        airportWeather.Sky = SkyType.Clear;
                    else if (skyConditions.Any(s => s.SkyCover == "OVC"))
                        airportWeather.Sky = SkyType.Overcast;
                    else if (skyConditions.Any(s => s.SkyCover == "BKN"))
                        airportWeather.Sky = SkyType.VeryCloudy;
                    else if (skyConditions.Any(s => s.SkyCover == "SCT"))
                        airportWeather.Sky = SkyType.SomeCloud;
                    else if (skyConditions.Any(s => s.SkyCover == "FEW"))
                        airportWeather.Sky = SkyType.FewClouds;
                }

                if (forecastRawLines.ContainsKey("AdditionalInfo"))
                {
                    string additionalInfo = forecastRawLines["AdditionalInfo"].ToString();

                    airportWeather.TAF = (!String.IsNullOrEmpty(airportWeather.TAF)) ? airportWeather.TAF : "";

                    airportWeather.WeatherType = GetWeatherType(additionalInfo);

                }

                listFutureLocalWeather.Add(airportWeather);

            }

            return listFutureLocalWeather;

        }

        /// <summary>
        /// Gets the weather type based on METAR or TAF standard
        /// </summary>
        /// <param name="weatherType"></param>
        /// <returns></returns>
        private static WeatherType GetWeatherType(string weatherType)
        {

            WeatherType weather = WeatherType.NoInformation;

            bool isHeavy = weatherType.StartsWith("+");
            bool isLight = weatherType.StartsWith("-");
            bool isThunderStorm = weatherType.Contains("TS");
            bool isVicinity = weatherType.StartsWith("VC");

            if (weatherType.EndsWith("FC"))
            {
                weather = WeatherType.Tornado;
            }
            else if (weatherType.Contains("DS"))
            {
                weather = WeatherType.Duststorm;
            }
            else if (weatherType.Contains("PY"))
            {
                weather = WeatherType.Poeira;
            }
            else if (weatherType.Contains("HZ"))
            {
                weather = WeatherType.Haze;
            }
            else if (weatherType.Contains("SA"))
            {
                weather = WeatherType.Sand;
            }
            else if (weatherType.Contains("VA"))
            {
                weather = WeatherType.VolcanicAsh;
            }
            else if (weatherType.Contains("FU"))
            {
                weather = WeatherType.Smoke;
            }
            else if (weatherType.Contains("FG"))
            {
                weather = WeatherType.Fog;
            }
            else if (weatherType.Contains("GR") || weatherType.Contains("GS"))
            {
                weather = WeatherType.Hail;
            }
            else if (weatherType.Contains("SG") || weatherType.Contains("SN"))
            {
                weather = WeatherType.Snow;
            }
            else if (weatherType.Contains("-TSRA") || weatherType.Contains("SHRA"))
            {
                weather = WeatherType.WeakRain;
            }
            else if (weatherType.Contains("+TSRA"))
            {
                weather = WeatherType.VeryStrongRain;
            }
            else if (weatherType.Contains("TSRA"))
            {
                weather = WeatherType.StrongRain;
            }
            else if (weatherType.Contains("VCTS"))
            {
                weather = WeatherType.NeighborRain;
            }
            else if (weatherType.Contains("RA") || isThunderStorm)
            {
                weather = WeatherType.Rain;
            }
            else if (weatherType.Contains("DZ"))
            {
                weather = WeatherType.Drizzle;
            }

            return weather;
        }

        /// <summary>
        /// Load the weather data (METAR) from webservice
        /// </summary>
        /// <param name="ICAO"></param>
        /// <param name="loadFutureWeather"></param>
        /// <returns></returns>
        private static async Task<Dictionary<string, object>> LoadMetarAPI(string ICAO, bool loadFutureWeather)
        {
            try
            {
                HttpClient httpClient = new HttpClient();

                string responseMetar = null;
                string responseTAF = null;

                responseMetar = await httpClient.GetStringAsync(urlMETAR + ICAO.ToUpper());
                //Task mytask = Task.Run(() =>
                //{

                XDocument allSBAirports = XDocument.Parse(responseMetar);
                var listOfAirports = allSBAirports.Descendants("METAR").Select(s => s.Descendants("station_id").FirstOrDefault().Value).Where(s => s != ICAO).Distinct().ToList();

                if (loadFutureWeather)
                    responseTAF = await httpClient.GetStringAsync(urlTAF + String.Join(" ", listOfAirports));

                string currentAirport = "";
                try
                {
                    foreach (var s in listOfAirports)
                    {
                        currentAirport = s;

                        GetWeatherMetar(currentAirport, responseMetar, responseTAF, loadFutureWeather);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

                loadAirportSBNextTime = NextTimeToLoadUrl();

                return MetarToDictionary(ICAO, responseMetar).Result;


            }
            catch (Exception e)
            {
                throw new ArgumentException("Metar Data error " + ICAO);
            }

        }

        /// <summary>
        /// Load TAF data from webservice
        /// </summary>
        /// <param name="ICAO"></param>
        /// <returns></returns>
        private static async Task<Dictionary<string, object>> LoadTafAPI(string ICAO)
        {
            try
            {
                HttpClient httpClient = new HttpClient();

                string responseBodyAsText = null;

                responseBodyAsText = httpClient.GetStringAsync(urlTAF + ICAO.ToUpper()).Result;

                return TafToDictionary(ICAO, responseBodyAsText).Result;


            }
            catch (Exception e)
            {
                throw new ArgumentException("Metar Data error " + ICAO);
            }

        }

        /// <summary>
        /// Translates the webservice (METAR) data in a dictionany to be used later
        /// </summary>
        /// <param name="ICAO"></param>
        /// <param name="xmlResponse"></param>
        /// <returns></returns>
        private static async Task<Dictionary<string, object>> MetarToDictionary(string ICAO, string xmlResponse)
        {
            XDocument document = XDocument.Parse(xmlResponse);

            var metarElement = document.Descendants("METAR").Where(s => s.Descendants("station_id").Any(j => j.Value == ICAO)).FirstOrDefault();

            if (metarElement == null)
                return null;

            var listTeste = metarElement.Elements().ToList();

            Dictionary<string, object> elementsData = new Dictionary<string, object>();

            elementsData.Add("Station", listTeste.Where(s => s.Name.LocalName == "station_id").FirstOrDefault().Value);
            if (listTeste.Any(s => s.Name.LocalName == "latitude"))
            {
                elementsData.Add("Latitude", listTeste.Where(s => s.Name.LocalName == "latitude").FirstOrDefault().Value);
                elementsData.Add("Longitude", listTeste.Where(s => s.Name.LocalName == "longitude").FirstOrDefault().Value);
            }
            if (listTeste.Any(s => s.Name.LocalName == "temp_c"))
                elementsData.Add("Temperature", listTeste.Where(s => s.Name.LocalName == "temp_c").FirstOrDefault().Value);
            if (listTeste.Any(s => s.Name.LocalName == "dewpoint_c"))
                elementsData.Add("TemperatureFloor", listTeste.Where(s => s.Name.LocalName == "dewpoint_c").FirstOrDefault().Value);
            if (listTeste.Any(s => s.Name.LocalName == "wind_dir_degrees"))
                elementsData.Add("WindDirection", listTeste.Where(s => s.Name.LocalName == "wind_dir_degrees").FirstOrDefault().Value);
            if (listTeste.Any(s => s.Name.LocalName == "wind_speed_kt"))
                elementsData.Add("WindSpeed", listTeste.Where(s => s.Name.LocalName == "wind_speed_kt").FirstOrDefault().Value);
            if (listTeste.Any(s => s.Name.LocalName == "visibility_statute_mi"))
                elementsData.Add("Visibility", listTeste.Where(s => s.Name.LocalName == "visibility_statute_mi").FirstOrDefault().Value);
            if (listTeste.Any(s => s.Name.LocalName == "sky_condition"))
                elementsData.Add("SkyConditions", listTeste.Where(s => s.Name.LocalName == "sky_condition").Select(s => new { SkyCover = s.Attribute("sky_cover") != null ? s.Attribute("sky_cover").Value : "", Altitude = s.Attribute("cloud_base_ft_agl") != null ? s.Attribute("cloud_base_ft_agl").Value : "" }));
            if (listTeste.Any(s => s.Name.LocalName == "flight_category"))
                elementsData.Add("FlightCategory", listTeste.Where(s => s.Name.LocalName == "flight_category").FirstOrDefault().Value);
            if (listTeste.Any(s => s.Name.LocalName == "raw_text"))
                elementsData.Add("RawMetar", listTeste.Where(s => s.Name.LocalName == "raw_text").FirstOrDefault().Value);
            if (listTeste.Any(s => s.Name.LocalName == "wx_string"))
                elementsData.Add("AdditionalInfo", listTeste.Where(s => s.Name.LocalName == "wx_string").FirstOrDefault().Value);

            return elementsData;
        }

        /// <summary>
        /// Translates the webservice (TAF) data in a dictionany to be used later
        /// </summary>
        /// <param name="ICAO"></param>
        /// <param name="xmlResponse"></param>
        /// <returns></returns>
        private static async Task<Dictionary<string, object>> TafToDictionary(string ICAO, string xmlResponse)
        {
            XDocument document = XDocument.Parse(xmlResponse);

            var metarElement = document.Descendants("TAF").Where(s => s.Descendants("station_id").Any(j => j.Value == ICAO)).FirstOrDefault();

            if (metarElement != null)
            {

                var listNodeElement = metarElement.Elements().ToList();

                Dictionary<string, object> elementsNodeData = new Dictionary<string, object>();

                elementsNodeData.Add("Station", listNodeElement.Where(s => s.Name.LocalName == "station_id").FirstOrDefault().Value);
                if (listNodeElement.Any(s => s.Name.LocalName == "latitude"))
                {
                    elementsNodeData.Add("Latitude", listNodeElement.Where(s => s.Name.LocalName == "latitude").FirstOrDefault().Value);
                    elementsNodeData.Add("Longitude", listNodeElement.Where(s => s.Name.LocalName == "longitude").FirstOrDefault().Value);
                }
                if (listNodeElement.Any(s => s.Name.LocalName == "raw_text"))
                    elementsNodeData.Add("RawTAF", listNodeElement.Where(s => s.Name.LocalName == "raw_text").FirstOrDefault().Value);


                var forescastElements = metarElement.Descendants("forecast").ToList();
                var forescastElementsList = new List<object>();


                foreach (var forescatSingleElement in forescastElements)
                {
                    var forescastSingleElementsList = forescatSingleElement.Elements().ToList();

                    Dictionary<string, object> elementsSingleData = new Dictionary<string, object>();

                    if (forescastSingleElementsList.Any(s => s.Name.LocalName == "fcst_time_from"))
                        elementsSingleData.Add("TimeStart", forescastSingleElementsList.Where(s => s.Name.LocalName == "fcst_time_from").FirstOrDefault().Value);
                    if (forescastSingleElementsList.Any(s => s.Name.LocalName == "fcst_time_to"))
                        elementsSingleData.Add("TimeEnd", forescastSingleElementsList.Where(s => s.Name.LocalName == "fcst_time_to").FirstOrDefault().Value);
                    if (forescastSingleElementsList.Any(s => s.Name.LocalName == "wind_dir_degrees"))
                        elementsSingleData.Add("WindDirection", forescastSingleElementsList.Where(s => s.Name.LocalName == "wind_dir_degrees").FirstOrDefault().Value);
                    if (forescastSingleElementsList.Any(s => s.Name.LocalName == "wind_speed_kt"))
                        elementsSingleData.Add("WindSpeed", forescastSingleElementsList.Where(s => s.Name.LocalName == "wind_speed_kt").FirstOrDefault().Value);
                    if (forescastSingleElementsList.Any(s => s.Name.LocalName == "sky_condition"))
                        elementsSingleData.Add("SkyConditions", forescastSingleElementsList.Where(s => s.Name.LocalName == "sky_condition").Select(s => new { SkyCover = s.Attribute("sky_cover") != null ? s.Attribute("sky_cover").Value : "", Altitude = s.Attribute("cloud_base_ft_agl") != null ? s.Attribute("cloud_base_ft_agl").Value : "" }));
                    if (forescastSingleElementsList.Any(s => s.Name.LocalName == "wx_string"))
                        elementsSingleData.Add("AdditionalInfo", forescastSingleElementsList.Where(s => s.Name.LocalName == "wx_string").FirstOrDefault().Value);
                    if (forescastSingleElementsList.Any(s => s.Name.LocalName == "change_indicator"))
                        elementsSingleData.Add("ChangeIndicator", forescastSingleElementsList.Where(s => s.Name.LocalName == "change_indicator").FirstOrDefault().Value);
                    if (forescastSingleElementsList.Any(s => s.Name.LocalName == "visibility_statute_mi"))
                        elementsSingleData.Add("Visibility", forescastSingleElementsList.Where(s => s.Name.LocalName == "visibility_statute_mi").FirstOrDefault().Value);
                    if (forescastSingleElementsList.Any(s => s.Name.LocalName == "probability"))
                        elementsSingleData.Add("Probality", forescastSingleElementsList.Where(s => s.Name.LocalName == "probability").FirstOrDefault().Value);

                    forescastElementsList.Add(elementsSingleData);
                }

                elementsNodeData.Add("Forecasts", forescastElementsList);

                return elementsNodeData;
            }
            else
                return null;
        }

        public override string ToString()
        {
            return this.ICAO.ToString() + " - " + this.WeatherType;
        }
    }
}
