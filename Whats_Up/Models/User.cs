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
        [RegularExpression(@"^[0-9]{2,5} [A-Z][a-z]{1,15}(Ave|St|Ct|Blvd|Rd|Way|Ln|Dr|Ter|Pl|Ct)$")]
        public string Email { get; set; }

        public string addressLine { get; set; }

        
        [RegularExpression(@"^[0 - 9]{5}$")]
        public string postalCode { get; set; }
    }
}