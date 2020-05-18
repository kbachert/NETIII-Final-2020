using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using DataObjects;

namespace LogicLayer
{
    public class SaleItemManager : ISaleItemManager
    {
        private ISaleItemAccessor _saleItemAccessor;

        public SaleItemManager()
        {
            _saleItemAccessor = new SaleItemAccessor();
        }

        public SaleItemManager(ISaleItemAccessor saleItemAccessor)
        {
            _saleItemAccessor = saleItemAccessor;
        }

        public int AddSaleItem(SaleItem item)
        {
            int saleItemID = 0;

            try
            {
                saleItemID = _saleItemAccessor.InsertSaleItem(item);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Add Sale Item Failed.", ex);
            }

            return saleItemID;
        }

        public bool AddSaleItemInventory(int saleItemID, string inventoryID, decimal quantity)
        {
            bool result = false;

            try
            {
                result = _saleItemAccessor.InsertSaleItemInventory(saleItemID, inventoryID, quantity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Sale Item Inventory Could Not Be Created", ex);
            }

            return result;
        }

        public void DeleteSaleItemFromSaleItemInventory(int saleItemID)
        {
            try
            {
                _saleItemAccessor.DeleteSaleItemInventoryBySaleItem(saleItemID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Delete Employee's Roles", ex);
            }
        }

        public bool EditSaleItem(SaleItem oldItem, SaleItem newItem)
        {
            bool oneItemUpdated = false;

            try
            {
                oneItemUpdated = (1 == _saleItemAccessor.UpdateSaleItem(oldItem, newItem));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Sale Item Update Failed", ex);
            }

            return oneItemUpdated;
        }

        public SaleItem GetSaleItemByID(int saleItemID)
        {
            try
            {
                return _saleItemAccessor.SelectSaleItemByID(saleItemID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Retrieve Sale Item", ex);
            }
        }

        public List<string> GetSaleItemNames()
        {
            try
            {
                return _saleItemAccessor.SelectSaleItemNames();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Retrieve List of Sale Item Types", ex);
            }
        }

        public List<SaleItem> GetSaleItemsByActive(bool active = true)
        {
            try
            {
                return _saleItemAccessor.SelectSaleItemsByActive(active);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Retrieve List of Sale Items", ex);
            }
        }

        public List<SaleItem> GetSaleItemsByActiveAndName(bool active, string itemName)
        {
            try
            {
                return _saleItemAccessor.SelectSaleItemsByActiveAndName(active, itemName);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Retrieve List of Sale Items", ex);
            }
        }

        public bool RemoveSaleItem(int saleItemID)
        {
            bool oneRowRemoved = false;

            try
            {
                oneRowRemoved = (1 == _saleItemAccessor.DeleteSaleItem(saleItemID));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Sale Item Could Not Be Removed", ex);
            }

            return oneRowRemoved;
        }

        public bool SetSaleItemActiveState(bool active, int saleItemID)
        {
            bool result = false;

            try
            {
                if (active)
                {
                    result = (1 == _saleItemAccessor.ActivateSaleItem(saleItemID));
                }
                else
                {
                    result = (1 == _saleItemAccessor.DeactivateSaleItem(saleItemID));
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
