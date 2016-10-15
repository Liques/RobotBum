using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Data;
using TowerBotFoundation;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TowerBotTests
{
    [TestClass]
    public class OtherTests
    {
        [Ignore]
        [TestMethod]
        public void MakeAirportJson()
        {


            var listAirp = Airport.ListAirports;//.Where(w => w.Key == "BSB");

            StreamWriter w = File.AppendText(Environment.GetFolderPath(
                        System.Environment.SpecialFolder.MyDocuments) + "\\airports_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt");


            SqlConnection sqlConnection1 = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=airportworld;Integrated Security=True");
            sqlConnection1.Open();

            SqlDataReader reader;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = @"SELECT [alternatenames], geonameid, country_code,  timezone, admin2_code
  FROM[airportworld].[dbo].[GeoNames]";//  ORDER BY population DESC";

            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection1;

            reader = cmd.ExecuteReader();

            var listObjs = new List<string[]>();

            while (reader.Read())
            {
                listObjs.Add(new string[] { reader["alternatenames"].ToString(), reader["geonameid"].ToString(), reader["country_code"].ToString(), reader["timezone"].ToString(), reader["admin2_code"].ToString() });
            }

            //Parallel.ForEach(listAirp, airportraw => 
            foreach (var airportraw in listAirp)
            {

                var selectedAirport = airportraw.Value;

                var airport = new Airport()
                {
                    City = selectedAirport["City"].ToString(),
                    Country = selectedAirport["Country"].ToString(),
                    Name = selectedAirport["Name"].ToString(),
                    IATA = airportraw.Key,
                    Latitude = Convert.ToDouble(selectedAirport["Lat"].ToString()),
                    Longitude = Convert.ToDouble(selectedAirport["Long"].ToString()),
                    IsValid = true,
                    ICAO = selectedAirport["ICAO"].ToString(),
                    Altitude = Convert.ToInt32(selectedAirport["Alt"].ToString()),
                };


                // if (!airportraw.Key.Any(char.IsDigit))
                // {

                var listEqual = listObjs.Where(wh => wh[0].Contains(airportraw.Key)).ToList();

                if (listEqual.Count > 0)
                {

                    airport.GeoNameCity = listEqual.First()[1];
                    airport.GeoCountry = listEqual.First()[2];
                    airport.TimeZone = listEqual.First()[3];
                    airport.GeoNameState = listEqual.First()[4];

                    airport.GeoNameCity = airport.GeoNameCity.Trim();
                    airport.GeoCountry = airport.GeoCountry.Trim();
                    airport.TimeZone = airport.TimeZone.Trim();
                    airport.GeoNameState = airport.GeoNameState.Trim();

                }



                // }

                Debug.WriteLine(airportraw.Key);



                var linha =
                string.Format(@"'{0}' : {{ 'City':'{1}', 'Country':'{2}', 'Name':'{3}', 'ICAO':'{4}', 'Lat':'{5}', 'Long':'{6}', 'Alt':'{7}','GeoNameCity':'{8}','GeoNameState':'{11}','CountryCode':'{9}','TimeZone':'{10}' }},",
               new string[] { airportraw.Key, airport.City, airport.Country, airport.Name, airport.ICAO, airport.Latitude.ToString(), airport.Latitude.ToString(), airport.Altitude.ToString(), airport.GeoNameCity, airport.GeoCountry, airport.TimeZone, airport.GeoNameState });

                w.WriteLine("{0}", linha);

                Debug.WriteLine(airportraw.Key);



            };
            //});


            w.Close();


            return;
        }

        [Ignore]
        [TestMethod]
        public void ProcessAlternativeNameRelatedToAirports()
        {


            var listAirp = Airport.ListAirports;//.Where(w => w.Key == "BSB");

            StreamWriter finalFile = File.AppendText(Environment.GetFolderPath(
                        System.Environment.SpecialFolder.MyDocuments) + "\\airports3_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt");

            var listOfGeoNames = Airport.ListAirports.Values.Where(w => !String.IsNullOrEmpty(w["GeoNameCity"].ToString())).Select(s => s["GeoNameCity"].ToString()).ToList();

            var listOfGeoNamesState = Airport.ListAirports.Values.Where(w => !String.IsNullOrEmpty(w["GeoNameState"].ToString())).Select(s => s["GeoNameState"].ToString()).ToList();

            var listOfCountriesGeoNames = new List<int>() { 3041565, 290557, 1149361, 3576396, 3573511, 783754, 174982, 3351879, 6697173, 3865483, 5880801, 2782113, 2077456, 3577279, 661882, 587116, 3277605, 3374084, 1210997, 2802361, 2361809, 732800, 290291, 433561, 2395170, 3578476, 3573345, 1820814, 3923057, 7626844, 3469034, 3572887, 1252634, 3371123, 933860, 630336, 3582678, 6251999, 1547376, 203312, 239880, 2260494, 2658434, 2287781, 1899402, 3895114, 2233387, 1814991, 3686110, 3624060, 3562981, 3374766, 7626836, 2078138, 146669, 3077311, 2921044, 223816, 2623032, 3575830, 3508796, 2589581, 3658394, 453733, 357994, 2461445, 338010, 2510769, 337996, 660013, 2205218, 3474414, 2081918, 2622320, 3017382, 2400553, 2635167, 3580239, 614540, 3381670, 3042362, 2300660, 2411586, 3425505, 2413451, 2420477, 3579143, 2309096, 390903, 3474415, 3595528, 4043988, 2372248, 3378535, 1819730, 1547314, 3608932, 3202326, 3723988, 719819, 1643084, 2963597, 294640, 3042225, 1269750, 1282588, 99237, 130758, 2629691, 3175395, 3042142, 3489940, 248816, 1861060, 192950, 1527747, 1831722, 4030945, 921929, 3575174, 1873107, 1835841, 831053, 285570, 3580718, 1522867, 1655842, 272103, 3576468, 3042058, 1227603, 2275384, 932692, 597427, 2960313, 458258, 2215636, 2542007, 2993457, 617790, 3194884, 3578421, 1062947, 2080185, 718075, 2453866, 1327865, 2029969, 1821275, 4041468, 3570311, 2378080, 3578097, 2562770, 934292, 1282028, 927384, 3996063, 1733045, 1036973, 3355338, 2139685, 2440476, 2155115, 2328926, 3617476, 2750405, 3144096, 1282988, 2110425, 4036232, 2186224, 286963, 3703430, 3932488, 4030656, 2088628, 1694008, 1168579, 798544, 3424932, 4030699, 4566966, 6254930, 2264397, 1559582, 3437598, 289688, 935317, 798549, 6290252, 2017370, 49518, 102358, 2103350, 241170, 366755, 7909807, 2661886, 1880251, 3370751, 3190538, 607072, 3057568, 2403846, 3168068, 2245662, 51537, 3382998, 2410758, 3585968, 7609695, 163843, 934841, 3576916, 2434508, 1546748, 2363686, 1605651, 1220409, 4031074, 1966436, 1218197, 2464461, 4032283, 298795, 3573591, 2110297, 1668284, 149590, 690791, 226074, 5854968, 6252001, 3439705, 1512440, 3164670, 3577815, 3625428, 3577718, 4796775, 1562822, 2134431, 4034749, 4034894, 69543, 1024031, 953987, 895949, 878675, 8505033, 8505032 };

            listOfGeoNamesState.AddRange(listOfCountriesGeoNames.Select(s => s.ToString()));

            listOfGeoNames.AddRange(listOfGeoNamesState);
            listOfGeoNames = listOfGeoNames.Distinct().ToList();

            StreamReader file =
   new System.IO.StreamReader("C:\\Users\\Liques\\Desktop\\alternateNames.txt");

            var lines = File.ReadAllLines("C:\\Users\\Liques\\Desktop\\alternateNames.txt");

            //Parallel.For(0, lines.Length,
            //       index => {
            //           var currentline = lines[index];

            //           lock (currentline)
            //           {
            //               var words = currentline.Split('\t');

            //               if (string.IsNullOrEmpty(words[2]) || words[2] == "link")
            //                   return;

            //               if (listOfGeoNames.Any(a => a == words[1]))
            //               {
            //                   finalFile.WriteLine("{0}", currentline);
            //                   Debug.WriteLine(currentline);

            //               }
            //               else {
            //                   Debug.WriteLine(index);
            //               }
            //           }
            //       });

            for (int i = 0; i < lines.Length; i++)
            {
                var currentline = lines[i];

                var words = currentline.Split('\t');

                if (string.IsNullOrEmpty(words[2]) || words[2] == "link")
                    continue;

                if (listOfGeoNames.Any(a => a == words[1]))
                {
                    finalFile.WriteLine("{0}", currentline);
                    Debug.WriteLine(currentline);

                }
                else {
                    Debug.WriteLine(i);
                }

            }


            //Parallel.ForEach(lines, currentline => {

            //    //var words = currentline.Split('\t');

            //    //if (string.IsNullOrEmpty(words[2]) || words[2] == "link")
            //    //    return;

            //    //if (listOfGeoNames.Any(a => a == words[1]))
            //    //{
            //    //    finalFile.WriteLine("{0}", currentline);
            //    //    Debug.WriteLine(currentline);

            //    //}
            //    //else {
            //    //    Debug.WriteLine(counter);
            //    //}
            //    counter++;


            //    Debug.WriteLine(counter);

            //});


            //while ((line = file.ReadLine()) != null)
            //{

            //}





            finalFile.Close();


            return;
        }

        [TestMethod]
        public void ProcessGeonamesOfAirports()
        {


            var listAirp = Airport.ListAirports.Select(s => Airport.GetAirportByIata(s.Key)).ToList();

            StreamWriter finalFile = File.AppendText(Environment.GetFolderPath(
                        System.Environment.SpecialFolder.MyDocuments) + "\\airports4_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt");

            var listOfICAO = listAirp.Select(s => s.ICAO).ToList();

            var listOfGeoNames = Airport.ListAirports.Values.Where(w => !String.IsNullOrEmpty(w["GeoNameCity"].ToString())).Select(s => s["GeoNameCity"].ToString()).ToList();

            var listOfGeoNamesState = Airport.ListAirports.Values.Where(w => !String.IsNullOrEmpty(w["GeoNameState"].ToString())).Select(s => s["GeoNameState"].ToString()).ToList();

            var listOfCountriesGeoNames = new List<int>() { 3041565, 290557, 1149361, 3576396, 3573511, 783754, 174982, 3351879, 6697173, 3865483, 5880801, 2782113, 2077456, 3577279, 661882, 587116, 3277605, 3374084, 1210997, 2802361, 2361809, 732800, 290291, 433561, 2395170, 3578476, 3573345, 1820814, 3923057, 7626844, 3469034, 3572887, 1252634, 3371123, 933860, 630336, 3582678, 6251999, 1547376, 203312, 239880, 2260494, 2658434, 2287781, 1899402, 3895114, 2233387, 1814991, 3686110, 3624060, 3562981, 3374766, 7626836, 2078138, 146669, 3077311, 2921044, 223816, 2623032, 3575830, 3508796, 2589581, 3658394, 453733, 357994, 2461445, 338010, 2510769, 337996, 660013, 2205218, 3474414, 2081918, 2622320, 3017382, 2400553, 2635167, 3580239, 614540, 3381670, 3042362, 2300660, 2411586, 3425505, 2413451, 2420477, 3579143, 2309096, 390903, 3474415, 3595528, 4043988, 2372248, 3378535, 1819730, 1547314, 3608932, 3202326, 3723988, 719819, 1643084, 2963597, 294640, 3042225, 1269750, 1282588, 99237, 130758, 2629691, 3175395, 3042142, 3489940, 248816, 1861060, 192950, 1527747, 1831722, 4030945, 921929, 3575174, 1873107, 1835841, 831053, 285570, 3580718, 1522867, 1655842, 272103, 3576468, 3042058, 1227603, 2275384, 932692, 597427, 2960313, 458258, 2215636, 2542007, 2993457, 617790, 3194884, 3578421, 1062947, 2080185, 718075, 2453866, 1327865, 2029969, 1821275, 4041468, 3570311, 2378080, 3578097, 2562770, 934292, 1282028, 927384, 3996063, 1733045, 1036973, 3355338, 2139685, 2440476, 2155115, 2328926, 3617476, 2750405, 3144096, 1282988, 2110425, 4036232, 2186224, 286963, 3703430, 3932488, 4030656, 2088628, 1694008, 1168579, 798544, 3424932, 4030699, 4566966, 6254930, 2264397, 1559582, 3437598, 289688, 935317, 798549, 6290252, 2017370, 49518, 102358, 2103350, 241170, 366755, 7909807, 2661886, 1880251, 3370751, 3190538, 607072, 3057568, 2403846, 3168068, 2245662, 51537, 3382998, 2410758, 3585968, 7609695, 163843, 934841, 3576916, 2434508, 1546748, 2363686, 1605651, 1220409, 4031074, 1966436, 1218197, 2464461, 4032283, 298795, 3573591, 2110297, 1668284, 149590, 690791, 226074, 5854968, 6252001, 3439705, 1512440, 3164670, 3577815, 3625428, 3577718, 4796775, 1562822, 2134431, 4034749, 4034894, 69543, 1024031, 953987, 895949, 878675, 8505033, 8505032 };

            listOfGeoNamesState.AddRange(listOfCountriesGeoNames.Select(s => s.ToString()));

            listOfGeoNames.AddRange(listOfGeoNamesState);
            listOfGeoNames = listOfGeoNames.Distinct().ToList();

            StreamReader file =
   new System.IO.StreamReader("C:\\Users\\Liques\\Desktop\\alternateNames.txt");

            var lines = File.ReadAllLines("C:\\Users\\Liques\\Desktop\\alternateNames.txt");

            var listOfICAOandGEONAMES = new Dictionary<string, string>();

            for (int i = 0; i < lines.Length; i++)
            {
                var currentline = lines[i];

                var words = currentline.Split('\t');

                if (words.Length > 3 && words[2] == "icao")
                {
                    if (!listOfICAOandGEONAMES.ContainsKey(words[3]))
                    {
                        listOfICAOandGEONAMES.Add(words[3], words[1]);
                        Debug.WriteLine(currentline);
                    }
                }
                else
                {
                    Debug.WriteLine(i);
                }



            }

            foreach (var airportraw in listAirp)
            {

                var airport = airportraw;

                if (!String.IsNullOrEmpty(airport.ICAO) && listOfICAOandGEONAMES.ContainsKey(airport.ICAO))
                {

                    airport.GeoNameAirport = listOfICAOandGEONAMES[airport.ICAO];
                    Debug.WriteLine("------------------------------");

                }



                Debug.WriteLine(airportraw.IATA);



                var linha =
                string.Format(@"'{0}' : {{ 'City':'{1}', 'Country':'{2}', 'Name':'{3}', 'ICAO':'{4}', 'Lat':'{5}', 'Long':'{6}', 'Alt':'{7}','GeoNameCity':'{8}','GeoNameState':'{11}','CountryCode':'{9}','TimeZone':'{10}','GeoNameAirport':'" + airport.GeoNameAirport + "' }},",
               new string[] { airportraw.IATA, airport.City, airport.Country, airport.Name, airport.ICAO, airport.Latitude.ToString(), airport.Latitude.ToString(), airport.Altitude.ToString(), airport.GeoNameCity, airport.GeoCountry, airport.TimeZone, airport.GeoNameState });

                finalFile.WriteLine("{0}", linha);




            };

            finalFile.Close();


            return;
        }

        [TestMethod]
        public void ProcessALLGeonamesOfAirports()
        {


            var listAirp = Airport.ListAirports.Select(s => Airport.GetAirportByIata(s.Key)).ToList();

            var listOfICAO = listAirp.Select(s => s.ICAO).ToList();

            var lines2 = File.ReadAllLines("C:\\Users\\Liques\\Desktop\\allICAOnames.txt").AsParallel();

            var listOfICAOandGEONAMES = new Dictionary<string, string>();

            var teste = lines2.Where(w => w.Contains("icao")).ToList();

            lines2 = null;

            var lines = teste.ToArray();

            for (int i = 0; i < lines.Length; i++)
            {
                var currentline = lines[i];

                var words = currentline.Split('\t');

                if (words.Length > 3 && words[2] == "icao")
                {
                    if (!listOfICAOandGEONAMES.ContainsKey(words[3]))
                    {
                        listOfICAOandGEONAMES.Add(words[3], words[1]);

                        Debug.WriteLine(currentline);
                    }
                }
                else
                {
                    Debug.WriteLine(i);
                }

            }


            StreamWriter finalFile = File.AppendText(Environment.GetFolderPath(
                        System.Environment.SpecialFolder.MyDocuments) + "\\allGeocodeOfAirports.txt");

            //var listOfGeoCodeFromICAO = listAirp.Select(s => s.ICAO).Where(w => !string.IsNullOrEmpty(w)).ToList();
            var listOfGeoCodeFromICAO = listOfICAOandGEONAMES.Select(s => s.Value).ToList();


            for (int j = 1; j <= 7; j++)
            {


                var lines21 = File.ReadAllLines("D:\\Temp\\allCountries.txt.00" + j).ToList();
                var lines22 = lines21.Select(s => s.Replace("\t", ";").Split(';').First()).ToArray();
                
                var linesFound = new List<string>();

                //continue;

                var listOfNumbers = new List<int>();

                listOfGeoCodeFromICAO = listOfGeoCodeFromICAO.Where(w => !string.IsNullOrEmpty(w)).ToList();

                for (int i = 0; i < listOfGeoCodeFromICAO.Count; i++)
                {

                    var linhas = lines22.Where(w => w == listOfGeoCodeFromICAO[i]).ToList();
                        

                    foreach (var linha in linhas)
                    {
                        string phrase = lines21[Array.IndexOf(lines22, linha)];
                        phrase += "\t" + listOfICAOandGEONAMES.Where(ww => ww.Value == listOfGeoCodeFromICAO[i]).First().Key;// ; 
                        finalFile.WriteLine("{0}", phrase); // lines21[i]);
                        listOfGeoCodeFromICAO[i] = string.Empty;
                        Debug.WriteLine(phrase);

                    }
                    
                    Debug.WriteLine(string.Format("{0} - {1}/{2}", j, i, listOfGeoCodeFromICAO.Count));
                    
                }
            }

            finalFile.Close();


            foreach (var airportraw in listAirp)
            {

                var airport = airportraw;

                if (!String.IsNullOrEmpty(airport.ICAO) && listOfICAOandGEONAMES.ContainsKey(airport.ICAO))
                {

                    airport.GeoNameAirport = listOfICAOandGEONAMES[airport.ICAO];
                    Debug.WriteLine("------------------------------");

                }



                Debug.WriteLine(airportraw.IATA);



                var linha =
                string.Format(@"'{0}' : {{ 'City':'{1}', 'Country':'{2}', 'Name':'{3}', 'ICAO':'{4}', 'Lat':'{5}', 'Long':'{6}', 'Alt':'{7}','GeoNameCity':'{8}','GeoNameState':'{11}','CountryCode':'{9}','TimeZone':'{10}','GeoNameAirport':'" + airport.GeoNameAirport + "' }},",
               new string[] { airportraw.IATA, airport.City, airport.Country, airport.Name, airport.ICAO, airport.Latitude.ToString(), airport.Latitude.ToString(), airport.Altitude.ToString(), airport.GeoNameCity, airport.GeoCountry, airport.TimeZone, airport.GeoNameState });

                finalFile.WriteLine("{0}", linha);




            };

            finalFile.Close();


            return;
        }

        [TestMethod]
        public void test()
        {


            var listAirp = Airport.ListAirports.Select(s => Airport.GetAirportByIata(s.Key)).ToList();

            var listOfICAO = listAirp.Select(s => s.ICAO).ToList();

            var lines2 = File.ReadAllLines("C:\\Users\\Liques\\Desktop\\allICAOnames.txt").AsParallel();

            var listOfICAOandGEONAMES = new Dictionary<string, string>();

            var teste = lines2.Where(w => w.Contains("icao")).ToList();

            lines2 = null;

            var lines = teste.ToArray();

            for (int i = 0; i < lines.Length; i++)
            {
                var currentline = lines[i];

                var words = currentline.Split('\t');

                if (words.Length > 3 && words[2] == "icao")
                {
                    if (!listOfICAOandGEONAMES.ContainsKey(words[3]))
                    {
                        listOfICAOandGEONAMES.Add(words[3], words[1]);

                        Debug.WriteLine(currentline);
                    }
                }
                else
                {
                    Debug.WriteLine(i);
                }

            }


            StreamWriter finalFile = File.AppendText(Environment.GetFolderPath(
                        System.Environment.SpecialFolder.MyDocuments) + "\\allGeocodeOfAirports.txt");

            var listOfGeoCodeFromICAO = listOfICAOandGEONAMES.Select(s => s.Value).ToList();

            for (int j = 1; j <= 7; j++)
            {


                lines2 = File.ReadAllLines("D:\\Temp\\allCountries.txt.00" + j).AsParallel();

                listOfGeoCodeFromICAO = listOfGeoCodeFromICAO.Where(s => !string.IsNullOrEmpty(s)).ToList();

                var linesFound = new List<string>();

                int airN = 0;
                for (int i = 0; i < listOfGeoCodeFromICAO.Count; i++)
                {

                    var linhas = lines2.Where(w => w.StartsWith(listOfGeoCodeFromICAO[i] + "\t"));

                    foreach (var linha in linhas)
                    {
                        finalFile.WriteLine("{0}", linha);
                        listOfGeoCodeFromICAO[i] = string.Empty;
                        Debug.WriteLine(linha);

                    }

                    airN++;

                    Debug.WriteLine(string.Format("{0} - {1}/{2}", j, airN, listOfGeoCodeFromICAO.Count));

                    //var linhas = lines2.Where(w => listOfGeoCodeFromICAO.Any(a => w.Contains(a))).ToList();
                    //foreach (var linha in linhas)
                    //{
                    //    finalFile.WriteLine("{0}", linha);
                    //    Debug.WriteLine(j + " - " + linha);

                    //}

                    //}
                }
            }

            finalFile.Close();


            foreach (var airportraw in listAirp)
            {

                var airport = airportraw;

                if (!String.IsNullOrEmpty(airport.ICAO) && listOfICAOandGEONAMES.ContainsKey(airport.ICAO))
                {

                    airport.GeoNameAirport = listOfICAOandGEONAMES[airport.ICAO];
                    Debug.WriteLine("------------------------------");

                }



                Debug.WriteLine(airportraw.IATA);



                var linha =
                string.Format(@"'{0}' : {{ 'City':'{1}', 'Country':'{2}', 'Name':'{3}', 'ICAO':'{4}', 'Lat':'{5}', 'Long':'{6}', 'Alt':'{7}','GeoNameCity':'{8}','GeoNameState':'{11}','CountryCode':'{9}','TimeZone':'{10}','GeoNameAirport':'" + airport.GeoNameAirport + "' }},",
               new string[] { airportraw.IATA, airport.City, airport.Country, airport.Name, airport.ICAO, airport.Latitude.ToString(), airport.Latitude.ToString(), airport.Altitude.ToString(), airport.GeoNameCity, airport.GeoCountry, airport.TimeZone, airport.GeoNameState });

                finalFile.WriteLine("{0}", linha);




            };

            finalFile.Close();


            return;
        }

    }
}
