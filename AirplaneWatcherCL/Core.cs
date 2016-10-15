using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TowerBotLib;

namespace TowerBotCL
{
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

        public static void Start(bool isConsole)
        {
            if (isConsole)
            {
                Console.Title = "Towerbot";
                Console.WriteLine("Bot Iniciado\n");

                if (ApplicationDeployment.IsNetworkDeployed)
                    Console.WriteLine("Versão: {0}\n", ApplicationDeployment.CurrentDeployment.CurrentVersion);

            }

            startDateTime = DateTime.Now;

            hoursToNextTwitterMediumAlertPost = 9;
            nextTimeTwitterMediumAlertPost = DateTime.Now.AddHours(hoursToNextTwitterMediumAlertPost);

            twitterManager = new TwitterManager();


            strPath = Environment.GetFolderPath(
                        System.Environment.SpecialFolder.MyDocuments) + "\\logs";

#if DEBUG
            strPath += "\\debug";
#endif


            bool exists = System.IO.Directory.Exists(strPath);


            if (!exists)
                System.IO.Directory.CreateDirectory(strPath);

#if DEBUG
            while (true)
            {
                OnTimer(null, null);
                Thread.Sleep(10000);
            }
#else

            OnTimer(null, null);
            

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 15000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimer);
            timer.Start();
#endif

            if (isConsole)
            {
                while (true)
                {
                    switch (Console.ReadLine())
                    {
                        case "log on":
                            showUpdates = true;
                            nConsoleUpdates = 0;
                            Console.WriteLine("Iniciando log...\n");
                            break;
                        case "log off":
                            nConsoleUpdates = 0;
                            showUpdates = false;
                            Console.WriteLine("Log finalizado.\n");
                            break;
                        case "updateall":
                            isToForceUpdateAll = true;
                            Console.WriteLine("Forçando todos updates...\n");
                            break;
                        case "online":
                            Console.WriteLine("Tempo online:{0}\n", DateTime.Now - startDateTime);
                            break;
                        case "twitter on":
                            isTwitterActive = true;
                            Console.WriteLine("Twitter ativado.\n");
                            break;
                        case "twitter off":
                            isTwitterActive = false;
                            Console.WriteLine("Twitter desativo.\n");
                            break;
                        case "twitter test":
                            twitterManager.PostMessage(Radar.GetRadar("BSB"), "Atualização: Correção de vários bugs. #dev ");
                            Console.WriteLine("Ok.\n");
                            break;
                        case "twitter postmedium":
                            nextTimeTwitterMediumAlertPost = new DateTime();// DateTime.Now.AddHours(hoursToNextTwitterMediumAlertPost);
                            Console.WriteLine("O próximo alerta médio será postado no Twitter.\n");
                            break;
                        case "twitter nextmedium":
                            Console.WriteLine("O proximo twitter de nível médio será em: {0}\n", nextTimeTwitterMediumAlertPost.ToShortTimeString());
                            break;
                        case "refresh":
                            FiltersManager.RefreshAll();
                            Console.WriteLine("Cache limpado.\n");
                            break;
                        case "filters":
                            FiltersManager.AccessFilterCommandLine();
                            break;

                        case "help":
                            Console.WriteLine("- log on/off");
                            Console.WriteLine("- updateall");
                            Console.WriteLine("- online");
                            Console.WriteLine("- twitter on/off/test/postmedium/nextmedium");
                            Console.WriteLine("- refresh");
                            Console.WriteLine("- filters");

                            break;
                        default:
                            Console.WriteLine("\nComando desconhecido.\n");

                            break;
                    }
                }
            }

        }

        private static void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            //Task.Run(async () =>
            //{


            TowerBotLib.AlertFilter currentAlertFilter = null; // Para tratatamento de erro.
            string messageFlow = "No Flow";
            try
            {
                var alerts = FiltersManager.GetAlerts(isToForceUpdateAll);
                isToForceUpdateAll = false;

                if (showUpdates)
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " - Alertas:" + alerts.Count + " - Twitter:" + isTwitterActive);

                for (int i = 0; i < alerts.Count; i++)
                {

                    currentAlertFilter = alerts[i];
                    ConsoleColor color = ConsoleColor.Gray;

                    switch (currentAlertFilter.AlertType)
                    {
                        case TowerBotLib.FilterAlertType.Low:
                            color = ConsoleColor.Green;
                            break;
                        case TowerBotLib.FilterAlertType.Medium:
                            color = ConsoleColor.Yellow;
                            break;
                        case TowerBotLib.FilterAlertType.High:
                            color = ConsoleColor.Red;
                            break;
                    }

                    messageFlow = "";
                    messageFlow += ">Tratando alerta ";
                    if (alerts[i].AlertType != TowerBotLib.FilterAlertType.NoAlert)
                    {
                        messageFlow += ">Gravação de log de alertas";

                        using (StreamWriter w = File.AppendText(strPath + "\\logAlerts_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt"))
                        {
                            Log(alerts[i].ToString() + ";" + alerts[i].Justify, w);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(alerts[i].Radar.Name);
                            Console.ForegroundColor = color;
                            Console.WriteLine(" " + DateTime.Now.ToString("HH:mm:ss") + " - " + (alerts[i].ToString().Length < 64 ? alerts[i].ToString() : alerts[i].ToString().Substring(0, 61) + "..."));

                        }
                    }
                    // Se o alerta for high, postar no twitter de todo jeito
                    if (alerts[i].AlertType == TowerBotLib.FilterAlertType.High ||
                        // Se passou 9 horas sem postar nada, então ver se é alerta médio, se não ta de madrugada e postar.
                        alerts[i].AlertType == TowerBotLib.FilterAlertType.Medium && nextTimeTwitterMediumAlertPost <= DateTime.Now && DateTime.Now.Hour >= 9)
                    {
                        messageFlow += ">Alerta High ";

                        using (StreamWriter w = File.AppendText(strPath + "\\logAlertsHigh_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt"))
                        {
                            if (alerts[i].AlertType == TowerBotLib.FilterAlertType.Medium)
                            {
                                messageFlow += ">Alerta Medium convertido em High ";
                                alerts[i].Message = "Curiosidade: " + alerts[i].Message;
                                alerts[i].AlertType = TowerBotLib.FilterAlertType.High;
                            }

                            if (isTwitterActive && !isFirstTime)
                            {
                                messageFlow += ">Enviado ao twitter ";
                                twitterManager.PostMessage(alerts[i].Radar, alerts[i].Message);
                                Console.WriteLine("> Um alerta foi postado no Twitter.");
                            }

                            messageFlow += ">Terminando o high ";
                            Log(alerts[i].ToString(), w);
                            nextTimeTwitterMediumAlertPost = DateTime.Now.AddHours(hoursToNextTwitterMediumAlertPost);

                        }
                    }
                    if (alerts[i].AlertType == TowerBotLib.FilterAlertType.NoAlert)
                    {
                        messageFlow += ">Gravando no log NoAlert ";

                        using (StreamWriter w = File.AppendText(strPath + "\\log_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt"))
                        {
                            Log(alerts[i].ToString(), w);
                        }
                    }

                }

                ServerWriter.UpdatePages(alerts);
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Core Geral");
            }
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.Title = "Robô Bum - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");


            if (isFirstTime && ApplicationDeployment.IsNetworkDeployed)
            {
#if !DEBUG
                isTwitterActive = true;
                Console.WriteLine("> Twitter ativado automaticamente.");
#endif
            }

            isFirstTime = false;

            if (nConsoleUpdates > 5 && showUpdates)
            {
                Console.WriteLine(" > Logs de update desligados automaticamente.");
                showUpdates = false;
            }
            else if (showUpdates)
            {
                nConsoleUpdates++;
            }
        }

        private static void Log(string logMessage, TextWriter w)
        {
            //w.Write("Log Entry : ");
            w.Write("{0}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            w.WriteLine(" ;{0};", logMessage);
            //w.WriteLine(".", logMessage);
        }

        private static void DumpLog(StreamReader r)
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }
    }
}
