using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TowerBotLib;
using System.Collections.Generic;
using TowerBotLib.Filters;
using TowerBotFoundation;
using System.Linq;

namespace TowerBotTests
{
    [TestClass]
    public class OrbitTests
    {
        [TestMethod]
        public void OrbitDetection()
        {
            AirplaneBasic previousAirplane = null;
            List<double[]> listCoordinates = new List<double[]>() {
                new double[]{ -15.637897, -47.722253 },
                new double[]{ -15.637912, -47.735029 },
                new double[]{ -15.638442, -47.752658 },
                new double[]{ -15.638370, -47.767550 },
                new double[]{ -15.638861, -47.781742 },
                new double[]{ -15.642613, -47.795268 },
                new double[]{ -15.646521, -47.799969 },
                new double[]{ -15.651835, -47.801083 },
                new double[]{ -15.656175, -47.796291 },
                new double[]{ -15.657303, -47.789564 },
                new double[]{ -15.658037, -47.777531 },
                new double[]{ -15.655509, -47.772461 },
                new double[]{ -15.651385, -47.770213 },
                new double[]{ -15.645517, -47.769580 },
                new double[]{ -15.642711, -47.772751 },
                new double[]{ -15.641467, -47.775882 },
                new double[]{ -15.641078, -47.781404 },
                new double[]{ -15.640833, -47.790656 },
                //new double[]{ 0, 0 },
                //new double[]{ 0, 0 },
                //new double[]{ 0, 0 },
                //new double[]{ 0, 0 },
                //new double[]{ 0, 0 },
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

        [TestMethod]
        public void OrbitDetection2()
        {
            // "new double[]{ 0, 0 },new double[]{ 0, 0 },new double[]{ 0, 0 },new double[]{ 0, 0 },new double[]{ -21.92166, -49.29715 },new double[]{ -21.93008, -49.28562 },new double[]{ -21.95939, -49.24576 },new double[]{ -21.98979, -49.20436 },new double[]{ -22.0207, -49.16219 },new double[]{ -22.05324, -49.11758 },new double[]{ -22.08182, -49.07867 },new double[]{ -22.11292, -49.03608 },new double[]{ -22.1516, -48.98324 },"
            AirplaneBasic previousAirplane = null;
            List<double[]> listCoordinates = new List<double[]>() {
                new double[]{ -15.637897, -47.722253 },
                new double[]{ -15.637912, -47.735029 },
                new double[]{ -15.638442, -47.752658 },
                new double[]{ -15.638370, -47.767550 },
                new double[]{ -15.638861, -47.781742 },
                new double[]{ -15.642613, -47.795268 },
                new double[]{ -15.646521, -47.799969 },
                new double[]{ -15.651835, -47.801083 },
                new double[]{ -15.656175, -47.796291 },
                new double[]{ -15.657303, -47.789564 },
                new double[]{ -15.658037, -47.777531 },
                new double[]{ -15.655509, -47.772461 },
                new double[]{ -15.651385, -47.770213 },
                new double[]{ -15.645517, -47.769580 },
                new double[]{ -15.642711, -47.772751 },
                new double[]{ -15.641467, -47.775882 },
                new double[]{ -15.641078, -47.781404 },
                new double[]{ -15.640833, -47.790656 },
            };

            // a unica diferença do outro teste praticamente...
            listCoordinates.Reverse();

            for (int i = 0; i < listCoordinates.Count; i++)
            {
                var airplaneTeste = new AirplaneBasic();

                airplaneTeste.ID = "x1";
                airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
                airplaneTeste.FlightName = "TAM3506X";
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

        [TestMethod]
        public void OrbitDetectionShouldNot()
        {
            // ""
            AirplaneBasic previousAirplane = null;
            List<double[]> listCoordinates = new List<double[]>() {
               //new double[]{ 0, 0 },new double[]{ 0, 0 },new double[]{ 0, 0 },new double[]{ 0, 0 },new double[]{ -21.92166, -49.29715 },new double[]{ -21.93008, -49.28562 },new double[]{ -21.95939, -49.24576 },new double[]{ -21.98979, -49.20436 },new double[]{ -22.0207, -49.16219 },new double[]{ -22.05324, -49.11758 },new double[]{ -22.08182, -49.07867 },new double[]{ -22.11292, -49.03608 },new double[]{ -22.1516, -48.98324 },
               //new double[]{ 0, 0 },new double[]{ -18.84196, -45.31797 },new double[]{ -18.84196, -45.31797 },new double[]{ -18.87666, -45.29614 },new double[]{ -18.95207, -45.24854 },new double[]{ -18.96725, -45.239 },new double[]{ -19.00733, -45.21373 },new double[]{ -19.0502, -45.18682 },new double[]{ -19.09368, -45.1593 },new double[]{ -19.13763, -45.13144 },new double[]{ -19.18087, -45.10417 },
              // new double[]{ -25.09355, -47.13191 },new double[]{ 0, 0 },new double[]{ 0, 0 },new double[]{ 0, 0 },new double[]{ 0, 0 },new double[]{ -25.26548, -47.39352 },new double[]{ -25.27233, -47.41082 },new double[]{ -25.28989, -47.45524 },new double[]{ -25.31106, -47.50867 },
              //new double[]{ -25.85563, -47.92654 },new double[]{ -25.92755, -48.00279 },new double[]{ -25.92755, -48.00279 },new double[]{ -26.02615, -48.09685 },new double[]{ -26.04259, -48.11245 },
              //new double[]{ -15.87142, -47.91949 },new double[]{ -15.8714, -47.91952 },new double[]{ -15.87143, -47.91949 },new double[]{ -15.87141, -47.91951 },new double[]{ -15.87143, -47.91949 },
              //new double[]{ -22.44465, -47.95629 },new double[]{ -22.45775, -47.98833 },new double[]{ -22.48874, -47.98928 },new double[]{ -22.50526, -47.96321 },new double[]{ -22.51817, -47.9325 },new double[]{ -22.52673, -47.90249 },new double[]{ -22.51262, -47.87852 },new double[]{ -22.48334, -47.88047 },new double[]{ -22.46736, -47.90858 },new double[]{ -22.45486, -47.93919 },new double[]{ -22.44265, -47.9691 },new double[]{ -22.43008, -47.99993 },new double[]{ -22.41579, -48.03497 },new double[]{ -22.40317, -48.0659 },
              new double[]{ -21.75665, -45.35176 },new double[]{ -21.88989, -45.41364 },new double[]{ -21.99849, -45.46407 },new double[]{ -22.06967, -45.49718 },new double[]{ -22.09515, -45.50891 },new double[]{ -22.15882, -45.53833 },new double[]{ -22.19398, -45.55421 },new double[]{ -22.26352, -45.58542 },new double[]{ -22.299, -45.60125 },new double[]{ -22.35243, -45.62541 },new double[]{ -22.40671, -45.65043 },new double[]{ -22.45712, -45.67474 },new double[]{ -22.51048, -45.70108 },new double[]{ -22.56429, -45.72856 },new double[]{ -22.61073, -45.76205 },new double[]{ -22.65779, -45.79861 },new double[]{ -22.7187, -45.83918 },new double[]{ -22.75877, -45.85983 },
            };

            // a unica diferença do outro teste praticamente...
            //listCoordinates.Reverse();

            for (int i = 0; i < listCoordinates.Count; i++)
            {
                var airplaneTeste = new AirplaneBasic();

                airplaneTeste.ID = "LA35";
                airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
                airplaneTeste.FlightName = "LA3506X";
                airplaneTeste.From = Airport.GetAirportByIata("GRU");
                airplaneTeste.To = Airport.GetAirportByIata("BOG");
                airplaneTeste.Altitude = 5500;
                airplaneTeste.VerticalSpeed = -500;
                airplaneTeste.Speed = 471;
                airplaneTeste.AircraftType = AircraftType.GetAircraftType("B763");
                airplaneTeste.Registration = new AircraftRegistration("CC-CJX");
                airplaneTeste.Latitude = listCoordinates[i][0];
                airplaneTeste.Longitude = listCoordinates[i][1];

                
                if (previousAirplane != null)
                {
                    airplaneTeste.Radars.First().LastAirplanes = new List<AirplaneBasic>();
                    airplaneTeste.Radars.First().LastAirplanes.Add(previousAirplane);
                }

                if(i == 13)
                {
                    string goat = "";
                }

                airplaneTeste.FinalConvertAirplaneRules();
                
                previousAirplane = airplaneTeste;
            }

            List<AirplaneBasic> listAirplanes = new List<AirplaneBasic>();
            listAirplanes.Add(previousAirplane);

            FilterRatification rati = new FilterRatification(false, false, false, false, true);
            rati.Radar = Radar.GetRadar("BSB");
            List<AlertFilter> listAlerts = rati.Analyser(listAirplanes);

            Assert.IsTrue(listAlerts.Count == 0);

        }

        [TestMethod]
        public void OrbitDetectionShouldNotDetect()
        {
            AirplaneBasic previousAirplane = null;
            List<double[]> listCoordinates = new List<double[]>() {
                new double[]{ -15.637897, -47.722253 },
                new double[]{ -15.637912, -47.735029 },
                new double[]{ -15.638442, -47.752658 },
                new double[]{ -15.638370, -47.767550 },
                new double[]{ -15.638861, -47.781742 },
                new double[]{ -15.642613, -47.795268 },
                new double[]{ -15.646521, -47.799969 },
                new double[]{ -15.651835, -47.801083 },
                new double[]{ -15.656175, -47.796291 },
                new double[]{ -15.657303, -47.789564 },
                new double[]{ -15.658037, -47.777531 },
                new double[]{ -15.655509, -47.772461 },
                new double[]{ -15.651385, -47.770213 },
                new double[]{ -15.645517, -47.769580 },
            };

            for (int i = 0; i < listCoordinates.Count; i++)
            {
                var airplaneTeste = new AirplaneBasic();

                airplaneTeste.ID = "x2";
                airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
                airplaneTeste.FlightName = "TAM3506X";
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

            Assert.IsTrue(listAlerts.Count == 0);

        }
    }
}
