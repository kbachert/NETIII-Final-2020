using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class Vendor
    {
        [AllowHtml]
        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; }

        [StringLength(11)]
        [Display(Name = "Vendor Phone")]
        public string VendorPhone { get; set; }

        public bool Active { get; set; }
    }
}
