using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TowerBotLib;
using TowerBotLib.Map;
using TowerBotFoundation;
using System.Linq;

namespace TowerBotTests
{
    [TestClass]
    public class MessageMakerTests
    {
        [TestMethod]
        public void MessageWithCharts()
        {

            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.ID = "XXX";
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
            airplaneTeste2.Longitude = CheckPoint.GetCheckPoint("PROVE").Longitude;
            airplaneTeste2.Latitude = CheckPoint.GetCheckPoint("PROVE").Latitude;
            airplaneTeste.Radars.First().LastAirplanes.Add(airplaneTeste2);

            airplaneTeste.FinalConvertAirplaneRules();

            MessageMaker mMaker = new MessageMaker(airplaneTeste, Radar.GetRadar("BSB"),1);

            if (mMaker.Message.StartsWith("Está descendo em Brasília um Airbus A330-300, o TAP1133X (CS-TIM), seguindo Star PROVE, de LIS"))
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);
        }

        [TestMethod]
        public void MessageLanding()
        {
            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "TAM9354X";
            airplaneTeste.From = Airport.GetAirportByIata("POA");
            airplaneTeste.To = Airport.GetAirportByIata("BSB");
            airplaneTeste.Altitude = 17569;
            airplaneTeste.VerticalSpeed = -100;
            airplaneTeste.Speed = 200;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("A330");
            airplaneTeste.State = AirplaneStatus.Landing;
            airplaneTeste.Registration = new AircraftRegistration("PR-REU");
            airplaneTeste.FinalConvertAirplaneRules();

            int dateUtcNow = Convert.ToInt32(new DateTime(2015, 12, 16, 10, 22, 41).ToUniversalTime().ToString("ddMMyyyyy"));
            int timeUtcNow = Convert.ToInt32(new DateTime(2015, 12, 16, 10, 22, 41).ToUniversalTime().ToString("HHmmss"));
            int seed = dateUtcNow + timeUtcNow;

            MessageMaker mMaker = new MessageMaker(airplaneTeste, Radar.GetRadar("BSB"), 85614);

            if (mMaker.Message.StartsWith("Está descendo em Brasília um Airbus A330, o TAM9354X (PR-REU) de Porto Alegre."))
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);
        }

        [TestMethod]
        public void MessageTakingOff()
        {
            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "TAM9374X";
            airplaneTeste.From = Airport.GetAirportByIata("BSB");
            airplaneTeste.To = Airport.GetAirportByIata("GRU");
            airplaneTeste.Altitude = 17569;
            airplaneTeste.VerticalSpeed = 100;
            airplaneTeste.Speed = 250;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("A320");
            airplaneTeste.State = AirplaneStatus.TakingOff;
            airplaneTeste.Registration = new AircraftRegistration("PR-REU");
            airplaneTeste.FinalConvertAirplaneRules();

            MessageMaker mMaker = new MessageMaker(airplaneTeste, Radar.GetRadar("BSB"), 85614);

            if (mMaker.Message.StartsWith("O TAM9374X (PR-REU) está decolando de Brasília, Airbus A320 para São Paulo(GRU)"))
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);
        }

        [TestMethod]
        public void MessageCruise()
        {
            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "TAM9351X";
            airplaneTeste.From = Airport.GetAirportByIata("THE");
            airplaneTeste.To = Airport.GetAirportByIata("GRU");
            airplaneTeste.Altitude = 33000;
            airplaneTeste.VerticalSpeed = 0;
            airplaneTeste.Speed = 450;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("A320");
            airplaneTeste.State = AirplaneStatus.Cruise;
            airplaneTeste.Registration = new AircraftRegistration("PR-REU");
            airplaneTeste.FinalConvertAirplaneRules();

            MessageMaker mMaker = new MessageMaker(airplaneTeste, Radar.GetRadar("BSB"), 785);

            if (mMaker.Message.StartsWith("Está passando pela capital o TAM9351X (PR-REU), Airbus A320 de Teresina para GRU"))
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);
        }


        [TestMethod]
        public void MessageOfAirplaneFromCountryDifferent()
        {
            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "EK456";
            airplaneTeste.From = Airport.GetAirportByIata("DXB");
            airplaneTeste.To = Airport.GetAirportByIata("EZE");
            airplaneTeste.Altitude = 33000;
            airplaneTeste.VerticalSpeed = 0;
            airplaneTeste.Speed = 450;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("B738");
            airplaneTeste.State = AirplaneStatus.Cruise;
            airplaneTeste.Registration = new AircraftRegistration("A6-ABC");
            airplaneTeste.FinalConvertAirplaneRules();

            MessageMaker mMaker = new MessageMaker(airplaneTeste, Radar.GetRadar("BSB"), 76515);

            if (mMaker.Message.StartsWith("O EK456 (A6-ABC), matrícula Emirados Árabes Unidos, está voando pela região, B738 de DXB para EZE"))
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);
        }

        [TestMethod]
        public void MessageWithChartsRatificationChart()
        {
            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.ID = "X22X";
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "TAP0233X";
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
            airplaneTeste2.Longitude = CheckPoint.GetCheckPoint("PROVE").Longitude;
            airplaneTeste2.Latitude = CheckPoint.GetCheckPoint("PROVE").Latitude;
            airplaneTeste.Radars.First().LastAirplanes.Add(airplaneTeste2);

            airplaneTeste.FinalConvertAirplaneRules();

            MessageMaker mMaker = new MessageMaker(airplaneTeste, Radar.GetRadar("BSB"), 1, RatificationType.Chart);

            if (mMaker.Message.StartsWith("O TAP0233X está seguindo a STAR PROVE"))
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);
        }

        [TestMethod]
        public void MessageWithChartsRatificationRuwnay()
        {
            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "GOL3771X";
            airplaneTeste.From = Airport.GetAirportByIata("RBR");
            airplaneTeste.To = Airport.GetAirportByIata("BSB");
            airplaneTeste.Altitude = 6999;
            airplaneTeste.VerticalSpeed = -501;
            airplaneTeste.Speed = 150;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("B738");
            airplaneTeste.State = AirplaneStatus.Landing;
            airplaneTeste.Registration = new AircraftRegistration("PR-MYH");
            airplaneTeste.Longitude = -47.945945;
            airplaneTeste.Latitude = -15.863983;           
            airplaneTeste.FinalConvertAirplaneRules();

            var airplaneTeste2 = new AirplaneBasic();
            airplaneTeste2.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste2.FlightName = "GOL3771X";
            airplaneTeste2.From = Airport.GetAirportByIata("RBR");
            airplaneTeste2.To = Airport.GetAirportByIata("BSB");
            airplaneTeste2.Altitude = 6999;
            airplaneTeste2.VerticalSpeed = -501;
            airplaneTeste2.Speed = 150;
            airplaneTeste2.AircraftType = AircraftType.GetAircraftType("B738");
            airplaneTeste2.State = AirplaneStatus.Landing;
            airplaneTeste2.Registration = new AircraftRegistration("PR-MYH");
            airplaneTeste2.Longitude = -47.936450;
            airplaneTeste2.Latitude = -15.863905;
            airplaneTeste2.Radars.First().LastAirplanes.Add(airplaneTeste);
            airplaneTeste2.FinalConvertAirplaneRules();

            MessageMaker mMaker = new MessageMaker(airplaneTeste2, Radar.GetRadar("BSB"), 1, RatificationType.FinalRunway);

            if (mMaker.Message.StartsWith("O GOL3771X está pousando na runway 11L"))
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);
        }

        [TestMethod]
        [Ignore]
        public void MessageWithChartsShouldNotRatificationRuwnay()
        {
            var airplaneTeste = new AirplaneBasic();
            airplaneTeste.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste.FlightName = "GOL3771X";
            airplaneTeste.From = Airport.GetAirportByIata("RBR");
            airplaneTeste.To = Airport.GetAirportByIata("BSB");
            airplaneTeste.Altitude = 6999;
            airplaneTeste.VerticalSpeed = -501;
            airplaneTeste.Speed = 150;
            airplaneTeste.AircraftType = AircraftType.GetAircraftType("B738");
            airplaneTeste.State = AirplaneStatus.Landing;
            airplaneTeste.Registration = new AircraftRegistration("PR-MYH");
            airplaneTeste.Longitude = -46.945945;
            airplaneTeste.Latitude = -15.863983;
            airplaneTeste.FinalConvertAirplaneRules();

            var airplaneTeste2 = new AirplaneBasic();
            airplaneTeste2.Radars.Add(Radar.GetRadar("BSB"));
            airplaneTeste2.FlightName = "GOL3771X";
            airplaneTeste2.From = Airport.GetAirportByIata("RBR");
            airplaneTeste2.To = Airport.GetAirportByIata("BSB");
            airplaneTeste2.Altitude = 6999;
            airplaneTeste2.VerticalSpeed = -501;
            airplaneTeste2.Speed = 150;
            airplaneTeste2.AircraftType = AircraftType.GetAircraftType("B738");
            airplaneTeste2.State = AirplaneStatus.Landing;
            airplaneTeste2.Registration = new AircraftRegistration("PR-MYH");
            airplaneTeste2.Longitude = -47.936450;
            airplaneTeste2.Latitude = -15.863905;
            airplaneTeste2.Radars.First().LastAirplanes.Add(airplaneTeste);
            airplaneTeste2.FinalConvertAirplaneRules();

            MessageMaker mMaker = new MessageMaker(airplaneTeste2, Radar.GetRadar("BSB"), 1, RatificationType.FinalRunway);

            if (mMaker.Message.StartsWith("O GOL3771X está pousando na runway 11L"))
                Assert.IsTrue(false);
            else
                Assert.IsTrue(true);
        }

        [TestMethod]
        public void MessageDescriptisonSpecial()
        {
            var airplaneTeste = AirplaneBasic.ConvertToAirplane(Radar.GetRadar("BSB"),
                                                            "A21D0B",
                                                            "",
                                                            "15000",
                                                            "0",
                                                            "0",
                                                            "200",
                                                            "-100",
                                                            "0",
                                                            "",
                                                            "",
                                                            "B752",
                                                            "");
            airplaneTeste.ID = "A21D0B";
            airplaneTeste.State = AirplaneStatus.Landing;
            airplaneTeste.FinalConvertAirplaneRules();
                        
            MessageMaker mMaker = new MessageMaker(airplaneTeste, Radar.GetRadar("BSB"), 85614);

            if (mMaker.Message.StartsWith("Vai chegando no aeroporto o COLT Cargo (PR-XCA), Boeing 757-200"))
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);
        }

        [TestMethod]
        public void MessageDescriptisonPaintSpecial()
        {
            var airplaneTeste = AirplaneBasic.ConvertToAirplane(Radar.GetRadar("BSB"),
                                                            "E483DE",
                                                            "TAM8853X",
                                                            "15000",
                                                            "0",
                                                            "0",
                                                            "200",
                                                            "-100",
                                                            "0",
                                                            "",
                                                            "",
                                                            "A320",
                                                            "");
            airplaneTeste.ID = "E483DE";
            airplaneTeste.State = AirplaneStatus.Landing;
            airplaneTeste.FinalConvertAirplaneRules();

            MessageMaker mMaker = new MessageMaker(airplaneTeste, Radar.GetRadar("BSB"), 454676);

            if (mMaker.Message.StartsWith("Vai chegando no aeroporto o COLT Cargo (PR-XCA), Boeing 757-200"))
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);
        }

    }
}
