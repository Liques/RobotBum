using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Web;

namespace TowerBotLib.Models
{
    [Table(Name = "Aircraft")]
    public class AlertModel
    {
        [Key]
        [Column(Name = "ID")]
        public int ID { get; set; }
        [Column(Name = "TimeCreation")]
        public string TimeCreation { get; set; }
        [Column(Name = "Icon")]
        public int Icon { get; set; }
        [Column(Name = "AlertType")]
        public string AlertType { get; set; }
        [Column(Name = "AirplaneID")]
        public string AirplaneID { get; set; }
        [Column(Name = "AlertID")]
        public string AlertID { get; set; }
        [Column(Name = "Message")]
        public string Message { get; set; }
        [Column(Name = "TimeToBeDeleted")]
        public string TimeToBeDeleted { get; set; }
        [Column(Name = "Airplane")]
        public string Airplane { get; set; }
    }
}
