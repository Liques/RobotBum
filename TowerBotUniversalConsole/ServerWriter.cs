using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TowerBotFoundationCore;
using TowerBotLibCore;

namespace TowerBotUniversalConsole

{
    static class ServerWriter
    {
        public static DateTime OpenDateTime;
        //public static List<Alert> listOldAlerts = new List<Alert>();

        private static string strJSONPath = System.IO.Directory.GetCurrentDirectory() + "\\logs";

        /// <summary>
        /// A string abaixo eh para testes, se tiver preenchido, favor retirar
        /// </summary>
        private static string specialFolderName = "";

        static ServerWriter()
        {

           

#if DEBUG
            strJSONPath += "\\debug";
#endif

            OpenDateTime = DateTime.Now;
           
        }

        //SQLiteConnection m_dbConnection = new SQLiteConnection(@"Data Source=d:\Documents\logs\LastAlerts.sqb;");

        //string sql = "SELECT * FROM Alerts";
        //SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
        //m_dbConnection.Open();
        //SQLiteDataReader reader = command.ExecuteReader();
        //var context = new DataContext(m_dbConnection);

        //var companies = context.GetTable<AircraftEntitie>().ToList();



        public static void UpdatePages(List<Alert> listNewAlerts)
        {





            // Verificar se algum alerta antigo passou da data de validade e remove-lo.
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
            toJSON = toJSON.Replace("TimeCreated", "@T").Replace("Icon", "@I").Replace("AlertType", "@A").Replace("2016", "$").Replace("-03:00", "%").Replace("BSB", "*").Replace("Message", "@M").Replace("Radar", "@R").Replace("Name", "@N").Replace("AirplaneID", "@U").Replace("TimeToBeDeleted", "@D");

            WriteFile(strJSONPath, "lastAlerts.json", toJSON);


            var strPath = @"c:\inetpub\wwwroot\aeroradartk" + specialFolderName;

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                strPath = System.IO.Directory.GetCurrentDirectory() + "\\logs";

#if DEBUG
            strPath += @"\debug";
#endif
            WriteFile(strPath, "index2.html", IndividualRadar(null, true));
            WriteFile(strPath, "index.html", IndividualRadar(null));

            bool exists = System.IO.Directory.Exists(strPath + @"\data\");
            if (!exists)
                System.IO.Directory.CreateDirectory(strPath + @"\data\");

            foreach (Radar radar in Radar.ListRadars)
            {
                try
                {
                    string currentPath = strPath + @"\main\" + radar.Name;

                    WriteFile(currentPath, "index.html", IndividualRadar(radar));
                    var listAlertByRadar = Alert.ListOfAlerts.Where(s => s.Radar != null && s.Radar.Name == radar.Name && s.AlertType != PluginAlertType.NoAlert).ToList();
                    WriteFile(strPath + @"\data\", radar.Name + ".json", JsonConvert.SerializeObject(listAlertByRadar));
                    var listAlertByRadarFiveMinutes = listAlertByRadar.Where(s => s.TimeCreated > DateTime.Now.AddMinutes(-5)).ToList();
                    WriteFile(strPath + @"\data\", radar.Name + "fast.json", JsonConvert.SerializeObject(listAlertByRadarFiveMinutes));
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            var listGeneralOnlyImportantAlert = Alert.ListOfAlerts.Where(s => s.AlertType == PluginAlertType.High).Take(100).ToList();
            WriteFile(strPath + @"\data\", "general.json", JsonConvert.SerializeObject(listGeneralOnlyImportantAlert));
        }

        private static void WriteFile(string currentPath, string fileName, string content)
        {

            bool exists = System.IO.Directory.Exists(currentPath);
            if (!exists)
                System.IO.Directory.CreateDirectory(currentPath);

            currentPath += @"\" + fileName;

            if (File.Exists(currentPath))
                File.Delete(currentPath);

            File.AppendAllText(currentPath, content, Encoding.UTF8);
        }

        private static string LoadFile(string currentPath, string fileName)
        {

            bool exists = System.IO.Directory.Exists(currentPath);
            if (!exists)
                return String.Empty;

            currentPath += @"\" + fileName;

            if (!File.Exists(currentPath))
                return String.Empty;

            return File.ReadAllText(currentPath).Replace("@T", "TimeCreated").Replace("@I", "Icon").Replace("@A", "AlertType").Replace("$", "2016").Replace("%", "-03:00").Replace("*", "BSB").Replace("@M", "Message").Replace("@R", "Radar").Replace("@N", "Name").Replace("@U", "AirplaneID").Replace("@D", "TimeToBeDeleted");
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
            strBuilder.Append("<link rel='shortcut icon' href='http://d1odq4u5rh2864.cloudfront.net/images/favicon.png' />");
            strBuilder.Append("<meta name=viewport content='width=device-width, initial-scale=1'>");
            strBuilder.Append("<link rel='stylesheet' type='text/css' href='https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.6/css/bootstrap.min.css'>");
            strBuilder.Append("<title>Robô Bum</title>");
            strBuilder.Append("<meta http-equiv='refresh' content='30' >");
            strBuilder.Append("<h3><b>Robô Bum (");
            if (radar != null)
                strBuilder.Append(listAlertByRadar.FirstOrDefault() != null ? listAlertByRadar.FirstOrDefault().Radar.Description : "Não definido");
            else
                strBuilder.Append("Brasil");


            strBuilder.Append(")</b></h3>");
#if DEBUG
            strBuilder.Append(" (Debug)<br/>");
#endif

            if (DateTime.UtcNow < new DateTime(2016, 5, 3, 7, 15, 0))
                strBuilder.Append("<span class='success hidden-lg hidden-md'><b>Psiu, o bum agora foi otimizado para o celular! Assim você vai poder acompanhar a tocha chegar melhor. ;-)</b><br></span>");

            strBuilder.Append("<span class='visible-lg-inline visible-md-inline'><b>Tempo online: " + (DateTime.Now - OpenDateTime).ToString(@"dd\ hh\:mm\:ss") + "</b></span><hr/>");

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
                            strBuilder.Append("<span ><b class='visible-lg-inline visible-md-inline'>" + listAlertByRadar[i].TimeCreated.ToString("dd/MM/yyyy ") + "</b></span>");
                            strBuilder.Append("<span><b>" + listAlertByRadar[i].TimeCreated.ToString("HH:mm") + "</b>");
                        }
                        else
                        {
                            strBuilder.Append("<span><b class='visible-lg-inline visible-md-inline'> " + Alert.ListOfAlerts[i].TimeCreated.ToString("dd/MM/yyyy ") + "</b></span>");
                            strBuilder.Append("<span><b>" + listAlertByRadar[i].TimeCreated.ToString("HH:mm") + "</b> - ");
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
#if true//DEBUG
                        strBuilder.Append("<span class='visible-lg-inline'><font color=lightgray> ");
                        if (!String.IsNullOrEmpty(listAlertByRadar[i].ID))
                            strBuilder.Append(listAlertByRadar[i].ID.Length > 8 ? listAlertByRadar[i].ID.Substring(0, 10) : listAlertByRadar[i].ID);

                        strBuilder.Append("</font></span>");
#endif
                        strBuilder.Append("<br/>");

                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            strBuilder.Append("<hr/>");
            strBuilder.Append("<font color=red>Alertas de importância alta (postados no Twitter).</font><br/>");
            strBuilder.Append("<font color=darkorange>Alertas de importância média.</font><br/>");
            strBuilder.Append("<font color=green>Alertas de importância baixa.</font><br/>");
            strBuilder.Append("<font color=darkgray>Alertas em testes.</font><br/>");
            strBuilder.Append("<hr/>");
            strBuilder.Append("<h5>Obs.: Esse é um site de testes, mantido pela comunidade de spotting de Brasília. Os alertas de Brasília são publicado no twitter @aeroradardf, os alertas de outros locais ainda estão em testes. Contato: aeroradardf@outlook.com.</h5>");



            return strBuilder.ToString();
        }


        public static void UpdateAlerts(List<Alert> listNewAlerts)
        {

        }
    }
}
