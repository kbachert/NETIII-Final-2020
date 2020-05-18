using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class SaleItem
    {
        public int SaleItemID { get; set; }

        [Required]
        //[MaxLength(35, ErrorMessage = "Item Name can be no longer than 35 characters!")]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        //[MaxLength(35, ErrorMessage = "Item Size can be no longer than 35 characters!")]
        [Display(Name = "Item Size")]
        public string ItemSize { get; set; }

        //[MaxLength(35, ErrorMessage = "Flavor can be no longer than 35 characters!")]
        public string Flavor { get; set; }

        [Required]
        //[MaxLength(9, ErrorMessage = "Price can be no longer than 9 characters!")]
        public decimal Price { get; set; }

        public bool Active { get; set; }
    }
}
