using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerBotLib
{
    public static class StatisticsManager
    {
        public static List<AirplaneBasic> ListAirplanes { get; set; }

        static StatisticsManager()
        {
            ListAirplanes = new List<AirplaneBasic>();
        }

        public static void UpdateAirplanes(List<AirplaneBasic> listAirplanes)
        {
            var listAircraftExpired = ListAirplanes.Where(s => s.DateExpiration <= DateTime.Now).ToList();
            for (int i = 0; i < listAircraftExpired.Count; i++)
            {
                ListAirplanes.Remove(listAircraftExpired[i]);
            }

            for (int i = 0; i < listAirplanes.Count; i++)
            {
                var icurrentAircraft = ListAirplanes.Where(s => s.ID == listAirplanes[i].ID).ToList().FirstOrDefault();

                if (icurrentAircraft == null)
                {
                    ListAirplanes.Add(icurrentAircraft);
                }
                else
                {
                    icurrentAircraft.Altitude = listAirplanes[i].Altitude;
                    icurrentAircraft.Direction = listAirplanes[i].Direction;
                    icurrentAircraft.From = listAirplanes[i].From;
                    icurrentAircraft.Latitude = listAirplanes[i].Latitude;
                    icurrentAircraft.Longitude = listAirplanes[i].Longitude;
                    icurrentAircraft.Registration = listAirplanes[i].Registration;
                    icurrentAircraft.Speed = listAirplanes[i].Speed;
                    icurrentAircraft.To = listAirplanes[i].To;
                    icurrentAircraft.VerticalSpeed = listAirplanes[i].VerticalSpeed;
                }

            }

         
        }

        public static double GetGPSDistance(double lati1, double lati2, double lon1, double lon2)
        {
            var R = 6371; // km
            var dLat = ConvertToRadians(lati2 - lati1);
            var dLon = ConvertToRadians(lon2 - lon1);
            var lat1 = ConvertToRadians(lati1);
            var lat2 = ConvertToRadians(lati2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c;

            return d;
        }

        private static double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

    }
}
