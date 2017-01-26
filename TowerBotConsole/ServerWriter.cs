using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TowerBotFoundationCore;
using TowerBotLibCore;

namespace TowerBotConsole

{
    static class ServerWriter
    {
        public static DateTime OpenDateTime;
        //public static List<Alert> listOldAlerts = new List<Alert>();

        private static string strJSONPath = String.Empty;

        /// <summary>
        /// A string abaixo eh para testes, se tiver preenchido, favor retirar
        /// </summary>
        private static string specialFolderName = String.Empty;

        public static string HTMLServerFolder {get;set;}

        static ServerWriter()
        {


            OpenDateTime = DateTime.Now;
           
        }
        
        public static void UpdatePages(List<Alert> listNewAlerts)
        {
            // Verify if any old alert is older than it's time to be removed 
            List<Alert> listOldBeyondValidationAlerts = Alert.ListOfAlerts.Where(s => s.TimeToBeRemoved <= DateTime.Now).ToList();
            for (int i = 0; i < listOldBeyondValidationAlerts.Count; i++)
            {
                Alert.ListOfAlerts.Remove(listOldBeyondValidationAlerts[i]);
            }

            for (int i = 0; i < listNewAlerts.Count; i++)
            {
                if (listNewAlerts[i].AlertType == PluginAlertType.NoAlert && listNewAlerts[i].Radar.Name == "BSB")                    
                    listNewAlerts[i].TimeToBeRemoved = DateTime.Now.AddDays(3);

            }

            Alert.ListOfAlerts.AddRange(listNewAlerts);
            Alert.ListOfRecentAlerts = Alert.ListOfAlerts.Where(w => w.TimeCreated > DateTime.Now.AddDays(-1)).ToList();

            var toJSON = JsonConvert.SerializeObject(Alert.ListOfAlerts.Where(w => w.Icon != IconType.GoodNightAnnoucement).ToList());
            
            WriteFile(strJSONPath, "lastAlerts.json", toJSON);

            if(String.IsNullOrEmpty(HTMLServerFolder))
            return;

            var strPath = HTMLServerFolder + specialFolderName;
                        
            WriteFile(strPath, "index.html", IndividualRadar(null));

            bool exists = System.IO.Directory.Exists(strPath + @"/data/");
            if (!exists)
                System.IO.Directory.CreateDirectory(strPath + @"/data/");

            foreach (Radar radar in Radar.ListRadars)
            {
                try
                {
                    string currentPath = strPath + @"/" + radar.Name;

                    WriteFile(currentPath, "index.html", IndividualRadar(radar));
                    var listAlertByRadar = Alert.ListOfAlerts.Where(s => s.Radar != null && s.Radar.Name == radar.Name && s.AlertType != PluginAlertType.NoAlert).ToList();
                    WriteFile(strPath + @"/data/", radar.Name + ".json", JsonConvert.SerializeObject(listAlertByRadar));
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            var listGeneralOnlyImportantAlert = Alert.ListOfAlerts.Where(s => s.AlertType == PluginAlertType.High).ToList();
            WriteFile(strPath + @"/data/", "general.json", JsonConvert.SerializeObject(listGeneralOnlyImportantAlert));
        }

        private static void WriteFile(string currentPath, string fileName, string content)
        {

            bool exists = System.IO.Directory.Exists(currentPath);
            if (!exists && !String.IsNullOrEmpty(currentPath))
                System.IO.Directory.CreateDirectory(currentPath);

            if(!String.IsNullOrEmpty(currentPath))
                currentPath += @"/" + fileName;
            else
                currentPath += fileName;
                

            if (File.Exists(currentPath))
                File.Delete(currentPath);

            File.AppendAllText(currentPath, content, Encoding.UTF8);
           
        }

        private static string LoadFile(string currentPath, string fileName)
        {

            bool exists = System.IO.Directory.Exists(currentPath);
            if (!exists)
                return String.Empty;

           if (!exists && !String.IsNullOrEmpty(currentPath))
                System.IO.Directory.CreateDirectory(currentPath);

            if(!String.IsNullOrEmpty(currentPath))
                currentPath += @"/" + fileName;
            else
                currentPath += fileName;

            return File.ReadAllText(currentPath);
        }

        private static string IndividualRadar(Radar radar, bool showTestAlert = false)
        {
            List<Alert> listAlertByRadar = null;
            if (radar != null)
                listAlertByRadar = Alert.ListOfAlerts.Where(s => s.Radar != null && s.Radar.Name == radar.Name).ToList();
            else
                listAlertByRadar = Alert.ListOfAlerts.Where(w => w.TimeCreated > DateTime.Now.AddHours(-6) && w.Radar.Name != "BRA").ToList();

            if(!showTestAlert)
                listAlertByRadar = listAlertByRadar.Where(w => w.Icon != IconType.GoodNightAnnoucement).ToList();

            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(@"<link rel='icon' type='image/png' href='data:image/png;base64,AAABAAEAEBAQAAEABAAoAQAAFgAAACgAAAAQAAAAIAAAAAEABAAAAAAAgAAAAAAAAAAAAAAAEAAA
AAAAAAAoqwAAT09PAP///wApKSkAgoKCAB6AAAAdegAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
AAAAAAAAAAAAAAAAZmZmZmZmZmZlVVVVVVVVVmUAAAAAAABWZQAzMzMzAFZlADAGYAMAVmAAAAZg
AAAGYFUABmAAVQZgVQAGYABVBmAAAAZgAAAGYAIiJmIiIAZgAjEmYjEgBmACMyZiMyAGYAJCJmJC
IAZgAAAAAAAABmUAAAAAAABWZmZmZmZmZmYAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA' />");
            strBuilder.Append("<meta name=viewport content='width=device-width, initial-scale=1'>");
            if(!System.Globalization.CultureInfo.CurrentCulture.TextInfo.IsRightToLeft)
                strBuilder.Append("<link rel='stylesheet' type='text/css' href='https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.6/css/bootstrap.min.css'>");
            else
                strBuilder.Append("<link rel='stylesheet' type='text/css' href='https://cdnjs.cloudflare.com/ajax/libs/bootstrap-rtl/3.2.0-rc2/css/bootstrap-rtl.min.css'>");
            
            strBuilder.Append("<title>Robot Bum </title>");
            strBuilder.Append("<meta http-equiv='refresh' content='30' >");
            strBuilder.Append("<h3><b>Robot Bum");
            if (radar != null)
                strBuilder.Append(String.Format(" ({0})", radar.Name));


            strBuilder.Append("</b></h3>");


            strBuilder.Append("<span class='visible-lg-inline visible-md-inline'><b>Online time: " + (DateTime.Now - OpenDateTime).ToString(@"dd\ hh\:mm\:ss") + "</b></span><hr/>");

            for (int i = listAlertByRadar.Count - 1; i >= 0; i--)
            {


                try
                {
                    if (listAlertByRadar[i].AlertType != PluginAlertType.NoAlert)
                    {
                        if (listAlertByRadar[i].AlertType == PluginAlertType.High)
                        {
                            strBuilder.Append("<font color=red>");
                        }
                        else if (listAlertByRadar[i].AlertType == PluginAlertType.Medium)
                        {
                            strBuilder.Append("<font color=darkorange>");
                        }
                        else if (listAlertByRadar[i].AlertType == PluginAlertType.Low)
                        {
                            strBuilder.Append("<font color=green>");
                        }
                        else if (listAlertByRadar[i].AlertType == PluginAlertType.Test)
                        {
                            strBuilder.Append("<font color=darkgray>");
                        }

                        if (radar != null)
                        {
                            strBuilder.Append("<span ><b class='visible-lg-inline visible-md-inline'>" + listAlertByRadar[i].TimeCreated.ToString() + "</b></span>");
                            //strBuilder.Append("<span><b>" + listAlertByRadar[i].TimeCreated.ToString("HH:mm") + "</b>");
                        }
                        else
                        {
                            strBuilder.Append("<span><b class='visible-lg-inline visible-md-inline'> " + Alert.ListOfAlerts[i].TimeCreated.ToString() + "</b></span>");
                            //strBuilder.Append("<span><b>" + listAlertByRadar[i].TimeCreated.ToString("HH:mm") + "</b> - ");
                            strBuilder.Append("<span><b><a href='main/" + listAlertByRadar[i].Radar.Name + "/index.html'>" + listAlertByRadar[i].Radar.Description + "</a></b></span>");

                        }

                        strBuilder.Append(" - ");
                        if (!String.IsNullOrEmpty(listAlertByRadar[i].Message))
                        {
                            var start = listAlertByRadar[i].Message.ToString().Split('#').ToList();
                            if (start.Count == 1)
                                strBuilder.Append(listAlertByRadar[i].Message.ToString());
                            else {
                                strBuilder.Append("<span>" + start.FirstOrDefault() + "</span>");
                                strBuilder.Append("<span class='visible-lg-inline'>" + listAlertByRadar[i].Message.ToString().Replace(start.FirstOrDefault(), "") + "</span>");
                            }

                        }
                        strBuilder.Append("</font></span>");

                        strBuilder.Append("<span class='visible-lg-inline'><font color=lightgray> ");
                        
                        strBuilder.Append("</font></span>");

                        strBuilder.Append("<br/>");

                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            strBuilder.Append("<hr/>");
           
            strBuilder.Append("<hr/>");
            strBuilder.Append("<h5>Robot Bum</h5>");



            return strBuilder.ToString();
        }


        public static void UpdateAlerts(List<Alert> listNewAlerts)
        {

        }
    }
}
