using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TowerBotLib;
using TowerBotFoundation;

namespace TowerBotTests
{
    [TestClass]
    public class RunwayDetectionTests
    {
        [TestMethod]
        public void RunwayTestsIfAirplaneFinal11L()
        {
            //, 
            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "TAM9771X";
            airplaneTeste.From = Airport.GetAirportByIata("MAO");
            airplaneTeste.To = Airport.GetAirportByIata("BSB");
            airplaneTeste.Altitude = 6999;
            airplaneTeste.VerticalSpeed = -501;
            airplaneTeste.Speed = 150;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("A320");
            airplaneTeste.State = AirplaneStatus.Landing;
            airplaneTeste.Registration = new AircraftRegistration("PR-MYH");
            airplaneTeste.Longitude = -47.988926;
            airplaneTeste.Latitude = -15.867119;
            airplaneTeste.FinalConvertAirplaneRules();

            var runway1 = new RunwayBasic()
            {
                NameSideOne = "11L",
                NameSideTwo = "29R",
                LatitudeSideOne = -15.861333,
                LongitudeSideOne = -47.930333,
                LatitudeSideTwo = -15.86,
                LongitudeSideTwo = -47.898167,
            };

            string runway = runway1.IsAirplaneInFinalRunway(airplaneTeste);

            if (runway == "11L")
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);

        }

        [TestMethod]
        public void RunwayTestsIfAirplaneFinal29R()
        {
           
            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "TAM9771X";
            airplaneTeste.From = Airport.GetAirportByIata("MAO");
            airplaneTeste.To = Airport.GetAirportByIata("BSB");
            airplaneTeste.Altitude = 6999;
            airplaneTeste.VerticalSpeed = -501;
            airplaneTeste.Speed = 150;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("A320");
            airplaneTeste.State = AirplaneStatus.Landing;
            airplaneTeste.Registration = new AircraftRegistration("PR-MYH");
            airplaneTeste.Longitude = -47.865509;
            airplaneTeste.Latitude = -15.858587;
            airplaneTeste.FinalConvertAirplaneRules();

            var runway1 = new RunwayBasic()
            {
                NameSideOne = "11L",
                NameSideTwo = "29R",
                LatitudeSideOne = -15.861333,
                LongitudeSideOne = -47.930333,
                LatitudeSideTwo = -15.86,
                LongitudeSideTwo = -47.898167,
            };

            string runway = runway1.IsAirplaneInFinalRunway(airplaneTeste);

            if (runway == "29R")
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);

        }


        [TestMethod]
        public void RunwayTestsIfAirplaneTakingOff11L()
        {

            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "TAM9071X";
            airplaneTeste.From = Airport.GetAirportByIata("BSB");
            airplaneTeste.To = Airport.GetAirportByIata("MAO");
            airplaneTeste.Altitude = 6999;
            airplaneTeste.VerticalSpeed = 1501;
            airplaneTeste.Speed = 170;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("A320");
            airplaneTeste.State = AirplaneStatus.TakingOff;
            airplaneTeste.Registration = new AircraftRegistration("PR-MYH");
            airplaneTeste.Longitude = -47.865509;
            airplaneTeste.Latitude = -15.858587;
            airplaneTeste.FinalConvertAirplaneRules();

            var runway1 = new RunwayBasic()
            {
                NameSideOne = "11L",
                NameSideTwo = "29R",
                LatitudeSideOne = -15.861333,
                LongitudeSideOne = -47.930333,
                LatitudeSideTwo = -15.86,
                LongitudeSideTwo = -47.898167,
            };

            string runway = runway1.IsAirplaneInFinalRunway(airplaneTeste);

            if (runway == "11L")
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);

        }

        [TestMethod]
        public void RunwayTestsIfAirplaneIsNoInRunway()
        {
            //, 
            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "TAM9771X";
            airplaneTeste.From = Airport.GetAirportByIata("MAO");
            airplaneTeste.To = Airport.GetAirportByIata("BSB");
            airplaneTeste.Altitude = 6999;
            airplaneTeste.VerticalSpeed = -501;
            airplaneTeste.Speed = 150;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("A320");
            airplaneTeste.State = AirplaneStatus.Landing;
            airplaneTeste.Registration = new AircraftRegistration("PR-MYH");
            airplaneTeste.Longitude = -47.988926;
            airplaneTeste.Latitude = -15.767119;
            airplaneTeste.FinalConvertAirplaneRules();

            if (String.IsNullOrEmpty(airplaneTeste.RunwayName))
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);

        }

    }
}
