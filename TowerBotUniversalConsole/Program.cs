using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerBotUniversalConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
#if DEBUG
            Console.WriteLine("Modo debug.\n");

#endif
            Core.Start(true);
        }
    }
}
