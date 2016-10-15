using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TowerBotLib.Map;
using TowerBotLib;
using TowerBotFoundation;
using System.Linq;

namespace TowerBotTests
{
    [TestClass]
    public class ChartTest
    {
        [TestMethod]
        public void IsPositionInLine()
        {
            bool isLine = Chart.GetChart("PROVE").IsFollowingChart(-046.8725000, -16.6278333);
            Assert.IsTrue(isLine);
        }

        [TestMethod]
        public void IsNotPositionInLine()
        {
            bool isLine = Chart.GetChart("PROVE").IsFollowingChart(-047.8505000, -17.5470000);
            Assert.IsFalse(isLine);
        }

        [TestMethod]
        public void IsAlmostPositionInLine()
        {
            bool isLine = Chart.GetChart("PROVE").IsFollowingChart(-046.8980000, -16.5751667);
            Assert.IsTrue(isLine);
        }

        [TestMethod]
        public void IsPositionInLineButNotInTheBeggining()
        {
            bool isLine = Chart.GetChart("PROVE").IsFollowingChart(-47.402106, -16.19111);
            Assert.IsTrue(isLine);
        }

        [TestMethod]
        public void IsAirplaneInLineRightDirection()
        {
            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.ID = "x";
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "TAP1133X";
            airplaneTeste.From = Airport.GetAirportByIata("LIS");
            airplaneTeste.To = Airport.GetAirportByIata("BSB");
            airplaneTeste.Altitude = 22000;
            airplaneTeste.VerticalSpeed = -100;
            airplaneTeste.Speed = 200;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("A333");
            airplaneTeste.State = AirplaneStatus.Landing;
            airplaneTeste.Registration = new AircraftRegistration("CS-TIM");            
            airplaneTeste.Latitude = -16.6278333;
            airplaneTeste.Longitude = -046.8505000;

            var airplaneTeste2 = new AirplaneBasic();
            airplaneTeste2.ID = "x";
            airplaneTeste2.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste2.FlightName = "TAP1133X";
            airplaneTeste2.From = Airport.GetAirportByIata("LIS");
            airplaneTeste2.To = Airport.GetAirportByIata("BSB");
            airplaneTeste2.Altitude = 22000;
            airplaneTeste2.VerticalSpeed = -1000;
            airplaneTeste2.Speed = 200;
            airplaneTeste2.AircraftType = AircraftType.GetAircraftType("A333");
            airplaneTeste2.State = AirplaneStatus.Landing;
            airplaneTeste2.Registration = new AircraftRegistration("CS-TIM");
            airplaneTeste2.Latitude = CheckPoint.GetCheckPoint("PROVE").Longitude;
            airplaneTeste2.Longitude = CheckPoint.GetCheckPoint("PROVE").Latitude;

            airplaneTeste2.UpdateAirplaneStatus();
            airplaneTeste.Radars.First().LastAirplanes.Add(airplaneTeste2);

            airplaneTeste.FinalConvertAirplaneRules();


            if (airplaneTeste.FollowingChart != null)
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);

        }

        // -15.36557 -47.3702
        [TestMethod]
        //[Ignore]
        public void IsAirplaneInLineRightDirection2()
        {
            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.ID = "E45674";
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "TAP1133X";
            airplaneTeste.From = Airport.GetAirportByIata("LIS");
            airplaneTeste.To = Airport.GetAirportByIata("BSB");
            airplaneTeste.Altitude = 22000;
            airplaneTeste.VerticalSpeed = -100;
            airplaneTeste.Speed = 200;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("A333");
            airplaneTeste.State = AirplaneStatus.Landing;
            airplaneTeste.Registration = new AircraftRegistration("CS-TIM");
            airplaneTeste.Latitude = -15.36557;
            airplaneTeste.Longitude = -47.3702;

            var airplaneTeste2 = new AirplaneBasic();
            airplaneTeste2.ID = "E45674";
            airplaneTeste2.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste2.FlightName = "TAP1133X";
            airplaneTeste2.From = Airport.GetAirportByIata("LIS");
            airplaneTeste2.To = Airport.GetAirportByIata("BSB");
            airplaneTeste2.Altitude = 22000;
            airplaneTeste2.VerticalSpeed = -100;
            airplaneTeste2.Speed = 200;
            airplaneTeste2.AircraftType = AircraftType.GetAircraftType("A333");
            airplaneTeste2.State = AirplaneStatus.Landing;
            airplaneTeste2.Registration = new AircraftRegistration("CS-TIM");
            airplaneTeste2.Latitude = -15.4893;
            airplaneTeste2.Longitude = -47.45732;
            // 15.4893;-47.45732
            
            airplaneTeste.Radars.First().LastAirplanes.Add(airplaneTeste2);

            airplaneTeste.FinalConvertAirplaneRules();


            if (airplaneTeste.FollowingChart != null)
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);

        }

        [TestMethod]
        public void IsAirplaneInChartButNotRightDirection()
        {
            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "TAP1133X";
            airplaneTeste.From = Airport.GetAirportByIata("LIS");
            airplaneTeste.To = Airport.GetAirportByIata("BSB");
            airplaneTeste.Altitude = 22000;
            airplaneTeste.VerticalSpeed = -100;
            airplaneTeste.Speed = 200;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("A333");
            airplaneTeste.State = AirplaneStatus.Landing;
            airplaneTeste.Registration = new AircraftRegistration("CS-TIM");
            airplaneTeste.Latitude = -16.6278333;
            airplaneTeste.Longitude = -046.8505000;

            var airplaneTeste2 = airplaneTeste;
            airplaneTeste2.Longitude = 50;
            airplaneTeste2.Latitude = 50;
            airplaneTeste.Radars.First().LastAirplanes.Add(airplaneTeste2);

            airplaneTeste.FinalConvertAirplaneRules();


            if (airplaneTeste.FollowingChart == null)
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);

        }

        [TestMethod]
        public void IsAirplaneInLineRightDirectionNotStar()
        {
            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "TAP1133X";
            airplaneTeste.From = Airport.GetAirportByIata("LIS");
            airplaneTeste.To = Airport.GetAirportByIata("BSB");
            airplaneTeste.Altitude = 22000;
            airplaneTeste.VerticalSpeed = 1251;
            airplaneTeste.Speed = 200;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("A333");
            airplaneTeste.Registration = new AircraftRegistration("CS-TIM");
            airplaneTeste.Latitude = -16.6278333;
            airplaneTeste.Longitude = -046.8505000;

            var airplaneTeste2 = airplaneTeste;
            airplaneTeste2.Longitude = CheckPoint.GetCheckPoint("PROVE").Longitude;
            airplaneTeste2.Latitude = CheckPoint.GetCheckPoint("PROVE").Latitude;
            airplaneTeste2.State = AirplaneStatus.TakingOff;
            airplaneTeste2.VerticalSpeed = -251;
            airplaneTeste.Radars.First().LastAirplanes.Add(airplaneTeste2);
            airplaneTeste.State = AirplaneStatus.TakingOff;
            airplaneTeste.FinalConvertAirplaneRules();


            if (airplaneTeste.FollowingChart == null)
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);

        }

    }
}
