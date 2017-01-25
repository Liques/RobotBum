using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TowerBotLib;
using TowerBotFoundation;
using System.Linq;

namespace TowerBotTests
{
    [TestClass]
    public class ModeSTests
    {
        [TestMethod]
        public void ModeSLanding2asdfs()
        {
            AirplaneBasic.IsSpecialPainting();
        }

        [TestMethod]
        public void ModeSLanding()
        {
            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "TAM9881X";
            airplaneTeste.From = Airport.GetAirportByIata("THE");
            airplaneTeste.To = Airport.GetAirportByIata("BSB");
            airplaneTeste.Altitude = 16558;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("A319");
            airplaneTeste.Registration = new AircraftRegistration("PR-MYK");
            airplaneTeste.FinalConvertAirplaneRules();
            airplaneTeste.Radars.First().LastAirplanes.Add(airplaneTeste);
            airplaneTeste.DateCreation = DateTime.Now;

            var airplaneTeste2 = new AirplaneBasic();
            airplaneTeste2.Radars.Add(airplaneTeste.Radars.First());
            airplaneTeste2.FlightName = "TAM9881X";
            airplaneTeste2.From = Airport.GetAirportByIata("THE");
            airplaneTeste2.To = Airport.GetAirportByIata("BSB");
            airplaneTeste2.Altitude = 15458;
            airplaneTeste2.AircraftType = AircraftType.GetAircraftType("A319");
            //airplaneTeste2.State = AirplaneStatus.Landing;
            airplaneTeste2.Registration = new AircraftRegistration("PR-MYK");
            airplaneTeste2.DateCreation = DateTime.Now.AddSeconds(30);

            airplaneTeste2.FinalConvertAirplaneRules();
            airplaneTeste2.UpdateAirplaneStatus();

            if (airplaneTeste2.State == AirplaneStatus.Landing)
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);

        }

        [TestMethod]
        public void ModeSTakingOff()
        {
            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "GOL9331X";
            airplaneTeste.From = Airport.GetAirportByIata("BSB");
            airplaneTeste.To = Airport.GetAirportByIata("THE");
            airplaneTeste.Altitude = 14558;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("B738");
            airplaneTeste.Registration = new AircraftRegistration("PR-GGF");
            airplaneTeste.FinalConvertAirplaneRules();
            airplaneTeste.Radars.First().LastAirplanes.Add(airplaneTeste);
            airplaneTeste.DateCreation = DateTime.Now;

            var airplaneTeste2 = new AirplaneBasic();
            airplaneTeste2.Radars.Add(airplaneTeste.Radars.First());
            airplaneTeste2.FlightName = "GOL9331X";
            airplaneTeste2.From = Airport.GetAirportByIata("BSB");
            airplaneTeste2.To = Airport.GetAirportByIata("THE");
            airplaneTeste2.Altitude = 16558;
            airplaneTeste2.AircraftType = AircraftType.GetAircraftType("B738");
            airplaneTeste2.Registration = new AircraftRegistration("PR-GGF");
            airplaneTeste2.DateCreation = DateTime.Now.AddSeconds(30);

            airplaneTeste2.FinalConvertAirplaneRules();
            airplaneTeste2.UpdateAirplaneStatus();

            if (airplaneTeste2.State == AirplaneStatus.TakingOff)
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);

        }

        [TestMethod]
        public void ModeSNotTakingOffBecauseIsHigh()
        {
            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "GOL9331X";
            airplaneTeste.From = Airport.GetAirportByIata("BSB");
            airplaneTeste.To = Airport.GetAirportByIata("THE");
            airplaneTeste.Altitude = 34558;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("B738");
            airplaneTeste.Registration = new AircraftRegistration("PR-GGF");
            airplaneTeste.FinalConvertAirplaneRules();
            airplaneTeste.Radars.First().LastAirplanes.Add(airplaneTeste);
            airplaneTeste.DateCreation = DateTime.Now;

            var airplaneTeste2 = new AirplaneBasic();
            airplaneTeste2.Radars.Add(airplaneTeste.Radars.First());
            airplaneTeste2.FlightName = "GOL9331X";
            airplaneTeste2.From = Airport.GetAirportByIata("BSB");
            airplaneTeste2.To = Airport.GetAirportByIata("THE");
            airplaneTeste2.Altitude = 36558;
            airplaneTeste2.AircraftType = AircraftType.GetAircraftType("B738");
            airplaneTeste2.Registration = new AircraftRegistration("PR-GGF");
            airplaneTeste2.DateCreation = DateTime.Now.AddSeconds(30);

            airplaneTeste2.FinalConvertAirplaneRules();
            airplaneTeste2.UpdateAirplaneStatus();

            if (airplaneTeste2.State != AirplaneStatus.TakingOff)
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);

        }

        [TestMethod]
        public void ModeSNotLandingBecauseIsHigh()
        {
            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "TAM9881X";
            airplaneTeste.From = Airport.GetAirportByIata("THE");
            airplaneTeste.To = Airport.GetAirportByIata("BSB");
            airplaneTeste.Altitude = 36558;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("A319");
            airplaneTeste.Registration = new AircraftRegistration("PR-MYK");
            airplaneTeste.FinalConvertAirplaneRules();
            airplaneTeste.Radars.First().LastAirplanes.Add(airplaneTeste);
            airplaneTeste.DateCreation = DateTime.Now;

            var airplaneTeste2 = new AirplaneBasic();
            airplaneTeste2.Radars.Add(airplaneTeste.Radars.First());
            airplaneTeste2.FlightName = "TAM9881X";
            airplaneTeste2.From = Airport.GetAirportByIata("THE");
            airplaneTeste2.To = Airport.GetAirportByIata("BSB");
            airplaneTeste2.Altitude = 35558;
            airplaneTeste2.AircraftType = AircraftType.GetAircraftType("A319");
            //airplaneTeste2.State = AirplaneStatus.Landing;
            airplaneTeste2.Registration = new AircraftRegistration("PR-MYK");
            airplaneTeste2.DateCreation = DateTime.Now.AddSeconds(30);

            airplaneTeste2.FinalConvertAirplaneRules();
            airplaneTeste2.UpdateAirplaneStatus();

            if (airplaneTeste2.State != AirplaneStatus.Landing)
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);

        }
    }
}
