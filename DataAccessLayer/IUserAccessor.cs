using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace DataAccessLayer
{
    public interface IUserAccessor
    {
        User AuthenticateUser(string email, string passwordHash);

        bool UpdatePasswordHash(int userID, string oldPassHash, string newPassHash);

        bool UpdateEmailAddress(string oldEmail, string newEmail);

        bool UpdatePhoneNumber(int employeeID, string oldPhone, string newPhone);

        bool UpdateEmployee(int employeeID, string newFirstName, string newLastName, string newPhoneNumber, 
            string newEmail, bool newActiveStatus, string oldFirstName, string oldLastName, 
            string oldPhoneNumber, string oldEmail);
        
        List<User> SelectUsersByActive(bool active = true);

        User SelectUserByEmail(string email);

        List<string> SelectEmployeeRoles();

        bool InsertEmployee(string firstName, string lastName, string phoneNumber, string email);

        bool InsertEmployeeRole(int employeeID, string roleID);

        bool DeleteEmployeeRole(int employeeID, string roleID);

        int DeleteEmployee(int employeeID);

        int DeactivateEmployee(int employeeID);

        int ActivateEmployee(int employeeID);
    }
}
