using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using RobotBumFoundationCore;

namespace RobotBumLibCore
{
    public enum PluginAlertType
    {
        NoData = -1,
        NoAlert = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Test = 4
    }

    public enum MessageType
    {
        General,
        Fixed
    }

    public enum RatificationType
    {
        NoRatification,
        Chart,
        FinalRunway,
        Orbit
    }

    public enum IconType
    {
        NoIcon = 0,
        Cruise = 1,
        Landing = 2,
        TakingOff = 3,
        Taxing = 4,
        Orbit = 5,
        Chart = 6,
        AirportWeather = 7,
        Runway = 8,
        TouchAndGo = 9,
        GoodNightAnnoucement = 8,
    }

    public class Alert
    {
        public static List<Alert> ListOfAlerts;
        public static List<Alert> ListOfRecentAlerts { get; set; }


        private string message = String.Empty;
        private string messageFixed = String.Empty;
        public string ID { get; set; }
        public DateTime TimeCreated { get; set; }
        
        public DateTime TimeToBeRemoved { get; set; }
        public string AirplaneID { get; set; }
        public IconType Icon { get; set; }

        public string AirplaneHexa { get; set; }
        public string AirplaneReg { get; set; }
        public string AirplaneFlight { get; set; }
        public string AirplaneFrom { get; set; }
        public string AirplaneTo { get; set; }


        private PluginAlertType alertType { get; set; }
        public PluginAlertType AlertType
        {
            get
            {
                return alertType;
            }
            set
            {
                alertType = value;

                #region falando ao avião qual tipo de alerta ele foi colocado
                if (this.Airplane != null)
                {
                    if (value == PluginAlertType.High ||
                        value == PluginAlertType.Medium && this.Airplane.LastAlertType != PluginAlertType.High ||
                        value == PluginAlertType.Low && this.Airplane.LastAlertType != PluginAlertType.Medium && this.Airplane.LastAlertType != PluginAlertType.High)

                        this.Airplane.LastAlertType = value;
                    
                }
                #endregion


            }
        }
        [IgnoreDataMemberAttribute]
        public string CustomMessage
        {
            set
            {
                message = value;
            }
            get
            {
                return message;
            }
        }
        [IgnoreDataMemberAttribute]
        public int Level { get; set; }
        [IgnoreDataMemberAttribute]
        public string Group { get; set; }
        public string Message
        {
            get
            {
                if (string.IsNullOrEmpty(message))
                    return messageFixed;
                else
                    return message;
            }
            set
            {
                messageFixed = value;
            }
        }
        [IgnoreDataMemberAttribute]

        public string Justify { get; set; }
        [IgnoreDataMemberAttribute]
        public AirplaneBasic Airplane { get; set; }
        [IgnoreDataMemberAttribute]
        public string PluginName { get; set; }
        [IgnoreDataMemberAttribute]
        public Radar Radar { get; set; }
        
        public string RadarName
        {
            get
            {
                return Radar.Name;
            }
            set
            {
                this.Radar = Radar.GetRadar(value);
            }
        }
        RatificationType RatificationType = RatificationType.NoRatification;

        static Alert()
        {
            try
            {
                string strJSONPath = String.Empty;

                var lastAlertsRaw = LoadFile(strJSONPath, "lastAlerts.json");
                Alert.ListOfAlerts = JsonConvert.DeserializeObject<List<Alert>>(lastAlertsRaw);

                //var teste = JsonConvert.DeserializeObject<List<Alert>>(lastAlertsRaw);

                if (Alert.ListOfAlerts == null)
                    Alert.ListOfAlerts = new List<Alert>();

                foreach (var item in Alert.ListOfAlerts)
                {


                    if (item.TimeToBeRemoved.Year < 2000)
                        item.TimeToBeRemoved = item.TimeCreated.AddDays(3);

                    item.Radar = Radar.GetRadar(item.Radar.Name);

                    if (item.Icon == IconType.Landing || item.Icon == IconType.TakingOff || item.Icon == IconType.Cruise)
                    {
                        // irplane.ID + "|" + airplane.Registration.Name + "|" + airplane.AircraftType.ICAO + "|" + ((int)airplane.Weight);
                        var objs = item.Message.Split('|');

                        if (objs.Length == 4)
                        {

                            var airplane = new AirplaneBasic();
                            airplane.ID = objs[0];
                            airplane.Registration = new AircraftRegistration(objs[1]);
                            airplane.AircraftType = AircraftType.GetAircraftType(objs[2]);
                            airplane.Weight = (AirplaneWeight)Convert.ToInt32(objs[3]);

                            item.Airplane = airplane;
                        }
                    }

                }

                if (Alert.ListOfAlerts != null)
                    Console.WriteLine("Messages rescued: {0}", Alert.ListOfAlerts.Count);

            }
            catch (Exception e)
            {
                throw new ArgumentException(@"lastAlerts.json");
            }
        }

        public Alert()
        {

            if (AirplaneHexa == null)
                AirplaneHexa = String.Empty;
            if (AirplaneReg == null)
                AirplaneReg = String.Empty;
            if (AirplaneFlight == null)
                AirplaneFlight = String.Empty;
            if (AirplaneFrom == null)
                AirplaneFrom = String.Empty;
            if (AirplaneTo == null)
                AirplaneTo = String.Empty;


            //    public string AirplaneHexa { get; set; }
            //public string AirplaneReg { get; set; }
            //public string AirplaneFlight { get; set; }
            //public string AirplaneFrom { get; set; }
            //public string AirplaneTo { get; set; }
        }

        public Alert(Radar radar, string Pluginname, string nameOrMessage, IconType iconType, MessageType messageType = MessageType.General)
        {
            ID = radar.Name + Pluginname + nameOrMessage.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace(",", "").Replace(".", "");
            ID = (ID.Length > 75) ? ID.Substring(0, 75) : ID;
            this.PluginName = Pluginname;
            this.Radar = radar;
            TimeCreated = DateTime.Now;
            TimeToBeRemoved = DateTime.Now.AddHours(1);
            this.Icon = iconType;
        }

        public Alert(Radar radar, string Pluginname, AirplaneBasic airplane, IconType iconType, MessageType messageType = MessageType.General, RatificationType ratificationType = RatificationType.NoRatification)
        {
            this.Airplane = airplane;
            this.PluginName = Pluginname;
            ID = radar.Name + Pluginname + Airplane.ID;
            ID = ID.Replace(" ", "").Replace("-", "");
            ID = (ID.Length > 25) ? ID.Substring(0, 24) : ID;

            if (ratificationType != RatificationType.NoRatification)
            {
                RatificationType = ratificationType;
                ID += "Ratification" + ratificationType.ToString();
            }

            this.Radar = radar;
            TimeCreated = DateTime.Now;
            TimeToBeRemoved = DateTime.Now.AddHours(1);
            this.Icon = iconType;
            TimeCreated = new DateTime(
                TimeCreated.Ticks - (TimeCreated.Ticks % TimeSpan.TicksPerSecond),
                TimeCreated.Kind
                ); ;

            AirplaneID = airplane.ID;

            this.AirplaneHexa = airplane.ID;
            this.AirplaneFlight = airplane.FlightName;
            this.AirplaneReg = airplane.Registration.Name;
            this.AirplaneFrom = airplane.From.ICAO;
            this.AirplaneTo = airplane.To.ICAO;

            SetMessage(messageType);
        }


        private static string LoadFile(string currentPath, string fileName)
        {

          bool exists = System.IO.Directory.Exists(currentPath);
           
           if (!exists && !String.IsNullOrEmpty(currentPath))
                System.IO.Directory.CreateDirectory(currentPath);

            if(!String.IsNullOrEmpty(currentPath))
                currentPath += @"/" + fileName;
            else
                currentPath += fileName;

            if (File.Exists(currentPath))
            {
              

            using (var stream = File.Open(currentPath, FileMode.Open))
            {
                var streamWriter = new StreamReader(stream);

                    return streamWriter.ReadToEnd();
            }

            //return File.ReadAllText(currentPath);

            }

            return String.Empty;
        }


        public void ReMakeID()
        {
            if (Airplane == null)
            {
                ID = this.Radar.Name + this.PluginName + this.Message.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace(",", "").Replace(".", "");
                ID = (ID.Length > 75) ? ID.Substring(0, 75) : ID;

            }
            else {
                ID = this.Radar.Name + this.PluginName + Airplane.ID;
                ID = ID.Replace(" ", "").Replace("-", "");
                ID = (ID.Length > 25) ? ID.Substring(0, 24) : ID;
            }
        }

        public override string ToString()
        {

            return this.Message + ";" + this.ID + ";" + this.AlertType.ToString() + ";" + this.Justify;

        }

        private void SetMessage(MessageType messageType)
        {
            if (messageType == MessageType.General)
            {
                var messageMaker = new MessageMaker(this.Airplane, this.Radar, this.RatificationType);
                if (!String.IsNullOrEmpty(messageMaker.Message))
                {
                    message = messageMaker.Message;
                }
            }
        }
    }


}
