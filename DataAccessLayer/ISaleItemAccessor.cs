using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace DataAccessLayer
{
    public interface ISaleItemAccessor
    {
        int InsertSaleItem(SaleItem item);

        bool InsertSaleItemInventory(int saleItemID, string inventoryID, decimal quantity);

        SaleItem SelectSaleItemByID(int saleItemID);

        List<SaleItem> SelectSaleItemsByActive(bool active);

        List<SaleItem> SelectSaleItemsByActiveAndName(bool active, string itemName);

        List<string> SelectSaleItemNames();

        int UpdateSaleItem(SaleItem oldItem, SaleItem newItem);

        void DeleteSaleItemInventoryBySaleItem(int saleItemID);

        int DeleteSaleItem(int saleItemID);

        int DeactivateSaleItem(int saleItemID);

        int ActivateSaleItem(int saleItemID);
    }
}
