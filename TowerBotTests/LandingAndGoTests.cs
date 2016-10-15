using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TowerBotLib;
using TowerBotLib.Filters;
using TowerBotFoundation;
using System.Linq;

namespace TowerBotTests
{
    [TestClass]
    [Ignore]
    public class LandingAndGoTests
    {
        [TestMethod]
        public void IsLandingAndGo()
        {
            AirplaneBasic previousAirplane = null;
            List<double[]> listCoordinates = new List<double[]>() {
                new double[]{ -15.864680, -47.957525 },
                new double[]{ -15.864099, -47.954031 },
                new double[]{ -15.864099, -47.954031 },
                new double[]{ -15.863672, -47.947210 },
                new double[]{ -15.863517, -47.940539},
                new double[]{ -15.863987, -47.936823 },
                new double[]{ -15.863889, -47.932252  }, // ultima posicao antes da pista
                new double[]{ -15.863430, -47.925508 },
                new double[]{ -15.863137, -47.920303 },
                new double[]{ -15.862791, -47.916622 },
            };

            for (int i = 0; i < listCoordinates.Count; i++)
            {
                var airplaneTeste = new AirplaneBasic();

                airplaneTeste.ID = "x";
                airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
                airplaneTeste.FlightName = "TAM3556X";
                airplaneTeste.From = Airport.GetAirportByIata("PHB");
                airplaneTeste.To = Airport.GetAirportByIata("BSB");
                airplaneTeste.Altitude = 15000;
                airplaneTeste.VerticalSpeed = 0;
                airplaneTeste.Speed = 200;
                airplaneTeste.AircraftType = AircraftType.GetAircraftType("A319");
                airplaneTeste.Registration = new AircraftRegistration("PR-GYW");
                airplaneTeste.Latitude = listCoordinates[i][0];
                airplaneTeste.Longitude = listCoordinates[i][1];

                if (i == 0)
                {
                    airplaneTeste.Altitude = 15500;
                    airplaneTeste.VerticalSpeed = -600;
                }

                if (previousAirplane != null)
                {
                    airplaneTeste.Radars.First().LastAirplanes = new List<AirplaneBasic>();
                    airplaneTeste.Radars.First().LastAirplanes.Add(previousAirplane);
                }

                airplaneTeste.FinalConvertAirplaneRules();

                //airplaneTeste.PreviousAirplane = previousAirplane;

                previousAirplane = airplaneTeste;
            }

            List<AirplaneBasic> listAirplanes = new List<AirplaneBasic>();
            listAirplanes.Add(previousAirplane);

            FilterRatification rati = new FilterRatification(false, false, false, false, true);
            rati.Radar = Radar.GetRadar("BSB");
            List<AlertFilter> listAlerts = rati.Analyser(listAirplanes);

            Assert.IsTrue(listAlerts.Count > 0);


        }
    }
}
