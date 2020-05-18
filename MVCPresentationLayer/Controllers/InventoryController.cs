using DataObjects;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCPresentationLayer.Models;

namespace MVCPresentationLayer.Controllers
{
    public class InventoryController : Controller
    {
        private IInventoryManager _inventoryManager = null;
        private IVendorManager _vendorManager = null;

        public InventoryController()
        {
            _inventoryManager = new InventoryManager();
            _vendorManager = new VendorManager();
        }

        // GET: Inventory
        [Authorize(Roles = "General Manager, Shift Manager, Administrator, Employee")]
        public ActionResult Index()
        {
            ViewBag.Title = "Dairy Queen Inventory";

            List<InventoryItem> inventoryItems;

            try
            {
                inventoryItems = _inventoryManager.GetInventoryItemsByActive();
            }
            catch (Exception ex)
            {
                return View();
            }

            return View(inventoryItems);
        }

        // GET: Inventory/Details/5
        public ActionResult Details(string id)
        {
            ViewBag.Title = "Inventory Item Details";

            InventoryItem inventoryItem;
            string preferredVendor = "";

            try
            {
                inventoryItem = _inventoryManager.GetInventoryItemByID(id);
                preferredVendor = _vendorManager.RetrievePreferredVendor(id);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

            ViewBag.PreferredVendor = preferredVendor;

            return View(inventoryItem);
        }

        // GET: Inventory/Create
        [Authorize(Roles = "General Manager, Shift Manager, Administrator")]
        public ActionResult Create()
        {
            ViewBag.Title = "New Inventory Item";

            var vendorList = new List<string>();

            vendorList.Add("");

            try
            {
                foreach (string vendor in _vendorManager.GetActiveVendorNames())
                {
                    vendorList.Add(vendor);
                }
            }
            catch
            {
                //Will simply populate an empty list of Vendors no action needed
            }

            ViewBag.VendorList = vendorList;

            return View();
        }

        // POST: Inventory/Create
        [HttpPost]
        [Authorize(Roles = "General Manager, Shift Manager, Administrator")]
        public ActionResult Create(FormCollection form)
        {
            InventoryItem inventoryItem = new InventoryItem();
            inventoryItem.ItemName = form.Get("ItemName");
            inventoryItem.PurchaseUnit = form.Get("PurchaseUnit");
            inventoryItem.SaleUnit = form.Get("SaleUnit");
            inventoryItem.SaleUnitsPerPurchaseUnit = Convert.ToDecimal(form.Get("SaleUnitsPerPurchaseUnit"));
            inventoryItem.QuantityOnHand = Convert.ToDecimal(form.Get("QuantityOnHand"));
            inventoryItem.ReorderLevel = Convert.ToDecimal(form.Get("ReorderLevel"));

            try
            {
                _inventoryManager.AddInventoryItem(inventoryItem);

                _vendorManager.SetPreferredVendor(inventoryItem.ItemName, form.Get("PreferredVendor"));

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Inventory/Edit/5
        [Authorize(Roles = "General Manager, Administrator")]
        public ActionResult Edit(string id)
        {
            ViewBag.Title = "Edit Inventory Item";

            InventoryItem inventoryItem;

            try
            {
                inventoryItem = _inventoryManager.GetInventoryItemByID(id);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

            string preferredVendor = _vendorManager.RetrievePreferredVendor(id);

            var vendorList = new List<SelectListItem>();
            vendorList.Add(new SelectListItem()
            {
                Text = "",
                Value = "",
            });

            try
            {
                foreach (string vendor in _vendorManager.GetActiveVendorNames())
                {
                    vendorList.Add(new SelectListItem()
                    {
                        Text = vendor,
                        Value = vendor,
                        Selected = vendor.Equals(preferredVendor)
                    });
                }
            }
            catch
            {
                //Will simply populate an empty list of Vendors no action needed
            }

            ViewBag.VendorList = vendorList;

            Session["oldInventoryItem"] = inventoryItem;

            return View(inventoryItem);
        }

        // POST: Inventory/Edit/5
        [HttpPost]
        [Authorize(Roles = "General Manager, Administrator")]
        public ActionResult Edit(FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {
                InventoryItem oldItem = (InventoryItem)Session["oldInventoryItem"];
                InventoryItem newItem = new InventoryItem()
                {
                    ItemName = formCollection["ItemName"],
                    PurchaseUnit = formCollection["PurchaseUnit"],
                    SaleUnit = formCollection["SaleUnit"],
                    SaleUnitsPerPurchaseUnit = Convert.ToDecimal(formCollection["SaleUnitsPerPurchaseUnit"]),
                    QuantityOnHand = Convert.ToDecimal(formCollection["QuantityOnHand"]),
                    ReorderLevel = Convert.ToDecimal(formCollection["ReorderLevel"]),
                    Active = Convert.ToBoolean(formCollection["Active"].Split(',')[0])
                };

                try
                {
                    _inventoryManager.EditInventoryItem(oldItem, newItem);

                    _vendorManager.SetPreferredVendor(newItem.ItemName, formCollection["PreferredVendor"]);

                    return RedirectToAction("Index");
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        // GET: Inventory/Delete/5
        [Authorize(Roles = "General Manager, Shift Manager, Administrator")]
        public ActionResult Delete(string id)
        {
            ViewBag.Title = "Delete Inventory Item";

            InventoryItem inventoryItem;

            try
            {
                inventoryItem = _inventoryManager.GetInventoryItemByID(id);
            }
            catch
            {
                return RedirectToAction("Index");
            }

            return View(inventoryItem);
        }

        // POST: Inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "General Manager, Shift Manager, Administrator")]
        public ActionResult DeletePost(string itemName)
        {
            try
            {
                _inventoryManager.RemoveInventoryItem(itemName);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
