using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations; //this is needed to add the boxes for the requirments & regex

namespace Whats_Up.Models
{
    public class User
    {

        [Key]
        [RegularExpression(@"([A-Za-z0-9]{3,})@([A-Za-z0-9]{3,})\.([A-Za-z0-9]{1,10})")]
        public string Email { get; set; }

        public string AddressLine { get; set; }

        

    }
}