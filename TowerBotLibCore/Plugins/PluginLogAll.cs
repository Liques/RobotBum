using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerBotFoundationCore;

namespace TowerBotLibCore.Plugins
{
    class PluginLogAll : IPlugin
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsTesting { get; set; }
        public Radar Radar { get; set; }


        public PluginLogAll()
        {
            Name = "LogAll";
            IsActive = true;
            IsTesting = false;

        }

        public List<Alert> Analyser(object parameter)
        {
            List<AirplaneBasic> listAirplanes = (List<AirplaneBasic>)parameter;

            List<Alert> listAlerts = new List<Alert>();

            try
            {

                if (IsActive)
                {

                }
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Plugin Log All DF");
            }

            return listAlerts;
        }




        public void CommandLine()
        {
            Console.WriteLine("---------------\nFiltro de fazer log de todos aviões do DF\n\n+Filtro ativo:" + this.IsActive + "\n\n---------------\n-disable\n-enable\n");
            string comando = Console.ReadLine();
            if (comando == "enable")
            {
                IsActive = true;
                Console.WriteLine("Ok");
            }
            else if (comando == "disable")
            {
                IsActive = false;
                Console.WriteLine("Ok");
            }
            else
            {
                Console.WriteLine("Comando do filtro não reconhecido.");
            }
        }
    }
}
