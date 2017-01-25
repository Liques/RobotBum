using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TowerBotFoundation;

namespace TowerBotTests
{
    [TestClass]
    public class FoundationTests
    {
        [TestMethod]
        public void AirportDetectionByICAOTest()
        {
            var airport = Airport.GetAirportByICAO("SBBR");
            Assert.IsTrue(airport.IATA == "BSB");
        }

        [TestMethod]
        public void AirlineDetectionTest()
        {
            var airline = Airline.GetAirlineByFlight("TAM7896");
            Assert.IsTrue(airline.Name == "TAM");
        }

        [TestMethod]
        public void WeatherMetarTest()
        {
            AirportWeather.ForceOnlyLoadSingleAirports = true;
            var testemetar = AirportWeather.GetWeather(Airport.GetAirportByICAO("SBSP"));
            Assert.IsNotNull(testemetar);
        }

        [TestMethod]
        public void WeatherTafTest()
        {
            AirportWeather.ForceOnlyLoadSingleAirports = true;
            
            var teste = AirportWeather.GetWeather(Airport.GetAirportByICAO("SBCB"));
            teste.GetFutureWeather(DateTime.Now);

            var teste2 = AirportWeather.GetWeather(Airport.GetAirportByICAO("SBGR"));
            var teste3 = AirportWeather.GetWeather(Airport.GetAirportByICAO("SWUZ"));
            var teste4 = AirportWeather.GetWeather(Airport.GetAirportByICAO("SWUZ"));
            var teste5 = AirportWeather.GetWeather(Airport.GetAirportByICAO("SWUZ"));
            Assert.IsTrue(teste.ListFutureWeather.Count > 0);
        }
    }
}
