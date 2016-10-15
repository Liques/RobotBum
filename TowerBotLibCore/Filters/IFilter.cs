using System.Collections.Generic;

namespace TowerBotLibCore.Filters
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
