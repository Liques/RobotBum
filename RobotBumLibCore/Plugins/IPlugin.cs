using System.Collections.Generic;

namespace RobotBumLibCore.Plugins
{
    public interface IPlugin
    {
        bool IsActive { get; set; }
        bool IsTesting { get; set; }
        string Name { get; set; }
        Radar Radar { get; set; }
        List<Alert> Analyser(object parameter);
        void CommandLine();
    }
}
