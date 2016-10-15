
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using TowerBotLib;
using System.Deployment.Application;
using System.Text.RegularExpressions;

namespace TowerBotCL
{
    public class BaseClass
    {
        public string M1() { return "BaseClass::M1"; }
        public virtual string M2() { return "BaseClass::M2"; }
        public virtual string M3() { return "BaseClass::M3"; }
    }

    public class DerivedClass : BaseClass
    {
        public string M1() { return "DerivedClass::M1"; }
        public new string M2() { return "DerivedClass::M2"; }
        public override string M3() { return "DerivedClass::M3"; }
    }

    class Program
    {
        static void Main(string[] args)
        {

            //Regex rgx = new Regex(@"[A-z]\w*[@].*(.org|.com)$", RegexOptions.IgnoreCase);
            //MatchCollection matchesEmail = rgx.Matches("a@a.com");
            //rgx = new Regex(@"(@|\.com|\.org)", RegexOptions.IgnoreCase);
            //string[] companyName = rgx.Split("a@a.com");
            //bool isValid = (companyName.Length > 0 &&
            //    companyName[0].Length > 0 && companyName[0].Length <= 9 &&
            //    companyName[2].Length > 0 && companyName[2].Length <= 9
            //    ? true : false);

            //DerivedClass ref1 = new DerivedClass();
            //BaseClass ref2 = ref1;

            //System.Console.WriteLine(ref2.M1());
            //System.Console.WriteLine(ref2.M2());
            //System.Console.WriteLine(ref2.M3());

            //string input = Console.ReadLine();

            //// The first line is how many numbers will be processed
            //var numberCount = int.Parse(input);

            //// Now we need to read and output "numberCount" times
            //for (var i = 0; i < numberCount; i++)
            //{
            //    var currentNumber = int.Parse(Console.ReadLine());

            //    int nextFbN = -1;

            //    int previousFbN2 = 1;
            //    int previousFbN1 = 1;

            //    while (nextFbN < 0)
            //    {
                   

            //        int nextN = previousFbN2 + previousFbN1;

            //        if (nextN > currentNumber)
            //            nextFbN = nextN;
            //        else
            //        {
            //            previousFbN2 = previousFbN1;
            //            previousFbN1 = nextN;
            //        }
            //    }

            //    // Example of output
            //    Console.WriteLine(nextFbN);
            //}

            //string input = Console.ReadLine();

            //// The first line is the size of the matrix
            //var matrixSize = int.Parse(input);

            //// Matrix with the given size
            //var matrix = new int[matrixSize, matrixSize];

            //// Foreach N line we have N columns separated by space
            //for (int i = 0; i < matrixSize; i++)
            //{
            //    var consoleLine = Console.ReadLine().Split(' ');
            //    for (int j = 0; j < matrixSize; j++)
            //        matrix[i, j] = int.Parse(consoleLine[j]);
            //}

            //// Here the "matrix" should have the input formatted as a two-dimensional array
            //var result = new int[matrixSize, matrixSize];
            //for (int i = 0; i < matrixSize; i++)
            //{
            //    for (int j = 0; j < matrixSize; j++)
            //    {
            //        int startPosY = (i > 0) ? i - 1 : 0;
            //        int endPosY = (i + 1 < matrixSize) ? i + 1 : i;
            //        int startPosX = (j > 0) ? j - 1 : 0;
            //        int endPosX = (j + 1 < matrixSize) ? j + 1 : i;


            //        int finalValue = 0;

            //        for (int ii = startPosY; ii <= endPosY; ii++)
            //        {
            //            for (int jj = startPosX; jj <= endPosX; jj++)
            //            {
            //                finalValue += matrix[ii, jj];
            //            }
            //        }

            //        finalValue -= matrix[i, j];

            //        result[i, j] = finalValue;

            //        Console.Write(j < matrixSize - 1 ? "{0} " : "{0}", result[i, j]);

            //    }
            //    Console.WriteLine();
            //}


#if DEBUG
            Console.WriteLine("Modo debug.\n");

#endif
            Core.Start(true);
            //Console.ReadKey();
        }


        
    }
}
