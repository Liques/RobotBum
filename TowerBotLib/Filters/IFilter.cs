using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerBotLib.Filters
{
    public interface IFilter
    {
        bool IsActive { get; set; }
        bool IsTesting { get; set; }
        string Name { get; set; }
        Radar Radar { get; set; }
        List<AlertFilter> Analyser(object parameter);
        void CommandLine();
    }
}
