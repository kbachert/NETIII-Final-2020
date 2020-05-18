using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace DataAccessLayer
{
    public class InventoryAccessor : IInventoryAccessor
    {
        public int ActivateInventoryItem(string inventoryID)
        {
            int rows = 0;
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_reactivate_inventory_item", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@InventoryID", inventoryID);

            try
            {
                conn.Open();
                rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return rows;
        }

        public int DeactivateInventoryItem(string inventoryID)
        {
            int rows = 0;
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_deactivate_inventory_item", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@InventoryID", inventoryID);

            try
            {
                conn.Open();
                rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return rows;
        }

        public int DeleteInventoryItem(string inventoryID)
        {
            int rows = 0;
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_delete_inventory_item", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@InventoryID", inventoryID);

            try
            {
                conn.Open();
                rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return rows;
        }

        public void DeleteInventoryItemFromVendorItems(string inventoryID)
        {
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_delete_inventory_item_from_vendor_items", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters and values
            cmd.Parameters.Add("@InventoryID", SqlDbType.NVarChar, 50);
            cmd.Parameters["@InventoryID"].Value = inventoryID;

            // execute
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        public string InsertInventoryItem(InventoryItem item)
        {
            string inventoryID = null;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_insert_inventory_item", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@InventoryID", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@PurchaseUnit", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@SaleUnit", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@SaleUnitsPerPurchaseUnit", SqlDbType.Decimal);
            cmd.Parameters.Add("@QuantityOnHand", SqlDbType.Decimal);
            cmd.Parameters.Add("@ReorderLevel", SqlDbType.Decimal);

            // values
            cmd.Parameters["@InventoryID"].Value = item.ItemName;
            cmd.Parameters["@PurchaseUnit"].Value = item.PurchaseUnit;
            cmd.Parameters["@SaleUnit"].Value = item.SaleUnit;
            cmd.Parameters["@SaleUnitsPerPurchaseUnit"].Value = item.SaleUnitsPerPurchaseUnit;
            cmd.Parameters["@QuantityOnHand"].Value = item.QuantityOnHand;
            cmd.Parameters["@ReorderLevel"].Value = item.ReorderLevel;

            try
            {
                conn.Open();
                inventoryID = Convert.ToString(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return inventoryID;
        }

        public bool InsertVendorItem(string inventoryID, string vendorID)
        {
            bool result = false;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_insert_vendor_item", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@InventoryID", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@VendorID", SqlDbType.NVarChar, 50);

            //values
            cmd.Parameters["@InventoryID"].Value = inventoryID;
            cmd.Parameters["@VendorID"].Value = vendorID;

            // execute
            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                result = (rowsAffected == 1); // true if vendor item created
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public List<InventoryItem> SelectAllInventoryItems()
        {
            List<InventoryItem> items = new List<InventoryItem>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_all_inventory_items", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var item = new InventoryItem();
                        item.ItemName = reader.GetString(0);
                        item.PurchaseUnit = reader.GetString(1);
                        item.SaleUnit = reader.GetString(2);
                        item.SaleUnitsPerPurchaseUnit = reader.GetDecimal(3);
                        item.QuantityOnHand = reader.GetDecimal(4);
                        item.ReorderLevel = reader.GetDecimal(5);

                        items.Add(item);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return items;
        }

        public InventoryItem SelectInventoryItemByID(string inventoryID)
        {
            InventoryItem item = null;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_inventory_item_by_id", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@InventoryID", SqlDbType.NVarChar, 50);
            cmd.Parameters["@InventoryID"].Value = inventoryID;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    {
                        item = new InventoryItem();
                        item.ItemName = reader.GetString(0);
                        item.PurchaseUnit = reader.GetString(1);
                        item.SaleUnit = reader.GetString(2);
                        item.SaleUnitsPerPurchaseUnit = reader.GetDecimal(3);
                        item.QuantityOnHand = reader.GetDecimal(4);
                        item.ReorderLevel = reader.GetDecimal(5);
                        item.Active = reader.GetBoolean(6);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conn.Close();
            }
            return item;
        }

        public List<InventoryItem> SelectInventoryItemsByActive(bool active)
        {
            List<InventoryItem> items = new List<InventoryItem>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_inventory_items_by_active", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Active", SqlDbType.Bit);
            cmd.Parameters["@Active"].Value = active;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var item = new InventoryItem();
                        item.ItemName = reader.GetString(0);
                        item.PurchaseUnit = reader.GetString(1);
                        item.SaleUnit = reader.GetString(2);
                        item.SaleUnitsPerPurchaseUnit = reader.GetDecimal(3);
                        item.QuantityOnHand = reader.GetDecimal(4);
                        item.ReorderLevel = reader.GetDecimal(5);
                        item.Active = reader.GetBoolean(6);

                        items.Add(item);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conn.Close();
            }
            return items;
        }

        public List<InventoryItem> SelectInventoryItemsByActiveAndQuantity(bool active, bool lowQuantity)
        {
            List<InventoryItem> items = new List<InventoryItem>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_inventory_items_by_active_and_quantity", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Active", SqlDbType.Bit);
            cmd.Parameters.Add("@LowQuantity", SqlDbType.Bit);
            cmd.Parameters["@Active"].Value = active;
            cmd.Parameters["@LowQuantity"].Value = lowQuantity;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var item = new InventoryItem();
                        item.ItemName = reader.GetString(0);
                        item.PurchaseUnit = reader.GetString(1);
                        item.SaleUnit = reader.GetString(2);
                        item.SaleUnitsPerPurchaseUnit = reader.GetDecimal(3);
                        item.QuantityOnHand = reader.GetDecimal(4);
                        item.ReorderLevel = reader.GetDecimal(5);
                        item.Active = reader.GetBoolean(6);

                        items.Add(item);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conn.Close();
            }
            return items;
        }

        public List<InventoryQuantity> SelectInventoryQuantitiesBySaleItem(int saleItemID)
        {
            List<InventoryQuantity> saleItemInventory = new List<InventoryQuantity>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_inventory_quantities_by_sale_item", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@SaleItemID", SqlDbType.Int);
            cmd.Parameters["@SaleItemID"].Value = saleItemID;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var item = new InventoryQuantity();
                        item.InventoryItemName = reader.GetString(0);
                        item.Quantity = reader.GetDecimal(1);

                        saleItemInventory.Add(item);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conn.Close();
            }
            return saleItemInventory;
        }

            public List<InventoryItem> SelectLowQuantityItems()
        {
            List<InventoryItem> items = new List<InventoryItem>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_low_quantity_inventory_items", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var item = new InventoryItem();
                        item.ItemName = reader.GetString(0);
                        item.PurchaseUnit = reader.GetString(1);
                        item.SaleUnit = reader.GetString(2);
                        item.SaleUnitsPerPurchaseUnit = reader.GetDecimal(3);
                        item.QuantityOnHand = reader.GetDecimal(4);
                        item.ReorderLevel = reader.GetDecimal(5);

                        items.Add(item);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return items;
        }


        public int UpdateInventoryItem(InventoryItem oldItem, InventoryItem newItem)
        {
            int rowsAffected = 0;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_update_inventory_item", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@NewItemName", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@NewPurchaseUnit", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@NewSaleUnit", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@NewSaleUnitsPerPurchaseUnit", SqlDbType.Decimal);
            cmd.Parameters.Add("@NewQuantityOnHand", SqlDbType.Decimal);
            cmd.Parameters.Add("@NewReorderLevel", SqlDbType.Decimal);
            cmd.Parameters.Add("@NewActiveStatus", SqlDbType.Bit);
            cmd.Parameters.Add("@OldItemName", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@OldPurchaseUnit", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@OldSaleUnit", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@OldSaleUnitsPerPurchaseUnit", SqlDbType.Decimal);
            cmd.Parameters.Add("@OldQuantityOnHand", SqlDbType.Decimal);
            cmd.Parameters.Add("@OldReorderLevel", SqlDbType.Decimal);

            // values
            cmd.Parameters["@NewItemName"].Value = newItem.ItemName;
            cmd.Parameters["@NewPurchaseUnit"].Value = newItem.PurchaseUnit;
            cmd.Parameters["@NewSaleUnit"].Value = newItem.SaleUnit;
            cmd.Parameters["@NewSaleUnitsPerPurchaseUnit"].Value = newItem.SaleUnitsPerPurchaseUnit;
            cmd.Parameters["@NewQuantityOnHand"].Value = newItem.QuantityOnHand;
            cmd.Parameters["@NewReorderLevel"].Value = newItem.ReorderLevel;
            cmd.Parameters["@NewActiveStatus"].Value = newItem.Active;
            cmd.Parameters["@OldItemName"].Value = oldItem.ItemName;
            cmd.Parameters["@OldPurchaseUnit"].Value = oldItem.PurchaseUnit;
            cmd.Parameters["@OldSaleUnit"].Value = oldItem.SaleUnit;
            cmd.Parameters["@OldSaleUnitsPerPurchaseUnit"].Value = oldItem.SaleUnitsPerPurchaseUnit;
            cmd.Parameters["@OldQuantityOnHand"].Value = oldItem.QuantityOnHand;
            cmd.Parameters["@OldReorderLevel"].Value = oldItem.ReorderLevel;

            // execute
            try
            {
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
            return rowsAffected;
        }
    }
}
