using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerBotFoundationCore;

namespace TowerBotLibCore.Map
{
    public enum ChartType
    {
        Star,
        SID
    }

    public class Chart
    {
        public string Name { get; set; }
        public List<CheckPoint> Doors { get; set; }
        public List<CheckPoint> CheckPoints { get; set; }
        public string Region { get; set; }
        public ChartType ChartType { get; set; }

        [JsonIgnoreAttribute]
        private static List<Chart> listCharts { get; set; }
        [JsonIgnoreAttribute]
        public static List<Chart> ListCharts
        {
            get
            {
                if (listCharts == null)
                    LoadCharts();

                return listCharts;
            }
        }

        public static Chart GetChart(string name)
        {
            return ListCharts.Where(s => s.Name == name).FirstOrDefault();
        }

        public static List<Chart> GetChartsByRegion(string region)
        {
            return ListCharts.Where(s => s.Region == region).ToList();
        }

        public static void LoadCharts()
        {
            try
            {
                listCharts = new List<Chart>();

                StreamReader file = File.OpenText(MultiOSFileSupport.ResourcesFolder + "charts.json");

                StringBuilder jsonstring = new StringBuilder();

                while (file.Peek() >= 0)
                {

                    jsonstring.Append(file.ReadLine());
                }

                var listCountires = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, string>>>(jsonstring.ToString());

                foreach (var item in listCountires)
                {
                    var chart = new Chart()
                    {
                        Name = item.Key,
                        Region = item.Value["Region"],
                        ChartType = (item.Value["Type"] == "STAR") ? ChartType.Star : ChartType.SID,
                        CheckPoints = new List<CheckPoint>(),
                        Doors = new List<CheckPoint>()
                    };

                    List<string> doors = item.Value["Doors"].Split(';').ToList();
                    List<string> checkpoints = item.Value["CheckPoints"].Split(';').ToList();

                    for (int i = 0; i < doors.Count; i++)
                    {
                        var chk = CheckPoint.GetCheckPoint(doors[i]);
                        if (chk != null)
                            chart.Doors.Add(chk);
                    }

                    for (int i = 0; i < checkpoints.Count; i++)
                    {
                        var chk = CheckPoint.GetCheckPoint(checkpoints[i]);
                        if (chk != null)
                            chart.CheckPoints.Add(chk);
                    }

                    listCharts.Add(chart);
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException(MultiOSFileSupport.ResourcesFolder + "charts.json");
            }

        }

        public bool IsFollowingChart(double longitude, double latitude, double direction)
        {
            return IsFollowingChart(longitude, latitude, direction, true);
        }

        public bool IsFollowingChart(double longitude, double latitude)
        {
            return IsFollowingChart(longitude, latitude, 0, false);
        }

        private bool IsFollowingChart(double longitude, double latitude, double direction, bool directionNeeded)
        {
            bool isFollowing = false;

            // Connect to the "entrances" of the chart
            if (this.Doors.Count >= 1 && this.CheckPoints.Count >= 1)
            {
                for (int i = 0; i < this.Doors.Count; i++)
                {
                    var firstPoint = this.Doors[i];
                    var secondPoint = this.CheckPoints[0];

                    if (directionNeeded)
                        isFollowing = MapMathHelper.IsInsideAngle(longitude, latitude, direction, firstPoint, secondPoint);
                    else
                        isFollowing = MapMathHelper.IsInsideAngle(longitude, latitude, firstPoint, secondPoint);


                    if (isFollowing)
                        break;
                }
            }

            // Connect to the checkpoints
            if (!isFollowing)
            {
                if (this.CheckPoints.Count >= 2)
                {
                    for (int i = 0; i < this.CheckPoints.Count - 1; i++)
                    {
                        var firstPoint = this.CheckPoints[i];
                        var secondPoint = this.CheckPoints[i + 1];

                        if (directionNeeded)
                            isFollowing = MapMathHelper.IsInsideAngle(longitude, latitude, direction, firstPoint, secondPoint);
                        else
                            isFollowing = MapMathHelper.IsInsideAngle(longitude, latitude, firstPoint, secondPoint);

                        if (isFollowing)
                            break;
                    }
                }
            }

            return isFollowing;
        }



        public override string ToString()
        {
            return this.ChartType.ToString() + " " + this.Name;
        }


    }
}
