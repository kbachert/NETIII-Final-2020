using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using DataObjects;

namespace LogicLayer
{
    public class UserManager : IUserManager
    {
        private IUserAccessor _userAccessor;

        public UserManager()
        {
            _userAccessor = new UserAccessor();
        }

        public UserManager(IUserAccessor userAccessor)
        {
            _userAccessor = userAccessor;
        }

        public User AuthenticateUser(string email, string password)
        {
            User user = null;

            try
            {
                var passwordHash = hashPassword(password);
                password = null;

                user = _userAccessor.AuthenticateUser(email, passwordHash);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Login failed.", ex);
            }

            return user;
        }

        public List<User> GetUsersByActive(bool active = true)
        {
            try
            {
                return _userAccessor.SelectUsersByActive(active);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Retrieve List of Employees", ex);
            }
        }

        public User GetEmployeeByEmail(string email)
        {
            try
            {
                return _userAccessor.SelectUserByEmail(email);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Find User", ex);
            }
        }

        public bool ResetPassword(int employeeID, string oldPassword, string newPassword)
        {
            bool result = false;

            try
            {
                string oldHash = hashPassword(oldPassword);
                string newHash = hashPassword(newPassword);

                result = _userAccessor.UpdatePasswordHash(employeeID, oldHash, newHash);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Update Failed", ex);
            }

            return result;
        }

        public bool CreateEmployee(string firstName, string lastName, string phoneNumber, string email)
        {
            bool result = false;

            try
            {
                result = _userAccessor.InsertEmployee(firstName, lastName, phoneNumber, email);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Employee Could Not Be Created", ex);
            }

            return result;
        }

        public bool CreateEmployeeRole(int employeeID, string roleID)
        {
            bool result = false;

            try
            {
                result = _userAccessor.InsertEmployeeRole(employeeID, roleID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Employee Role Could Not Be Created", ex);
            }

            return result;
        }

        public bool DeleteEmployeeRole(int employeeID, string roleID)
        {
            bool result = false;

            try
            {
                result = _userAccessor.DeleteEmployeeRole(employeeID, roleID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Employee Role Could Not Be fffffffgggggggggggggggggggDeleted", ex);
            }

            return result;
        }

        private string hashPassword(string source)
        {
            string result = "";

            // create a byte array - cryptography uses bits and bytes
            byte[] data;

            using (SHA256 sha256hash = SHA256.Create())
            {
                data = sha256hash.ComputeHash(Encoding.UTF8.GetBytes(source));
                var s = new StringBuilder();
                // loop through the bytes and build the string
                for (int i = 0; i < data.Length; i++)
                {
                    s.Append(data[i].ToString("x2")); // x2 causes the value to be hex digits
                }

                result = s.ToString().ToUpper();
            }
            return result;
        }

        public bool UpdateEmployee(int employeeID, string newFirstName, string newLastName, 
            string newPhoneNumber, string newEmail, bool newActiveStatus, string oldFirstName, 
            string oldLastName, string oldPhoneNumber, string oldEmail)
        {
            bool result = false;

            try
            {
                result = _userAccessor.UpdateEmployee(employeeID, newFirstName, newLastName, newPhoneNumber, 
                    newEmail, newActiveStatus, oldFirstName,oldLastName, oldPhoneNumber, oldEmail);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Employee Could Not Be Updated", ex);
            }

            return result;
        }

        public bool SetEmployeeActiveState(bool active, int employeeID)
        {
            bool result = false;

            try
            {
                if (active)
                {
                    result = (1 == _userAccessor.ActivateEmployee(employeeID));
                }
                else
                {
                    result = (1 == _userAccessor.DeactivateEmployee(employeeID));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Active Status Change Failed", ex);
            }

            return result;
        }

        public bool RemoveEmployee(int employeeID)
        {
            bool oneRowRemoved = false;

            try
            {
                oneRowRemoved = (1 == _userAccessor.DeleteEmployee(employeeID));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Employee Could Not Be Removed", ex);
            }

            return oneRowRemoved;
        }

        public bool EditEmail(string oldEmail, string newEmail)
        {
            bool result = false;

            try
            {
                result = _userAccessor.UpdateEmailAddress(oldEmail, newEmail);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Email Could Not Be Updated", ex);
            }

            return result;
        }

        public bool EditPhoneNumber(int employeeID, string oldPhoneNumber, string newPhoneNumber)
        {
            bool result = false;

            try
            {
                result = _userAccessor.UpdatePhoneNumber(employeeID, oldPhoneNumber, newPhoneNumber);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Phone Number Could Not Be Updated", ex);
            }

            return result;
        }

        public List<string> GetEmployeeRoles()
        {
            try
            {
                return _userAccessor.SelectEmployeeRoles();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Retrieve List of Roles", ex);
            }
        }

        public bool FindUser(string email)
        {
            try
            {
                return _userAccessor.SelectUserByEmail(email) != null;
            }
            catch (ApplicationException ax)
            {
                if (ax.Message == "User not found.")
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Database Error", ex);
            }
        }

        public int RetrieveUserIdFromEmail(string email)
        {
            try
            {
                return _userAccessor.SelectUserByEmail(email).EmployeeID;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Database Error", ex);
            }
        }
    }
}
