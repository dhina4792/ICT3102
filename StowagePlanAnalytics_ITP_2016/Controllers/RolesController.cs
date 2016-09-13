using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StowagePlanAnalytics_ITP_2016.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Security;

namespace StowagePlanAnalytics_ITP_2016.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        // GET: Roles
        public ActionResult Index()
        {
            var roles = context.Roles.ToList();
            return View(roles);
        }

        // GET: Roles/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: /Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            if (ModelState.IsValid) //Check for validation issues on server for user inputs
            {
                DAL.CRUDGateway<Port> gw = new DAL.CRUDGateway<Port>();
                string Name = collection["RoleName"];

                if (!gw.RoleNameExist(Name)) //Check for existing port code
                {
                    context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                    {
                        Name = collection["RoleName"]
                    });
                    
                    context.SaveChanges();
                    TempData["Status"] = "Success";
                    TempData["message"] = " " + Name + " was successfully created in the system."; //Saved Error Message
                    return View();
                }
                else
                {
                    TempData["Status"] = "Fail";
                    TempData["message"] = " " + Name + " is already existed in the system."; //Saved Error Message
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        // GET: Roles/Delete/5
        public ActionResult Delete(string RoleName)
        {
            var thisRole = context.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            context.Roles.Remove(thisRole);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ManageUserRoles(string DeleteRolesResult, string GetRolesResult, string AddRolesResult)
        {
            ViewBag.AddRolesResult = AddRolesResult;
            ViewBag.GetRolesResult = GetRolesResult;
            ViewBag.DeleteRolesResult = DeleteRolesResult;
            // prepopulate roles for the view dropdown
            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;
            return View();
        }

        public ActionResult AddRoleToUser()
        {
            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRoleToUser(string UserName, string RoleName)
        {
            var account = new AccountController();

            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;            

            if (account.UserManager.FindByName(UserName) != null)
            {
                ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                account.UserManager.AddToRole(user.Id, RoleName);

                ViewBag.ResultMessage = "Role created successfully !";
            }
            // Check if role is selected
            else if (RoleName == string.Empty)
            {
                TempData["Status"] = "Fail";
                TempData["message"] = "No roles selected."; //Saved Error Message
                return PartialView();
            }
            else
            {
                TempData["Status"] = "Fail";
                TempData["message"] = "User does not exist."; //Saved Error Message
                return PartialView();
            }
            TempData["Status"] = "Success";
            TempData["message"] = "Roles has been successfully added for " + UserName; //Saved Error Message
            return PartialView();
        }

        public ActionResult GetUserRoles()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetUserRoles(string UserName)
        {
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                var account = new AccountController();
                // Check if account existed in server
                if (account.UserManager.FindByName(UserName) != null)
                {
                    ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                    ViewBag.RolesForThisUser = account.UserManager.GetRoles(user.Id);
                }
                else
                {
                    TempData["message"] = "User does not exist."; //Saved Error Message
                    return PartialView();
                }
            }
            else
            {
                TempData["message"] = "User does not exist."; //Saved Error Message
                return PartialView();

            }

            return PartialView();
        }

        public ActionResult DeleteRoleFormUser()
        {
            // prepopulate roles for the view dropdown
            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;

            return PartialView();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleFormUser(string UserName, string RoleName)
        {
            var account = new AccountController();
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            // prepopulat roles for the view dropdown
            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;
            // Both "Admin" and "Manager" account cannot be edited
            if (UserName != "Admin" && UserName != "Manager")
            {

                // Check if role is selected
                if (RoleName == string.Empty)
                {
                    TempData["Status"] = "Fail";
                    TempData["message"] = "No roles selected."; //Saved Error Message
                    return PartialView();
                }

                // Check if account existed in server
                if (account.UserManager.FindByName(UserName) != null)
                {
                   
                    if (account.UserManager.IsInRole(user.Id, RoleName))
                    {
                      

                        account.UserManager.RemoveFromRole(user.Id, RoleName);
                        ViewBag.ResultMessage = "Role removed from this user successfully !";
                    }
                    else
                    {
                        TempData["Status"] = "Fail";
                        TempData["message"] = "Role not assigned to user."; //Saved Error Message
                        return PartialView();
                    }
                }
                else
                {
                    TempData["Status"] = "Fail";
                    TempData["message"] = "User does not exist."; //Saved Error Message
                    return PartialView();
                }
            }
            else
            {
                TempData["Status"] = "Fail";
                TempData["message"] = "Unable to remove roles from default account."; //Saved Error Message
                return PartialView();
            }
            
            TempData["Status"] = "Success";
            TempData["message"] = "Roles has been successfully deleted for " +UserName; //Saved Error Message
            return PartialView();
        }
    }
}
