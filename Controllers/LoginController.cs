using DotNetOpenAuth.GoogleOAuth2;
using Microsoft.AspNet.Membership.OpenAuth;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeaveManagementSystemValueCreed.Models;

namespace LeaveManagementSystemValueCreed.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        // GET: Login/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Login/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Login/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Login/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Login/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Login/Delete/5
        public ActionResult Autherize(LeaveManagementSystemValueCreed.Models.UsersTable userid)
        {
            if (Session["userID"] != null)
            {
               
                return View("Index", userid);
            }

            using (VC_LMSEntities d = new VC_LMSEntities())
            {
                ////var detail = dd.userids.Contains(userid);
                //var details = dd.userids.Where(x => x.userName == userid.userName && x.Password == userid.Password && userid.role == x.role).FirstOrDefault();
                //var name = userid.userName.ToString();
                //var role = userid.role.ToString();
                //var role = d.roles.Find(name).NAME.ToString();
                var details = d.UsersTables.Where(x => x.EMAIL == userid.EMAIL&&x.Status=="Active" ).FirstOrDefault();





                if (details == null)
                {
                    ViewBag.Message = "No such Email Id Present in Database";
                    return View("Index", userid);
                }
                else
                {
                    VC_LMSEntities e = new VC_LMSEntities();
                    VC_LMSEntities dd = new VC_LMSEntities();
                 
          
                    var aprover = e.approvers.Where(x => x.approver_id == details.EMPLOYEE_ID).FirstOrDefault();
                    if (aprover != null)
                    {
                        var id = dd.UsersTables.Where(x => x.EMPLOYEE_ID == aprover.approver_id).FirstOrDefault();
                        Session["userID"] = id.EMPLOYEE_ID;
                        Session["userName"] = id.FIRST_NAME;
                        Session["Name"] = id.EMAIL;
                        return RedirectToAction("Index", "approver");
                    }
                    foreach (var item in d.UsersTables)
                    {
                        if (userid.EMAIL == item.EMAIL)
                        {
                            if (item.ROLES == "Admin")
                            {
                                Session["userName"] = item.FIRST_NAME;
                                Session["Name"] = item.EMAIL;
                                Session["userID"] = item.EMPLOYEE_ID;
                                return RedirectToAction("Index", "admin");
                            }
                            else if (item.ROLES == "Employee")
                            {
                                Session["userName"] = item.FIRST_NAME;
                                Session["userID"] = item.EMPLOYEE_ID;
                                Session["Name"] = item.EMAIL;
                                return RedirectToAction("Index", "Employee");
                            }
                            else if (item.ROLES == "HR")
                            {
                                Session["userName"] = item.FIRST_NAME;
                                Session["userID"] = item.EMPLOYEE_ID;
                                Session["Name"] = item.EMAIL;
                                return RedirectToAction("Index", "HR");
                            }
                        }
                    }
                }


                return View("Index", userid);
            }

        }

        public ActionResult LogOff()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Login");

        }
        public ActionResult RedirectToGoogle()
        {
            if (Session["userID"] != null)
            {
                 ViewBag.Message= " A Session is Active on this System";
                return RedirectToAction("Index", "Login");
            }
            string provider = "google";
            string returnUrl = "";
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }
        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OpenAuth.RequestAuthentication(Provider, ReturnUrl);
            }
        }
        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            string ProviderName = OpenAuth.GetProviderNameFromCurrentRequest();
           
            if (ProviderName == null || ProviderName == "")
            {
                NameValueCollection nvs = Request.QueryString;
                if (nvs.Count > 0)
                {
                    if (nvs["state"] != null)
                    {
                        NameValueCollection provideritem = HttpUtility.ParseQueryString(nvs["state"]);
                        if (provideritem["__provider__"] != null)
                        {
                            ProviderName = provideritem["__provider__"];
                        }
                    }
                }
            }

            GoogleOAuth2Client.RewriteRequest();

            var redirectUrl = Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl });
            var retUrl = returnUrl;
            var authResult = OpenAuth.VerifyAuthentication(redirectUrl);
           

            if (!authResult.IsSuccessful)
            {
                ViewBag.Message = "Not A Valid EmailID";
                return Redirect(Url.Action("Account", "Login"));
            }

            // User has logged in with provider successfully
            // Check if user is already registered locally
            //You can call you user data access method to check and create users based on your model
            VC_LMSEntities d = new VC_LMSEntities();
            string email=authResult.ExtraData["email"];
            var row = d.UsersTables.Where(x => x.EMAIL == email).FirstOrDefault();
            if(row==null)
            {
                ViewBag.Message = "Email not in Database";
                return RedirectToAction("Index");
            }
            else
            {
                var id = row.EMPLOYEE_ID;
                //var manager = d.managers.Where(x => x.manager_emp_id == id).FirstOrDefault();
                //if ( manager!= null&&row.Status=="Active")
                //{

                //    Session["userID"] = row.EMPLOYEE_ID;
                //    Session["userName"] = row.FIRST_NAME;
                //    Session["Name"] = row.EMAIL;
                //    return RedirectToAction("Index", "manager");
                //}
                //else 
                {
                    var row1 = d.approvers.Where(x => x.approver_id == row.EMPLOYEE_ID).FirstOrDefault();
                    if (row1 != null && row.Status == "Active")
                    {
                        Session["userID"] = row.EMPLOYEE_ID;
                        Session["userName"] = row.FIRST_NAME;
                        Session["Name"] = row.EMAIL;
                        return RedirectToAction("Index","Approver");
                    }
                    else if (row.ROLES == "HR" && row.Status == "Active")
                    {
                        Session["userID"] = row.EMPLOYEE_ID;
                        Session["userName"] = row.FIRST_NAME;
                        Session["Name"] = row.EMAIL;
                        return RedirectToAction("Index","HR");
                    }
                    else if (row.ROLES == "Employee" && row.Status == "Active")
                    {
                        Session["userID"] = row.EMPLOYEE_ID;
                        Session["userName"] = row.FIRST_NAME;
                        Session["Name"] = row.EMAIL;
                        return RedirectToAction("Index","Employee");
                    }
                    else if (row.ROLES == "Admin" && row.Status == "Active")
                    {   Session["userID"] = row.EMPLOYEE_ID;
                        Session["userName"] = row.FIRST_NAME;
                        Session["Name"] = row.EMAIL;
                        return RedirectToAction("Index","Admin");
                    }
                }
            }
            if (OpenAuth.Login(authResult.Provider, authResult.ProviderUserId, createPersistentCookie: false))
            {
                return Redirect(Url.Action("Index", "Home"));
            }

            //Get provider user details
            string ProviderUserId = authResult.ProviderUserId;
            string ProviderUserName = authResult.UserName;

            string Email = null;
            if (Email == null && authResult.ExtraData.ContainsKey("email"))
            {
                Email = authResult.ExtraData["email"];
            }

            if (User.Identity.IsAuthenticated)
            {
                // User is already authenticated, add the external login and redirect to return url
                OpenAuth.AddAccountToExistingUser(ProviderName, ProviderUserId, ProviderUserName, User.Identity.Name);
                return Redirect(Url.Action("Index", "Home"));
            }
            else
            {
                // User is new, save email as username
                string membershipUserName = Email ?? ProviderUserId;
                var createResult = OpenAuth.CreateUser(ProviderName, ProviderUserId, ProviderUserName, membershipUserName);

                if (!createResult.IsSuccessful)
                {
                    ViewBag.Message = "User cannot be created";
                    return View();
                }
                else
                {
                    // User created
                    if (OpenAuth.Login(ProviderName, ProviderUserId, createPersistentCookie: false))
                    {
                        return Redirect(Url.Action("Index", "Home"));
                    }
                }
            }
            return View();
        }
    }
}
