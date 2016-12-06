using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TowerBotLibrary
{

    public static class AirplanesData
    {
        static async public Task<List<AirplaneBasic>> GetAirplanes(Radar radar)
        {
            try
            {
                List<AirplaneBasic> listAirplane = await DesarializeAirplanes(radar);

                return listAirplane;
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Load Airplane Data");
                return null;
            }
        }

        static async private Task<List<AirplaneBasic>> DesarializeAirplanes(Radar radar)
        {

            string responseBodyAsText = String.Empty;
            List<AirplaneBasic> listAirplanes = null;
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = null;

            try
            {
                if (!String.IsNullOrEmpty(radar.EndpointUrl))
                {
                    response = httpClient.PostAsync(radar.EndpointUrl, new StringContent("{\"req\":\"getStats\",\"data\":{\"statsType\":\"flights\",\"id\":38209319}}", Encoding.UTF8, "application/x-www-form-urlencoded")).Result;
                    responseBodyAsText = response.Content.ReadAsStringAsync().Result;
                    radar.ModeSAllowed = true;
                }
                else if (radar.RadarParent != null)
                {
                    listAirplanes = CallParentRadar(radar);
                }
            }
            catch (Exception e)
            {

                listAirplanes = CallParentRadar(radar);

                Console.WriteLine("Radar " + radar.Name + " is out.");
                ErrorManager.ThrowError(e, "Radar " + radar.Name + " is out.", false);


            }

            try
            {
                listAirplanes = new List<AirplaneBasic>();

                ErrorManager.LastRowData = responseBodyAsText;

                if (response.IsSuccessStatusCode && !String.IsNullOrEmpty(responseBodyAsText))
                {


                    Dictionary<string, object> routes_list = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBodyAsText);
                    routes_list = JsonConvert.DeserializeObject<Dictionary<string, object>>(routes_list["stats"].ToString());
                    string flights = routes_list["flights"].ToString();//.Replace("{[", "[").Replace("]}", "]");
                    var flightsJArray = JsonConvert.DeserializeObject<JArray>(routes_list["flights"].ToString()).ToList();


                    for (int i = 0; i < flightsJArray.Count; i++)
                    {
                        var flightDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(flightsJArray[i].ToString());

                        string hexcode = !flightDictionary.ContainsKey("I") ? String.Empty : flightDictionary["I"]; ;
                        
                        string flight = !flightDictionary.ContainsKey("CS") ? String.Empty : flightDictionary["CS"];
                        string altitude = !flightDictionary.ContainsKey("A") ? String.Empty : flightDictionary["A"];
                        string longitude = !flightDictionary.ContainsKey("LO") ? String.Empty : flightDictionary["LO"];
                        string latitudade = !flightDictionary.ContainsKey("LA") ? String.Empty : flightDictionary["LA"];

                        string speed = !flightDictionary.ContainsKey("S") ? String.Empty : flightDictionary["S"];
                        string direction = !flightDictionary.ContainsKey("D") ? String.Empty : flightDictionary["D"];
                        string verticalSpeed = !flightDictionary.ContainsKey("V") ? String.Empty : flightDictionary["V"];

                        string fromToPhrase = !flightDictionary.ContainsKey("FR") ? String.Empty : flightDictionary["FR"];
                        string[] fromToArray = String.IsNullOrEmpty(fromToPhrase) && !fromToPhrase.Contains('-') ? null : fromToPhrase.Split('-');


                        string from = fromToArray == null ? String.Empty : fromToArray[0];
                        string to = fromToArray == null ? String.Empty : fromToArray.Length <= 0 ? String.Empty : fromToArray[1];
                        string model = !flightDictionary.ContainsKey("ITC") ? String.Empty : flightDictionary["ITC"];
                        string registration = !flightDictionary.ContainsKey("RG") ? String.Empty : flightDictionary["RG"];

                        if (!String.IsNullOrEmpty(altitude))
                        {
                            AirplaneBasic airplane = AirplaneBasic.ConvertToAirplane(radar, hexcode, flight, altitude, latitudade, longitude, speed, verticalSpeed, direction, from, to, model, registration);
                            listAirplanes.Add(airplane);
                        }


                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("Deserialize Radar " + radar.Name + " is out.");
                ErrorManager.ThrowError(e, "Deserialize Radar " + radar.Name + " is out.");
            }

            // Se tudo der errado, buscar no outro endpoint
            if (listAirplanes == null)
            {
                listAirplanes = new List<AirplaneBasic>();

                string location = radar.LatitudeX + "," + radar.LatitudeY + "," + radar.LongitudeX + "," + radar.LongitudeY;
                string url = "http://arn.data.fr24.com/zones/fcgi/feed.js?bounds=" + location + "&faa=1&mlat=1&flarm=1&adsb=1&gnd=1&air=1&vehicles=1&estimated=1&maxage=900&gliders=1&stats=1&";


                responseBodyAsText = httpClient.GetStringAsync(url).Result;

                ErrorManager.LastRowData = responseBodyAsText;

                List<KeyValuePair<string, object>> routes_list = null;
                if (responseBodyAsText.Length > 5)
                    routes_list = JsonConvert.DeserializeObject<IDictionary<string, object>>(responseBodyAsText).ToList();


                if (routes_list != null)
                {
                    for (int i = 2; i < routes_list.Count - 1; i++)
                    {
                        object[] teste = (object[])(routes_list[i].Value as JArray).ToObject(typeof(object[]));
                        listAirplanes.Add(AirplaneBasic.ConvertToAirplane(radar, teste, routes_list[i].Key));
                    }
                }
            }

            radar.CurrentAirplanes = listAirplanes;
            radar.LastAirplanes = listAirplanes;


            return listAirplanes;

        }

        private static List<AirplaneBasic> CallParentRadar(Radar radar)
        {
            List<AirplaneBasic> listAirplanes = null;
            if (radar.RadarParent != null)
            {
                if (DateTime.Now - radar.RadarParent.LastAirplaneListUpdate > TimeSpan.FromSeconds(10))
                {

                    listAirplanes = radar.RadarParent.CurrentAirplanes.Where(s =>
                    s.Longitude > radar.LongitudeX &&
                    s.Longitude < radar.LongitudeY &&
                    s.Latitude < radar.LatitudeX &&
                    s.Latitude > radar.LatitudeY
                    ).ToList();
                    radar.ModeSAllowed = false;

                }
            }
            return listAirplanes;
        }
    }
}
