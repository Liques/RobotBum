using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotBumFoundationCore;
using RobotBumLibCore.Map;

namespace RobotBumLibCore
{
    public class RunwayBasic : Runway
    {
       
        /// <summary>
        /// Will return the name of the runway
        /// </summary>
        /// <param name="airplane"></param>
        /// <returns></returns>
        public string IsAirplaneInFinalRunway(AirplaneBasic airplane, double direction = 0)
        {
            string name = String.Empty;
            double degreesAperture = 5;

            double finalOneDirection = MapMathHelper.GetAngle(LongitudeSideOne, LongitudeSideTwo, LatitudeSideOne, LatitudeSideTwo);
            double finalTwoDirection = MapMathHelper.GetAngle(LongitudeSideTwo, LongitudeSideOne, LatitudeSideTwo, LatitudeSideOne);
            double degreesOneFromPosition = MapMathHelper.GetAngle(LongitudeSideOne, airplane.Longitude, LatitudeSideOne, airplane.Latitude);
            double degreesTwoFromPosition = MapMathHelper.GetAngle(LongitudeSideTwo, airplane.Longitude, LatitudeSideTwo, airplane.Latitude);

            bool isTargetInAngleFromOne = finalOneDirection - degreesAperture < degreesOneFromPosition && finalOneDirection + degreesAperture > degreesOneFromPosition;
            bool isTargetInAngleFromTwo = finalTwoDirection - degreesAperture < degreesTwoFromPosition && finalTwoDirection + degreesAperture > degreesTwoFromPosition;

            if (isTargetInAngleFromOne && airplane.State == AirplaneStatus.Landing || isTargetInAngleFromTwo && airplane.State == AirplaneStatus.TakingOff)
                name = this.NameSideTwo;
            else if (isTargetInAngleFromOne && airplane.State == AirplaneStatus.TakingOff || isTargetInAngleFromTwo && airplane.State == AirplaneStatus.Landing)
                name = this.NameSideOne;

            return name;
        }

    }
}
