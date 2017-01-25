using System;
using System.IO;

namespace TowerBotLibCore
{
    public static class ErrorManager
    {
        static string strPath;
        public static string LastRowData { get; set; }
       
        public static void ThrowError(string codePlace)
        {
            ThrowError(new Exception(), codePlace);
        }

        public static void ThrowError(Exception e, string codePlace, bool throwException = true)
        {
            
            using (StreamWriter w = File.AppendText("errors.txt"))
            {
                w.WriteLine("");
                w.WriteLine("");
                w.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                w.WriteLine("Code Place: {0}", codePlace);
                if(e.InnerException != null)
                    w.WriteLine("C# Message: {0}", e.InnerException.Message);
                else
                    w.WriteLine("C# Message: {0}", e.Message);
                w.WriteLine("C# Row Data: {0}", LastRowData);
                Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " ERROR. Code Place:" + codePlace);
            }

        }
    }
}
