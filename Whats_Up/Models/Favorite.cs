using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Whats_Up.Models
{
    public class Favorite
    {
        //Making changes baby! More changes baby!!
        [Key]
        public int FavID { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Category { get; set; }
    }
}