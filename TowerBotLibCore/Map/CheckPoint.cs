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
    public class CheckPoint
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Region { get; set; }
        [JsonIgnoreAttribute]
        private static List<CheckPoint> listCheckPoints { get; set; }
        [JsonIgnoreAttribute]
        public static List<CheckPoint> ListCheckPoints
        {
            get
            {
                if (listCheckPoints == null)
                    LoadCheckPoints();

                return listCheckPoints;
            }
        }

        public static CheckPoint GetCheckPoint(string name)
        {
            return ListCheckPoints.Where(s => s.Name == name).FirstOrDefault();
        }

        public static void LoadCheckPoints()
        {
            try
            {
                listCheckPoints = new List<CheckPoint>();

                StreamReader file = File.OpenText(MultiOSFileSupport.ResourcesFolder + "checkpoints.json");

                StringBuilder jsonstring = new StringBuilder();

                while (file.Peek() >= 0)
                {

                    jsonstring.Append(file.ReadLine());
                }

                var listCountires = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, string>>>(jsonstring.ToString());

                foreach (var item in listCountires)
                {
                    listCheckPoints.Add(new CheckPoint()
                    {
                        Name = item.Key,
                        Latitude = Convert.ToDouble(item.Value["Latitude"]),
                        Longitude = Convert.ToDouble(item.Value["Longitude"]),
                        Region = item.Value["Region"],
                    });
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException(@"\Resources\checkpoints.jsonr");
            }

        }

        public override string ToString()
        {
            return this.Name;
        }


    }
}
