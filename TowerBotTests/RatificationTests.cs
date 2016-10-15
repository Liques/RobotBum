using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TowerBotLib.Filters;
using TowerBotLib;
using System.Collections.Generic;
using TowerBotFoundation;
using System.Linq;

namespace TowerBotTests
{
    [TestClass]
    public class RatificationTests
    {

        [TestMethod]
        public void RatificationRunwayOnFilterTestHeavyAirraft()
        {
            var filterWide = new FilterRatification(false, true, false, false, false)
            {
                Radar = Radar.GetRadar("BSB"),
            };


            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "DOL3771X";
            airplaneTeste.From = Airport.GetAirportByIata("RBR");
            airplaneTeste.To = Airport.GetAirportByIata("BSB");
            airplaneTeste.Altitude = 6999;
            airplaneTeste.VerticalSpeed = -501;
            airplaneTeste.Speed = 150;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("B773");
            airplaneTeste.State = AirplaneStatus.Landing;
            airplaneTeste.Registration = new AircraftRegistration("PR-MYH");
            airplaneTeste.Longitude = -48.988926;
            airplaneTeste.Latitude = -16.867119;
            airplaneTeste.FinalConvertAirplaneRules();

            var listAirplanes = new List<AirplaneBasic>();
            listAirplanes.Add(airplaneTeste);
            var alertList = filterWide.Analyser(listAirplanes);
            airplaneTeste.Radars.First().LastAirplanes.Add(airplaneTeste);

            var airplaneTeste2 = new AirplaneBasic();
            airplaneTeste2.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste2.FlightName = "DOL3771X";
            airplaneTeste2.From = Airport.GetAirportByIata("RBR");
            airplaneTeste2.To = Airport.GetAirportByIata("BSB");
            airplaneTeste2.Altitude = 6999;
            airplaneTeste2.VerticalSpeed = -501;
            airplaneTeste2.Speed = 150;
            airplaneTeste2.AircraftType = AircraftType.GetAircraftType("B773");
            airplaneTeste2.State = AirplaneStatus.Landing;
            airplaneTeste2.Registration = new AircraftRegistration("PR-MYH");
            airplaneTeste2.Longitude = -47.988926;
            airplaneTeste2.Latitude = -15.867119;
            airplaneTeste2.FinalConvertAirplaneRules();
            airplaneTeste2.Radars.First().LastAirplanes.Add(airplaneTeste);
            airplaneTeste2.FinalConvertAirplaneRules();

            var listAirplanes2 = new List<AirplaneBasic>();
            listAirplanes2.Add(airplaneTeste2);
            var alertList2 = filterWide.Analyser(listAirplanes2);

            MessageMaker mMaker = new MessageMaker(airplaneTeste2, Radar.GetRadar("BSB"), 1, RatificationType.FinalRunway);

            if (mMaker.Message.StartsWith("O DOL3771X está pousando na runway 11L"))
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);
        }

        [TestMethod]
        public void RatificationRunwayOnFilterTestLowAirraft()
        {
            var filterWide = new FilterRatification(false, true, false, false, false)
            {
                Radar = Radar.GetRadar("BSB"),
            };


            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "DOL3371X";
            airplaneTeste.From = Airport.GetAirportByIata("RBR");
            airplaneTeste.To = Airport.GetAirportByIata("BSB");
            airplaneTeste.Altitude = 6999;
            airplaneTeste.VerticalSpeed = -501;
            airplaneTeste.Speed = 150;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("A319");
            airplaneTeste.State = AirplaneStatus.Landing;
            airplaneTeste.Registration = new AircraftRegistration("PR-MYH");
            airplaneTeste.Longitude = -48.988926;
            airplaneTeste.Latitude = -16.867119;
            airplaneTeste.FinalConvertAirplaneRules();

            var listAirplanes = new List<AirplaneBasic>();
            listAirplanes.Add(airplaneTeste);
            var alertList = filterWide.Analyser(listAirplanes);
            airplaneTeste.Radars.First().LastAirplanes.Add(airplaneTeste);

            var airplaneTeste2 = new AirplaneBasic();
            airplaneTeste2.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste2.FlightName = "DOL3771X";
            airplaneTeste2.From = Airport.GetAirportByIata("RBR");
            airplaneTeste2.To = Airport.GetAirportByIata("BSB");
            airplaneTeste2.Altitude = 6999;
            airplaneTeste2.VerticalSpeed = -501;
            airplaneTeste2.Speed = 150;
            airplaneTeste2.AircraftType = AircraftType.GetAircraftType("B773");
            airplaneTeste2.State = AirplaneStatus.Landing;
            airplaneTeste2.Registration = new AircraftRegistration("PR-MYH");
            airplaneTeste2.Longitude = -47.988926;
            airplaneTeste2.Latitude = -15.867119;
            airplaneTeste2.FinalConvertAirplaneRules();
            airplaneTeste2.Radars.First().LastAirplanes.Add(airplaneTeste);
            airplaneTeste2.FinalConvertAirplaneRules();

            var listAirplanes2 = new List<AirplaneBasic>();
            listAirplanes2.Add(airplaneTeste2);
            var alertList2 = filterWide.Analyser(listAirplanes2);

            MessageMaker mMaker = new MessageMaker(airplaneTeste2, Radar.GetRadar("BSB"), 1, RatificationType.FinalRunway);

            if (mMaker.Message.StartsWith("O DOL3771X está pousando na runway 11L"))
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);
        }
    }
}
