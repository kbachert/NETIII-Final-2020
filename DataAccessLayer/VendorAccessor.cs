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
    public class VendorAccessor : IVendorAccessor
    {
        public int ActivateVendor(string vendorID)
        {
            int rows = 0;
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_reactivate_vendor", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@VendorID", vendorID);

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

        public int DeactivateVendor(string vendorID)
        {
            int rows = 0;
            var conn = DBConnection.GetConnection();
            var cmd1 = new SqlCommand("sp_deactivate_vendor", conn);
            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Parameters.AddWithValue("@VendorID", vendorID);

            try
            {
                conn.Open();
                rows = cmd1.ExecuteNonQuery();

                //If deactivation was successful, delete references to vendor in vendorItem table
                var cmd2 = new SqlCommand("sp_delete_vendor_from_vendor_items", conn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.AddWithValue("@VendorID", vendorID);

                try
                {
                    cmd2.ExecuteNonQuery(); 
                }
                catch (Exception ex)
                {
                    throw ex;
                }
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

        public int DeleteVendor(string vendorID)
        {
            int rows = 0;
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_delete_vendor", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@VendorID", vendorID);

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

        public bool InsertVendor(string vendorName, string phoneNumber)
        {
            bool result = false;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_insert_vendor", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@VendorID", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@VendorPhone", SqlDbType.NVarChar, 11);

            // values
            cmd.Parameters["@VendorID"].Value = vendorName;
            cmd.Parameters["@VendorPhone"].Value = phoneNumber;

            // execute
            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                result = (rowsAffected == 1); // true if vendor created
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

        public List<string> SelectActiveVendorNames()
        {
            List<string> vendorNames = new List<string>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_active_vendor_names", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        vendorNames.Add(reader.GetString(0));
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
            return vendorNames;
        }

        public List<Vendor> SelectAllVendors()
        {
            List<Vendor> vendors = new List<Vendor>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_all_vendors", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var vendor = new Vendor();
                        vendor.VendorName = reader.GetString(0);
                        vendor.VendorPhone = reader.GetString(1);
                        vendor.Active = reader.GetBoolean(2);

                        vendors.Add(vendor);
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
            return vendors;
        }

        public string SelectPreferredVendor(string inventoryID)
        {
            string vendorName = null;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_select_preferred_vendor_by_inventory_item", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@InventoryID", SqlDbType.NVarChar, 50);
            cmd.Parameters["@InventoryID"].Value = inventoryID;

            // execute
            try
            {
                conn.Open();
                vendorName = (string)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return vendorName;
        }

        public Vendor SelectVendorByName(string vendorName)
        {
            Vendor vendor = new Vendor();

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_select_vendor_by_name", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@VendorID", SqlDbType.NVarChar, 50);
            cmd.Parameters["@VendorID"].Value = vendorName;

            // execute
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        vendor.VendorName = reader.GetString(0);
                        vendor.VendorPhone = reader.GetString(1);
                        vendor.Active = reader.GetBoolean(2);
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

            return vendor;
        }

        public List<Vendor> SelectVendorsByActive(bool active)
        {
            List<Vendor> vendors = new List<Vendor>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_vendors_by_active", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
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
                        var vendor = new Vendor();
                        vendor.VendorName = reader.GetString(0);
                        vendor.VendorPhone = reader.GetString(1);
                        vendor.Active = reader.GetBoolean(2);

                        vendors.Add(vendor);
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
            return vendors;
        }

        public List<string> SelectVendorsByInventoryID(string inventoryID)
        {
            List<string> vendorNames = new List<string>();

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_select_vendor_names_by_inventory_item", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@InventoryID", SqlDbType.NVarChar, 50);
            cmd.Parameters["@InventoryID"].Value = inventoryID;

            // execute
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        vendorNames.Add(reader.GetString(0));
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

            return vendorNames;
        }

        public int SetPreferredVendor(string inventoryID, string vendorName)
        {
            int rowsAffected = 0;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_set_preferred_vendor", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@InventoryID", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@VendorID", SqlDbType.NVarChar, 50);

            // values
            cmd.Parameters["@InventoryID"].Value = inventoryID;
            cmd.Parameters["@VendorID"].Value = vendorName;

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

        public bool UpdateVendor(string newVendorName, string newVendorPhone, bool newActiveStatus,
            string oldVendorName, string oldVendorPhone)
        {
            bool result = false;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_update_vendor", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@NewVendorID", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@NewVendorPhone", SqlDbType.NVarChar, 11);
            cmd.Parameters.Add("@NewActiveStatus", SqlDbType.Bit);
            cmd.Parameters.Add("@OldVendorID", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@OldVendorPhone", SqlDbType.NVarChar, 11);

            // values
            cmd.Parameters["@NewVendorID"].Value = newVendorName;
            cmd.Parameters["@NewVendorPhone"].Value = newVendorPhone;
            cmd.Parameters["@NewActiveStatus"].Value = newActiveStatus;
            cmd.Parameters["@OldVendorID"].Value = oldVendorName;
            cmd.Parameters["@OldVendorPhone"].Value = oldVendorPhone;

            // execute
            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                result = (rowsAffected == 1); // true if Vendor updated
                
                //Deletes deactivated Vendors from VendorItems
                if (result == true && newActiveStatus == false)
                {
                    var cmd2 = new SqlCommand("sp_delete_vendor_from_vendor_items", conn);
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@VendorID", newVendorName);

                    try
                    {
                        cmd2.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
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
    }
}
