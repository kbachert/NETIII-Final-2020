using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace DataAccessLayer
{
    public interface IInventoryAccessor
    {
        string InsertInventoryItem(InventoryItem item);

        bool InsertVendorItem(string inventoryID, string vendorID);

        List<InventoryItem> SelectAllInventoryItems();

        List<InventoryItem> SelectInventoryItemsByActive(bool active);

        InventoryItem SelectInventoryItemByID(string inventoryID);

        List<InventoryQuantity> SelectInventoryQuantitiesBySaleItem(int saleItemID);

        List<InventoryItem> SelectLowQuantityItems();

        List<InventoryItem> SelectInventoryItemsByActiveAndQuantity(bool active, bool lowQuantity);

        int UpdateInventoryItem(InventoryItem oldItem, InventoryItem newItem);

        int DeactivateInventoryItem(string inventoryID);

        int ActivateInventoryItem(string inventoryID);

        void DeleteInventoryItemFromVendorItems(string inventoryID);

        int DeleteInventoryItem(string inventoryID);
    }
}
