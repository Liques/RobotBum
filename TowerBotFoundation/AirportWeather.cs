using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TowerBotFoundation;

namespace TowerBotFoundation
{
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

    public enum SkyType
    {
        NoInformation,
        Clear,
        FewClouds,
        SomeCloud,
        VeryCloudy,
        Overcast
    }

    public enum WeatherChangeType
    {
        NoInformation,
        Tempo,
        BECMG
    }

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
        public Airport Airport { get; set; }
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

        public static AirportWeather GetWeather(Airport airport)
        {
            return GetWeatherMetar(airport, null, null, true);
        }
        
        public List<AirportWeather> GetFutureWeather(DateTime dateToAnalyse)
        {
            List<AirportWeather> listOfAirportsFutureWithPresent = new List<AirportWeather>();
            if (this.ListFutureWeather != null)
                listOfAirportsFutureWithPresent.AddRange(this.ListFutureWeather);
            listOfAirportsFutureWithPresent.Add(this);

            return listOfAirportsFutureWithPresent.Where(s => dateToAnalyse >= s.DateBegin && dateToAnalyse <= s.DateEnd).ToList();

        }

        private static DateTime NextTimeToLoadUrl()
        {
            // Algoritmo para sempre pegar o próximo hora + 5 minutos;
            return DateTime.Now.AddMinutes(65 - DateTime.Now.Minute);
        }

        public static void PreLoadAirportsWeather(List<string> listOfICAOs, bool loadFutureWeather)
        {
            var listOfUrls = new List<string>();

            string urlFormating = String.Empty;

            for (int i = 0; i < listOfICAOs.Count; i++)
            {
                urlFormating += listOfICAOs[i]+" ";

                if(i % 40 == 0 || i == listOfICAOs.Count - 1)
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
                        var airport = Airport.GetAirportByICAO(s);

                        icao = s;

                        if (airport.IsValid)
                            GetWeatherMetar(airport, responseMetar, responseTAF, loadFutureWeather);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

            }

            loadAirportSBNextTime = NextTimeToLoadUrl();
        }

        private static AirportWeather GetWeatherMetar(Airport airport, string xmlMetarResult, string xmTAFResult, bool loadFutureWeather)
        {
            if (airport != null || !String.IsNullOrEmpty(airport.ICAO))
            {
                if (listRecentWeather == null)
                    listRecentWeather = new List<AirportWeather>();

                List<AirportWeather> listExpired = listRecentWeather.Where(s => s.DateExperition <= DateTime.Now).ToList();
                for (int i = 0; i < listExpired.Count; i++)
                {
                    listRecentWeather.Remove(listExpired[i]);
                }

                AirportWeather airportWeather = listRecentWeather.Where(s => s.Airport.Name == airport.Name).FirstOrDefault();

                if (airportWeather != null)
                {
                    // Se o usuario quer um aerporto com TAF, o aero está na lista recente e ta sem TAF, tentar fazer o load novamente.
                    if (loadFutureWeather && airportWeather.ListFutureWeather == null && airportWeather.LastTAFAttempet < DateTime.Now)
                    {
                        airportWeather.ListFutureWeather = GetWeatherTAF(airport);

                        // Se ainda assim o TAF vem nulo, verificar de novo só daqui uma hora
                        if (airportWeather.ListFutureWeather == null)
                            airportWeather.LastTAFAttempet = DateTime.Now.AddHours(1);
                    }
                    return airportWeather;
                }

                // Se o aeroporto não está na lista recente, então iniciar os procedimentos
                airportWeather = new AirportWeather();
                airportWeather.Airport = airport;
                // No codigo abaixo de expiração, foi colocado para atualizar 10 minutos depois da virada de hora
                airportWeather.DateExperition = NextTimeToLoadUrl();
                airportWeather.DateBegin = DateTime.Now;
                airportWeather.DateEnd = airportWeather.DateExperition;

                Dictionary<string, object> listWeatherLines = null;

                if (String.IsNullOrEmpty(xmlMetarResult))
                    listWeatherLines = LoadMetarAPI(airport.ICAO, loadFutureWeather).Result;
                else
                    listWeatherLines = MetarToDictionary(airport.ICAO, xmlMetarResult).Result;

                // Se o serviço não tem detalhes sobre o METAR do aerporto...
                if (listWeatherLines == null)
                {
                    // Se ainda assim o TAF vem nulo, verificar de novo só daqui seis horas
                    airportWeather.DateExperition = DateTime.Now.AddHours(6);
                    listRecentWeather.Add(airportWeather);
                    return airportWeather;
                }

                if (loadFutureWeather && !String.IsNullOrEmpty(xmTAFResult))
                {

                    if (airport.ICAO == "SBBE")
                    {
                        string sfa = "";
                    }

                    airportWeather.ListFutureWeather = GetWeatherTAF(airport, xmTAFResult);

                    // Se ainda assim o TAF vem nulo, verificar de novo só daqui uma hora
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

                    // Setando o Visibility
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

                    // Setando as condições do céu
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
            }
            else
            {
                return null;
            }
        }

        private static List<AirportWeather> GetWeatherTAF(Airport airport, string xmlTafResult = null)
        {
            List<AirportWeather> listFutureLocalWeather = new List<AirportWeather>();
            if (airport != null || !String.IsNullOrEmpty(airport.ICAO))
            {
                var airportWeatherStandart = new AirportWeather();
                airportWeatherStandart.Airport = airport;


                Dictionary<string, object> listNodeWeatherLines = null;

                try
                {
                    if (String.IsNullOrEmpty(xmlTafResult))
                        listNodeWeatherLines = LoadTafAPI(airport.ICAO).Result;
                    else
                    {
                        listNodeWeatherLines = TafToDictionary(airport.ICAO, xmlTafResult).Result;
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
                    airportWeather.Airport = airportWeatherStandart.Airport;


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

                        // Setando as condições do céu
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
            else
            {
                return null;
            }
        }

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

        private static async Task<Dictionary<string, object>> LoadMetarAPI(string ICAO, bool loadFutureWeather)
        {
            try
            {
                HttpClient httpClient = new HttpClient();

                string responseMetar = null;
                string responseTAF = null;

                // Se não começar com SB... então fazer o load individual
                // Ou então se o objeto for forçado a fazer o load de cada um separadamente...
                if (!ICAO.StartsWith("SB") || ForceOnlyLoadSingleAirports || (ICAO.StartsWith("SB") && loadAirportSBNextTime > DateTime.Now))
                {
                    responseMetar = await httpClient.GetStringAsync(urlMETAR + ICAO.ToUpper());                    
                }
                // Mas se não começar com SB, então trazer logo todos os aero que começam com SB!
                else
                {
                    responseMetar = await httpClient.GetStringAsync(urlMETAR + "SB* " + ICAO.ToUpper());
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

                            var airport = Airport.GetAirportByICAO(s);

                            if (airport.IsValid)
                                GetWeatherMetar(airport, responseMetar, responseTAF, loadFutureWeather);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    loadAirportSBNextTime = NextTimeToLoadUrl();

                }




                return MetarToDictionary(ICAO, responseMetar).Result;


            }
            catch (Exception e)
            {
                throw new ArgumentException("Metar Data error " + ICAO);
            }

        }

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
            return this.Airport.ToString() + " - " + this.WeatherType;
        }
    }
}
