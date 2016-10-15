using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerBotLibCore.Map
{
    public static class MapMathHelper
    {

        private static bool IsInsideAngle(double targetLongitude, double targetLatitude, double targetDirection, CheckPoint point1, CheckPoint point2, bool mustHaveDirection)
        {

            bool result = false;

            double degreesAperture = 3;
            
            double degreesOne = GetAngle(point1.Longitude, point2.Longitude, point1.Latitude, point2.Latitude);
            double degreesTwo = GetAngle(point2.Longitude, point1.Longitude, point2.Latitude, point1.Latitude);
            double degreesOneFromPosition = GetAngle(point1.Longitude, targetLongitude, point1.Latitude, targetLatitude);
            double degreesTwoFromPosition = GetAngle(point2.Longitude, targetLongitude, point2.Latitude, targetLatitude);

            bool isTargetInAngleFromOne = degreesOne - degreesAperture < degreesOneFromPosition && degreesOne + degreesAperture > degreesOneFromPosition ||
                                          degreesTwo - degreesAperture < degreesTwoFromPosition && degreesTwo + degreesAperture > degreesTwoFromPosition;
            bool targetCompatibleHasAngle = targetDirection  - degreesAperture < degreesOneFromPosition && targetDirection + degreesAperture > degreesOneFromPosition ||
                                            targetDirection  - degreesAperture < degreesTwoFromPosition && targetDirection + degreesAperture > degreesTwoFromPosition;

            if (isTargetInAngleFromOne && targetCompatibleHasAngle && mustHaveDirection)
                result = true;
            else if (isTargetInAngleFromOne && !mustHaveDirection)
                result = true;

            return result;
        }
        
        public static bool IsInsideAngle(double targetLongitude, double targetLatitude, CheckPoint point1, CheckPoint point2)
        {
            return IsInsideAngle(targetLongitude, targetLatitude, 0, point1, point2, false);
        }

        public static bool IsInsideAngle(double targetLongitude, double targetLatitude, double targetDirection, CheckPoint point1, CheckPoint point2)
        {
            return IsInsideAngle(targetLongitude, targetLatitude, targetDirection, point1, point2, true);
        }

        public static double GetAngle(double long1, double long2, double lat1, double lat2)
        {
            double radians = Math.Atan2(long1 - long2, lat1 - lat2);
            return radians * (180 / Math.PI) - 180;
        }
    }
}
