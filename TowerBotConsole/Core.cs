﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TowerBotLibCore;

namespace TowerBotConsole
{
    /// <summary>
    /// Core object
    /// </summary>
    public class Core
    {
        static bool showUpdates = true;
        static bool isToForceUpdateAll = false;
        static bool isTwitterActive = false;
        static bool isFirstTime = true;
        static int nConsoleUpdates = 0;
        static string strPath = String.Empty;
        static DateTime startDateTime;
        static TwitterManager twitterManager;
        static DateTime nextTimeTwitterMediumAlertPost;
        static int hoursToNextTwitterMediumAlertPost;
         
        /// <summary>
        /// Start App
        /// </summary>
        /// <param name="isConsole"></param>
        public static void Start(bool isConsole)
        {
            if (isConsole)
            {
                Console.Title = "Towerbot\n";
                Console.WriteLine("Starting...\n");

            }

            startDateTime = DateTime.Now;

            hoursToNextTwitterMediumAlertPost = 9;
            nextTimeTwitterMediumAlertPost = DateTime.Now.AddHours(hoursToNextTwitterMediumAlertPost);

            twitterManager = new TwitterManager();


            strPath = "logs";

            bool exists = System.IO.Directory.Exists(strPath);


            if (!exists)
                System.IO.Directory.CreateDirectory(strPath);

                 var radarBSB = new Radar()
            {
                Name = "BSB",
                Description = "Brasília - DF",
                MainAirportICAO = "SBBR",
               
                EndpointUrl = "http://bsbradar.ddns.net:8081/json",
                LongitudeX = -48.336099,
                LatitudeX = -15.364184,
                LongitudeY = -47.256692,
                LatitudeY = -16.194103,
                ListRunways = new List<RunwayBasic>() {
                    new RunwayBasic()
                    {
                        NameSideOne = "11L",
                        NameSideTwo = "29R",
                        LatitudeSideOne = -15.861333,
                        LongitudeSideOne = -47.930333,
                        LatitudeSideTwo = -15.86,
                        LongitudeSideTwo = -47.898167,
                    },
                    new RunwayBasic()
                    {
                        NameSideOne = "11R",
                        NameSideTwo = "29L",
                        LatitudeSideOne = -15.879167,
                        LongitudeSideOne = -47.942,
                        LatitudeSideTwo = -15.8765,
                        LongitudeSideTwo = -47.9085,
                    }
                },
                 TwitterConsumerKey = "3r8wBciRbW7wniT7DYIofy60G",
        TwitterConsumerSecret  = "ozfqugyE2hihws5AkGw8yXVvuZMqY5u9rpIOjdKxxHjqo3KM5T",
       TwitterAccessToken  = "3087708189-bkr12ClOMZyBeiHmw7i9EZeXlnSNAjx3QjKnxe4",
        TwitterAccessTokenSecret  = "cVL2s1kCzJl3nAydDXkIz1fVY07g1XWnUGByjb92ZO8wj",

            };

            Radar.AddRadar(radarBSB);

            var autoEvent = new AutoResetEvent(false);

            var timer = new System.Threading.Timer(new TimerCallback(CheckStatus), null, new TimeSpan(0), new TimeSpan(0, 0, 10));

            if (isConsole)
            {
                while (true)
                {
                    switch (Console.ReadLine())
                    {
                        case "log on":
                            showUpdates = true;
                            nConsoleUpdates = 0;
                            Console.WriteLine("Starting log...\n");
                            break;
                        case "log off":
                            nConsoleUpdates = 0;
                            showUpdates = false;
                            Console.WriteLine("Log done.\n");
                            break;
                        case "updateall":
                            isToForceUpdateAll = true;
                            Console.WriteLine("Forcing updates...\n");
                            break;
                        case "online":
                            Console.WriteLine("Online time:{0}\n", DateTime.Now - startDateTime);
                            break;
                        case "twitter on":
                            isTwitterActive = true;
                            Console.WriteLine("Twitter actived.\n");
                            break;
                        case "twitter off":
                            isTwitterActive = false;
                            Console.WriteLine("Twitter desactived.\n");
                            break;
                        case "refresh":
                            PluginsManager.RefreshAll();
                            Console.WriteLine("Cache cleaned.\n");
                            break;
                        case "plugins":
                            PluginsManager.AccessPluginCommandLine();
                            break;

                        case "help":
                            Console.WriteLine("- log on/off");
                            Console.WriteLine("- updateall");
                            Console.WriteLine("- online");
                            Console.WriteLine("- twitter on/off/test");
                            Console.WriteLine("- refresh");
                            Console.WriteLine("- Plugins");

                            break;
                        case "exit":
                            Environment.Exit(0);

                            break;
                        default:
                            Console.WriteLine("\nComando desconhecido.\n");

                            break;
                    }
                }
            }



        }
        
        private static void CheckStatus(object stateInfo)
        {
            TowerBotLibCore.Alert currentAlert = null; // Para tratatamento de erro.
            string messageFlow = "No Flow";

            var alerts = PluginsManager.GetAlerts(isToForceUpdateAll);
            isToForceUpdateAll = false;

            for (int i = 0; i < alerts.Count; i++)
            {

                currentAlert = alerts[i];
                ConsoleColor color = ConsoleColor.Gray;

                switch (currentAlert.AlertType)
                {
                    case TowerBotLibCore.PluginAlertType.Low:
                        color = ConsoleColor.Green;
                        break;
                    case TowerBotLibCore.PluginAlertType.Medium:
                        color = ConsoleColor.Yellow;
                        break;
                    case TowerBotLibCore.PluginAlertType.High:
                        color = ConsoleColor.Red;
                        break;
                }

                messageFlow = "";
                messageFlow += ">Tratando alerta ";
                if (alerts[i].AlertType != TowerBotLibCore.PluginAlertType.NoAlert)
                {
                    messageFlow += ">Gravação de log de alertas";

                    using (StreamWriter w = File.AppendText(strPath + "\\logAlerts_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt"))
                    {
                        Log(alerts[i].ToString() + ";" + alerts[i].Justify, w);
                        Console.ForegroundColor = ConsoleColor.White;
                        //Console.Write(alerts[i].Radar.Name);
                        Console.ForegroundColor = color;
                        //Console.WriteLine(" " + DateTime.Now.ToString("HH:mm:ss") + " - " + (alerts[i].ToString().Length < 64 ? alerts[i].ToString() : alerts[i].ToString().Substring(0, 61) + "..."));

                    }
                }
                // Se o alerta for high, postar no twitter de todo jeito
                if (alerts[i].AlertType == TowerBotLibCore.PluginAlertType.High ||
                    // Se passou 9 horas sem postar nada, então ver se é alerta médio, se não ta de madrugada e postar.
                    alerts[i].AlertType == TowerBotLibCore.PluginAlertType.Medium && nextTimeTwitterMediumAlertPost <= DateTime.Now && DateTime.Now.Hour >= 9)
                {
                    messageFlow += ">Alerta High ";

                    using (StreamWriter w = File.AppendText(strPath + "\\logAlertsHigh_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt"))
                    {                        
                        if (isTwitterActive && !isFirstTime)
                        {
                            twitterManager.PostMessage(alerts[i].Radar, alerts[i].Message);
                        };

                        Log(alerts[i].ToString(), w);
                        
                    }
                }
                if (alerts[i].AlertType == TowerBotLibCore.PluginAlertType.NoAlert)
                {
                    messageFlow += ">Gravando no log NoAlert ";

                    using (StreamWriter w = File.AppendText(strPath + "\\log_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt"))
                    {
                        Log(alerts[i].ToString(), w);
                    }
                }

            }

            if(alerts != null && alerts.Count > 0) {
                Console.WriteLine(String.Format("{0} - {1} issued alert(s).",DateTime.Now.ToString(), alerts.Count));
            }

            var htmlFolder = "/var/www/html/";

            if(System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)) {
                htmlFolder = @"server";
            }

            ServerWriter.UpdatePages(alerts, htmlFolder);
/*
#if !DEBUG
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Core Geral");
            }
#endif
*/
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.Title = "Robot Bum (Core) - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");


            if (isFirstTime)
            {

//#if !DEBUG
                isTwitterActive = true;
                Console.WriteLine("> Twitter ativado automaticamente.");
//#endif
            }

            isFirstTime = false;

        }

        private static void Log(string logMessage, TextWriter w)
        {
            w.Write("{0}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            w.WriteLine(" ;{0};", logMessage);
        }

    }
}
