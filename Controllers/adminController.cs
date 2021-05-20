using LeaveManagementSystemValueCreed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeaveManagementSystemValueCreed.Controllers
{
    public class adminController : Controller
    {
        // GET: admin
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Index()
        {
            try
            {
                VC_LMSEntities d = new VC_LMSEntities();
                int c = 0;
            
                ViewBag.Name = Session["userName"].ToString();
                return View();
            }
            catch (Exception ex)
            {
                return View("Index","Login");
            }
        }

        // GET: admin/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: admin/Create
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Create()
        {
            VC_LMSEntities d = new VC_LMSEntities();
            List<department_tbl> l = new List<department_tbl>();
            List<RolesTable> r = new List<RolesTable>();
            foreach(var item in d.RolesTables)
            {
                if (item.NAME != "Admin")
                {
                    r.Add(item);
                }
            }
            //ViewBag.Managers = new SelectList(d.managers, "manager_emp_id", "name");
            ViewBag.Departments = new SelectList(d.department_tbl, "DEPARTMENT", "DEPARTMENT");
            ViewBag.Roles = new SelectList(r, "NAME", "NAME");
            return View();
        }

        // POST: admin/Create
        [HttpPost]
        public ActionResult Create(LeaveManagementSystemValueCreed.Models.UsersTable usr)
        {
            try
            {
                bool t = usr.EMAIL.Contains("valuecreed.com");
                if (t == false)
                {
                    ViewBag.Message = "Please Enter internal Email of ValueCreed";
                    return View();
                }
                VC_LMSEntities e = new VC_LMSEntities();
                //var managername = e.UsersTables.Where(x => x.EMPLOYEE_ID == usr.manager_id).FirstOrDefault();
                //if (managername == null)
                //{
                //    ViewBag.Message = "Please Select Manager ";
                //    return View();
                //}
                //usr.manager_name = managername.FIRST_NAME;
                //var row = e.UsersTables.Where(x => x.EMPLOYEE_ID == usr.manager_id).FirstOrDefault();

                List<department_tbl> l = new List<department_tbl>();
                List<RolesTable> r = new List<RolesTable>();
                foreach (var item in e.RolesTables)
                {
                    if (item.NAME != "Admin")
                    {
                        r.Add(item);
                    }
                }
                //ViewData["Manager"] = new SelectList(e.managers, "name", "name");
                ViewData["Department"] = new SelectList(e.department_tbl, "DEPARTMENT", "DEPARTMENT");
                ViewData["Roles"] = new SelectList(r, "NAME", "NAME");
                VC_LMSEntities d = new VC_LMSEntities();
                if (1 <= usr.Emp_start_date.Month && usr.Emp_start_date.Month < 3)
                {
                    if (usr.Emp_start_date.Date < new DateTime(usr.Emp_start_date.Year, 1, 15))
                    {
                        usr.leaves_alloted = 5;
                    }
                    else if (usr.Emp_start_date.Date > new DateTime(usr.Emp_start_date.Year, 1, 15) && usr.Emp_start_date.Date < new DateTime(usr.Emp_start_date.Year, 2, 15))
                    {
                        usr.leaves_alloted = 3;
                    }
                    else if (usr.Emp_start_date.Date > new DateTime(usr.Emp_start_date.Year, 2, 15))
                    {
                        usr.leaves_alloted = 1;
                    }
                }
                else if (4 <= usr.Emp_start_date.Month && usr.Emp_start_date.Month < 6)
                {
                    if (usr.Emp_start_date.Date < new DateTime(usr.Emp_start_date.Year, 4, 15))
                    {
                        usr.leaves_alloted = 5;
                    }
                    else if (usr.Emp_start_date.Date > new DateTime(usr.Emp_start_date.Year, 4, 15) && usr.Emp_start_date.Date < new DateTime(usr.Emp_start_date.Year, 5, 15))
                    {
                        usr.leaves_alloted = 3;
                    }
                    else if (usr.Emp_start_date.Date > new DateTime(usr.Emp_start_date.Year, 5, 15))
                    {
                        usr.leaves_alloted = 1;
                    }
                }
                else if (7 <= usr.Emp_start_date.Month && usr.Emp_start_date.Month < 9)
                {


                    if (usr.Emp_start_date.Date < new DateTime(usr.Emp_start_date.Year, 7, 15))
                    {
                        usr.leaves_alloted = 5;
                    }
                    else if (usr.Emp_start_date.Date > new DateTime(usr.Emp_start_date.Year, 7, 15) && usr.Emp_start_date.Date < new DateTime(usr.Emp_start_date.Year, 8, 15))
                    {
                        usr.leaves_alloted = 3;
                    }
                    else if (usr.Emp_start_date.Date > new DateTime(usr.Emp_start_date.Year, 8, 15))
                    {
                        usr.leaves_alloted = 1;
                    }
                }
                else if (10 <= usr.Emp_start_date.Month && usr.Emp_start_date.Month < 12)
                {
                    if (usr.Emp_start_date.Date < new DateTime(usr.Emp_start_date.Year, 10, 15))
                    {
                        usr.leaves_alloted = 5;
                    }
                    else if (usr.Emp_start_date.Date > new DateTime(usr.Emp_start_date.Year, 10, 15) && usr.Emp_start_date.Date < new DateTime(usr.Emp_start_date.Year, 11, 15))
                    {
                        usr.leaves_alloted = 3;
                    }
                    else if (usr.Emp_start_date.Date > new DateTime(usr.Emp_start_date.Year, 11, 15))
                    {
                        usr.leaves_alloted = 1;
                    }
                }
                using (VC_LMSEntities dd = new VC_LMSEntities())
                {
                    var user = dd.UsersTables.Where(x => x.EMAIL == usr.EMAIL).FirstOrDefault();
                    if (user != null)
                    {
                        ViewBag.Message = "Email Already Present";
                        return View();
                    }



                    d.UsersTables.Add(usr);
                    d.SaveChanges();
                }

                ModelState.Clear();
                ViewBag.Meassage = "Creation Success";
                return View();
            }
            catch (Exception ex)
            {

                ViewBag.Message = "Some Invalid Values have been given,Please Retry";
                return View();
            }
        }

        // GET: admin/Edit/5
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Edit(string id)
        {
            VC_LMSEntities d = new VC_LMSEntities();
            List<department_tbl> l = new List<department_tbl>();
            List<RolesTable> r = new List<RolesTable>();
            foreach (var item in d.RolesTables)
            {
                if (item.NAME != "Admin")
                {
                    r.Add(item);
                }
            }
            //ViewBag.Managers = new SelectList(d.managers, "manager_emp_id", "name");
            ViewBag.Departments = new SelectList(d.department_tbl, "DEPARTMENT", "DEPARTMENT");
            ViewBag.Roles = new SelectList(r, "NAME", "NAME");

            return View(d.UsersTables.Where(x => x.EMPLOYEE_ID == id).FirstOrDefault());
        }

        // POST: admin/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, LeaveManagementSystemValueCreed.Models.UsersTable user)
        {
            try
            {
                VC_LMSEntities d = new VC_LMSEntities();
                List<RolesTable> r = new List<RolesTable>();
                foreach (var item in d.RolesTables)
                {
                    if (item.NAME != "Admin")
                    {
                        r.Add(item);
                    }
                }


                //ViewBag.Managers = new SelectList(d.managers, "manager_emp_id", "name");
                ViewBag.Departments = new SelectList(d.department_tbl, "DEPARTMENT", "DEPARTMENT");
                ViewBag.Roles = new SelectList(r, "NAME", "NAME");
                if (user.Emp_end_date < user.Emp_start_date)
                {
                    ViewBag.Message = "Date of Joining Can't be Before Date of Leaving";
                    return View();
                }
                // TODO: Add update logic here
                using (VC_LMSEntities db = new VC_LMSEntities())
                {
                 
                    //var row = db.managers.Where(x => x.manager_emp_id == manger_id).FirstOrDefault();
                    //if (row != null)
                    //{
                    //    user.manager_name = row.name;
                    //}
                    //else
                    //{
                    //    ViewBag.Message = "No Such Manager Exists,Please give correct input or Create a manager";
                    //    return View();
                    //}


                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: admin/Delete/5
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Delete(string id)
        {
            VC_LMSEntities d = new VC_LMSEntities();
            return View(d.UsersTables.Where(x => x.EMPLOYEE_ID == id).FirstOrDefault());
           
        }

        // POST: admin/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                using (VC_LMSEntities d = new VC_LMSEntities())
                {
                    var row = d.UsersTables.Where(x => x.EMPLOYEE_ID == id && x.ROLES == "Admin").FirstOrDefault();
                    if (row != null)
                    {
                        ViewBag.Message = "Cannot delete Admin";
                        return View();
                    }
                    foreach (var item in d.Leaves)
                    {
                        if (item.Employee_Id == id)
                        {
                            d.Leaves.Remove(item);
                        }
                    }
                    UsersTable usertable = d.UsersTables.Where(x => x.EMPLOYEE_ID == id).FirstOrDefault();
                    d.UsersTables.Remove(usertable);
                    d.SaveChanges();
                }

                return RedirectToAction("view");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Manager()
        {
            List<UsersTable> l = new List<UsersTable>();
            VC_LMSEntities d = new VC_LMSEntities();
            foreach (var item in d.UsersTables)
            {
                l.Add(item);
            }
            ViewData["Manager"] = new SelectList(l, "EMPLOYEE_ID", "FIRST_NAME");
            return View();
        }
        [HttpPost]
        //public ActionResult Manager(LeaveManagementSystemValueCreed.Models.manager manager)
        //{

        //    VC_LMSEntities dd = new VC_LMSEntities();
        //    var row1 = dd.UsersTables.Where(x => x.EMPLOYEE_ID == manager.manager_emp_id).FirstOrDefault();
        //    if (row1 == null)
        //    {
        //        ViewBag.Message = "Cannot Make a Manager who is not an Employee";
        //        return View();
        //    }
        //    //List<manager> l = new List<manager>();
        //    //foreach (var item in dd.managers)
        //    //{
        //    //    l.Add(item);
        //    //}
        //    //ViewData["Manager"] = new SelectList(l, "manager_emp_id", "name");
        //    //using (VC_LMSEntities d = new VC_LMSEntities())
        //    //{

        //    //    d.managers.Add(manager);
        //    //    d.SaveChanges();
        //    //}
        //    //ModelState.Clear();
        //    //ViewBag.SuccessMeassage = "Creation Success";
        //    //return View("Manager", new manager());
        //}
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult view()
        {
            VC_LMSEntities d = new VC_LMSEntities();
            //List<UsersTable> user = new List<UsersTable>();
            List<UsersTable> user = d.UsersTables.ToList();
            //foreach (var item in d.UsersTables)
            //{
            //    if (item.EMPLOYEE_ID != (Session["usrID"]).ToString())
            //    {
            //        user.Add(item);
            //    }
            //}
            return View(user);
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Leave()
        {
            try
            {
                VC_LMSEntities d = new VC_LMSEntities();
                List<Leave> leave = new List<Leave>();
                foreach (var item in d.Leaves)
                {
                    if (item.Status != "Cancelled")
                    {
                        leave.Add(item);
                    }
                }
                return View(leave);
            }
            catch(Exception ex)
            {
                return View();
            }
        }

     
    }
}
