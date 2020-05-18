using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace LogicLayer
{
    public interface ISaleItemManager
    {
        int AddSaleItem(SaleItem item);

        bool AddSaleItemInventory(int saleItemID, string inventoryID, decimal quantity);

        SaleItem GetSaleItemByID(int saleItemID);

        List<SaleItem> GetSaleItemsByActive(bool active = true);

        List<SaleItem> GetSaleItemsByActiveAndName(bool active, string itemName);

        List<string> GetSaleItemNames();

        bool EditSaleItem(SaleItem oldItem, SaleItem newItem);

        bool RemoveSaleItem(int saleItemID);

        void DeleteSaleItemFromSaleItemInventory(int saleItemID);

        bool SetSaleItemActiveState(bool active, int saleItemID);
    }
}
