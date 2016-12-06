using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TowerBotFoundation;
using TowerBotLibrary.Map;

namespace TowerBotLibrary
    
{

    public class AirplaneBasic : Airplane
    {

        public List<Radar> Radars { get; set; }
        public Chart FollowingChart { get; set; }
        public PluginAlertType LastAlertType { get; set; }
        public AirplaneBasic PreviousAirplane { get; set; }        

        private static Dictionary<string, string> ListSpecialPainitngs = new Dictionary<string, string>();

        public AirplaneBasic()
        {
            Radars = new List<Radar>();
        }


        public static AirplaneBasic ConvertToAirplane(Radar radar, object[] jsonData, string keyName)
        {
            try
            {
                AirplaneBasic airplane = new AirplaneBasic()
                {
                    ID = !String.IsNullOrEmpty(jsonData[0].ToString()) ? jsonData[0].ToString() : keyName,
                    AircraftType = (AircraftType)jsonData[8].ToString(),
                    Altitude = Convert.ToInt32(jsonData[4]),
                    Direction = Convert.ToInt32(jsonData[3]),
                    From = Airport.GetAirportByIata(jsonData[11].ToString()),
                    FlightName = jsonData[16].ToString(),
                    Airline = Airline.GetAirlineByFlight(jsonData[16].ToString()),
                    Latitude = Convert.ToDouble(jsonData[1]),
                    Longitude = Convert.ToDouble(jsonData[2]),
                    Registration = new AircraftRegistration(jsonData[9].ToString()),
                    Speed = Convert.ToInt32(jsonData[5]),
                    To = Airport.GetAirportByIata(jsonData[12].ToString()),
                    VerticalSpeed = Convert.ToDouble(jsonData[15]),
                    DateCreation = DateTime.Now,
                    SpecialDescription = String.Empty
                };

                airplane.Radars.Add(radar);
                airplane.FinalConvertAirplaneRules();

                airplane.UpdateAirplaneStatus();


                return airplane;
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Airplane Conversion from JSON");
                return null;
            }

        }

        public static AirplaneBasic ConvertToAirplane(Radar radar, string hexCode, string flightName, string altitude, string latitude, string longitude, string speed, string verticalSpeed, string direction, string from, string to, string model, string registration)
        {
            try
            {
                AirplaneBasic airplane = new AirplaneBasic()
                {
                    ID = hexCode,
                    AircraftType = (AircraftType)(model),
                    Altitude = String.IsNullOrEmpty(altitude) ? 0 : Math.Round(Convert.ToDouble(altitude)),
                    Direction = String.IsNullOrEmpty(direction) ? 0 : Convert.ToDouble(direction),
                    From = Airport.GetAirportByIata(from),
                    FlightName = flightName,
                    Airline = Airline.GetAirlineByFlight(flightName),
                    Latitude = String.IsNullOrEmpty(latitude) ? 0 : Convert.ToDouble(latitude),
                    Longitude = String.IsNullOrEmpty(longitude) ? 0 : Convert.ToDouble(longitude),
                    Registration = new AircraftRegistration(registration),
                    Speed = String.IsNullOrEmpty(speed) ? 0 : Math.Round(Convert.ToDouble(speed)),
                    To = Airport.GetAirportByIata(to),
                    VerticalSpeed = String.IsNullOrEmpty(verticalSpeed) ? 0 : Convert.ToDouble(verticalSpeed),
                    DateCreation = DateTime.Now,
                };

                airplane.Radars.Add(radar);


                airplane.FinalConvertAirplaneRules();

                airplane.UpdateAirplaneStatus();


                return airplane;
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Airplane Conversion from JSON");
                return null;
            }

        }


        public void FinalConvertAirplaneRules()
        {
            try
            {
                this.ID = String.IsNullOrEmpty(this.ID) ? this.Registration.Name : this.ID;

                var lastAirplanesFromRadars = new List<AirplaneBasic>();
                this.Radars.ForEach(radar =>
                {
                    if (radar.LastAirplanes != null && radar.LastAirplanes.Count > 0)
                        lastAirplanesFromRadars.AddRange(radar.LastAirplanes);
                });

                this.PreviousAirplane = lastAirplanesFromRadars.Where(s => s.ID == this.ID).FirstOrDefault();


                this.Weight = TowerBotLibrary.Plugins.HelperPlugin.ListWideAirplanes.Where(s => this.AircraftType.ICAO.StartsWith(s)).Count() > 0 ? AirplaneWeight.Heavy : AirplaneWeight.NotSet;

                if (this.Weight == AirplaneWeight.NotSet)
                    this.Weight = TowerBotLibrary.Plugins.HelperPlugin.ListCommonAirplanes.Where(s => this.AircraftType.ICAO.StartsWith(s)).Count() > 0 ? AirplaneWeight.Medium : AirplaneWeight.Light;
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Error on ID, PreviousAirplane or Weight");
            }

            try
            {
                this.DateExpiration = DateTime.Now.AddHours(1);
                if (this.From != null || this.To != null)
                    this.FlightDistance = (!string.IsNullOrEmpty(this.From.City) && !string.IsNullOrEmpty(this.To.City)) ? MathHelper.GetGPSDistance(this.From.Latitude, this.To.Latitude, this.To.Longitude, this.To.Longitude) : 0;

                if (!this.AircraftType.IsValid || !this.Registration.IsValid)
                {
                    var dataByHexCode = new HexCodeAirplane(this.ID);
                    if (dataByHexCode.IsValid)
                    {
                        if (!this.AircraftType.IsValid && dataByHexCode.AircraftType.IsValid && dataByHexCode.AircraftType.Name != ".NO-REG")
                            this.AircraftType = dataByHexCode.AircraftType;

                        if (!this.Registration.IsValid && dataByHexCode.Registration.IsValid)
                            this.Registration = dataByHexCode.Registration;

                        if (!String.IsNullOrEmpty(dataByHexCode.Description))
                            this.SpecialDescription = dataByHexCode.Description;
                    }
                }
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Error on DateExpiration, FlightDistance, dataByHexCode or Registration");
            }

            #region parte de pinturas e avioes especiais
            try
            {
                if (ListSpecialPainitngs.Count <= 0)
                {
                    string specialPaintings = @"A3C880;N343AN;Com a pintura One World|
E48E74;PT-MSY;O avião da tocha olímpica|
E06413;LV-FPS;Com a pintura SKYTEAM Livery|
E48A71;PR-AVR;Com a pintura Star Alliance|
E485A4;PR-AVB;Com a pintura Star Alliance|
E490F0;PR-OCN;Com a pintura da Turma da Mônica|
E484E9;PP-PTU;Com a pintura Tudo Azul|
E48AD0;PR-ATB;De pintura Rosa|
E48A98;PP-PJQ;Com a pintura Canarinho|
E48F6C;PR-AUA;Com a pintura Canarinho|
E48AFC;PR-AXB;Com a pintura da Coca-Cola Zero|
E48CEA;PR-AXH;Com a pintura Verão Azul|
E48EA8;PR-AXV;Com a pintura Espírito de União|
E486ED;PR-AYL;Com a pintura da SKY|
E48763;PR-AYO;Pintado de Rosa|
E48963;PR-AYV;Pintado com a bandeira|
E489DD;PR-AYX;De verde|
E48A09;PR-AYY;Com a pintura Azul Viagens|
E4898A;PR-GUM;Com a pintura da CBF|
E48AFE;PR-GUO;O avião arte dos Gêmeos|
E47ECD;PR-GTA;Com as novas cores da Gol|
E47ECE;PR-GTB;De novas cores da Gol|
E48007;PR-GTP;Com as novas cores da Gol|
E4800A;PR-GTT;Com as novas cores da Gol|
E49224;PR-GYB;De as novas cores da Gol|
E48005;PR-GTN;Com as novas cores da Gol|
E491C2;PR-GXZ;Com as novas cores da Gol|
E47E4B;PR-GOH;O líder das novas cores da Gol|
E48161;PR-GIT;Com a pintura Smiles|
E48853;PR-GUG;Com Scimitar na asa|
E48854;PR-GUH;Com Split Scimitar na asa|
E4851B;PT-TMD;Com a pintura Rio 450 Anos|
E483DE;PR-MYF;Com a pintura One World|
4D012A;;Com a pintura híbrida CLX/Cathay|
4D010C;LX-WCV;Com a pintura New Colors|
4D0128;LX-SCV;Com a pintura New Colors|
E48B1D;PR-ATH;Com a pintura Sticker 50 Aeronaves|
E48E7D;PR-AXS;Com a pintura Sticker 100.000.000|
3C7068;;Com a pintura Cargo Human Care|
3C7063;D-ALCC;Com a pintura Sticker A.D.H|
E48909;PR-AYU;Com a pintura Senna Sempre|
E491A0;;Com a pintura World|
E49069;;Com a pintura One World|
N174AA;;Com a pintura One World|
0C202C;;Com a pintura Copa Baseball|
0C203A;;Com a pintura Copa Seleção|
0C2075;;Com a pintura Star Allience|
0C2081;;Com a pintura Biomuseo|
0C20A8;;Com a pintura Star Alliance|
0C20BB;;Com a pintura ConnectMilles|
AE20C4;PIU-PIU;O avião da galinha pintadinha (cócó!)|
AE4F16;XU-XA;A nave espacial da Xuxa (êêêêê)|
4951E8;;Com a pintura Star Alliance";

                    List<string> listSpecialPainitngsRaw = specialPaintings.Trim().Replace("\n", "").Replace("\r", "").Split('|').ToList();

                    for (int i = 0; i < listSpecialPainitngsRaw.Count; i++)
                    {
                        string[] valuestring = listSpecialPainitngsRaw[i].Split(';');
                        ListSpecialPainitngs.Add(valuestring[0], valuestring[2]);

                        //var registration = new HexCodeAirplane(valuestring[0]).Registration;

                        //System.Diagnostics.Debug.WriteLine("{0};{1};{2}|", valuestring[0], registration, valuestring[1]);
                    }
                }
                if (ListSpecialPainitngs.ContainsKey(this.ID))
                {
                    this.IsSpecial = true;
                    this.SpecialDescription = ListSpecialPainitngs[this.ID];
                }
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Error on Special Painting");
            }
            #endregion

            #region parte que vai detectar mais dados de um aviao em modo S
            try
            {
                bool isModeS = this.Latitude == 0 && this.Longitude == 0 && this.VerticalSpeed == 0 && this.Altitude > 0;

                if (isModeS && this.PreviousAirplane != null)
                {
                    double currentUpdateTimeInSeconds = (this.DateCreation - this.PreviousAirplane.DateCreation).TotalSeconds;
                    double percentage = currentUpdateTimeInSeconds / 60;

                    this.VerticalSpeed = (this.Altitude - this.PreviousAirplane.Altitude) * percentage;
                }
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Erro on Mode S detection");
            }
            #endregion


            #region Verificar se o avião é de algum país conhecido
            try
            {
                string[] listKnownCountries = new string[] { "Brasil", "EUA", "Inglaterra", "Canadá", "Uruguai", "Bolivia", "Argentina", "Chile", "Espanha", "Portugal", "França", "Panama", "Colômbia", "Países Baixos", "México", "Reino Unido", "Coreia do Sul" };
                this.IsKnowCountry = listKnownCountries.Where(s => s == this.Registration.Country).Count() > 0 || !this.Registration.IsValid; // Ou se não tiver registro, deixar como conhecido
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Error on listKnownCountries");
            }
            #endregion

            if (this.Radars.Count == 1 && this.Radars.First().Name == "BRA")
            {
               

                Airport airport = null;

                double distance = 500;

                foreach (var airportLoop in PluginsManager.ListMainAirports)
                {
                    var discanceLoop = MathHelper.GetGPSDistance(this.Latitude, airportLoop.Latitude, this.Longitude, airportLoop.Longitude);

                    if (discanceLoop < distance)
                    {
                        airport = airportLoop;
                        distance = discanceLoop;
                    }
                }
                if (airport != null)
                {
                    this.Radars.Add(Radar.GetRadar(airport.IATA));
                }
            }



            UpdateAirplaneStatus();


            if (this.PreviousAirplane != null)
            {

                #region Verificar se esta seguindo algum chart
                try
                {
                    if (this.PreviousAirplane.FollowingChart == null)
                    {
                        foreach (var radar in this.Radars)
                        {
                            var regionChart = Chart.ListCharts.Where(s => s.Region == radar.Name).ToList();
                            double direction = MapMathHelper.GetAngle(this.PreviousAirplane.Longitude, this.Longitude, this.PreviousAirplane.Latitude, this.Latitude);
                            for (int i = 0; i < regionChart.Count; i++)
                            {
                                if (regionChart[i].IsFollowingChart(this.Longitude, this.Latitude, direction))
                                {
                                    if (regionChart[i].ChartType == ChartType.Star && this.PreviousAirplane.State == AirplaneStatus.Landing && this.State == AirplaneStatus.Landing ||
                                        regionChart[i].ChartType == ChartType.SID && this.PreviousAirplane.State == AirplaneStatus.TakingOff && this.State == AirplaneStatus.TakingOff)
                                        this.FollowingChart = regionChart[i];
                                    break;
                                }
                            }

                            if (this.FollowingChart != null)
                                break;

                        }


                    }
                    else
                    {
                        this.FollowingChart = this.PreviousAirplane.FollowingChart;
                    }

                }
                catch (Exception e)
                {
                    ErrorManager.ThrowError(e, "Error on Charts");
                }
                #endregion

                #region Verificar se esta na final de alguma runway
                try
                {
                    foreach (var radar in this.Radars)
                    {
                        for (int i = 0; i < radar.ListRunways.Count; i++)
                        {
                            double direction = MapMathHelper.GetAngle(this.PreviousAirplane.Longitude, this.Longitude, this.PreviousAirplane.Latitude, this.Latitude);
                            string runwayName = radar.ListRunways[i].IsAirplaneInFinalRunway(this, direction);

                            if (!String.IsNullOrEmpty(runwayName))
                            {
                                this.RunwayName = runwayName;
                                break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorManager.ThrowError(e, "Error on runway detection");
                }

                #endregion

                #region Detectar diferença de valor de mudança de lado
                if (this.PreviousAirplane.PreviousAirplane != null)
                {

                    double directionFirst = MapMathHelper.GetAngle(this.PreviousAirplane.PreviousAirplane.Longitude, this.PreviousAirplane.Longitude, this.PreviousAirplane.PreviousAirplane.Latitude, this.PreviousAirplane.Latitude);
                    double directionSecond = MapMathHelper.GetAngle(this.PreviousAirplane.Longitude, this.Longitude, this.PreviousAirplane.Latitude, this.Latitude);

                    var diffrenceDegrees = directionFirst - directionSecond;
                    if (diffrenceDegrees > 180)
                        directionFirst -= 360;
                    else if (diffrenceDegrees < -180)
                        directionSecond -= 360;

                    this.DirectionChange = directionSecond - directionFirst;




                }
                #endregion

                #region Detecção de órbitas
                if (this.State != AirplaneStatus.ParkingOrTaxing)
                {
                    int maxReq = 18;
                    int minReq = 8;
                    int numReq = 0;
                    AirplaneBasic currentAir = this;
                    double directionSum = 0;

                    string log = "";
                    string gps = "";

                    while (true)
                    {
                        numReq++;

                        if (currentAir.PreviousAirplane == null || numReq >= maxReq)
                            break;
                        else if (currentAir.PreviousAirplane.State == AirplaneStatus.ParkingOrTaxing)
                            break;

                        decimal distanceLatitude = Convert.ToDecimal(currentAir.Latitude) - Convert.ToDecimal(currentAir.PreviousAirplane.Latitude);
                        decimal distanceLongitude = Convert.ToDecimal(currentAir.Longitude) - Convert.ToDecimal(currentAir.PreviousAirplane.Longitude);
                        decimal distanceToCalculate = Convert.ToDecimal(0.00005);

                        if (currentAir.Latitude == 0 || currentAir.Longitude == 0 ||
                            currentAir.Latitude == currentAir.PreviousAirplane.Latitude && currentAir.Longitude == currentAir.PreviousAirplane.Longitude ||
                            distanceLatitude < distanceToCalculate * -1 && distanceLatitude > distanceToCalculate &&
                           distanceLongitude < distanceToCalculate * -1 && distanceLongitude > distanceToCalculate ||
                           currentAir.DirectionChange < -90 || currentAir.DirectionChange > 90
                           )
                        {
                            currentAir = currentAir.PreviousAirplane;
                            continue;
                        }

                        directionSum += currentAir.DirectionChange;
                        log += " " + currentAir.DirectionChange;

                        gps += "new double[]{ " + currentAir.Latitude + ", " + currentAir.Longitude + " },";


                        currentAir = currentAir.PreviousAirplane;


                    }

                    if (numReq >= minReq && (directionSum < -300 || directionSum > 300))
                    {
                        this.IsOrbiting = true;
                    }
                }
                #endregion
            }

        }

        public static string lll = "kkkk";

        public void UpdateAirplaneStatus()
        {
            try
            {
                if (this.Altitude <= 28000 && this.VerticalSpeed < -500)
                {
                    this.State = AirplaneStatus.Landing;
                    this.StateJustify = "Velocidade vertical abaixo de -500:" + this.VerticalSpeed + " e altude menor que 28000:" + this.Altitude;
                }
                else if (this.Altitude <= 28000 && this.VerticalSpeed < 0 && this.PreviousAirplane != null)
                {
                    if (this.PreviousAirplane.State == AirplaneStatus.Landing)
                    {
                        this.State = AirplaneStatus.Landing;
                        this.StateJustify = "(PreviousAirplane)Velocidade vertical abaixo de -500:" + this.VerticalSpeed + " e altude menor que 28000:" + this.Altitude;
                    }
                }

                else if (this.Altitude >= 4000 && this.Altitude <= 18000 && this.VerticalSpeed > 500)
                {
                    this.State = AirplaneStatus.TakingOff;
                    this.StateJustify = "Velocidade vertical superior a 500:" + this.VerticalSpeed + " e altude entre 4000 e 18000:" + this.Altitude;
                }
                else if (this.Altitude >= 4000 && this.Altitude <= 180000 && this.VerticalSpeed > 0 && this.PreviousAirplane != null)
                {
                    if (this.PreviousAirplane.State == AirplaneStatus.TakingOff)
                    {
                        this.State = AirplaneStatus.TakingOff;
                        this.StateJustify = "(PreviousAirplane) Velocidade vertical superior a 500:" + this.VerticalSpeed + " e altude entre 4000 e 18000:" + this.Altitude;
                    }
                }
                else if (this.Altitude == 0 && this.Speed < 35)
                {
                    this.State = AirplaneStatus.ParkingOrTaxing;
                    this.StateJustify = "Velocidade (speed) menor que 35:" + this.Speed + "kts e altude igual a zero:" + this.Altitude;
                }
                else if (this.Altitude >= 28000 && this.VerticalSpeed < 200)
                {
                    this.State = AirplaneStatus.Cruise;
                    this.StateJustify = "Velocidade (speed) abaixo de 200:" + this.Speed + "kts e altude maior que 28000:" + this.Altitude;

                }
                else if (this.Altitude < 28000 && this.PreviousAirplane != null)
                {
                    if (this.PreviousAirplane.PreviousAirplane != null)
                    {
                        if (this.PreviousAirplane.State == AirplaneStatus.Landing && this.PreviousAirplane.PreviousAirplane.State == AirplaneStatus.Landing)
                        {
                            this.State = AirplaneStatus.Landing;
                            this.StateJustify = "(PreviousAirplane) Velocidade (speed) abaixo de 200:" + this.Speed + "kts e altude menor que 28000:" + this.Altitude;
                        }
                        else if (this.PreviousAirplane.State == AirplaneStatus.TakingOff && this.PreviousAirplane.PreviousAirplane.State == AirplaneStatus.TakingOff)
                        {
                            this.State = AirplaneStatus.TakingOff;
                            this.StateJustify = "(PreviousAirplane) Velocidade (speed) acima de 200:" + this.Speed + "kts e altude menor que 28000:" + this.Altitude;
                        }
                    }
                }

                if (this.Altitude > 0 && this.Longitude == 0 && this.Latitude == 0)
                {
                    //this.State = AirplaneStatus.ModeS;
                    this.StateJustify = ",em Modo S,";
                }

                this.StateJustify += ", Hex:" + this.ID + ", ";

            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Error on UpdateAirplaneStatus");
            }
        }

        public override string ToString()
        {
            return this.FlightName + " _ " + this.Registration;
        }

        public static bool IsSpecialPainting()
        {
            string responseBodyAsText = String.Empty;
            List<AirplaneBasic> listAirplanes = null;
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = null;

            response = httpClient.GetAsync("https://docs.google.com/spreadsheets/d/1kgqJ9RUx4irbnNxhhMOnzh9G9twyagJxqAS4mHjRw-k/export?gid=0&format=csv", HttpCompletionOption.ResponseContentRead).Result;
            responseBodyAsText = response.Content.ReadAsStringAsync().Result;
            return true;
        }

        public override bool Equals(object obj)
        {
            Airplane other = obj as Airplane;
            return other != null && other.ID != this.ID;
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }

    }


}
