using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public interface IRoleAccessor
    {
        List<String> SelectAllRoles();

        List<String> SelectEmployeeRoles(int employeeID);

        void DeleteEmployeesRoles(int employeeID);
    }
}
