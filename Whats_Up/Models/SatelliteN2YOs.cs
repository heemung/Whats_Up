//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Whats_Up.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class SatelliteN2YOs
    {
        
        public int ID { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public int TransactionsCount { get; set; }
        [Required]
        public int SatCount { get; set; }

        public Nullable<int> SatId { get; set; }
        public string SatName { get; set; }
        public string Designator { get; set; }
        public string LaunchDate { get; set; }
        public Nullable<double> SatLat { get; set; }
        public Nullable<double> SatLng { get; set; }
        public Nullable<double> SatAlt { get; set; }
        public string AtTime { get; set; }
    }
}
