using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerBotLib
{
    public static class ErrorManager
    {
        static string strPath;
        public static string LastRowData { get; set; }
        static ErrorManager()
        {
            strPath = Environment.GetFolderPath(
                      System.Environment.SpecialFolder.MyDocuments) + "\\logs\\errors";

            bool exists = System.IO.Directory.Exists(strPath);


            if (!exists)
                System.IO.Directory.CreateDirectory(strPath);
        }

        public static void ThrowError(string codePlace)
        {
            ThrowError(new Exception(), codePlace);
        }

        public static void ThrowError(Exception e, string codePlace, bool throwException = true)
        {
            using (StreamWriter w = File.AppendText(strPath + "\\errors_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt"))
            {
                w.WriteLine("");
                w.WriteLine("");
                w.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                w.WriteLine("Code Place: {0}", codePlace);
                if(e.InnerException != null)
                    w.WriteLine("C# Message: {0}", e.InnerException.Message);
                else
                    w.WriteLine("C# Message: {0}", e.Message);
                //w.WriteLine("C# Row Data: {0}", LastRowData);
                Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " ERROR. Code Place:" + codePlace);
            }
#if DEBUG
            if (throwException) { }
                //throw new ArgumentException(codePlace,e);
#endif
        }
    }
}
