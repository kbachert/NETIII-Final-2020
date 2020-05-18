using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class InventoryItem
    {
        [Required]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [Required]
        [Display(Name = "Purchase Unit")]
        public string PurchaseUnit { get; set; }

        [Required]
        [Display(Name = "Sale Unit")]
        public string SaleUnit { get; set; }

        [Required]
        [Display(Name = "Sale Units Per Purchase Unit")]
        public decimal SaleUnitsPerPurchaseUnit { get; set; }

        [Required]
        [Display(Name = "Quantity On Hand (Sale Units)")]
        public decimal QuantityOnHand { get; set; }

        [Display(Name = "Reorder Level")]
        public decimal ReorderLevel { get; set; }

        public bool Active { get; set; }

        public List<string> Vendors { get; set; }
    }
}
