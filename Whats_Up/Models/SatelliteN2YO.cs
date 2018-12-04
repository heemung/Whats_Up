using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Whats_Up.Models
{
    public class SatelliteN2YO
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public int TransactionsCount { get; set; }
        [Required]
        public int SatCount { get; set; }

        public int? SatId { get; set; }
        public string SatName { get; set; }
        public string Designator { get; set; }
        public string LaunchDate { get; set; }
        public int? SatLat { get; set; }
        public int? SatLng { get; set; }
        public int? SatAlt { get; set; }
        public string AtTime{get; set;}

    }
}