using LeaveManagementSystemValueCreed.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.EnterpriseServices;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace leavemangementValueCreed.Controllers
{
    public class EmployeeController : Controller
    {
        int g = 0;
        // GET: Employee
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
       [HandleError]
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
            catch(Exception ex)
            {
                return RedirectToAction("Index", "Login");
            }
        }

        // GET: Employee/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            return View();
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ApplyLeave()
        {
            return View();
        }

        // POST: Employee/Create
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
        [HttpPost]
        public ActionResult ApplyLeave(LeaveManagementSystemValueCreed.Models.Leave leave)
        {
            //    leave.Start_Date = TimeZoneInfo.ConvertTimeToUtc(leave.Start_Date);
            //    leave.End_Date = TimeZoneInfo.ConvertTimeToUtc(leave.End_Date);
            var months = leave.Start_Date.Subtract(DateTime.Now);
            if (months.Days > 180)
            {
                ViewBag.Message = "Cannot Apply For more than 6 month in Advance";
                return View();
            }
            VC_LMSEntities r = new VC_LMSEntities();
            var user_info = r.UsersTables.Where(w => w.EMPLOYEE_ID == leave.Employee_Id).FirstOrDefault();
            
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

                if (leave.Start_Date.Subtract (DateTime.Now.Date).Days > 360)
                {
                    ViewBag.Message = "Cannot Apply for such date,as it is too Far";
                    return View();
                }
            }
            bool status = false;
            string message = "";
            if (DateTime.Now.Date.Kind != DateTimeKind.Utc)
            {
                var datediff = DateTime.Now.Date.Subtract(leave.End_Date.Date);
                if (datediff.TotalDays > 0)
                {
                    ViewBag.Message = "Cannot Apply Leave  if End Date is in the Past";
                    ViewBag.Status = false;
                    return View();
                }
            }
            else if(DateTime.Now.Date.Kind == DateTimeKind.Utc)
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
                    if ((Session["userID"].ToString()) == item.Employee_Id)
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
                            ViewBag.Message = "Matching Date For Already Applied Leave Found,Please Cancel that Leave ";
                            ViewBag.Status = false;
                            ModelState.Clear();
                            return View();
                        }
                    }
                    if (item.Employee_Id == (Session["userID"].ToString()) && (item.Status == "Pending on Manager" || item.Status == "Approved By Manager" || item.Status=="Pending on Approvers" || item.Status == "Approved By Approvers"))
                    {
                        c += item.Number_of_Days;
                    }
                }
                var x = (Session["userID"].ToString());
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
                    var number_of_day = leave.End_Date.Date.Subtract(leave.Start_Date.Date);
                    int number_of_days = 0;
                    number_of_days = number_of_day.Days + 1;
                    int d = 0;
                    VC_LMSEntities4 p = new VC_LMSEntities4();
                    for (DateTime i = leave.Start_Date; i <= leave.End_Date; i = i.AddDays(1))
                    {
                        if (i.DayOfWeek == DayOfWeek.Saturday || i.DayOfWeek == DayOfWeek.Sunday)
                        {
                            e=e+1;
                        }
                        
                        foreach(var item in p.holidays)
                        {
                            if (item.Holiday_date == i)
                                e++;

                        }
                    }

                    number_of_days = number_of_days - e;

                    if ((c + number_of_days > account.leaves_alloted))
                    {
                        message = "Already Applied or Approved leaves Exceeds Your Leave Balance";
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


                            if (DateTime.Now.Date.Kind == DateTimeKind.Utc)
                            {
                                leave.Leave_apply_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.Date, indian_zone);
                            }
                            else if (DateTime.Now.Date.Kind != DateTimeKind.Utc)
                            {
                                leave.Leave_apply_date = DateTime.Now.Date;
                            }
                            VC_LMSEntities vc = new VC_LMSEntities();
                            int k = 0;
                            foreach (var item in vc.approvers)
                            {
                                if (item.Emplyoee_id == (Session["userID"].ToString()))
                                {
                                    k++;
                                }
                            }
                            if (k > 0)
                            {
                                leave.Status = "Pending on Approvers";
                            }
                            else if (k == 0)
                            {
                                leave.Status = "Pending on HR";
                            }
                            leave.Number_of_Days = number_of_days;
                            leave.Employe_name = Session["userName"].ToString();
                            leave.Employee_Id = (Session["userID"].ToString());
                            using (VC_LMSEntities f = new VC_LMSEntities())
                            {

                                var row = f.UsersTables.Where(m => m.ROLES == "HR").FirstOrDefault();
                                string EmailID = row.EMAIL;
                                string employeename = leave.Employe_name;
                                int days_Leave = leave.Number_of_Days;
                                string hrname = row.FIRST_NAME;
                                SendverificationLinkEmail(EmailID, employeename, days_Leave,hrname,leave.LeaveType);
                                var emp_id=Session["userID"].ToString();
                                var apporvers = f.approvers.Where(y => y.Emplyoee_id == emp_id).FirstOrDefault();
                                if (apporvers != null)
                                {
                                    foreach(var item in f.approvers)
                                    {
                                        if (item.Emplyoee_id == emp_id)
                                        {
                                            var approvr = f.UsersTables.Where(q => q.EMPLOYEE_ID == item.approver_id).FirstOrDefault();
                                            var approvername = approvr.FIRST_NAME;
                                            SendverificationLinkEmail(approvr.EMAIL, employeename, days_Leave, approvername,leave.LeaveType);
                                        }
                                    }
                                }
                                //var userinfo = f.UsersTables.Where(s => s.EMPLOYEE_ID == leave.Employee_Id).FirstOrDefault();
                                //var row2 = f.UsersTables.Where(m => m.EMPLOYEE_ID == userinfo.manager_id).FirstOrDefault();
                                //EmailID = row2.EMAIL;
                                //SendverificationLinkEmail(EmailID, employeename);


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
                    else if (number_of_days<account.leaves_alloted) 
                    {
                        if(1<= DateTime.Now.Month && DateTime.Now.Month <= 3)
                        {
                            if(4<= leave.Start_Date.Month && leave.Start_Date.Month <= 6)
                            {
                                if (number_of_days < account.leaves_alloted + 5)
                                {

                                }
                            }
                            else if(7 <= leave.Start_Date.Month && leave.Start_Date.Month <= 9)
                            {
                                if (number_of_days < account.leaves_alloted + 10)
                                {

                                }
                            }
                        }
                        else if(4 <= DateTime.Now.Month && DateTime.Now.Month <= 6) 
                        {
                            if (7 <= leave.Start_Date.Month && leave.Start_Date.Month <= 9)
                            {
                                if (number_of_days < account.leaves_alloted + 5)
                                {

                                }
                            }
                            else if (10<= leave.Start_Date.Month && leave.Start_Date.Month <= 12)
                            {
                                if (number_of_days < account.leaves_alloted + 10)
                                {

                                }
                            }
                        }
                        else if (7 <= DateTime.Now.Month && DateTime.Now.Month <= 9)
                        {
                            if (10<= leave.Start_Date.Month && leave.Start_Date.Month <= 12)
                            {
                                if (number_of_days < account.leaves_alloted + 5)
                                {

                                }
                            }
                            else if (1 <= leave.Start_Date.Month && leave.Start_Date.Month <= 3)
                            {
                                if (account.leaves_alloted > 10)
                                {

                                }
                            }
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
                    //else
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

                            var row = f.UsersTables.Where(m => m.ROLES == "HR").FirstOrDefault();
                            string EmailID = row.EMAIL;
                            string employeename = leave.Employe_name;
                            int days_Leave = leave.Number_of_Days;
                            SendverificationLinkEmail(EmailID, employeename, days_Leave,row.FIRST_NAME,leave.LeaveType);
                            var emp_id = Session["userID"].ToString();
                            var apporvers = f.approvers.Where(y => y.Emplyoee_id == emp_id).FirstOrDefault();
                            if (apporvers != null)
                            {
                                foreach (var item in f.approvers)
                                {
                                    if (item.Emplyoee_id == emp_id)
                                    {
                                        var approvr = f.UsersTables.Where(q => q.EMPLOYEE_ID == item.approver_id).FirstOrDefault();
                                        SendverificationLinkEmail(approvr.EMAIL, employeename, days_Leave,approvr.FIRST_NAME,leave.LeaveType);
                                    }
                                }
                            }
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
        public ActionResult Leave(LeaveManagementSystemValueCreed.Models.Leave user)
        {
            try
            {
                ViewBag.Message = Session["Message"];
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
                return View();
            }
            //return RedirectToAction("Index", "Employee");


        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            VC_LMSEntities d = new VC_LMSEntities();
            if (d.Leaves.Where(x => x.Status == "Approved By HR" && x.ID==id && x.Start_Date>DateTime.Now).FirstOrDefault() != null)
            {
                g++;
                return View(d.Leaves.Where(x => x.ID == id).FirstOrDefault());
            }
            if (d.Leaves.Where(x => x.ID == id && (x.Status == "Pending on Manager"||x.Status=="Pending on Approvers"||x.Status=="Pending on HR")).FirstOrDefault() != null)
            {
                return View(d.Leaves.Where(x => x.ID == id).FirstOrDefault());
            }
            else
            {
                ViewBag.Message = "Can't Edit Approved Leave";
                return RedirectToAction("Leave");
            }
        }

        // POST: Employee/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, LeaveManagementSystemValueCreed.Models.Leave leave)
        {
            int n = 0;

            //try
            VC_LMSEntities j = new VC_LMSEntities();
            var approval_tbl = j.approvals.Where(x => x.Leave_id == leave.ID && x.status == "Approved By HR").FirstOrDefault();
 
            if (approval_tbl != null)
            {
                int leave_id = leave.ID;
                VC_LMSEntities d = new VC_LMSEntities();
                var user_id = d.UsersTables.Where(v => v.EMPLOYEE_ID == leave.Employee_Id).FirstOrDefault();
                var approver_number = d.approvers.Where(w => w.Emplyoee_id == leave.Employee_Id).FirstOrDefault();
                if (approver_number != null)
                {
                    leave.Status = "Requested Approvers For Cancellation";
                    d.Entry(leave).State = System.Data.Entity.EntityState.Modified;
                    d.SaveChanges();
                }
                else
                {
                    leave.Status = "Cancellation Pending on HR";
                    d.Entry(leave).State = System.Data.Entity.EntityState.Modified;
                    d.SaveChanges();
                }
                return RedirectToAction("Leave", "Employee");

            }
            else { 
            VC_LMSEntities l = new VC_LMSEntities();
                if (leave.Status == "Cancel")
                {
                    leave.Status = "Cancelled";

                    l.Entry(leave).State = System.Data.Entity.EntityState.Modified;
                    l.SaveChanges();
                    return RedirectToAction("Leave", "Employee");
                }
                else
                {

                    using (VC_LMSEntities db = new VC_LMSEntities())
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
                                if ((Session["userID"].ToString()) == item.Employee_Id)
                                {
                                    if ((item.Start_Date < leave.Start_Date) && (leave.End_Date < item.End_Date) && (item.Status != "Cancelled") && leave.ID != item.ID)
                                    {
                                        ViewBag.Message = "Leave range falling under Another Leave Range ";
                                        ViewBag.Status = false;
                                        ModelState.Clear();
                                        return View();
                                    }
                                    else if ((item.Start_Date < leave.Start_Date) && (leave.End_Date < item.End_Date) && (item.Status != "Cancelled") && leave.ID != item.ID)
                                    {
                                        ViewBag.Message = "Leave date conflict,Please Apply after previous leave Ends ";
                                        ViewBag.Status = false;
                                        ModelState.Clear();
                                        return View();
                                    }
                                    else if ((item.Start_Date < leave.Start_Date) && (leave.Start_Date < item.End_Date) && (item.Status != "Cancelled") && leave.ID != item.ID)
                                    {
                                        ViewBag.Message = "Leave date conflict,Please Apply after previous leave Ends,or Edit ";
                                        ViewBag.Status = false;
                                        ModelState.Clear();
                                        return View();
                                    }

                                    if ((leave.Start_Date == item.Start_Date || leave.End_Date == item.End_Date) && item.Status != "Cancelled" && leave.ID != item.ID)
                                    {
                                        ViewBag.Message = "Matching Date For Already Applied Leave Found,Please Cancel that Leave ";
                                        ViewBag.Status = false;
                                        ModelState.Clear();
                                        return View();
                                    }
                                }
                                if (item.Employee_Id == (Session["userID"].ToString()) && (item.Status == "Pending on Manager" || item.Status == "Approved By Manager" || item.Status == "Pending on Approvers" || item.Status == "Approved By Approvers") && leave.ID != item.ID)
                                {
                                    c += item.Number_of_Days;
                                }
                            }
                            var x = (Session["userID"].ToString());
                            var account = dd.UsersTables.Where(a => a.EMPLOYEE_ID == x).FirstOrDefault();

                            if (account.leaves_alloted == 0)
                            {
                                message = "Sorry Your Leave Balance is Zero and you cannot apply";
                                status = false;


                            }
                            else
                            {
                                int e = 0;
                                var number_of_day = leave.End_Date.Date.Subtract(leave.Start_Date);
                                int number_of_days = 0;
                                number_of_days = number_of_day.Days + 1;
                                int d = 0;
                                for (DateTime i = leave.Start_Date; i < leave.End_Date; i = i.AddDays(1))
                                {
                                    if (i.DayOfWeek == DayOfWeek.Saturday || i.DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        e = e + 1;
                                    }
                                }

                                number_of_days = number_of_days - e;

                                if ((c + number_of_days > account.leaves_alloted))
                                {
                                    message = "Already Applied or Approved leaves Exceeds Your Leave Balance";
                                    status = false;
                                    ViewBag.Message = message;
                                    ViewBag.Status = status;

                                }
                                else if (number_of_days < account.leaves_alloted)
                                {
                                    try
                                    {

                                        leave.Start_Date = leave.Start_Date.Date;
                                        leave.End_Date = leave.End_Date.Date;


                                        if (DateTime.Now.Date.Kind == DateTimeKind.Utc)
                                        {
                                            leave.Leave_apply_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.Date, indian_zone);
                                        }
                                        else if (DateTime.Now.Date.Kind != DateTimeKind.Utc)
                                        {
                                            leave.Leave_apply_date = DateTime.Now.Date;
                                        }
                                        VC_LMSEntities vc = new VC_LMSEntities();
                                        int k = 0;
                                        foreach (var item in vc.approvers)
                                        {
                                            if (item.Emplyoee_id == (Session["userID"].ToString()))
                                            {
                                                k++;
                                            }
                                        }
                                        if (k > 0)
                                        {
                                            leave.Status = "Pending on Approvers";
                                        }
                                        else if (k == 0)
                                        {
                                            leave.Status = "Pending on HR";
                                        }
                                        leave.Number_of_Days = number_of_days;
                                        leave.Employe_name = Session["userName"].ToString();
                                        leave.Employee_Id = (Session["userID"].ToString());
                                        using (VC_LMSEntities f = new VC_LMSEntities())
                                        {

                                            var row = f.UsersTables.Where(m => m.ROLES == "HR").FirstOrDefault();
                                            string EmailID = row.EMAIL;
                                            string employeename = leave.Employe_name;
                                            //SendverificationLinkEmail(EmailID, employeename);
                                            //var userinfo = f.UsersTables.Where(s => s.EMPLOYEE_ID == leave.Employee_Id).FirstOrDefault();
                                            //var row2 = f.UsersTables.Where(m => m.EMPLOYEE_ID == userinfo.manager_id).FirstOrDefault();
                                            //EmailID = row2.EMAIL;
                                            //SendverificationLinkEmail(EmailID, employeename);

                                            l.Entry(leave).State = System.Data.Entity.EntityState.Modified;
                                            l.SaveChanges();
                                            message = "Successfully Applied";
                                            ViewBag.Status = status;
                                            return RedirectToAction("Index", "Employee");

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
                                //else

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
                                leave.Status = "Pending on Manager";
                                leave.Employe_name = Session["userName"].ToString();
                                leave.Employee_Id = (Session["userID"].ToString());
                                using (VC_LMSEntities f = new VC_LMSEntities())
                                {

                                    f.Entry(leave).State = System.Data.Entity.EntityState.Modified;
                                    f.SaveChanges();
                                    message = "Successfully Applied";
                                    ViewBag.Status = status;
                                    return RedirectToAction("Index", "Employee");

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
                }
            } 
        }
                    


                    
         
        

        // GET: Employee/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employee/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult approvals(int id)
        {
            VC_LMSEntities d = new VC_LMSEntities();
            List<approval> ap = new List<approval>();
            foreach(var item in d.approvals)
            {
                if (item.Leave_id == id)
                {
                    ap.Add(item);
                }
            }
            return View(ap);
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult approverslist()
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
            catch(Exception ex)
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public void SendverificationLinkEmail(string EmailID, string employeename,int number_of_days,string approvername,string leavetype)
        {
            var fromemailPassword = "Sbinarela@07";
            var fromemail = new MailAddress("sumit.kumar@valuecreed.com", "ValueCreed");
            var toemail = new MailAddress(EmailID);
            var subject = "Leave Application";
            var body = "Hello  " + approvername +",< br /> < br />" + employeename + "  has applied for  "+ leavetype+"  Leave for  " + number_of_days + "  Days. < br /> <br /> Warm Regards, <br /> LMS Team.";
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
        public ActionResult holiday()
        {
            VC_LMSEntities4 d = new VC_LMSEntities4();
            var holiday = d.holidays.ToList();
            return View(holiday);
        }

    }
}
