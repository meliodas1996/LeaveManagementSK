using DotNetOpenAuth;
using LeaveManagementSystemValueCreed.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace LeaveManagementSystemValueCreed.Controllers
{
    public class HRController : Controller
    {
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        // GET: HR
        public ActionResult Index()
        {
            try
            {
                VC_LMSEntities d = new VC_LMSEntities();
                int c = 0;
                var id = (Session["userID"].ToString());
                var user = d.UsersTables.Where(x => x.EMPLOYEE_ID == id).FirstOrDefault();

                string b = user.leaves_alloted.ToString();
                ViewBag.leave = b;
                ViewBag.Name = Session["userName"].ToString();
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: HR/Details/5
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Details(string id)
        {
            VC_LMSEntities d = new VC_LMSEntities();
            return View(d.UsersTables.Where(x => x.EMPLOYEE_ID == id).FirstOrDefault());
        }

        // GET: HR/Create
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Create()
        {
            VC_LMSEntities d = new VC_LMSEntities();
            //List<manager> l = new List<manager>();
            List<RolesTable> r = new List<RolesTable>();
            foreach (var item in d.RolesTables)
            {
                if (item.NAME != "Admin")
                {
                    r.Add(item);
                }
            }
            //foreach (var item in d.managers)
            //{
            //    l.Add(item);
            //}
            //ViewData["Manager"] = new SelectList(l, "manager_emp_id", "name");
            ViewData["Department"] = new SelectList(d.department_tbl, "DEPARTMENT", "DEPARTMENT");
            ViewData["Roles"] = new SelectList(r, "NAME", "NAME");
            return View();
        }

        // POST: HR/Create
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
              
                ViewData["Department"] = new SelectList(e.department_tbl, "DEPARTMENT", "DEPARTMENT");
                ViewData["Roles"] = new SelectList(r, "NAME", "NAME");
                VC_LMSEntities d = new VC_LMSEntities();
                if (1 <= usr.Emp_start_date.Month && usr.Emp_start_date.Month <= 3)
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
                else if (4 <= usr.Emp_start_date.Month && usr.Emp_start_date.Month <= 6)
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
                else if (7 <= usr.Emp_start_date.Month && usr.Emp_start_date.Month <= 9)
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
                else if (10 <= usr.Emp_start_date.Month && usr.Emp_start_date.Month <= 12)
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

                //ViewBag.Message = ex.InnerException.Message.ToString();
                //    /* "Some Invalid Values have been given,Please Retry"*/
                return View();
            }
        }

        // GET: HR/Edit/5
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

        // POST: HR/Edit/5
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
                    //    var manger_id = user.manager_id;
                    //    var row = db.managers.Where(x => x.manager_emp_id == manger_id).FirstOrDefault();
                    //    if (row != null)
                    //    {
                    //        user.manager_name = row.name;
                    //    }
                    //    else
                    //    {
                    //        ViewBag.Message = "No Such Manager Exists,Please give correct input or Create a manager";
                    //        return View();
                    //    }


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

        // GET: HR/Delete/5
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Delete(int id)
        {
            VC_LMSEntities d = new VC_LMSEntities();
            List<approver> app = new List<approver>();

            return View(d.approvers.Where(x => x.ID == id).FirstOrDefault());
        }

        // POST: HR/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                using (VC_LMSEntities d = new VC_LMSEntities())
                {

                    approver usertable = d.approvers.Where(x => x.ID == id).FirstOrDefault();
                    d.approvers.Remove(usertable);
                    d.SaveChanges();
                    return RedirectToAction("approvers");
                }
            }
            catch
            {
                return View();
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult view()
        {
            VC_LMSEntities d = new VC_LMSEntities();
            //List<UsersTable> user = new List<UsersTable>();
            List<UsersTable> user = new List<UsersTable>();
            foreach(var item in d.UsersTables)
            {
                if (item.Status == "Active")
                {
                    user.Add(item);
                }
            }
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
            ViewBag.message = Session["Message"];
            VC_LMSEntities d = new VC_LMSEntities();

            List<Leave> leave = new List<Leave>();
            foreach (var item in d.Leaves)
            {
                if (item.Status == "Approved By All Approvers"||item.Status=="Pending on HR"||item.Status== "Cancellation Pending on HR")
                {
                    leave.Add(item);
                }
            }
            return View(leave);
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Editleave(int id)
        {

            VC_LMSEntities d = new VC_LMSEntities();
            var row = d.Leaves.Where(x => x.ID == id).FirstOrDefault();
            if (row.Status == "Approved By All Approvers" || row.Status == "Pending on HR"||row.Status== "Cancellation Pending on HR")
            {
                return View(d.Leaves.Where(x => x.ID == id).FirstOrDefault());
            }
            else
            {
                ViewBag.Message = "Cannot Edit Selected Leave";
                Session["Message"] = ViewBag.Message;
                return RedirectToAction("Leave");
            }
        }
        [HttpPost]
        public ActionResult Editleave(Leave leave)
        {
            VC_LMSEntities h = new VC_LMSEntities();
            var approval_row = h.approvals.Where(r => r.Leave_id == leave.ID && r.status == "Approved By HR").FirstOrDefault();
            if (approval_row != null)
            {
                if (leave.Status == "Approved By HR")
                {
                    leave.Status = "Cancellation Approved";
                    var user = h.UsersTables.Where(u => u.EMPLOYEE_ID == leave.Employee_Id).FirstOrDefault();
                    user.leaves_alloted += leave.Number_of_Days;
                    h.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    h.Entry(leave).State = System.Data.Entity.EntityState.Modified;
                    h.SaveChanges();
                    return RedirectToAction("Index", "HR");
                }
            }


            using (VC_LMSEntities db = new VC_LMSEntities())
            {
                var row = db.UsersTables.Where(x => x.EMPLOYEE_ID == leave.Employee_Id).FirstOrDefault();
                if (leave.Status == "Approved By HR")
                {
                    db.Entry(leave).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    //var row = db.user_info.Where(x => x.EMPLOYEE_ID == leave.Employee_Id).FirstOrDefault();
                    row.leaves_alloted = row.leaves_alloted - leave.Number_of_Days;
                    var l = db.Leaves.Where(x => x.ID == leave.ID).FirstOrDefault();
                    l.Leave_balance = row.leaves_alloted;
                    l.Leave_debit = leave.Number_of_Days;
                    db.Entry(l).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(row).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    approval approval1 = new approval();
                    approval1.Aprrover_id = Session["userID"].ToString();
                    approval1.Employee_id = leave.Employee_Id;
                    approval1.Leave_id = leave.ID;
                    approval1.status = "Approved By HR";
                    db.approvals.Add(approval1);
                    db.SaveChanges();
                }
                else if (leave.Status == "Rejected By HR")
                {
                    db.Entry(leave).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    approval approval1 = new approval();
                    approval1.Aprrover_id = Session["userID"].ToString();
                    approval1.Employee_id = leave.Employee_Id;
                    approval1.Leave_id = leave.ID;
                    approval1.status = "Rejected By HR";
                    db.approvals.Add(approval1);
                    db.SaveChanges();
                }
                string employeename = row.FIRST_NAME;
                string EmailID = row.EMAIL;
                string body = "Hello " + employeename +",< br /> Please see Your Leave Status On LMS website as the Status of your LEave has changed. <br /> <br /> Warm Regards,< br />LMS Team." ;
                //SendverificationLinkEmail(EmailID, body);

                ////var mangermail = db.UsersTables.Where(r => r.EMPLOYEE_ID == row.manager_id).FirstOrDefault();
                ////string emailid = mangermail.EMAIL;
                //body = "Hi,</br> PLease see the Leave status   for the concerned employee as it has changed:" + employeename;
                //SendverificationLinkEmail(emailid, body);

            }
            // TODO: Add update logic here

            return RedirectToAction("Index");

            //catch (Exception ex)
            //{
            //    string s = ex.Message;
            //    return View();
            //}
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult LeaveApply()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LeaveApply(LeaveManagementSystemValueCreed.Models.Leave leave)
        {
            var indian_zone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            if (DateTime.Now.Date.Kind == DateTimeKind.Utc)
            {
                if (leave.Start_Date.Subtract(TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.Date, indian_zone)).Days > 360)
                {
                    ViewBag.Message = "Cannot Apply for such date,as it is too Far";
                    return View();
                }
            }
            else
            {

                if (leave.Start_Date.Subtract(DateTime.Now.Date).Days > 360)
                {
                    ViewBag.Message = "Cannot Apply for such date,as it is too Far";
                    return View();
                }
            }
            bool status = false;
            string message = "";
            if (DateTime.Now.Date.Kind != DateTimeKind.Utc)
            {
                var datediff = DateTime.Now.Date.Subtract(leave.End_Date);
                if (datediff.TotalDays > 0)
                {
                    ViewBag.Message = "Cannot Apply Leave  if End Date is in the Past";
                    ViewBag.Status = false;
                    return View();
                }
            }
            else if (DateTime.Now.Date.Kind == DateTimeKind.Utc)
            {
                var datediff = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.Date, indian_zone).Subtract(leave.End_Date);
                if (datediff.TotalDays > 0)
                {
                    ViewBag.Message = "Cannot Apply Leave  if End Date is in the Past";
                    ViewBag.Status = false;
                    return View();
                }
            }
            if (leave.End_Date > leave.Start_Date)
            {
                int c = 0;
                VC_LMSEntities dd = new VC_LMSEntities();
                foreach (var item in dd.Leaves)
                {
                    if ((Session["userID"]).ToString() == item.Employee_Id)
                    {
                        if ((item.Start_Date < leave.Start_Date) && (leave.End_Date < item.End_Date) && (item.Status != "Cancelled"))
                        {
                            ViewBag.Message = "Leave range falling under Another Leave Range ";
                            ViewBag.Status = false;
                            ModelState.Clear();
                            return View();
                        }
                        else if ((item.Start_Date < leave.Start_Date) && (leave.End_Date < item.End_Date) && (item.Status != "Cancelled"))
                        {
                            ViewBag.Message = "Leave date conflict,Please Apply after previous leave Ends ";
                            ViewBag.Status = false;
                            ModelState.Clear();
                            return View();
                        }
                        else if ((item.Start_Date < leave.Start_Date) && (leave.Start_Date < item.End_Date) && (item.Status != "Cancelled"))
                        {
                            ViewBag.Message = "Leave date conflict,Please Apply after previous leave Ends,or Edit ";
                            ViewBag.Status = false;
                            ModelState.Clear();
                            return View();
                        }
                        if ((leave.Start_Date == item.Start_Date || leave.End_Date == item.End_Date) && item.Status != "Cancelled")
                        {
                            ViewBag.Message = "Matching Date For Already Applied Leave Found,Please Edit that Leave ";
                            ViewBag.Status = false;
                            ModelState.Clear();
                            return View();
                        }
                    }
                    if (item.Employee_Id == (Session["userID"].ToString()) && item.Status == "Pending on Approvers")
                    {
                        c += item.Number_of_Days;
                    }
                }
                string x = (Session["userID"].ToString());
                var account = dd.UsersTables.Where(a => a.EMPLOYEE_ID == x).FirstOrDefault();

                if (account.leaves_alloted == 0)
                {
                    message = "Sorry Your Leave Balance is Zero and you cannot apply";
                    //return RedirectToAction("ApplyLeave");
                    status = false;
                }
                else
                {
                    int e = 0;
                    var number_of_day = leave.End_Date.Date.Subtract(leave.Start_Date);
                    int number_of_days = 0;
                    number_of_days = number_of_day.Days+1;
                    int d = 0;
                    VC_LMSEntities4 p = new VC_LMSEntities4();
                    for (DateTime time = leave.Start_Date; time <= leave.End_Date; time = time.AddDays(1))
                    {
                        if (time.DayOfWeek == DayOfWeek.Saturday || time.DayOfWeek == DayOfWeek.Sunday)
                        {
                            e++;
                        }
                        foreach (var item in p.holidays)
                        {
                            if (item.Holiday_date == time)
                                e++;

                        }
                    }

                    number_of_days = number_of_days - e;
                    if ((c + number_of_days > account.leaves_alloted))
                    {
                        message = "Already Applied leaves Exceeds Your Leave Balance";
                        status = false;
                        ViewBag.Message = message;
                        ViewBag.Status = status;

                    }
                    else if (number_of_days <= account.leaves_alloted)
                    {
                        try
                        {

                            leave.Start_Date = leave.Start_Date.Date;
                            leave.End_Date = leave.End_Date.Date;

                            leave.Number_of_Days = number_of_days - e;
                            if (DateTime.Now.Date.Kind == DateTimeKind.Utc)
                            {
                                leave.Leave_apply_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.Date, indian_zone);
                            }
                            else if (DateTime.Now.Date.Kind != DateTimeKind.Utc)
                            {
                                leave.Leave_apply_date = DateTime.Now.Date;
                            }
                            leave.Status = "Pending on Approvers";
                            leave.Employe_name = Session["userName"].ToString();
                            leave.Employee_Id = (Session["userID"].ToString());
                            using (VC_LMSEntities f = new VC_LMSEntities())
                            {
                                var userinfo = f.UsersTables.Where(s => s.EMPLOYEE_ID == leave.Employee_Id).FirstOrDefault();
                                string employeename = userinfo.FIRST_NAME;
                                //var row2 = f.UsersTables.Where(m => m.EMPLOYEE_ID == userinfo.manager_id).FirstOrDefault();
                                //if (row2 != null)
                                //{
                                //    string EmailID = row2.EMAIL;
                                //    string body = "Hi,</br> We Got your request for Leave from" + employeename;
                                //    //SendverificationLinkEmail(EmailID, body);
                                //}

                                f.Leaves.Add(leave);
                                f.SaveChanges();
                                ModelState.Clear();
                                message = "Successfully Applied";
                                ViewBag.Status = status;
                            }
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "Exclude Special Characters from Reason";
                            return View();
                        }

                    }
                    else
                    {
                        message = "Leave Applied for is greater than your Leave Balance";
                        ViewBag.Status = false;
                    }
                }
            }
            else if (leave.End_Date < leave.Start_Date)
            {
                message = "End Date should be greater then Start Date";
                status = false;
            }
            else if ((leave.End_Date == leave.Start_Date) && (leave.Start_Date == null))
            {
                message = "Cannot pass Null in Required Field";
                status = false;
            }
            else if (leave.End_Date == leave.Start_Date)
            {
                try
                {
                    if (leave.Start_Date.DayOfWeek == DayOfWeek.Saturday || leave.Start_Date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        ViewBag.Message = "You are applying Leave on day which is already a Holiday";
                        return View();
                    }
                    else
                    {
                        leave.Start_Date = leave.Start_Date.Date;
                        leave.End_Date = leave.End_Date.Date;

                        leave.Number_of_Days = 1;
                        if (DateTime.Now.Date.Kind == DateTimeKind.Utc)
                        {
                            leave.Leave_apply_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.Date, indian_zone);
                        }
                        else if (DateTime.Now.Date.Kind != DateTimeKind.Utc)
                        {
                            leave.Leave_apply_date = DateTime.Now.Date;
                        }
                        leave.Status = "Pending on Approvers";
                        leave.Employe_name = Session["userName"].ToString();
                        leave.Employee_Id = (Session["userID"].ToString());
                        using (VC_LMSEntities f = new VC_LMSEntities())
                        {

                            f.Leaves.Add(leave);
                            f.SaveChanges();
                            ModelState.Clear();
                            message = "Successfully Applied";
                            ViewBag.Status = status;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Exclude Special Characters from Reason";
                    return View();
                }


            }
            ViewBag.Message = message;
            ViewBag.Status = status;
            ModelState.Clear();
            return View();


        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult myleave()
        {
            try
            {
                List<Leave> l = new List<Leave>();
                VC_LMSEntities ds = new VC_LMSEntities();
                foreach (var item in ds.Leaves)
                {
                    if (item.Employee_Id.ToString() == Session["userID"].ToString())
                    {
                        l.Add(item);
                    }
                }
                List<Leave> leaves = ds.Leaves.ToList();
                return View(l);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Editmyleave(int id)
        {
            VC_LMSEntities d = new VC_LMSEntities();
            if (d.Leaves.Where(x => x.ID == id && ( x.Status == "Pending on Approvers")).FirstOrDefault() != null)
            {
                return View(d.Leaves.Where(x => x.ID == id).FirstOrDefault());
            }
            else
            {
                ViewBag.Message = "Selected Leave Can't be Edited";
                ViewBag.Status = false;
                return RedirectToAction("myleave");
            }

        }
        [HttpPost]
        public ActionResult Editmyleave(int id, LeaveManagementSystemValueCreed.Models.Leave leave)
        {
            if (leave.Status == "Cancel")
            {
                VC_LMSEntities d = new VC_LMSEntities();
                leave.Status = "Cancelled";
                d.Entry(leave).State = System.Data.Entity.EntityState.Modified;
                d.SaveChanges();
                return RedirectToAction("Index");
            }
            try
            {
                var indian_zone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                if (DateTime.Now.Date.Kind == DateTimeKind.Utc)
                {
                    if (leave.Start_Date.Subtract(TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.Date, indian_zone)).Days > 360)
                    {
                        ViewBag.Message = "Cannot Apply for such date,as it is too Far";
                        return View();
                    }
                }
                else
                {

                    if (leave.Start_Date.Subtract(DateTime.Now.Date).Days > 360)
                    {
                        ViewBag.Message = "Cannot Apply for such date,as it is too Far";
                        return View();
                    }
                }
                bool status = false;
                string message = "";
                if (DateTime.Now.Date.Kind != DateTimeKind.Utc)
                {
                    var datediff = DateTime.Now.Date.Subtract(leave.End_Date);
                    if (datediff.TotalDays > 0)
                    {
                        ViewBag.Message = "Cannot Apply Leave  if End Date is in the Past";
                        ViewBag.Status = false;
                        return View();
                    }
                }
                else if (DateTime.Now.Date.Kind == DateTimeKind.Utc)
                {
                    var datediff = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.Date, indian_zone).Subtract(leave.End_Date);
                    if (datediff.TotalDays > 0)
                    {
                        ViewBag.Message = "Cannot Apply Leave  if End Date is in the Past";
                        ViewBag.Status = false;
                        return View();
                    }
                }
                if (leave.End_Date > leave.Start_Date)
                {
                    int c = 0;
                    VC_LMSEntities dd = new VC_LMSEntities();
                    foreach (var item in dd.Leaves)
                    {
                        if ((Session["userID"]).ToString() == item.Employee_Id)
                        {
                            if ((item.Start_Date < leave.Start_Date) && (leave.End_Date < item.End_Date) && (item.Status != "Cancelled" && leave.ID != item.ID))
                            {
                                ViewBag.Message = "Leave range falling under Another Leave Range ";
                                ViewBag.Status = false;
                                ModelState.Clear();
                                return View();
                            }
                            else if ((item.Start_Date < leave.Start_Date) && (leave.End_Date < item.End_Date) && (item.Status != "Cancelled" && leave.ID != item.ID))
                            {
                                ViewBag.Message = "Leave date conflict,Please Apply after previous leave Ends ";
                                ViewBag.Status = false;
                                ModelState.Clear();
                                return View();
                            }
                            else if ((item.Start_Date < leave.Start_Date) && (leave.Start_Date < item.End_Date) && (item.Status != "Cancelled" && leave.ID != item.ID))
                            {
                                ViewBag.Message = "Leave date conflict,Please Apply after previous leave Ends,or Edit ";
                                ViewBag.Status = false;
                                ModelState.Clear();
                                return View();
                            }
                            if ((leave.Start_Date == item.Start_Date || leave.End_Date == item.End_Date) && item.Status != "Cancelled" && leave.ID!=item.ID)
                            {
                                ViewBag.Message = "Matching Date For Already Applied Leave Found,Please Edit that Leave ";
                                ViewBag.Status = false;
                                ModelState.Clear();
                                return View();
                            }
                        }
                        if (item.Employee_Id == (Session["userID"].ToString()) && item.Status == "Pending on Approvers")
                        {
                            c += item.Number_of_Days;
                        }
                    }
                    string x = (Session["userID"].ToString());
                    var account = dd.UsersTables.Where(a => a.EMPLOYEE_ID == x).FirstOrDefault();

                    if (account.leaves_alloted == 0)
                    {
                        message = "Sorry Your Leave Balance is Zero and you cannot apply";
                        //return RedirectToAction("ApplyLeave");
                        status = false;
                    }
                    else
                    {
                        int e = 0;
                        var number_of_day = leave.End_Date.Date.Subtract(leave.Start_Date);
                        int number_of_days = 0;
                        number_of_days = number_of_day.Days + 1;
                        int d = 0;
                        VC_LMSEntities4 p = new VC_LMSEntities4();
                        for (DateTime time = leave.Start_Date; time <= leave.End_Date; time = time.AddDays(1))
                        {
                            if (time.DayOfWeek == DayOfWeek.Saturday || time.DayOfWeek == DayOfWeek.Sunday)
                            {
                                e++;
                            }
                            foreach (var item in p.holidays)
                            {
                                if (item.Holiday_date == time)
                                    e++;

                            }
                        }

                        number_of_days = number_of_days - e;
                        if ((c + number_of_days > account.leaves_alloted))
                        {
                            message = "Already Applied leaves Exceeds Your Leave Balance";
                            status = false;
                            ViewBag.Message = message;
                            ViewBag.Status = status;

                        }
                        else if (number_of_days <= account.leaves_alloted)
                        {
                            try
                            {

                                leave.Start_Date = leave.Start_Date.Date;
                                leave.End_Date = leave.End_Date.Date;

                                leave.Number_of_Days = number_of_days ;
                                if (DateTime.Now.Date.Kind == DateTimeKind.Utc)
                                {
                                    leave.Leave_apply_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.Date, indian_zone);
                                }
                                else if (DateTime.Now.Date.Kind != DateTimeKind.Utc)
                                {
                                    leave.Leave_apply_date = DateTime.Now.Date;
                                }
                                leave.Status = "Pending on Approvers";
                                leave.Employe_name = Session["userName"].ToString();
                                leave.Employee_Id = (Session["userID"].ToString());
                                using (VC_LMSEntities f = new VC_LMSEntities())
                                {
                           

                                    f.Entry(leave).State = System.Data.Entity.EntityState.Modified;
                                    ViewBag.Message = "Changes Saved";    
                                     f.SaveChanges();    
                                    message = "Successfully Applied";
                                    ViewBag.Status = status;
                                }
                            }
                            catch (Exception ex)
                            {
                                ViewBag.Message = "Exclude Special Characters from Reason";
                                return View();
                            }

                        }
                        else
                        {
                            message = "Leave Applied for is greater than your Leave Balance";
                            ViewBag.Status = false;
                        }
                    }
                }
                else if (leave.End_Date < leave.Start_Date)
                {
                    message = "End Date should be greater then Start Date";
                    status = false;
                }
                else if ((leave.End_Date == leave.Start_Date) && (leave.Start_Date == null))
                {
                    message = "Cannot pass Null in Required Field";
                    status = false;
                }
                else if (leave.End_Date == leave.Start_Date)
                {
                    try
                    {
                        if (leave.Start_Date.DayOfWeek == DayOfWeek.Saturday || leave.Start_Date.DayOfWeek == DayOfWeek.Sunday)
                        {
                            ViewBag.Message = "You are applying Leave on day which is already a Holiday";
                            return View();
                        }
                        else
                        {
                            leave.Start_Date = leave.Start_Date.Date;
                            leave.End_Date = leave.End_Date.Date;

                            leave.Number_of_Days = 1;
                            if (DateTime.Now.Date.Kind == DateTimeKind.Utc)
                            {
                                leave.Leave_apply_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.Date, indian_zone);
                            }
                            else if (DateTime.Now.Date.Kind != DateTimeKind.Utc)
                            {
                                leave.Leave_apply_date = DateTime.Now.Date;
                            }
                            leave.Status = "Pending on Approvers";
                            leave.Employe_name = Session["userName"].ToString();
                            leave.Employee_Id = (Session["userID"].ToString());
                            using (VC_LMSEntities f = new VC_LMSEntities())
                            {

                                f.Entry(leave).State = System.Data.Entity.EntityState.Modified;
                                ViewBag.Message = "Changes Saved";
                                 f.SaveChanges();
                                message = "Successfully Edited";
                                ViewBag.Status = status;
                                return RedirectToAction("Index");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "Exclude Special Characters from Reason";
                        return View();
                    }


                }
   
                //using (VC_LMSEntities db = new VC_LMSEntities())
                //{
                //    var developer = "Sumit K";
                //    var days = leave.End_Date.Subtract(leave.Start_Date);
                //    int day = days.Days;
                //    leave.Number_of_Days = day;

                //    db.Entry(leave).State = System.Data.Entity.EntityState.Modified;
                //    ViewBag.Message = "Changes Saved";
                //    db.SaveChanges();
                //}
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return View();
            }
        }
        public ActionResult Update()
        {
            //using (IEnumerator<leavemangementValueCreed.Models.user_info> User=new IEnumerator<leavemangementValueCreed.Models.user_info>)
            Models.UsersTable info;

            using (VC_LMSEntities d = new VC_LMSEntities())
            {
                var indian_zone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                List<UsersTable> user = new List<UsersTable>();
                if (DateTime.Now.Kind == DateTimeKind.Utc)
                {
                    if (1 == TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now, indian_zone).Month)
                    {
                        foreach (var item in d.UsersTables.ToList())
                        {
                            if (item.leaves_alloted > 10)
                            {
                                item.leaves_alloted = 10 + 5;

                            }
                            else if (item.leaves_alloted < 10)
                            {
                                item.leaves_alloted = item.leaves_alloted + 5;

                            }
                            //item.FIRST_NAME = item.FIRST_NAME;
                            //item.LAST_NAME = item.LAST_NAME;
                            //item.EMPLOYEE_ID = item.EMPLOYEE_ID;
                            //item.EMAIL = item.EMAIL;
                            //item.PHONE = item.PHONE;
                            //item.ADDRESS = item.ADDRESS;
                            //item.ALTERNATE_PHONE = item.ALTERNATE_PHONE;
                            //item.EMERGENCY_NUMBER = item.EMERGENCY_NUMBER;
                            //item.LOCATION = item.LOCATION;
                            //item.DESCRIPTION = item.DESCRIPTION;
                            //item.GENDER = item.GENDER;

                            //item.Emp_start_date = item.Emp_start_date;
                            //item.Emp_end_date = item.Emp_end_date;
                            //item.manager_id = item.manager_id;
                            //item.Status = item.Status;
                            //item.ROLES = item.ROLES;
                            //item.DEPARTMENT = item.DEPARTMENT;
                            d.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            d.SaveChanges();

                        }



                    }
                    else if (3 == TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now, indian_zone).Month)
                    {
                        foreach (var item in d.UsersTables.ToList())
                        {
                            item.leaves_alloted += 5;


                        }



                    }
                    else if (7 == TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now, indian_zone).Month)
                    {
                        foreach (var item in d.UsersTables.ToList())
                        {
                            item.leaves_alloted += 5;
                            //item.FIRST_NAME = item.FIRST_NAME;
                            //item.LAST_NAME = item.LAST_NAME;
                            //item.EMPLOYEE_ID = item.EMPLOYEE_ID;
                            //item.EMAIL = item.EMAIL;
                            //item.PHONE = item.PHONE;
                            //item.ADDRESS = item.ADDRESS;
                            //item.ALTERNATE_PHONE = item.ALTERNATE_PHONE;
                            //item.EMERGENCY_NUMBER = item.EMERGENCY_NUMBER;
                            //item.LOCATION = item.LOCATION;
                            //item.DESCRIPTION = item.DESCRIPTION;
                            //item.GENDER = item.GENDER;
                            //item.Emp_start_date = item.Emp_start_date;
                            //item.Emp_end_date = item.Emp_end_date;
                            //item.manager_id = item.manager_id;
                            //item.Status = item.Status;
                            //item.ROLES = item.ROLES;
                            //item.DEPARTMENT = item.DEPARTMENT;
                            d.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            d.SaveChanges();
                        }



                    }
                    else if (10 == TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now, indian_zone).Month)
                    {
                        foreach (var item in d.UsersTables.ToList())
                        {


                            item.leaves_alloted += 5;



                            //item.FIRST_NAME = item.FIRST_NAME;
                            //item.LAST_NAME = item.LAST_NAME;
                            //item.EMPLOYEE_ID = item.EMPLOYEE_ID;
                            //item.EMAIL = item.EMAIL;
                            //item.PHONE = item.PHONE;
                            //item.ADDRESS = item.ADDRESS;
                            //item.ALTERNATE_PHONE = item.ALTERNATE_PHONE;
                            //item.EMERGENCY_NUMBER = item.EMERGENCY_NUMBER;
                            //item.LOCATION = item.LOCATION;
                            //item.DESCRIPTION = item.DESCRIPTION;
                            //item.GENDER = item.GENDER;

                            //item.Emp_start_date = item.Emp_start_date;
                            //item.Emp_end_date = item.Emp_end_date;
                            //item.manager_id = item.manager_id;
                            //item.Status = item.Status;
                            //item.ROLES = item.ROLES;
                            //item.DEPARTMENT = item.DEPARTMENT;
                            d.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            d.SaveChanges();

                        }
                        //foreach (var item in d.user_info.ToList())
                        //{
                        //    //item.leaves_alloted += 5;
                        //    //item.FIRST_NAME = item.FIRST_NAME;
                        //    //item.LAST_NAME = item.LAST_NAME;
                        //    //item.EMPLOYEE_ID = item.EMPLOYEE_ID;
                        //    //item.EMAIL = item.EMAIL;
                        //    //item.PHONE = item.PHONE;
                        //    //item.ADDRESS = item.ADDRESS;
                        //    //item.ALTERNATE_PHONE = item.ALTERNATE_PHONE;
                        //    //item.EMERGENCY_NUMBER = item.EMERGENCY_NUMBER;
                        //    //item.LOCATION = item.LOCATION;
                        //    //item.DESCRIPTION = item.DESCRIPTION;
                        //    //item.GENDER = item.GENDER;
                        //    //item.PASS = item.PASS;
                        //    //item.Emp_start_date = item.Emp_start_date;
                        //    //item.Emp_end_date = item.Emp_end_date;
                        //    //item.manager_id = item.manager_id;
                        //    //item.Status = item.Status;
                        //    //item.ROLES = item.ROLES;
                        //    //item.DEPARTMENT = item.DEPARTMENT;

                        //    //d.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        //}

                        //d.SaveChanges();

                    }
                    return RedirectToAction("Index");
                }




                else
                {
                   
                    if (1 == DateTime.Now.Month)
                    {
                        foreach (var item in d.UsersTables.ToList())
                        {
                            if (item.leaves_alloted > 10)
                            {
                                item.leaves_alloted = 10 + 5;

                            }
                            else if (item.leaves_alloted < 10)
                            {
                                item.leaves_alloted = item.leaves_alloted + 5;

                            }
                            //item.FIRST_NAME = item.FIRST_NAME;
                            //item.LAST_NAME = item.LAST_NAME;
                            //item.EMPLOYEE_ID = item.EMPLOYEE_ID;
                            //item.EMAIL = item.EMAIL;
                            //item.PHONE = item.PHONE;
                            //item.ADDRESS = item.ADDRESS;
                            //item.ALTERNATE_PHONE = item.ALTERNATE_PHONE;
                            //item.EMERGENCY_NUMBER = item.EMERGENCY_NUMBER;
                            //item.LOCATION = item.LOCATION;
                            //item.DESCRIPTION = item.DESCRIPTION;
                            //item.GENDER = item.GENDER;

                            //item.Emp_start_date = item.Emp_start_date;
                            //item.Emp_end_date = item.Emp_end_date;
                            //item.manager_id = item.manager_id;
                            //item.Status = item.Status;
                            //item.ROLES = item.ROLES;
                            //item.DEPARTMENT = item.DEPARTMENT;
                            d.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            d.SaveChanges();

                        }



                    }
                    else if (3 == DateTime.Now.Month)
                    {
                        foreach (var item in d.UsersTables.ToList())
                        {
                            item.leaves_alloted += 5;


                        }



                    }
                    else if (7 == DateTime.Now.Month)
                    {
                        foreach (var item in d.UsersTables.ToList())
                        {
                            item.leaves_alloted += 5;
                            //item.FIRST_NAME = item.FIRST_NAME;
                            //item.LAST_NAME = item.LAST_NAME;
                            //item.EMPLOYEE_ID = item.EMPLOYEE_ID;
                            //item.EMAIL = item.EMAIL;
                            //item.PHONE = item.PHONE;
                            //item.ADDRESS = item.ADDRESS;
                            //item.ALTERNATE_PHONE = item.ALTERNATE_PHONE;
                            //item.EMERGENCY_NUMBER = item.EMERGENCY_NUMBER;
                            //item.LOCATION = item.LOCATION;
                            //item.DESCRIPTION = item.DESCRIPTION;
                            //item.GENDER = item.GENDER;
                            //item.Emp_start_date = item.Emp_start_date;
                            //item.Emp_end_date = item.Emp_end_date;
                            //item.manager_id = item.manager_id;
                            //item.Status = item.Status;
                            //item.ROLES = item.ROLES;
                            //item.DEPARTMENT = item.DEPARTMENT;
                            d.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            d.SaveChanges();
                        }



                    }
                    else if (10 == DateTime.Now.Month)
                    {
                        foreach (var item in d.UsersTables.ToList())
                        {


                            item.leaves_alloted += 5;



                            //item.FIRST_NAME = item.FIRST_NAME;
                            //item.LAST_NAME = item.LAST_NAME;
                            //item.EMPLOYEE_ID = item.EMPLOYEE_ID;
                            //item.EMAIL = item.EMAIL;
                            //item.PHONE = item.PHONE;
                            //item.ADDRESS = item.ADDRESS;
                            //item.ALTERNATE_PHONE = item.ALTERNATE_PHONE;
                            //item.EMERGENCY_NUMBER = item.EMERGENCY_NUMBER;
                            //item.LOCATION = item.LOCATION;
                            //item.DESCRIPTION = item.DESCRIPTION;
                            //item.GENDER = item.GENDER;

                            //item.Emp_start_date = item.Emp_start_date;
                            //item.Emp_end_date = item.Emp_end_date;
                            //item.manager_id = item.manager_id;
                            //item.Status = item.Status;
                            //item.ROLES = item.ROLES;
                            //item.DEPARTMENT = item.DEPARTMENT;
                            d.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            d.SaveChanges();

                        }
                        //foreach (var item in d.user_info.ToList())
                        //{
                        //    //item.leaves_alloted += 5;
                        //    //item.FIRST_NAME = item.FIRST_NAME;
                        //    //item.LAST_NAME = item.LAST_NAME;
                        //    //item.EMPLOYEE_ID = item.EMPLOYEE_ID;
                        //    //item.EMAIL = item.EMAIL;
                        //    //item.PHONE = item.PHONE;
                        //    //item.ADDRESS = item.ADDRESS;
                        //    //item.ALTERNATE_PHONE = item.ALTERNATE_PHONE;
                        //    //item.EMERGENCY_NUMBER = item.EMERGENCY_NUMBER;
                        //    //item.LOCATION = item.LOCATION;
                        //    //item.DESCRIPTION = item.DESCRIPTION;
                        //    //item.GENDER = item.GENDER;
                        //    //item.PASS = item.PASS;
                        //    //item.Emp_start_date = item.Emp_start_date;
                        //    //item.Emp_end_date = item.Emp_end_date;
                        //    //item.manager_id = item.manager_id;
                        //    //item.Status = item.Status;
                        //    //item.ROLES = item.ROLES;
                        //    //item.DEPARTMENT = item.DEPARTMENT;

                        //    //d.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        //}

                        //d.SaveChanges();

                    }

                    return RedirectToAction("Index");
                }

            }
        }



        public void SendverificationLinkEmail(string EmailID, string employeename, int number_of_days)
        {
            var fromemailPassword = "Sbinarela@07";
            var fromemail = new MailAddress("sumit.kumar@valuecreed.com", "ValueCreed");
            var toemail = new MailAddress(EmailID);
            var subject = "Leave Application";
            var body = "Hi,</br>An Employee has applied for Leave" + "<br/>Employee applied for leave is" + employeename + "</br> Number of days for the Leave is:" + number_of_days;
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(fromemail.Address, fromemailPassword)
            };

            using (var message = new MailMessage(fromemail, toemail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })

                smtp.Send(message);

        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Approvers()
        {
            VC_LMSEntities d = new VC_LMSEntities();
            List<approver> approver = new List<approver>();
            approver = d.approvers.ToList();
            return View(approver);
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Addapprover()
        {
            VC_LMSEntities d = new VC_LMSEntities();
            ViewData["Emplyoee_id"] = new SelectList(d.UsersTables, "EMPLOYEE_ID", "FIRST_NAME");
            ViewData["approver_id"] = new SelectList(d.UsersTables, "EMPLOYEE_ID", "FIRST_NAME");
            return View();
        }
        [HttpPost]

        public ActionResult Addapprover(LeaveManagementSystemValueCreed.Models.approver approver)
        {
            VC_LMSEntities d = new VC_LMSEntities();
            ViewData["Emplyoee_id"] = new SelectList(d.UsersTables, "EMPLOYEE_ID", "FIRST_NAME");
            ViewData["approver_id"] = new SelectList(d.UsersTables, "EMPLOYEE_ID", "FIRST_NAME");
            if (approver.approver_id == approver.Emplyoee_id)
            {
                ViewBag.Message = "Employee ID cannot be same as Approver ID";
                return View();
            }
           
            var validation = d.approvers.Where(x => x.Emplyoee_id == approver.Emplyoee_id && x.approver_id == approver.approver_id).FirstOrDefault();
            if (validation != null)
            {
                ViewBag.Message = "Approver Already Present";
                return View();
            }
         
            var row = d.UsersTables.Where(x => x.EMPLOYEE_ID == approver.Emplyoee_id).FirstOrDefault();
            approver.Employee_name = row.FIRST_NAME +" "+ row.LAST_NAME;
            var row2 = d.UsersTables.Where(x => x.EMPLOYEE_ID == approver.approver_id).FirstOrDefault();
            approver.Approver_name = row2.FIRST_NAME +" "+ row2.LAST_NAME;
            approver.department = row2.DEPARTMENT;
            d.approvers.Add(approver);
            d.SaveChanges();
            ViewBag.Message = "Approver Creation Successfull";
            ModelState.Clear();
            return View(new approver());
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult holiday()
        {
            VC_LMSEntities4 d = new VC_LMSEntities4();
            List<holiday> holiday = d.holidays.ToList();
            return View(holiday);
        }
        public ActionResult YourApprovers()
        {
            try
            {
                VC_LMSEntities d = new VC_LMSEntities();
                List<approver> approvers = new List<approver>();
                foreach (var item in d.approvers)
                {
                    if (item.Emplyoee_id == Session["userId"].ToString())
                    {
                        approvers.Add(item);
                    }
                }
                return View(approvers);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult approvals(int id)
        {
            VC_LMSEntities d = new VC_LMSEntities();
            List<approval> ap = new List<approval>();
            foreach (var item in d.approvals)
            {
                if (item.Leave_id == id)
                {
                    ap.Add(item);
                }
            }
            return View(ap);
        }
        public ActionResult onLeave()
        {
            List<Leave> leave = new List<Leave>();
            VC_LMSEntities d = new VC_LMSEntities();
        foreach(var item in d.Leaves)
            {
                if((item.Status=="Approved By HR") && ((item.Start_Date > DateTime.Now.Date) || (item.End_Date > DateTime.Now.Date)))
                {
                    leave.Add(item);
                }
            }
            return View(leave);
        }
       
        public ActionResult deleteholiday(int id)
        {
            VC_LMSEntities4 d = new VC_LMSEntities4();
            var holidayrow = d.holidays.Where(x => x.ID == id).FirstOrDefault();
            return View(holidayrow);
        }
        [HttpPost]
        public ActionResult deleteholiday(int id,holiday holidate)
        {
            VC_LMSEntities4 d = new VC_LMSEntities4();
            var holidayrow = d.holidays.Where(x => x.ID == id).FirstOrDefault();
            d.holidays.Remove(holidayrow);
            d.SaveChanges();
            ModelState.Clear();
            ViewBag.Message = "Deleted Successfully";
            return View();
        }

        public ActionResult addholiday()
        {
            return View();
        }
        [HttpPost]
        public ActionResult addholiday(holiday holiday)
        {
            VC_LMSEntities4 d = new VC_LMSEntities4();
            d.holidays.Add(holiday);
            d.SaveChanges();
            ViewBag.Message = "Added successfully";
            ModelState.Clear();
            return View();
        }
        public ActionResult EditApprover(int id)
        {
            VC_LMSEntities d = new VC_LMSEntities();
            ViewData["Emplyoee_id"] = new SelectList(d.UsersTables, "EMPLOYEE_ID", "FIRST_NAME");
            ViewData["approver_id"] = new SelectList(d.UsersTables, "EMPLOYEE_ID", "FIRST_NAME");
            var apporver = d.approvers.Where(x => x.ID == id).FirstOrDefault();
            return View(apporver);        
        }

        [HttpPost]

        public ActionResult EditApprover(LeaveManagementSystemValueCreed.Models.approver approver)
        {
            VC_LMSEntities d = new VC_LMSEntities();
            ViewData["Emplyoee_id"] = new SelectList(d.UsersTables, "EMPLOYEE_ID", "FIRST_NAME");
            ViewData["approver_id"] = new SelectList(d.UsersTables, "EMPLOYEE_ID", "FIRST_NAME");
            if (approver.approver_id == approver.Emplyoee_id)
            {
                ViewBag.Message = "Employee ID cannot be same as Approver ID";
                return View();
            }

            var validation = d.approvers.Where(x => x.Emplyoee_id == approver.Emplyoee_id && x.approver_id == approver.approver_id).FirstOrDefault();
            if (validation != null)
            {
                ViewBag.Message = "Approver Already Present";
                return View();
            }

            var row = d.UsersTables.Where(x => x.EMPLOYEE_ID == approver.Emplyoee_id).FirstOrDefault();
            approver.Employee_name = row.FIRST_NAME + " " + row.LAST_NAME;
            var row2 = d.UsersTables.Where(x => x.EMPLOYEE_ID == approver.approver_id).FirstOrDefault();
            approver.Approver_name = row2.FIRST_NAME + " " + row2.LAST_NAME;
            approver.department = row2.DEPARTMENT;
            d.Entry(approver).State = System.Data.Entity.EntityState.Modified;
            d.SaveChanges();
            ViewBag.Message = "Approver Edit Successfull";
            ModelState.Clear();
            return View(new approver());
        }
    }
}
    