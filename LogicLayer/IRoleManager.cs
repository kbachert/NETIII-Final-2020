using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public interface IRoleManager
    {
        List<String> GetAllRoles();

        List<String> GetEmployeeRoles(int employeeID);

        void DeleteEmployeesRoles(int employeeID);
    }
}
