using DataObjects;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPresentationLayer.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserController : Controller
    {
        private IUserManager _userManager = null;

        public UserController()
        {
            _userManager = new UserManager();
        }

        // GET: User
        public ActionResult Index()
        {
            ViewBag.Title = "Employee List";

            List<User> users;

            try
            {
                users = _userManager.GetUsersByActive();
            }
            catch (Exception ex)
            {
                return View();
            }

            return View(users);
        }

        // GET: User/Details/5
        public ActionResult Details(string email)
        {
            ViewBag.Title = "Employee Details";

            User user;

            try
            {
                user = _userManager.GetEmployeeByEmail(email);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            ViewBag.Title = "New Employee";

            return View();
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            User user = new User();
            user.FirstName = form.Get("FirstName");
            user.LastName = form.Get("LastName");
            user.Email = form.Get("Email");
            user.PhoneNumber = form.Get("PhoneNumber");

            try
            {
                _userManager.CreateEmployee(user.FirstName, user.LastName, user.PhoneNumber, user.Email);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Edit/5
        public ActionResult Edit(string email)
        {
            ViewBag.Title = "Edit Employee";

            User user;

            try
            {
                user = _userManager.GetEmployeeByEmail(email);
                Session["oldUser"] = user;
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Edit(FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {
                User oldUser = (User)Session["oldUser"];
                User newUser = new User()
                {
                    FirstName = formCollection["FirstName"],
                    LastName = formCollection["LastName"],
                    Email = formCollection["Email"],
                    PhoneNumber = formCollection["PhoneNumber"],
                    Active = Convert.ToBoolean(formCollection["Active"].Split(',')[0])
                };

                try
                {
                    _userManager.UpdateEmployee(oldUser.EmployeeID, newUser.FirstName, newUser.LastName,
                        newUser.PhoneNumber, newUser.Email, newUser.Active, oldUser.FirstName,
                        oldUser.LastName, oldUser.PhoneNumber, oldUser.Email);

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

        // GET: User/Delete/5
        public ActionResult Delete(string email)
        {
            ViewBag.Title = "Delete Employee";

            User user;

            try
            {
                user = _userManager.GetEmployeeByEmail(email);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                _userManager.RemoveEmployee(id);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
