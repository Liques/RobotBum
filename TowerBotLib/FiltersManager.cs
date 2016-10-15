using TowerBotLib.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerBotFoundation;

namespace TowerBotLib
{
    public static class FiltersManager
    {
        public static List<GroupFilters> ListFilterGroup { get; set; }
        public static List<AirplaneBasic> ListOldAirplanesDF { get; set; }
        private static List<Airport> listMainAirports = new List<Airport>();
        public static List<Airport> ListMainAirports { get { return listMainAirports; } }


        static FiltersManager()
        {

            listMainAirports = Airport.ListAirports.Where(s => s.Value["ICAO"].ToString().StartsWith("SB")).Select(s => Airport.GetAirportByIata(s.Key)).ToList();
            listMainAirports.Add(Airport.GetAirportByIata("SWUZ"));

            ListOldAirplanesDF = new List<AirplaneBasic>();
            ListFilterGroup = new List<GroupFilters>();

            GroupFilters listFilterBrazilArea = new GroupFilters();
            listFilterBrazilArea.Period = new TimeSpan(0, 0, 15);
            listFilterBrazilArea.Radar = Radar.GetRadar("BRA");
            listFilterBrazilArea.AddFilter((IFilter)Activator.CreateInstance(null, "TowerBotLib.Filters.FilterLogAll").Unwrap());
            listFilterBrazilArea.AddFilter(new FilterUnknowAirplanes(false, false));
            listFilterBrazilArea.AddFilter(new FilterWide());
            listFilterBrazilArea.AddFilter(new FilterRatification(false, false, false, false, true));
            listFilterBrazilArea.AddFilter(new FilterGoodEvening());
           
            listFilterBrazilArea.AddFilter(new FilterAlertAll());

            ListFilterGroup.Add(listFilterBrazilArea);

            GroupFilters listFilterBrasiliaArea = new GroupFilters();
            listFilterBrasiliaArea.Period = new TimeSpan(0, 0, 15);
            listFilterBrasiliaArea.Radar = Radar.GetRadar("BSB");
            listFilterBrasiliaArea.AddFilter(new FilterLogAll());
            listFilterBrasiliaArea.AddFilter(new FilterUnknowAirplanes(false, false));
            listFilterBrasiliaArea.AddFilter(new FilterWide());
            listFilterBrasiliaArea.AddFilter(new FilterBackingOrGo());
            listFilterBrasiliaArea.AddFilter(new FilterRatification(false, true, false, true, true));
            listFilterBrasiliaArea.AddFilter(new FilterMetarDF());
            listFilterBrasiliaArea.AddFilter(new FilterAlertAll());
            ListFilterGroup.Add(listFilterBrasiliaArea);

            //            GroupFilters listFilterCuritibaArea = new GroupFilters();
            //            listFilterCuritibaArea.Period = new TimeSpan(0, 0, 15);
            //            listFilterCuritibaArea.Radar = Radar.GetRadar("CWB");
            //            listFilterCuritibaArea.AddFilter(new FilterLogAll());
            //            listFilterCuritibaArea.AddFilter(new FilterUnknowAirplanes(false, true));
            //            listFilterCuritibaArea.AddFilter(new FilterWide());
            //            listFilterCuritibaArea.AddFilter(new FilterBackingOrGo());
            //            listFilterCuritibaArea.AddFilter(new FilterRatification(true, true, true, true, true));
            //            listFilterCuritibaArea.AddFilter(new FilterMetarCWB());
            //            //ListFilterGroup.Add(listFilterCuritibaArea);


            GroupFilters listFilterSAOArea = new GroupFilters();
            listFilterSAOArea.Period = new TimeSpan(0, 0, 15);
            listFilterSAOArea.Radar = Radar.GetRadar("SAO");
            listFilterSAOArea.AddFilter(new FilterLogAll());
            listFilterSAOArea.AddFilter(new FilterUnknowAirplanes(false, false));
            listFilterSAOArea.AddFilter(new FilterWide());
            listFilterSAOArea.AddFilter(new FilterBackingOrGo());
            listFilterSAOArea.AddFilter(new FilterRatification(true, true, true, true, true));
            listFilterBrasiliaArea.AddFilter(new FilterAlertAll());
            ListFilterGroup.Add(listFilterSAOArea);


            //            GroupFilters listFilterDeepRec = new GroupFilters();
            //            listFilterDeepRec.Period = new TimeSpan(0, 0, 25);
            //            //listFilterDeepRec.ListFilters = new List<IFilter>();
            //            listFilterDeepRec.AddFilter(new FilterDeepRecorder());
            //            ListFilterGroup.Add(listFilterDeepRec);


        }

        public static List<AlertFilter> GetAlerts(bool updateAll)
        {
            List<AlertFilter> listAlerts = new List<AlertFilter>();

#if DEBUG
            for (int i = 0; i < ListFilterGroup.Count; i++)
            {
                var filterGroup = ListFilterGroup[i];
#else
            Parallel.ForEach(ListFilterGroup, filterGroup => {
#endif

                if (filterGroup.TimeNext == null || filterGroup.TimeNext <= DateTime.Now || updateAll)
                {
                    List<AirplaneBasic> listAirplanes = null;
                    if (filterGroup.Radar != null)
                    {

                        listAirplanes = AirplanesData.GetAirplanes(filterGroup.Radar).Result;
                        var newAlerts = filterGroup.Run(listAirplanes);

                        var listToDelete = new List<AlertFilter>();


                        if (filterGroup.Radar.Name == "BRA")
                        {
                            var listOfAirports = new List<Airport>();

                            Airport airport = null;

                            if (newAlerts.Count > 0)
                            {
                                listOfAirports = Airport.ListAirports.Where(s => s.Value["ICAO"].ToString().StartsWith("SB")).Select(s => Airport.GetAirportByIata(s.Key)).ToList();                                
                            }


                            foreach (var item in newAlerts)
                            {

                                if (AlertFilter.ListOfRecentAlerts != null && AlertFilter.ListOfRecentAlerts.Any(a => a.ID == item.ID))
                                {
                                    listToDelete.Add(item);
                                    continue;
                                }

                                if (AlertFilter.ListOfRecentAlerts != null && AlertFilter.ListOfRecentAlerts.Where(w => w.TimeCreated > DateTime.Now.AddMinutes(-15) && w.TimeCreated < DateTime.Now.AddMinutes(-2)).Any(a => a.AirplaneID == item.AirplaneID && item.Icon == a.Icon))
                                {
                                    listToDelete.Add(item);
                                    continue;
                                }
                                
                            }
                        }

                        listToDelete.ForEach(item => newAlerts.Remove(item));

                        listAlerts.AddRange(newAlerts);

                    }


                    ListOldAirplanesDF = listAirplanes;

                    filterGroup.TimeNext = DateTime.Now + filterGroup.Period;


                }
#if DEBUG
            }
#else
            });
#endif





            return listAlerts;


        }

        public static void RefreshAll()
        {
            for (int i = 0; i < ListFilterGroup.Count; i++)
            {
                ListFilterGroup[i].Refresh();
            }
        }

        public static void AccessFilterCommandLine()
        {
            Console.WriteLine("Qual filtro você deseja acessar?\n");

            for (int i = 0; i < ListFilterGroup.Count; i++)
            {
                for (int j = 0; j < ListFilterGroup[i].ListFilters.Count; j++)
                {
                    Console.WriteLine("-{0} (ativo:{1}, em teste:{2})", ListFilterGroup[i].ListFilters[j].Name, ListFilterGroup[i].ListFilters[j].IsActive, ListFilterGroup[i].ListFilters[j].IsTesting);
                }
            }
            string comando = Console.ReadLine();
            IFilter selectedFilter = null;
            for (int i = 0; i < ListFilterGroup.Count; i++)
            {
                for (int j = 0; j < ListFilterGroup[i].ListFilters.Count; j++)
                {
                    if (ListFilterGroup[i].ListFilters[j].Name.ToLower().StartsWith(comando.ToLower()))
                    {
                        selectedFilter = ListFilterGroup[i].ListFilters[j];
                        selectedFilter.CommandLine();
                        break;
                    }

                }
            }

            if (selectedFilter == null)
            {
                Console.WriteLine("Filtro não encontrado.");
            }

        }

    }
}
