using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using DataObjects;

namespace LogicLayer
{
    public class InventoryManager : IInventoryManager
    {
        private IInventoryAccessor _inventoryAccessor;

        public InventoryManager()
        {
            _inventoryAccessor = new InventoryAccessor();
        }

        public InventoryManager(IInventoryAccessor inventoryAccessor)
        {
            _inventoryAccessor = inventoryAccessor;
        }

        public string AddInventoryItem(InventoryItem item)
        {
            string itemName = null;

            try
            {
                itemName = _inventoryAccessor.InsertInventoryItem(item);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Add Inventory Item Failed.", ex);
            }

            return itemName;
        }

        public bool AddVendorItem(string inventoryID, string vendorID)
        {
            bool result = false;

            try
            {
                result = _inventoryAccessor.InsertVendorItem(inventoryID, vendorID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Vendor Item Could Not Be Created", ex);
            }

            return result;
        }

        public void DeleteInventoryItemFromVendorItems(string inventoryID)
        {
            try
            {
                _inventoryAccessor.DeleteInventoryItemFromVendorItems(inventoryID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Delete Inventory Item From Vendor Items", ex);
            }
        }

        public bool EditInventoryItem(InventoryItem oldItem, InventoryItem newItem)
        {
            bool oneItemUpdated = false;

            try
            {
                oneItemUpdated = (1 == _inventoryAccessor.UpdateInventoryItem(oldItem, newItem));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Inventory Update Failed", ex);
            }

            return oneItemUpdated;
        }

        public List<InventoryItem> GetAllInventoryItems()
        {
            try
            {
                return _inventoryAccessor.SelectAllInventoryItems();
            }
            catch (Exception ex)
            {

                throw new ApplicationException("List Not Available", ex);
            }
        }

        public InventoryItem GetInventoryItemByID(string inventoryID)
        {
            try
            {
                return _inventoryAccessor.SelectInventoryItemByID(inventoryID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Find Specified Inventory Item", ex);
            }
        }

        public List<InventoryItem> GetInventoryItemsByActive(bool active = true)
        {
            try
            {
                return _inventoryAccessor.SelectInventoryItemsByActive(active);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Retrieve List of Inventory Items", ex);
            }
        }

        public List<InventoryItem> GetInventoryItemsByActiveAndQuantity(bool active, bool lowQuantity)
        {
            try
            {
                return _inventoryAccessor.SelectInventoryItemsByActiveAndQuantity(active, lowQuantity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Retrieve List of Inventory Items", ex);
            }
        }

        public List<InventoryQuantity> GetInventoryQuantitiesBySaleItem(int saleItemID)
        {
            try
            {
                return _inventoryAccessor.SelectInventoryQuantitiesBySaleItem(saleItemID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Retrieve List of Inventory Quantities", ex);
            }
        }

        public List<InventoryItem> GetLowQuantityItems()
        {
            try
            {
                return _inventoryAccessor.SelectLowQuantityItems();
            }
            catch (Exception ex)
            {

                throw new ApplicationException("List Not Available", ex);
            }
        }

        public bool RemoveInventoryItem(string inventoryID)
        {
            bool oneRowRemoved = false;

            try
            {
                oneRowRemoved = (1 == _inventoryAccessor.DeleteInventoryItem(inventoryID));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Inventory Item Could Not Be Removed", ex);
            }

            return oneRowRemoved;
        }

        public bool SetInventoryItemActiveState(bool active, string inventoryID)
        {
            bool result = false;

            try
            {
                if (active)
                {
                    result = (1 == _inventoryAccessor.ActivateInventoryItem(inventoryID));
                }
                else
                {
                    result = (1 == _inventoryAccessor.DeactivateInventoryItem(inventoryID));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Active Status Change Failed", ex);
            }

            return result;
        }
    }
}
