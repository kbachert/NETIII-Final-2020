using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class RoleAccessor : IRoleAccessor
    {
        public List<String> SelectAllRoles()
        {
            List<String> roles = new List<String>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_all_roles", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        roles.Add(reader.GetString(0));
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
            return roles;
        }

        public List<string> SelectEmployeeRoles(int employeeID)
        {
            List<String> roles = new List<String>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_roles_by_employeeid", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters and values
            cmd.Parameters.Add("@EmployeeID", SqlDbType.Int);
            cmd.Parameters["@EmployeeID"].Value = employeeID;

            // execute
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        roles.Add(reader.GetString(0));
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
            return roles;
        }

        public void DeleteEmployeesRoles(int employeeID)
        {
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_delete_employees_roles", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters and values
            cmd.Parameters.Add("@EmployeeID", SqlDbType.Int);
            cmd.Parameters["@EmployeeID"].Value = employeeID;

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
    }
}
