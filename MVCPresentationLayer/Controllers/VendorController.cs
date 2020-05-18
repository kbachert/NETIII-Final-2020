using DataObjects;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPresentationLayer.Controllers
{
    [Authorize(Roles = "General Manager, Shift Manager, Administrator")]
    public class VendorController : Controller
    {
        private IVendorManager _vendorManager = null;

        public VendorController()
        {
            _vendorManager = new VendorManager();
        }

        // GET: Vendor
        public ActionResult Index()
        {
            ViewBag.Title = "Vendor List";

            List<Vendor> vendors;

            try
            {
                vendors = _vendorManager.GetVendorsByActive();
            }
            catch (Exception ex)
            {
                return View();
            }

            return View(vendors);
        }

        // GET: Vendor/Details/5
        public ActionResult Details(string id)
        {
            ViewBag.Title = "Vendor Details";

            Vendor vendor;

            try
            {
                vendor = _vendorManager.GetVendorByName(id);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

            return View(vendor);
        }

        // GET: Vendor/Create
        public ActionResult Create()
        {
            ViewBag.Title = "New Vendor";

            return View();
        }

        // POST: Vendor/Create
        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            Vendor vendor = new Vendor();
            vendor.VendorName = form.Get("VendorName");
            vendor.VendorPhone = form.Get("VendorPhone");

            try
            {
                _vendorManager.CreateVendor(vendor.VendorName, vendor.VendorPhone);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Vendor/Edit/5
        public ActionResult Edit(string id)
        {
            ViewBag.Title = "Edit Vendor";

            Vendor vendor;

            try
            {
                vendor = _vendorManager.GetVendorByName(id);
                Session["oldVendor"] = vendor;
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

            return View(vendor);
        }

        // POST: Vendor/Edit/5
        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            Vendor newVendor = new Vendor();
            newVendor.VendorName = form.Get("VendorName");
            newVendor.VendorPhone = form.Get("VendorPhone");
            newVendor.Active = Convert.ToBoolean(form.Get("Active").Split(',')[0]);

            try
            {
                _vendorManager.UpdateVendor(newVendor.VendorName, newVendor.VendorPhone, newVendor.Active, 
                    ((Vendor)Session["oldVendor"]).VendorName, ((Vendor)Session["oldVendor"]).VendorPhone);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Vendor/Delete/5
        public ActionResult Delete(string id)
        {
            ViewBag.Title = "Delete Vendor";

            Vendor vendor;

            try
            {
                vendor = _vendorManager.GetVendorByName(id);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

            return View(vendor);
        }

        // POST: Vendor/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeletePost(string id)
        {
            try
            {
                _vendorManager.RemoveVendor(id);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
