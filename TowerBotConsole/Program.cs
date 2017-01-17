using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerBotConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
//Console.WriteLine(File.OpenText(System.IO.Directory.GetCurrentDirectory());
            Console.OutputEncoding = Encoding.Unicode;

            Console.WriteLine(Directory.GetCurrentDirectory());
#if DEBUG
            Console.WriteLine("Modo debug.\n");

#endif
            Core.Start(true);
        }
    }
    
}
