using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;

namespace LogicLayer
{
    public class RoleManager : IRoleManager
    {
        private IRoleAccessor _roleAccessor;

        public RoleManager()
        {
            _roleAccessor = new RoleAccessor();
        }

        public RoleManager(IRoleAccessor roleAccessor)
        {
            _roleAccessor = roleAccessor;
        }

        public void DeleteEmployeesRoles(int employeeID)
        {
            try
            {
                _roleAccessor.DeleteEmployeesRoles(employeeID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Delete Employee's Roles", ex);
            }
        }

        public List<string> GetAllRoles()
        {
            try
            {
                return _roleAccessor.SelectAllRoles();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Retrieve Roles", ex);
            }
        }

        public List<string> GetEmployeeRoles(int employeeID)
        {
            try
            {
                return _roleAccessor.SelectEmployeeRoles(employeeID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Retrieve Employee Roles", ex);
            }
        }
    }
}

