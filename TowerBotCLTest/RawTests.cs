using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerBotLib;

namespace TowerBotCLTest
{
    [TestClass]
    public class RawTests
    {

        [TestMethod]
        public void MoreThanSixthSkusRenewOrder()
        {
           

            AirplanesData.GetAirplanesRaw("http://www.samuelr.com/log.txt");
        }
    }
}
