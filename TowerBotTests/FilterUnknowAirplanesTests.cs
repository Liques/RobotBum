using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TowerBotLib;
using TowerBotLib.Filters;
using System.Collections.Generic;
using TowerBotFoundation;

namespace TowerBotTests
{
    [TestClass]
    public class FilterUnknowAirplanesTests
    {
        [TestMethod]
        public void UnknowIsSpecialPainting()
        {
            var filterWide = new FilterUnknowAirplanes(false,false)
            {
                Radar = Radar.GetRadar("BSB"),
            };


            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.ID = "E48CEA";
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "AZU1234";
            airplaneTeste.From = Airport.GetAirportByIata("RBR");
            airplaneTeste.To = Airport.GetAirportByIata("BSB");
            airplaneTeste.Altitude = 6999;
            airplaneTeste.VerticalSpeed = -501;
            airplaneTeste.Speed = 150;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("A319");
            airplaneTeste.State = AirplaneStatus.Landing;
            airplaneTeste.Registration = new AircraftRegistration("PR-AXH");
            airplaneTeste.Longitude = -48.988926;
            airplaneTeste.Latitude = -16.867119;
            airplaneTeste.FinalConvertAirplaneRules();

            var listAirplanes = new List<AirplaneBasic>();
            listAirplanes.Add(airplaneTeste);
            var alertList = filterWide.Analyser(listAirplanes);

            if (alertList.Count > 0)
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);
        }

        [TestMethod]
        public void UnknowAnyDifferent1()
        {
            var filterWide = new FilterUnknowAirplanes(false, false)
            {
                Radar = Radar.GetRadar("BSB"),
            };


            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.ID = "E48A78";
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "PT-XTS";
            airplaneTeste.From = Airport.GetAirportByIata("RBR");
            airplaneTeste.To = Airport.GetAirportByIata("BSB");
            airplaneTeste.Altitude = 6999;
            airplaneTeste.VerticalSpeed = -501;
            airplaneTeste.Speed = 150;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("A319");
            airplaneTeste.State = AirplaneStatus.Landing;
            airplaneTeste.Registration = new AircraftRegistration("PT-XTS");
            airplaneTeste.Longitude = -48.988926;
            airplaneTeste.Latitude = -16.867119;
            airplaneTeste.FinalConvertAirplaneRules();

            var listAirplanes = new List<AirplaneBasic>();
            listAirplanes.Add(airplaneTeste);
            var alertList = filterWide.Analyser(listAirplanes);

            if (alertList.Count > 0)
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);
        }


        [TestMethod]
        public void UnknowShoudNotDetectHeavyAirplaneBasic()
        {
            var filterWide = new FilterUnknowAirplanes(false, false)
            {
                Radar = Radar.GetRadar("BSB"),
            };


            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.ID = "E48A78";
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "PT-XTS";
            airplaneTeste.From = Airport.GetAirportByIata("RBR");
            airplaneTeste.To = Airport.GetAirportByIata("BSB");
            airplaneTeste.Altitude = 6999;
            airplaneTeste.VerticalSpeed = -501;
            airplaneTeste.Speed = 150;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("B772");
            airplaneTeste.State = AirplaneStatus.Landing;
            airplaneTeste.Registration = new AircraftRegistration("PT-XTS");
            airplaneTeste.Longitude = -48.988926;
            airplaneTeste.Latitude = -16.867119;
            airplaneTeste.FinalConvertAirplaneRules();

            var listAirplanes = new List<AirplaneBasic>();
            listAirplanes.Add(airplaneTeste);
            var alertList = filterWide.Analyser(listAirplanes);

            if (alertList.Count == 0)
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);
        }
        
    }
}
