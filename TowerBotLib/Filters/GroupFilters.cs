using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerBotLib.Filters
{
    public class GroupFilters
    {
        public string Name { get; set; }
        private List<IFilter> listFilters { get; set; }
        public List<IFilter> ListFilters
        {
            get
            {
                return listFilters;
            }
        }
        public DateTime TimeNext { get; set; }
        public TimeSpan Period { get; set; }
        private List<AlertFilter> listOldAlerts = new List<AlertFilter>();
        public Radar Radar { get; set; }

        public List<AlertFilter> Run(object parameter = null)
        {

            List<AlertFilter> listAlerts = new List<AlertFilter>();

            try
            {

                for (int i = 0; i < ListFilters.Count; i++)
                {
                    listAlerts.AddRange(ListFilters[i].Analyser(parameter));
                }

                // Verificar se algum alerta antigo passou da data de validade e remove-lo.
                if (listAlerts.Count > 0)
                {
                    List<AlertFilter> listOldBeyondValidationAlerts = listOldAlerts.Where(s => s.TimeToBeDeleted <= DateTime.Now).ToList();
                    for (int i = 0; i < listOldBeyondValidationAlerts.Count; i++)
                    {
                        listOldAlerts.Remove(listOldBeyondValidationAlerts[i]);
                    }
                }

                // Verificar se já existe algum alert igual emitido.
                List<AlertFilter> listAlertLessThenOneHour = listOldAlerts;
                if (Radar.Name == "BRA")
                    listAlertLessThenOneHour = listOldAlerts.Where(s => s.TimeCreated > DateTime.Now.AddHours(-1)).ToList();

                for (int i = 0; i < listAlertLessThenOneHour.Count; i++)
                {
                    var alertEqual = listAlerts.Where(s => s.ID == listAlertLessThenOneHour[i].ID && s.AlertType == listAlertLessThenOneHour[i].AlertType).ToList().LastOrDefault();
                    if (alertEqual != null)
                    {
                        if (alertEqual.Level <= listAlertLessThenOneHour[i].Level)
                        {
                            listAlerts.Remove(alertEqual);
                        }
                    }

                    // Verificar se já existe algum alert dos mesmo grupo
                    if (!String.IsNullOrEmpty(listAlertLessThenOneHour[i].Group))
                    {
                        var alertSameGroup = listAlerts.Where(s => s.ID != listAlertLessThenOneHour[i].ID && s.Group == listAlertLessThenOneHour[i].Group).ToList();
                        for (int j = 0; j < alertSameGroup.Count; j++)
                        {
                            listAlertLessThenOneHour[i].Group = String.Empty;
                            listAlertLessThenOneHour[i].TimeToBeDeleted = listAlertLessThenOneHour[i].TimeCreated.AddDays(1);
                            listAlertLessThenOneHour[i].ID += listAlertLessThenOneHour[i].TimeCreated.ToString("ddMMyyyyhhmm");

                        }
                    }

                }


                listOldAlerts.AddRange(listAlerts);
            }
            catch (Exception e)
            {
                ErrorManager.ThrowError(e, "Group Filters");
            }

            return listAlerts;

        }

        public void Refresh()
        {
            TimeNext = new DateTime(1988, 4, 1);
            listOldAlerts = new List<AlertFilter>();
        }

        public void AddFilter(IFilter filter)
        {
            if (listFilters == null)
                listFilters = new List<IFilter>();

            filter.Radar = this.Radar;
            listFilters.Add(filter);
        }

    }
}
