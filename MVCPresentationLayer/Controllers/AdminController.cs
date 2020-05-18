using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCPresentationLayer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace MVCPresentationLayer.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        //private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager userManager;

        // GET: Admin
        public ActionResult Index()
        {
            ViewBag.Title = "User List";

            userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            return View(userManager.Users.OrderBy(n => n.FamilyName).ToList());
            //return View(db.ApplicationUsers.ToList());
        }

        // GET: Admin/Details/5
        public ActionResult Details(string id)
        {
            ViewBag.Title = "User Details";

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //ApplicationUser applicationUser = db.ApplicationUsers.Find(id);
            userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationUser applicationUser = userManager.FindById(id);

            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            // get a list of roles the user has and put them into a viewbag as roles
            // along with a list of roles the user doesn't have as noRoles
            var usrMgr = new LogicLayer.UserManager();
            var allRoles = usrMgr.GetEmployeeRoles();

            var roles = userManager.GetRoles(id);
            var noRoles = allRoles.Except(roles);

            ViewBag.Roles = roles;
            ViewBag.NoRoles = noRoles;

            return View(applicationUser);
        }

        public ActionResult RemoveRole(string id, string role)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.Users.First(u => u.Id == id);

            // code to prevent removing the last administrator
            if (role == "Administrator")
            {
                var adminUsers = userManager.Users.ToList()
                    .Where(u => userManager.IsInRole(u.Id, "Administrator"))
                    .ToList().Count();
                if (adminUsers < 2)
                {
                    //For some reason ViewBag does not work here. Switched to Session and now it works.
                    Session["CannotRemoveLastAdminError"] = "Cannot remove last administrator.";
                    return RedirectToAction("Details", "Admin", new { id = user.Id });
                }
            }
            userManager.RemoveFromRole(id, role);

            if (user.EmployeeID != null)
            {
                try
                {
                    var usrMgr = new LogicLayer.UserManager();
                    usrMgr.DeleteEmployeeRole((int)user.EmployeeID, role);
                }
                catch (Exception)
                {
                    // nothing to do
                }
            }
            return RedirectToAction("Details", "Admin", new { id = user.Id });
        }

        public ActionResult AddRole(string id, string role)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.Users.First(u => u.Id == id);

            userManager.AddToRole(id, role);

            if (user.EmployeeID != null)
            {
                try
                {
                    var usrMgr = new LogicLayer.UserManager();
                    usrMgr.CreateEmployeeRole((int)user.EmployeeID, role);
                }
                catch (Exception)
                {
                    // nothing to do
                }
            }
            return RedirectToAction("Details", "Admin", new { id = user.Id });
        }
    }
}
