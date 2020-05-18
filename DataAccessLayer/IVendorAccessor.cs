using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace DataAccessLayer
{
    public interface IVendorAccessor
    {
        Vendor SelectVendorByName(string vendorName);

        List<Vendor> SelectVendorsByActive(bool active);

        List<Vendor> SelectAllVendors();

        List<string> SelectActiveVendorNames();

        List<string> SelectVendorsByInventoryID(string inventoryID);

        bool UpdateVendor(string newVendorName, string newVendorPhone,  bool newActiveStatus,
            string oldVendorName, string oldVendorPhone);

        bool InsertVendor(string vendorName, string phoneNumber);

        string SelectPreferredVendor(string inventoryID);

        int SetPreferredVendor(string inventoryID, string vendorID);

        int DeactivateVendor(string vendorID);

        int ActivateVendor(string vendorID);

        int DeleteVendor(string vendorID);
    }
}
