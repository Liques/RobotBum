using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerBotFoundation;

namespace TowerBotLib.Filters
{
    class SpecialAlert : IFilter
    {

        private class SpecialAlertSchedule
        {
            public string ID { get; set; }
            public string Description { get; set; }
            public AircraftRegistration aircraftRegistration;
            public AircraftType aircraftType;
            public string flightName;
            public DateTime startTime;
            public DateTime endTime;
            public DateTime nextAnalyse;
            public TimeSpan periodToAnalyse;
            public string message;
            public FilterAlertType alertType;
            public string lat1, lat2, lon1, lon2 = String.Empty;
            public AirplaneStatus Status { get; set; }



            public SpecialAlertSchedule()
            {
                ID = "SpecialAlertSchedule" + DateTime.Now.ToString("ddMMyyyyHHmmss");
                Description = string.Empty;
                startTime = DateTime.Now;
                nextAnalyse = startTime;
                aircraftRegistration = null;
                aircraftType = null;
                endTime = startTime;
                periodToAnalyse = new TimeSpan(0);
                message = string.Empty;
                flightName = string.Empty;
                alertType = FilterAlertType.Low;

            }

            public override string ToString()
            {
                return this.Description +
                    "\nInicio: " + startTime.ToString("dd/MM/yyyy HH:mm") +
                    "\nFim: " + endTime.ToString("dd/MM/yyyy HH:mm") +
                    "\nPeriodo: " + periodToAnalyse.ToString() +
                    "\nMensagem: " + message +
                    "\nState: " + Status.ToString() +
                    "\nNível de alerta: " + alertType.ToString() +
                    "\nID: " + ID;
            }


        }

        //private bool isWorking = false;
        //private ZoneType choosedZone = ZoneType.DF;
        //private AircraftRegistration aircraftRegistration;
        //private AircraftType aircraftType;
        //private DateTime startTime;
        //private DateTime endTime;
        //private DateTime nextAnalyse;
        //private TimeSpan periodToAnalyse;
        //private string message;
        //private FilterAlertType alertType;

        private List<SpecialAlertSchedule> listSpecialAlertSchedule;

        public bool IsActive { get; set; }
        public bool IsTesting { get; set; }
        public string Name { get; set; }
        public Radar Radar { get; set; }


        public SpecialAlert()
        {
            Name = "SpecialAlert";
            IsActive = true;
            IsTesting = false;

            listSpecialAlertSchedule = new List<SpecialAlertSchedule>();
        }

        public List<AlertFilter> Analyser(object parameter)
        {
            // Já vem como padrão a lista dos aviões do DF
            List<AirplaneBasic> listAirplanes = (List<AirplaneBasic>)parameter;
            List<AlertFilter> listAlerts = new List<AlertFilter>();

            try
            {
                if (IsActive)
                {
                    List<SpecialAlertSchedule> listSpecialAlertScheduleToDelete = listSpecialAlertSchedule.Where(s => s.endTime <= DateTime.Now).ToList();

                    foreach (var item in listSpecialAlertScheduleToDelete)
                    {
                        listSpecialAlertSchedule.Remove(item);
                    }

                    List<SpecialAlertSchedule> listSpecialAlertScheduleActive = listSpecialAlertSchedule.Where(s => s.startTime <= DateTime.Now).ToList();

                    foreach (var specialAlertSchedule in listSpecialAlertScheduleActive)
                    {
                        if (specialAlertSchedule.nextAnalyse <= DateTime.Now)
                        {
                            listAirplanes = AirplanesData.GetAirplanes(specialAlertSchedule.lat1, specialAlertSchedule.lat2, specialAlertSchedule.lon1, specialAlertSchedule.lon2).Result;

                            specialAlertSchedule.nextAnalyse = DateTime.Now + specialAlertSchedule.periodToAnalyse;

                            if (specialAlertSchedule.Status != AirplaneStatus.DataImcomplete)
                            {
                                listAirplanes = listAirplanes.Where(s => s.State == specialAlertSchedule.Status).ToList();
                            }

                            AirplaneBasic airplaneFiltered = null;

                            if (specialAlertSchedule.aircraftRegistration != null)
                                airplaneFiltered = listAirplanes.Where(s => s.Registration.Name.ToLower().StartsWith(specialAlertSchedule.aircraftRegistration.Name.ToLower())).FirstOrDefault();
                            else if (specialAlertSchedule.aircraftType != null)
                                airplaneFiltered = listAirplanes.Where(s => s.AircraftType.ICAO.ToLower().Contains(specialAlertSchedule.aircraftType.ICAO.ToLower())).FirstOrDefault();
                            else if (!String.IsNullOrEmpty(specialAlertSchedule.flightName))
                                airplaneFiltered = listAirplanes.Where(s => s.FlightName.ToLower().Contains(specialAlertSchedule.flightName.ToLower())).FirstOrDefault();

                            if (airplaneFiltered != null)
                            {
                                MessageType messageType = (!String.IsNullOrEmpty(specialAlertSchedule.message)) ? MessageType.Fixed : MessageType.General;

                                AlertFilter alert = new AlertFilter(this.Radar, specialAlertSchedule.ID, airplaneFiltered, IconType.NoIcon, messageType);
                                alert.Message = specialAlertSchedule.message;
                                alert.AlertType = specialAlertSchedule.alertType;
                                listAlerts.Add(alert);

                                specialAlertSchedule.endTime = new DateTime();

                            }

                        }
                    }
                }

            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Filter Special Alert");
            }

            return listAlerts;
        }

        private string AddSpecialAlertSchedule()
        {
            while (true)
            {
                string result = string.Empty;

                SpecialAlertSchedule schedule = new SpecialAlertSchedule();
                Console.WriteLine("\n\nNovo Alerta\n\n");
                Console.WriteLine("> Descrição:");
                schedule.Description = Console.ReadLine();
                Console.WriteLine("\n> Registro do avião (ou começa com...)");
                string aircraftRegistration = Console.ReadLine();
                schedule.aircraftRegistration = (!string.IsNullOrEmpty(aircraftRegistration)) ? new AircraftRegistration(aircraftRegistration) : null;
                if (string.IsNullOrEmpty(aircraftRegistration))
                {
                    Console.WriteLine("\n> Modelo do avião (ou começa com...)");
                    string aircraftType = Console.ReadLine();
                    schedule.aircraftType = (!string.IsNullOrEmpty(aircraftType)) ? AircraftType.GetAircraftType(aircraftType) : null;

                    if (string.IsNullOrEmpty(aircraftType))
                    {
                        Console.WriteLine("\n> Voo (ou começa com...)");
                        string flightName = Console.ReadLine();
                        schedule.flightName = flightName;
                        if (string.IsNullOrEmpty(flightName))
                        {
                            return "\n> Não adicionado: Cancelado por falta de dados necessários";
                        }
                        else
                        {
                            schedule.Description += "/ Voo " + schedule.flightName;
                        }
                    }
                    else
                    {
                        schedule.Description += "/ Modelo " + schedule.aircraftType.Name;
                    }
                }
                else
                {
                    schedule.Description += "/ Registro " + schedule.aircraftRegistration.Name;
                }

                bool newDateTime = true;

                if (listSpecialAlertSchedule.Count > 0)
                {
                    var lastSchedule = listSpecialAlertSchedule.LastOrDefault();

                    Console.WriteLine("\n> Deve estar junto com último alerta? (S/N)");
                    if (Console.ReadKey().Key == ConsoleKey.S)
                    {
                        newDateTime = false;
                    }
                    else if (listSpecialAlertSchedule.Count > 1 && newDateTime == true)
                    {
                        Console.WriteLine("\n> Quer associar com outro alerta? (S/N)");

                        if (Console.ReadKey().Key == ConsoleKey.S)
                        {
                            Console.WriteLine("\n> Qual nome do outro alerta?");
                            string idsche = Console.ReadLine().ToLower();
                            lastSchedule = listSpecialAlertSchedule.Where(s => s.ID.ToLower().StartsWith(idsche)).FirstOrDefault();
                            if (lastSchedule != null)
                            {
                                newDateTime = false;
                            }
                            else
                            {
                                Console.WriteLine("\n> ID não encontrado, continuar sem associar? (S/N)");

                                if (Console.ReadKey().Key != ConsoleKey.S)
                                {
                                    return "\n> Não adicionado: ID de alerta anterior não encontrado.";
                                }
                            }
                        }
                    }

                    if (newDateTime == false)
                    {
                        schedule.startTime = lastSchedule.startTime;
                        schedule.endTime = lastSchedule.endTime;
                        schedule.periodToAnalyse = lastSchedule.periodToAnalyse;
                        schedule.nextAnalyse = lastSchedule.nextAnalyse;
                    }
                }

                if (newDateTime)
                {
                    Console.WriteLine("\n> Data de início (dd/MM/yyyy HH:mm, nulo para data atual)");
                    if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture, DateTimeStyles.None, out schedule.startTime))
                    {
                        schedule.startTime = DateTime.Now;
                    }
                    Console.WriteLine("\n> Quantas horas eles vai funcionar?");
                    int horas;
                    if (!int.TryParse(Console.ReadLine(), out horas))
                    {
                        return "\n> Não adicionado: Hora digitada em formato errado.";
                    }
                    schedule.endTime = schedule.startTime.AddHours(horas);

                    Console.WriteLine("\n> De quanto em quantos minutos? (nulo para padrão)");
                    int minutos = 0;
                    int.TryParse(Console.ReadLine(), out minutos);

                    schedule.periodToAnalyse = new TimeSpan(0, minutos, 0);

                }

                Console.WriteLine("\n> Em qual Status para capturar o voo? (Cruise/TakingOff/Landing/Taxing, nulo para default)");
                string status = Console.ReadLine().ToLower();
                if ("cruise".StartsWith(status))
                    schedule.Status = AirplaneStatus.Cruise;
                else if ("takingoff".StartsWith(status))
                    schedule.Status = AirplaneStatus.TakingOff;
                else if ("landing".StartsWith(status))
                    schedule.Status = AirplaneStatus.Landing;
                else if ("taxing".StartsWith(status))
                    schedule.Status = AirplaneStatus.ParkingOrTaxing;

                Console.WriteLine("\n> Qual é a zona? (DF/Brazil/World/Custom, nulo para default)");

                Console.WriteLine("\n> Qual é a latitude 1?");
                schedule.lat1 = Console.ReadLine();
                Console.WriteLine("\n> Qual é a longitude 1?");
                schedule.lon1 = Console.ReadLine();
                Console.WriteLine("\n> Qual é a latitude 2?");
                schedule.lat2 = Console.ReadLine();
                Console.WriteLine("\n> Qual é a longitude 2?");
                schedule.lon2 = Console.ReadLine();

                Console.WriteLine("\n> Qual mensagem deve ser o alerta? (nulo para padrão)");
                schedule.message = Console.ReadLine();

                Console.WriteLine("\n> É alerta de nível alto? (S/N)");
                if (Console.ReadKey().Key == ConsoleKey.S)
                    schedule.alertType = FilterAlertType.High;
                else
                    schedule.alertType = FilterAlertType.Medium;

                Console.WriteLine("\n\n-----------Revisão-----------\n");
                Console.WriteLine(schedule.ToString());
                Console.WriteLine("\n------------------------------\nEstá tudo correto? (S/N)");
                if (Console.ReadKey().Key == ConsoleKey.S)
                {
                    listSpecialAlertSchedule.Add(schedule);
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
            Console.WriteLine("---------------\nFiltro Special Alert\n\n+Filtro ativo:" + this.IsActive + "\n+Alertas schedules ativos: " + listSpecialAlertSchedule.Count + "\n---------------\n-test on\\off\n-disable\n-enable\n-add\n-list\n-cancel all\n");
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
                Console.WriteLine(AddSpecialAlertSchedule());
            }
            else if (comando == "list")
            {
                foreach (var item in listSpecialAlertSchedule)
                {
                    Console.WriteLine("\n\n{0}\n\n", item.ToString());
                }
                Console.WriteLine("Total de itens: {0}\n", listSpecialAlertSchedule.Count);

            }
            else if (comando == "cancel all")
            {
                foreach (var item in listSpecialAlertSchedule)
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
