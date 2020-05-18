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
    public class SaleItemAccessor : ISaleItemAccessor
    {
        public int ActivateSaleItem(int saleItemID)
        {
            int rows = 0;
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_reactivate_sale_item", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SaleItemID", saleItemID);

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

        public int DeactivateSaleItem(int saleItemID)
        {
            int rows = 0;
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_deactivate_sale_item", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SaleItemID", saleItemID);

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

        public int DeleteSaleItem(int saleItemID)
        {
            int rows = 0;
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_delete_sale_item", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SaleItemID", saleItemID);

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

        public void DeleteSaleItemInventoryBySaleItem(int saleItemID)
        {
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_delete_sale_item_inventory_by_sale_item", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@SaleItemID", SqlDbType.Int);

            // valies
            cmd.Parameters["@SaleItemID"].Value = saleItemID;

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

        public int InsertSaleItem(SaleItem item)
        {
            int saleItemID = 0;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_insert_sale_item", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@ItemName", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@ItemSize", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@Flavor", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@Price", SqlDbType.Money);

            // values
            cmd.Parameters["@ItemName"].Value = item.ItemName;
            if (item.ItemSize.Equals(""))
            {
                cmd.Parameters["@ItemSize"].Value = DBNull.Value;
            }
            else
            {
                cmd.Parameters["@ItemSize"].Value = item.ItemSize;
            }
            if (item.Flavor.Equals(""))
            {
                cmd.Parameters["@Flavor"].Value = DBNull.Value;
            }
            else
            {
                cmd.Parameters["@Flavor"].Value = item.Flavor;
            }
            cmd.Parameters["@Price"].Value = item.Price;

            try
            {
                conn.Open();
                saleItemID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return saleItemID;
        }

        public bool InsertSaleItemInventory(int saleItemID, string inventoryID, decimal quantity)
        {
            bool result = false;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_insert_sale_item_inventory", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@SaleItemID", SqlDbType.Int);
            cmd.Parameters.Add("@InventoryID", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@Quantity", SqlDbType.Decimal);

            //values
            cmd.Parameters["@SaleItemID"].Value = saleItemID;
            cmd.Parameters["@InventoryID"].Value = inventoryID;
            cmd.Parameters["@Quantity"].Value = quantity;

            // execute
            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                result = (rowsAffected == 1); // true if sale item inventory record created
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

        public SaleItem SelectSaleItemByID(int saleItemID)
        {
            SaleItem saleItem = new SaleItem();

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_select_sale_item_by_id", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@SaleItemID", SqlDbType.Int);
            cmd.Parameters["@SaleItemID"].Value = saleItemID;

            // execute
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        saleItem.SaleItemID = reader.GetInt32(0);
                        saleItem.ItemName = reader.GetString(1);
                        if (!reader.IsDBNull(2))
                        {
                            saleItem.ItemSize = reader.GetString(2);
                        }
                        if (!reader.IsDBNull(3))
                        {
                            saleItem.Flavor = reader.GetString(3);
                        }
                        saleItem.Price = Math.Round((Decimal)reader.GetSqlMoney(4), 2);
                        saleItem.Active = reader.GetBoolean(5);
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

            return saleItem;
        }

        public List<string> SelectSaleItemNames()
        {
            List<string> saleItemNames = new List<string>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_sale_item_names", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        saleItemNames.Add(reader.GetString(0));
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
            return saleItemNames;
        }

        public List<SaleItem> SelectSaleItemsByActive(bool active)
        {
            List<SaleItem> items = new List<SaleItem>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_sale_items_by_active", conn);
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
                        var item = new SaleItem();
                        item.SaleItemID = reader.GetInt32(0);
                        item.ItemName = reader.GetString(1);
                        if (!reader.IsDBNull(2))
                        {
                            item.ItemSize = reader.GetString(2);
                        }
                        if (!reader.IsDBNull(3))
                        {
                            item.Flavor = reader.GetString(3);
                        }
                        item.Price = Math.Round((Decimal)reader.GetSqlMoney(4), 2);
                        item.Active = reader.GetBoolean(5);

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

        public List<SaleItem> SelectSaleItemsByActiveAndName(bool active, string itemName)
        {
            List<SaleItem> items = new List<SaleItem>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_sale_items_by_active_and_name", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Active", SqlDbType.Bit);
            cmd.Parameters.Add("@ItemName", SqlDbType.NVarChar, 50);
            cmd.Parameters["@Active"].Value = active;
            cmd.Parameters["@ItemName"].Value = itemName;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var item = new SaleItem();
                        item.SaleItemID = reader.GetInt32(0);
                        item.ItemName = reader.GetString(1);
                        if (!reader.IsDBNull(2))
                        {
                            item.ItemSize = reader.GetString(2);
                        }
                        if (!reader.IsDBNull(3))
                        {
                            item.Flavor = reader.GetString(3);
                        }
                        item.Price = Math.Round((Decimal)reader.GetSqlMoney(4), 2);
                        item.Active = reader.GetBoolean(5);

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

        public int UpdateSaleItem(SaleItem oldItem, SaleItem newItem)
        {
            int rowsAffected = 0;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_update_sale_item", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@SaleItemID", SqlDbType.Int);
            cmd.Parameters.Add("@NewItemName", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@NewItemSize", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@NewFlavor", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@NewPrice", SqlDbType.Money);
            cmd.Parameters.Add("@NewActiveStatus", SqlDbType.Bit);
            cmd.Parameters.Add("@OldItemName", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@OldPrice", SqlDbType.Money);

            // values
            cmd.Parameters["@SaleItemID"].Value = oldItem.SaleItemID;
            cmd.Parameters["@NewItemName"].Value = newItem.ItemName;
            if (newItem.ItemSize.Equals(""))
            {
                cmd.Parameters["@NewItemSize"].Value = DBNull.Value;
            }
            else
            {
                cmd.Parameters["@NewItemSize"].Value = newItem.ItemSize;
            }
            if (newItem.Flavor.Equals(""))
            {
                cmd.Parameters["@NewFlavor"].Value = DBNull.Value;
            }
            else
            {
                cmd.Parameters["@NewFlavor"].Value = newItem.Flavor;
            }
            cmd.Parameters["@NewPrice"].Value = newItem.Price;
            cmd.Parameters["@NewActiveStatus"].Value = newItem.Active;
            cmd.Parameters["@OldItemName"].Value = oldItem.ItemName;
            cmd.Parameters["@OldPrice"].Value = oldItem.Price;

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
