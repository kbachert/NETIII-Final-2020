using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace LogicLayer
{
    public interface IInventoryManager
    {
        string AddInventoryItem(InventoryItem item);

        bool AddVendorItem(string inventoryID, string vendorID);

        List<InventoryItem> GetAllInventoryItems();

        List<InventoryItem> GetInventoryItemsByActive(bool active = true);

        InventoryItem GetInventoryItemByID(string inventoryID);

        List<InventoryQuantity> GetInventoryQuantitiesBySaleItem(int saleItemID);

        List<InventoryItem> GetLowQuantityItems();

        List<InventoryItem> GetInventoryItemsByActiveAndQuantity(bool active, bool lowQuantity);

        bool EditInventoryItem(InventoryItem oldItem, InventoryItem newItem);

        bool RemoveInventoryItem(string inventoryID);

        void DeleteInventoryItemFromVendorItems(string inventoryID);

        bool SetInventoryItemActiveState(bool active, string inventoryID);
    }
}
