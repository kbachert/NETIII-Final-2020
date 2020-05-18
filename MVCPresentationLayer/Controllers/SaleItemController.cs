using DataObjects;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPresentationLayer.Controllers
{
    public class SaleItemController : Controller
    {
        private ISaleItemManager _saleItemManager = null;
        private IInventoryManager _inventoryManager = null;

        public SaleItemController()
        {
            _saleItemManager = new SaleItemManager();
            _inventoryManager = new InventoryManager();
        }

        // GET: SaleItem
        [Authorize(Roles = "General Manager, Shift Manager, Administrator, Employee")]
        public ActionResult Index()
        {
            ViewBag.Title = "Sale Item List";

            List<SaleItem> saleItems;

            try
            {
                saleItems = _saleItemManager.GetSaleItemsByActive();
            }
            catch (Exception ex)
            {
                return View();
            }

            return View(saleItems);
        }

        // GET: SaleItem/Details/5
        [Authorize(Roles = "General Manager, Shift Manager, Administrator, Employee")]
        public ActionResult Details(int id)
        {
            ViewBag.Title = "Sale Item Details";

            SaleItem saleItem;
            List<InventoryQuantity> inventoryItemsInSaleItem = new List<InventoryQuantity>();

            try
            {
                saleItem = _saleItemManager.GetSaleItemByID(id);
                inventoryItemsInSaleItem = _inventoryManager.GetInventoryQuantitiesBySaleItem(id);
                Session["inventoryItemsInSaleItem"] = inventoryItemsInSaleItem;
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

            return View(saleItem);
        }

        // GET: SaleItem/Create
        [Authorize(Roles = "General Manager, Administrator")]
        public ActionResult Create()
        {
            ViewBag.Title = "New Sale Item";

            var inventoryItemList = new List<string>();

            inventoryItemList.Add("");

            try
            {
                foreach (InventoryItem inventoryItem in _inventoryManager.GetInventoryItemsByActive())
                {
                    inventoryItemList.Add(inventoryItem.ItemName);
                }
            }
            catch
            {
                //Will simply populate an empty list of Vendors no action needed
            }

            ViewBag.InventoryItemList = inventoryItemList;

            return View();
        }

        // POST: SaleItem/Create
        [HttpPost]
        [Authorize(Roles = "General Manager, Administrator")]
        public ActionResult Create(List<string> inventoryQuantityStringList, string itemName, string itemSize, string flavor, string price)
        {
            int saleItemID = 0;

            List<InventoryQuantity> saleItemInventory = new List<InventoryQuantity>();

            SaleItem saleItem = new SaleItem()
            {
                ItemName = itemName,
                ItemSize = itemSize,
                Flavor = flavor,
                Price = Math.Round(Convert.ToDecimal(price), 2)
            };

            //Populate saleItemInventory with all InventoryQuantities added to the SaleItem
            if (!(null == inventoryQuantityStringList))
            {
                for (int i = 0; i < inventoryQuantityStringList.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        saleItemInventory.Add(new InventoryQuantity()
                        {
                            InventoryItemName = inventoryQuantityStringList[i]

                        });
                    }
                    else
                    {
                        saleItemInventory[saleItemInventory.Count - 1].Quantity = Convert.ToDecimal(inventoryQuantityStringList[i]);
                    }
                }
            }

            try
            {
                saleItemID = _saleItemManager.AddSaleItem(saleItem);
            }
            catch
            {
                return View();
            }

            //SaleItem was not created
            if (saleItemID == 0)
            {
                return View();
            }
            else
            {
                foreach (InventoryQuantity item in saleItemInventory)
                {
                    try
                    {
                        _saleItemManager.AddSaleItemInventory(
                            saleItemID, item.InventoryItemName, item.Quantity);
                    }
                    catch (Exception ex)
                    {
                        return Json(Url.Action("Index", "SaleItem"));
                    }
                }
            }

            return Json(Url.Action("Index", "SaleItem"));
        }

        // GET: SaleItem/Edit/5
        [Authorize(Roles = "General Manager, Administrator")]
        public ActionResult Edit(int id)
        {
            ViewBag.Title = "Edit Sale Item";

            SaleItem saleItem;
            List<InventoryQuantity> inventoryItemsInSaleItem = new List<InventoryQuantity>();
            List<string> inventoryItemList = new List<string>();

            inventoryItemList.Add("");

            try
            {
                saleItem = _saleItemManager.GetSaleItemByID(id);
                inventoryItemsInSaleItem = _inventoryManager.GetInventoryQuantitiesBySaleItem(id);
                Session["inventoryItemsInSaleItem"] = inventoryItemsInSaleItem;

                foreach (InventoryItem inventoryItem in _inventoryManager.GetInventoryItemsByActive())
                {
                    inventoryItemList.Add(inventoryItem.ItemName);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

            ViewBag.InventoryItemList = inventoryItemList;

            Session["oldSaleItem"] = saleItem;

            return View(saleItem);
        }

        // POST: SaleItem/Edit/5
        [HttpPost]
        [Authorize(Roles = "General Manager, Administrator")]
        public ActionResult Edit(List<string> inventoryQuantityStringList, string newItemName, string newItemSize, string newFlavor, string newPrice, bool newActiveStatus)
        {
            bool itemUpdated = false;

            SaleItem newSaleItem = new SaleItem()
            {
                ItemName = newItemName.Trim(),
                ItemSize = newItemSize.Trim(),
                Flavor = newFlavor.Trim(),
                Price = Math.Round(Convert.ToDecimal(newPrice.Trim()), 2),
                Active = newActiveStatus
            };

            List<InventoryQuantity> saleItemInventory = new List<InventoryQuantity>();

            try
            {
                itemUpdated = _saleItemManager.EditSaleItem((SaleItem)Session["oldSaleItem"], newSaleItem);
            }
            catch (Exception ex)
            {
                return Json(Url.Action("Index", "SaleItem"));
            }
            if (itemUpdated == false) //Item not updated OR multiple items updated
            {
                return Json(Url.Action("Index", "SaleItem"));
            }
            else //Item updated
            {
                //Populate saleItemInventory with all InventoryQuantities added to the SaleItem
                if (!(null == inventoryQuantityStringList))
                {
                    for (int i = 0; i < inventoryQuantityStringList.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            saleItemInventory.Add(new InventoryQuantity()
                            {
                                InventoryItemName = inventoryQuantityStringList[i]

                            });
                        }
                        else
                        {
                            saleItemInventory[saleItemInventory.Count - 1].Quantity = Convert.ToDecimal(inventoryQuantityStringList[i]);
                        }
                    }
                }

                //Deletes all of selected Sale Item's Inventory Items (for next step)
                try
                {
                    _saleItemManager.DeleteSaleItemFromSaleItemInventory(((SaleItem)Session["oldSaleItem"]).SaleItemID);
                }
                catch (Exception ex) {
                    return Json(Url.Action("Index", "SaleItem"));
                }

                //Adds all selected InventoryQuantities to the SaleItem
                foreach (InventoryQuantity item in saleItemInventory)
                {
                    try
                    {
                        _saleItemManager.AddSaleItemInventory(
                            ((SaleItem)Session["oldSaleItem"]).SaleItemID, item.InventoryItemName, item.Quantity);
                    }
                    catch (Exception ex)
                    {
                        return Json(Url.Action("Index", "SaleItem"));
                    }
                }
            }

            return Json(Url.Action("Index", "SaleItem"));
        }


        // GET: SaleItem/Delete/5
        [Authorize(Roles = "General Manager, Administrator")]
        public ActionResult Delete(int id)
        {
            ViewBag.Title = "Delete Sale Item";

            SaleItem saleItem;

            try
            {
                saleItem = _saleItemManager.GetSaleItemByID(id);
            }
            catch
            {
                return RedirectToAction("Index");
            }

            return View(saleItem);
        }

        // POST: SaleItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "General Manager, Administrator")]
        public ActionResult DeletePost(int id)
        {
            try
            {
                _saleItemManager.RemoveSaleItem(id);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
