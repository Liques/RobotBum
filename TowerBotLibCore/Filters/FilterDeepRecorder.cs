using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TowerBotFoundationCore;

namespace TowerBotLibCore.Filters
{
    class FilterDeepRecorder : IFilter
    {

        private class BlockAnalyserSchedule
        {
            public string ID { get; set; }
            public string Description { get; set; }
            public DateTime startTime;
            public DateTime endTime;
            public DateTime nextAnalyse;
            public TimeSpan periodToAnalyse;
            public string lat1, lat2, lon1, lon2 = String.Empty;
            



            public List<AirplaneBasic> LastAirplanes { get; set; }

            public BlockAnalyserSchedule()
            {
                ID = "BlockAnalyserSchedule" + DateTime.Now.ToString("ddMMyyyyHHmmss");
                Description = string.Empty;
                startTime = DateTime.Now;
                nextAnalyse = startTime;
                endTime = startTime;
                periodToAnalyse = new TimeSpan(0);
                LastAirplanes = new List<AirplaneBasic>();
            }

            public override string ToString()
            {
                return this.Description +
                    "\n Inicio: " + startTime.ToString("dd/MM/yyyy HH:mm") +
                    "\n Fim: " + endTime.ToString("dd/MM/yyyy HH:mm") +
                    "\n Periodo: " + periodToAnalyse.ToString() +
                    "\n LatLong1: " +lat1 + " " + lon1 +
                    "\n LatLong2: " + lat2 + " " + lon2 +
                    "\n ID: " + ID;
            }


        }

        private List<BlockAnalyserSchedule> listBlockAnalyserSchedule;

        public bool IsActive { get; set; }
        public bool IsTesting { get; set; }
        public string Name { get; set; }
        public Radar Radar { get; set; }



        string strPath = String.Empty;

        public FilterDeepRecorder()
        {
            Name = "DeepRec";
            IsActive = true;
            IsTesting = false;

            listBlockAnalyserSchedule = new List<BlockAnalyserSchedule>();
        }

        public List<AlertFilter> Analyser(object parameter)
        {
            // Já vem como padrão a lista dos aviões do DF
            List<AirplaneBasic> listAirplanes = null;
            List<AlertFilter> listAlerts = new List<AlertFilter>();

            try
            {
                if (IsActive)
                {
                    List<BlockAnalyserSchedule> listBlockAnalyserScheduleToDelete = listBlockAnalyserSchedule.Where(s => s.endTime <= DateTime.Now).ToList();

                    foreach (var item in listBlockAnalyserScheduleToDelete)
                    {
                        listBlockAnalyserSchedule.Remove(item);
                    }

                    List<BlockAnalyserSchedule> listBlockAnalyserScheduleActive = listBlockAnalyserSchedule.Where(s => s.startTime <= DateTime.Now).ToList();

                    strPath = System.IO.Directory.GetCurrentDirectory() + "\\logs\\deep";

                    bool exists = System.IO.Directory.Exists(strPath);


                    if (!exists)
                        System.IO.Directory.CreateDirectory(strPath);


                    foreach (var blockAnalyserSchedule in listBlockAnalyserScheduleActive)
                    {

                        if (blockAnalyserSchedule.nextAnalyse <= DateTime.Now)
                        {

                            listAirplanes = AirplanesData.GetAirplanes(blockAnalyserSchedule.lat1, blockAnalyserSchedule.lat2, blockAnalyserSchedule.lon1, blockAnalyserSchedule.lon2).Result;

                            foreach (AirplaneBasic airplane in listAirplanes)
                            {
                                AirplaneBasic previousRecord = null;
                                if (!String.IsNullOrEmpty(airplane.ID))
                                    previousRecord = blockAnalyserSchedule.LastAirplanes.Where(s => s.ID == airplane.ID).FirstOrDefault();

                                List<string> lstRecords = new List<string>();

                                string stateString = "N";
                                switch (airplane.State)
                                {
                                    case AirplaneStatus.Cruise:
                                        stateString = "C";
                                        break;
                                    case AirplaneStatus.Landing:
                                        stateString = "L";
                                        break;
                                    case AirplaneStatus.ParkingOrTaxing:
                                        stateString = "P";
                                        break;
                                    case AirplaneStatus.TakingOff:
                                        stateString = "T";
                                        break;
                                }

                                if (previousRecord != null)
                                {
                                    TimeSpan timeSpan = airplane.DateCreation - previousRecord.DateCreation;
                                    if (timeSpan.TotalMinutes < 3)
                                    {
                                        double totalSeconds = timeSpan.TotalSeconds;
                                        // Single percentage de 3 segundos

                                        for (int i = 3; i < totalSeconds; i += 3)
                                        {
                                            AirplaneBasic ghostAirplane = new AirplaneBasic();
                                            lock (ghostAirplane)
                                            {
                                                double singlePercentage = i * 100 / totalSeconds;

                                                ghostAirplane.DateCreation = previousRecord.DateCreation.AddSeconds(i);
                                                ghostAirplane.ID = previousRecord.ID;
                                                ghostAirplane.FlightName = previousRecord.FlightName;
                                                ghostAirplane.Registration = previousRecord.Registration;
                                                ghostAirplane.AircraftType = previousRecord.AircraftType;
                                                ghostAirplane.From = previousRecord.From;
                                                ghostAirplane.To = previousRecord.To;
                                                ghostAirplane.Weight = previousRecord.Weight;



                                                double totalLongitude = airplane.Longitude - previousRecord.Longitude;
                                                double totalLatitude = airplane.Latitude - previousRecord.Latitude;

                                                ghostAirplane.Longitude = Math.Round(previousRecord.Longitude + ((singlePercentage * totalLongitude / 100)),6);
                                                ghostAirplane.Latitude = Math.Round(previousRecord.Latitude + (singlePercentage * totalLatitude / 100),6);

                                                double totalSpeed = airplane.Speed - previousRecord.Speed;
                                                ghostAirplane.Speed = previousRecord.Speed + (int)Math.Round(singlePercentage * totalSpeed / 100);

                                                double totalVSpeed = airplane.VerticalSpeed - previousRecord.VerticalSpeed;
                                                ghostAirplane.VerticalSpeed = previousRecord.VerticalSpeed + (singlePercentage * totalVSpeed / 100);

                                                double totalDirection = previousRecord.Direction - airplane.Direction;
                                                ghostAirplane.Direction = previousRecord.Direction + (int)Math.Round(singlePercentage * totalDirection / 100);

                                                string regMsg = TransformAirplaneToLine(ghostAirplane, stateString);
                                                lstRecords.Add(regMsg);
                                            }


                                        }
                                    }
                                }

                                // Date;ID;Flight;Registration;Model;FromPlace;ToPlace;Latitude;Longitude;Speed;VerticalSpeed;Direction;IsWide;State
                                string regMsgLast = TransformAirplaneToLine(airplane, stateString);
                                lstRecords.Add(regMsgLast);

                                for (int i = 0; i < lstRecords.Count; i++)
                                {
                                    using (StreamWriter w = File.AppendText(strPath + "\\" + blockAnalyserSchedule.ID + ".txt"))
                                    {
                                        //w.Write("{0}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                                        w.WriteLine("{0};", lstRecords[i]);
                                    }
                                }


                            }

                            blockAnalyserSchedule.LastAirplanes = listAirplanes;

                        }
                    }
                }

            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Filter Deep Recorder");
            }

            return listAlerts;
        }

        private string TransformAirplaneToLine(AirplaneBasic airplane, string stateString)
        {
            lock (airplane)
                return airplane.DateCreation.ToString("dd/MM/yyyy HH:mm:ss") + ";" + airplane.ID + ";" + airplane.FlightName + ";" + airplane.Registration + ";" + airplane.AircraftType.ICAO + ";" + airplane.From.IATA + ";" + airplane.To.IATA + ";" + airplane.Latitude + ";" + airplane.Longitude + ";" + airplane.Speed + ";" + airplane.VerticalSpeed + ";" + airplane.Direction + ";" + airplane.Weight + ";" + stateString;
        }

        private string AddBlockAnalyserSchedule()
        {
            while (true)
            {
                string result = string.Empty;

                BlockAnalyserSchedule schedule = new BlockAnalyserSchedule();
                Console.WriteLine("\n\nNovo Bloco de Deep Recording\n\n");
                Console.WriteLine("> Nome da Localidade:");
                schedule.ID = Console.ReadLine();

                Console.WriteLine("\n> Data de início (dd/MM/yyyy HH:mm, nulo para data atual)");
                if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture, DateTimeStyles.None, out schedule.startTime))
                {
                    schedule.startTime = DateTime.Now;
                }
                Console.WriteLine("\n> Quantas horas eles vai funcionar? (nulo para 24 horas)");
                double horas;
                if (!double.TryParse(Console.ReadLine(), out horas))
                {
                    horas = 24;
                }
                schedule.endTime = schedule.startTime.AddHours(horas);

                schedule.periodToAnalyse = new TimeSpan(0, 0, 0);

                
                Console.WriteLine("\n> Qual é a latitude 1?");
                schedule.lat1 = Console.ReadLine();
                Console.WriteLine("\n> Qual é a longitude 1?");
                schedule.lon1 = Console.ReadLine();
                Console.WriteLine("\n> Qual é a latitude 2?");
                schedule.lat2 = Console.ReadLine();
                Console.WriteLine("\n> Qual é a longitude 2?");
                schedule.lon2 = Console.ReadLine();


                Console.WriteLine("\n\n-----------Revisão-----------\n");
                Console.WriteLine(schedule.ToString());
                Console.WriteLine("\n------------------------------\nEstá tudo correto? (S/N)");
                if (Console.ReadKey().Key == ConsoleKey.S)
                {
                    listBlockAnalyserSchedule.Add(schedule);

                    using (StreamWriter w = File.AppendText(strPath + "\\" + schedule.ID + "_info.txt"))
                    {
                        w.WriteLine(schedule.ToString());
                    }

                    break;
                }
                else
                {
                    Console.WriteLine("\nDesfazendo...\n");
                }

            }
            return "\n\nAlerta Adicionado com sucesso!\n\n";

        }


        public void CommandLine()
        {
            Console.WriteLine("---------------\nFiltro Deep Recorder\n\n+Filtro ativo:" + this.IsActive + "\n+Blocos schedules ativos: " + listBlockAnalyserSchedule.Count + "\n---------------\n-test on\\off\n-disable\n-enable\n-add\n-list\n-cancel all\n");
            string comando = Console.ReadLine();
            if (comando.Contains("test on"))
            {
                IsTesting = true;
                Console.WriteLine("Filtros colocados em teste");
            }
            else if (comando.Contains("test off"))
            {
                IsTesting = false;
                Console.WriteLine("Filtros retirados de teste");
            }
            else if (comando == "enable")
            {
                IsActive = true;
                Console.WriteLine("Ok");
            }
            else if (comando == "disable")
            {
                IsActive = false;
                Console.WriteLine("Ok");
            }
            else if (comando == "add")
            {
                Console.WriteLine(AddBlockAnalyserSchedule());
            }
            else if (comando == "list")
            {
                foreach (var item in listBlockAnalyserSchedule)
                {
                    Console.WriteLine("\n\n{0}\n\n", item.ToString());
                }
                Console.WriteLine("Total de itens: {0}\n", listBlockAnalyserSchedule.Count);

            }
            else if (comando == "cancel all")
            {
                foreach (var item in listBlockAnalyserSchedule)
                {
                    item.endTime = new DateTime();
                }
                Console.WriteLine("Todos os itens foram marcados para cancelamento.\n");

            }
            else
            {
                Console.WriteLine("Comando do filtro não reconhecido.");
            }
        }
    }
}
