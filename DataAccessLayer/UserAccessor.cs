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
    public class UserAccessor : IUserAccessor
    {
        public User AuthenticateUser(string email, string passwordHash)
        {
            int result = 0; // this will be 1 if the user is authenticated
            User user = null;

            // get the SqlConnection object
            var conn = DBConnection.GetConnection();

            // next get a command object
            var cmd = new SqlCommand("sp_authenticate_user", conn);

            // set the command type
            cmd.CommandType = CommandType.StoredProcedure;

            // add the parameters to the command
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 100);

            // provide values for the parameters
            cmd.Parameters["@Email"].Value = email;
            cmd.Parameters["@PasswordHash"].Value = passwordHash;

            // now that the command is ready,
            // open a connection, execute and process results
            try
            {
                conn.Open();
                result = Convert.ToInt32(cmd.ExecuteScalar());

                if (result == 1) // user authenticated
                {
                    user = SelectUserByEmail(email);
                }
                else
                {
                    throw new ApplicationException("User not found!");
                }
            }
            catch (Exception up)
            {
                throw up;
            }
            return user;
        }

        public User SelectUserByEmail(string email)
        {
            User user = null;

            // get SqlConnection object
            var conn = DBConnection.GetConnection();

            // commands
            var cmd1 = new SqlCommand("sp_select_user_by_email", conn);
            var cmd2 = new SqlCommand("sp_select_roles_by_employeeid", conn);

            // command types
            cmd1.CommandType = CommandType.StoredProcedure;
            cmd2.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd1.Parameters.Add("@Email", SqlDbType.NVarChar, 250);
            cmd2.Parameters.Add("@EmployeeID", SqlDbType.Int);

            // values for first command, second requires first results
            cmd1.Parameters["@Email"].Value = email;

            try
            {
                // open the connection
                conn.Open();

                var reader1 = cmd1.ExecuteReader();

                user = new User();

                user.Email = email;

                if (reader1.Read())
                { //Read gets one row with multiple columns
                    user.EmployeeID = reader1.GetInt32(0); // 0 represents table column
                    user.FirstName = reader1.GetString(1);
                    user.LastName = reader1.GetString(2);
                    user.PhoneNumber = reader1.GetString(3);
                    user.Active = reader1.GetBoolean(4);
                }
                else
                {
                    throw new ApplicationException("User not found.");
                }
                reader1.Close();

                // now cmd2 needs a parameter value
                cmd2.Parameters["@EmployeeID"].Value = user.EmployeeID;

                var reader2 = cmd2.ExecuteReader();

                List<string> roles = new List<string>();

                while (reader2.Read())
                {
                    roles.Add(reader2.GetString(0));
                }
                reader2.Close();

                user.Roles = roles;
            }
            catch (Exception up)
            {

                throw up;
            }

            return user;
        }

        public List<User> SelectUsersByActive(bool active = true)
        {
            List<User> users = new List<User>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_users_by_active", conn);
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
                        var user = new User();
                        user.EmployeeID = reader.GetInt32(0);
                        user.FirstName = reader.GetString(1);
                        user.LastName = reader.GetString(2);
                        user.PhoneNumber = reader.GetString(3);
                        user.Email = reader.GetString(4);
                        user.Active = reader.GetBoolean(5);

                        users.Add(user);
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
            return users;
        }

        public bool UpdatePasswordHash(int userID, string oldPassHash, string newPassHash)
        {
            bool result = false;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_update_password", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@EmployeeID", SqlDbType.Int);
            cmd.Parameters.Add("@OldPasswordHash", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@NewPasswordHash", SqlDbType.NVarChar, 100);

            // values
            cmd.Parameters["@EmployeeID"].Value = userID;
            cmd.Parameters["@OldPasswordHash"].Value = oldPassHash;
            cmd.Parameters["@NewPasswordHash"].Value = newPassHash;

            // execute
            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                result = (rowsAffected == 1); // true if update successful
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

        public bool InsertEmployee(string firstName, string lastName, string phoneNumber, string email)
        {
            bool result = false;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_insert_employee", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 11);
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 250);

            // values
            cmd.Parameters["@FirstName"].Value = firstName;
            cmd.Parameters["@LastName"].Value = lastName;
            cmd.Parameters["@PhoneNumber"].Value = phoneNumber;
            cmd.Parameters["@Email"].Value = email;

            // execute
            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                result = (rowsAffected == 1); // true if employee created
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

        public bool InsertEmployeeRole(int employeeID, string roleID)
        {
            bool result = false;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_insert_employee_role", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@EmployeeID", SqlDbType.Int);
            cmd.Parameters.Add("@RoleID", SqlDbType.NVarChar, 50);

            //values
            cmd.Parameters["@EmployeeID"].Value = employeeID;
            cmd.Parameters["@RoleID"].Value = roleID;

            // execute
            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                result = (rowsAffected == 1); // true if employee role created
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

        public bool DeleteEmployeeRole(int employeeID, string roleID)
        {
            bool result = false;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_delete_employee_role", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@EmployeeID", SqlDbType.Int);
            cmd.Parameters.Add("@RoleID", SqlDbType.NVarChar, 50);

            //values
            cmd.Parameters["@EmployeeID"].Value = employeeID;
            cmd.Parameters["@RoleID"].Value = roleID;

            // execute
            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                result = (rowsAffected == 1); // true if employee role deleted
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

        public bool UpdateEmployee(int employeeID, string newFirstName, string newLastName, string newPhoneNumber, 
            string newEmail, bool newActiveStatus, string oldFirstName, string oldLastName, 
            string oldPhoneNumber, string oldEmail)
        {
            bool result = false;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_update_employee", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@EmployeeID", SqlDbType.Int);
            cmd.Parameters.Add("@NewFirstName", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@NewLastName", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@NewPhoneNumber", SqlDbType.NVarChar, 11);
            cmd.Parameters.Add("@NewEmail", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@NewActiveStatus", SqlDbType.Bit);
            cmd.Parameters.Add("@OldFirstName", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@OldLastName", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@OldPhoneNumber", SqlDbType.NVarChar, 11);
            cmd.Parameters.Add("@OldEmail", SqlDbType.NVarChar, 250);

            // values
            cmd.Parameters["@EmployeeID"].Value = employeeID;
            cmd.Parameters["@NewFirstName"].Value = newFirstName;
            cmd.Parameters["@NewLastName"].Value = newLastName;
            cmd.Parameters["@NewPhoneNumber"].Value = newPhoneNumber;
            cmd.Parameters["@NewEmail"].Value = newEmail;
            cmd.Parameters["@NewActiveStatus"].Value = newActiveStatus;
            cmd.Parameters["@OldFirstName"].Value = oldFirstName;
            cmd.Parameters["@OldLastName"].Value = oldLastName;
            cmd.Parameters["@OldPhoneNumber"].Value = oldPhoneNumber;
            cmd.Parameters["@OldEmail"].Value = oldEmail;

            // execute
            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                result = (rowsAffected == 1); // true if employee updated
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

        public int DeactivateEmployee(int employeeID)
        {
            int rows = 0;
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_deactivate_employee", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmployeeID", employeeID);

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

        public int ActivateEmployee(int employeeID)
        {
            int rows = 0;
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_reactivate_employee", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmployeeID", employeeID);

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

        public int DeleteEmployee(int employeeID)
        {
            int rows = 0;
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_delete_employee", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmployeeID", employeeID);

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

        public bool UpdateEmailAddress(string oldEmail, string newEmail)
        {
            bool result = false;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_update_email", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@OldEmail", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@NewEmail", SqlDbType.NVarChar, 250);

            // values
            cmd.Parameters["@OldEmail"].Value = oldEmail;
            cmd.Parameters["@NewEmail"].Value = newEmail;

            // execute
            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                result = (rowsAffected == 1); // true if update successful
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

        public bool UpdatePhoneNumber(int employeeID, string oldPhone, string newPhone)
        {
            bool result = false;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_update_employee_phone_number", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@EmployeeID", SqlDbType.Int);
            cmd.Parameters.Add("@OldPhoneNumber", SqlDbType.NVarChar, 12);
            cmd.Parameters.Add("@NewPhoneNumber", SqlDbType.NVarChar, 12);

            // values
            cmd.Parameters["@EmployeeID"].Value = employeeID;
            cmd.Parameters["@OldPhoneNumber"].Value = oldPhone;
            cmd.Parameters["@NewPhoneNumber"].Value = newPhone;

            // execute
            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                result = (rowsAffected == 1); // true if update successful
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

        public List<string> SelectEmployeeRoles()
        {
            List<string> roles = new List<string>();

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
    }
}
