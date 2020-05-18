using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace LogicLayer
{
    public interface IVendorManager
    {
        Vendor GetVendorByName(string vendorName);

        List<Vendor> GetVendorsByActive(bool active = true);

        List<Vendor> GetAllVendors();

        List<string> GetActiveVendorNames();

        List<string> GetVendorsByInventoryID(string inventoryID);

        bool UpdateVendor(string newVendorName, string newVendorPhone, bool newActiveStatus,
            string oldVendorName, string oldVendorPhone);

        bool CreateVendor(string vendorName, string phoneNumber);

        string RetrievePreferredVendor(string inventoryID);

        bool SetPreferredVendor(string inventoryID, string vendorName);

        bool SetVendorActiveState(bool active, string vendorName);

        bool RemoveVendor(string vendorName);
    }
}
