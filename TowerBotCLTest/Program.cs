using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using TowerBotLib;
using TowerBotLib.Filters;

namespace TowerBotCLTest
{
    class Program
    {


        static void Main(string[] args)
        {
            Airplane airplane = new Airplane();
            airplane.Latitude = -15.807222;
            airplane.Longitude = -47.69667;
            airplane.Direction = 270;
        }

    }
}
