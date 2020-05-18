using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace LogicLayer
{
    public interface IUserManager
    {
        User AuthenticateUser(string email, string password);

        bool ResetPassword(int employeeID, string oldPassword, string newPassword);

        bool UpdateEmployee(int employeeID, string newFirstName, string newLastName, string newPhoneNumber,
            string newEmail, bool newActiveStatus, string oldFirstName, string oldLastName,
            string oldPhoneNumber, string oldEmail);

        bool EditEmail(string oldEmail, string newEmail);

        bool EditPhoneNumber(int employeeID, string oldPhoneNumber, string newPhoneNumber);

        List<User> GetUsersByActive(bool active = true);

        User GetEmployeeByEmail(string email);

        bool FindUser(string email);

        List<string> GetEmployeeRoles();

        bool CreateEmployee(string firstName, string lastName, string phoneNumber, string email);

        bool CreateEmployeeRole(int employeeID, string roleID);

        bool DeleteEmployeeRole(int employeeID, string roleID);

        bool RemoveEmployee(int employeeID);

        bool SetEmployeeActiveState(bool active, int employeeID);

        int RetrieveUserIdFromEmail(string email);
    }
}