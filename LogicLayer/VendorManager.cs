using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using DataObjects;

namespace LogicLayer
{
    public class VendorManager : IVendorManager
    {
        private IVendorAccessor _vendorAccessor;

        public VendorManager()
        {
            _vendorAccessor = new VendorAccessor();
        }

        public VendorManager(IVendorAccessor vendorAccessor)
        {
            _vendorAccessor = vendorAccessor;
        }
        public bool CreateVendor(string vendorName, string phoneNumber)
        {
            bool result = false;

            try
            {
                result = _vendorAccessor.InsertVendor(vendorName, phoneNumber);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Vendor Could Not Be Created", ex);
            }

            return result;
        }

        public List<string> GetActiveVendorNames()
        {
            try
            {
                return _vendorAccessor.SelectActiveVendorNames();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Retrieve List of Vendors", ex);
            }
        }

        public List<Vendor> GetAllVendors()
        {
            try
            {
                return _vendorAccessor.SelectAllVendors();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Retrieve List of Vendors", ex);
            }
        }

        public Vendor GetVendorByName(string vendorName)
        {
            try
            {
                return _vendorAccessor.SelectVendorByName(vendorName);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Retrieve Vendor", ex);
            }
        }

        public List<Vendor> GetVendorsByActive(bool active = true)
        {
            try
            {
                return _vendorAccessor.SelectVendorsByActive(active);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Retrieve List of Vendors", ex);
            }
        }

        public List<string> GetVendorsByInventoryID(string inventoryID)
        {
            try
            {
                return _vendorAccessor.SelectVendorsByInventoryID(inventoryID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Couldn't Retrieve List of Vendors", ex);
            }
        }

        public bool RemoveVendor(string vendorName)
        {
            bool oneRowRemoved = false;

            try
            {
                oneRowRemoved = (1 == _vendorAccessor.DeleteVendor(vendorName));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Vendor Could Not Be Removed", ex);
            }

            return oneRowRemoved;
        }

        public string RetrievePreferredVendor(string inventoryID)
        {
            string vendorName = null;

            try
            {
                vendorName = _vendorAccessor.SelectPreferredVendor(inventoryID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Preferred Vendor Could Not Be Retrieved", ex);
            }

            return vendorName;
        }

        public bool SetPreferredVendor(string inventoryID, string vendorName)
        {
            bool oneRecordAffected = true;

            try
            {
                oneRecordAffected = (_vendorAccessor.SetPreferredVendor(inventoryID, vendorName) == 1);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Preferred Vendor Could Not Be Set", ex);
            }

            return oneRecordAffected;
        }

        public bool SetVendorActiveState(bool active, string vendorName)
        {
            bool result = false;

            try
            {
                if (active)
                {
                    result = (1 == _vendorAccessor.ActivateVendor(vendorName));
                }
                else
                {
                    result = (1 == _vendorAccessor.DeactivateVendor(vendorName));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Active Status Change Failed", ex);
            }

            return result;
        }

        public bool UpdateVendor(string newVendorName, string newVendorPhone, bool newActiveStatus,
            string oldVendorName, string oldVendorPhone)
        {
            bool result = false;

            try
            {
                result = _vendorAccessor.UpdateVendor(newVendorName, newVendorPhone, newActiveStatus,
                    oldVendorName, oldVendorPhone);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Vendor Could Not Be Updated", ex);
            }

            return result;
        }
    }
}
