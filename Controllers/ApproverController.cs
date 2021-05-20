using LeaveManagementSystemValueCreed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace leavemangementValueCreed.Controllers
{
    public class ApproverController : Controller
    {
        // GET: Approver
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
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
                return View();
            }
                
            
        }


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult myleave()
        {
            VC_LMSEntities d = new VC_LMSEntities();
            List<Leave> leave = new List<Leave>();
            foreach(var item in d.Leaves)
            {
                if (item.Employee_Id == Session["userID"].ToString())
                {
                    leave.Add(item);
                }
            }
            return View(leave);
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Editmyleave(int id)
        {
            VC_LMSEntities d = new VC_LMSEntities();
            if (d.Leaves.Where(x => x.ID == id && (x.Status == "Pending on HR" || x.Status == "Pending on Approvers")).FirstOrDefault() != null)
            {
                return View(d.Leaves.Where(x => x.ID == id).FirstOrDefault());
            }
            else
            {
                ViewBag.Message = "Can't Edit Approved Leave";
                return RedirectToAction("myleave");
            }
        }
        [HttpPost]
        public ActionResult Editmyleave(int id, LeaveManagementSystemValueCreed.Models.Leave leave)
        {
            //try

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
                var indian_zone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                using (VC_LMSEntities db = new VC_LMSEntities())
                {
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
                                    else if(DateTime.Now.Date.Kind != DateTimeKind.Utc){
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
                                        leave.Status = "Pending on Manager";
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
                                        return RedirectToAction("Index", "approver");

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
                                return RedirectToAction("Index", "approver");

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
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Edit(int id)
        {
            VC_LMSEntities d = new VC_LMSEntities();
            var row = d.Leaves.Where(x => x.ID == id).FirstOrDefault();

            return View(row);
        }

        // POST: Approver/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, LeaveManagementSystemValueCreed.Models.Leave leaf)
        {
            try
            {
                VC_LMSEntities h = new VC_LMSEntities();
                var approval_row = h.approvals.Where(r => r.Leave_id == leaf.ID && r.status == "Approved By HR").FirstOrDefault();
                if (approval_row != null)
                {
                    if (leaf.Status == "Approved")
                    {
                        leaf.Status = "Cancellation Pending on HR";
                        h.Entry(leaf).State = System.Data.Entity.EntityState.Modified;
                        h.SaveChanges();
                        return RedirectToAction("Index", "Approver");
                    }
                }
            
               
                int c = 0;
                int e = 0;
                if(leaf.Status== "Approved") { c++; }
                VC_LMSEntities d = new VC_LMSEntities();
                var hr1 = d.UsersTables.Where(x => x.EMPLOYEE_ID == leaf.Employee_Id).FirstOrDefault();
                if (leaf.Status == "Approved" && hr1.ROLES == "HR")
                {
                    approval approval2 = new approval();
                    approval2.Aprrover_id = Session["userID"].ToString();
                    approval2.Employee_id = leaf.Employee_Id;
                    approval2.Leave_id = leaf.ID;
                    approval2.status = "Approved By  " + Session["Name"].ToString();
                    d.approvals.Add(approval2);
               
                    leaf.Status = "Approved";
                    hr1.leaves_alloted = hr1.leaves_alloted - leaf.Number_of_Days;
                    d.Entry(leaf).State = System.Data.Entity.EntityState.Modified;
                    d.Entry(hr1).State = System.Data.Entity.EntityState.Modified;
                    d.SaveChanges();
                    return RedirectToAction("Index", "Approver");

                }
                foreach (var item in d.approvals)
                {
                    if (item.Leave_id == leaf.ID)
                    {
                        c++;
                    }
                }
                if (leaf.Status == "Approved")
                {
                  

                    var i = leaf.Employee_Id;
                    foreach(var item in d.approvers)
                    {
                        if (item.Emplyoee_id == leaf.Employee_Id)
                        {
                            e++;
                        }
                    }
                    if (c == e)
                    {

                        leaf.Status = "Approved By All Approvers";
                        var hr = d.UsersTables.Where(x => x.EMPLOYEE_ID == leaf.Employee_Id && x.ROLES == "HR").FirstOrDefault();
                        if (hr != null)
                        {
                            hr.leaves_alloted = hr.leaves_alloted - leaf.Number_of_Days;
                            d.Entry(hr).State=System.Data.Entity.EntityState.Modified;
                            approval approval2 = new approval();
                            approval2.Aprrover_id = Session["userID"].ToString();
                            approval2.Employee_id = leaf.Employee_Id;
                            approval2.Leave_id = leaf.ID;
                            approval2.status = "Approved By  " + Session["Name"].ToString();
                            d.approvals.Add(approval2);
                            d.SaveChanges();
                        }
                        var emp = d.UsersTables.Where(x => x.EMPLOYEE_ID == leaf.Employee_Id).FirstOrDefault();
                        var body = "Hello " + emp.FIRST_NAME + ",< br /> <br /> your Leave has been approved by all the Approvers. <br /> <br /> Warm Regards, <br /> LMS Team.";
                        SendverificationLinkEmail(emp.EMAIL, body);
                        var hrmail = d.UsersTables.Where(x => x.ROLES == "HR").FirstOrDefault();
                        body= "Hello  " + hrmail.FIRST_NAME + ",< br /> <br /> Leave application of  " +leaf.Employe_name+ "  has been approved by all approvers and now it is pending on you. <br/> <br /> Warm Regards, <br /> LMS Team.";
                        //SendverificationLinkEmail(hrmail.EMAIL, body);
                        approval approval1 = new approval();
                        approval1.Aprrover_id = Session["userID"].ToString();
                        approval1.Employee_id = leaf.Employee_Id;
                        approval1.Leave_id = leaf.ID;
                        approval1.status = "Approved By  " + Session["Name"].ToString();
                        d.approvals.Add(approval1);
                        d.SaveChanges();
                    }
                    else
                    {
                        var emp = d.UsersTables.Where(x => x.EMPLOYEE_ID == leaf.Employee_Id).FirstOrDefault();

                        var body = "Hello " + emp.FIRST_NAME + ",< br /> <br />  your Leave has been approved by  " + Session["userName"]+ "< br /> <br /> Warm Regards, <br /> LMS Team.";
                        //SendverificationLinkEmail(emp.EMAIL, body);
                        leaf.Status = "Pending on Approvers";
                        approval approval1 = new approval();
                        approval1.Aprrover_id = Session["userID"].ToString();
                        approval1.Employee_id = leaf.Employee_Id;
                        approval1.Leave_id = leaf.ID;
                        approval1.status = "Approved By  " + Session["Name"].ToString() ;
                        d.approvals.Add(approval1);
                        d.SaveChanges();
                        
                    }
                }
                else if (leaf.Status == "Rejected By Approver")
                {
                    approval approval1 = new approval();
                    approval1.Aprrover_id = Session["userID"].ToString();
                    approval1.Employee_id = leaf.Employee_Id;
                    approval1.Leave_id = leaf.ID;
                    approval1.status = "Rejected By  " + Session["Name"].ToString();
                    d.approvals.Add(approval1);
                    d.SaveChanges();
                }
                d.Entry(leaf).State = System.Data.Entity.EntityState.Modified;
                d.SaveChanges();

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewBag.Message = "Unable to change the status";
                return View();
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult viewleave()
        {
            try
            {
                VC_LMSEntities d = new VC_LMSEntities();
                List<Leave> leaves = new List<Leave>();
                var a = (Session["userID"].ToString());
                var aproverrow = d.approvers.Where(x => x.approver_id == a).FirstOrDefault();
                List<approver> app = new List<approver>();
                foreach (var apporver in d.approvers)
                {
                    if (apporver.approver_id == a)
                    {
                        app.Add(apporver);
                    }
                }
                foreach (var row in app)
                {
                    foreach (var leaf in d.Leaves)
                    {
                        if(leaf.Status== "Requested Approvers For Cancellation")
                        {
                            leaves.Add(leaf);
                        }
                        if (leaf.Employee_Id == row.Emplyoee_id && leaf.Status != "Cancelled" && leaf.Status != "Pending on Manager" && leaf.Status != "Rejected By Approver" && leaf.Status != "Rejected By HR" && leaf.Status != "Approved By HR")
                        {
                            var approval = d.approvals.Where(x => x.Aprrover_id == row.approver_id && x.Employee_id == row.Emplyoee_id && leaf.ID == x.Leave_id).FirstOrDefault();
                            if (approval == null)
                            {
                                leaves.Add(leaf);
                            }
                        }
                    }
                }



                return View(leaves);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "Login");
            }
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
            var datediff = (DateTime.Now.Date).Subtract(leave.End_Date);
            if (datediff.TotalDays > 0)
            {
                ViewBag.Message = "Cannot Apply Leave  if End Date is in the Past";
                ViewBag.Status = false;
                return View();
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
                    if (item.Employee_Id == (Session["userID"].ToString()) && (item.Status == "Pending on Manager" || item.Status == "Approved By Manager" || item.Status == "Pending on Approvers" || item.Status == "Approved By Approvers"))
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
                    var number_of_day = leave.End_Date.Date.Subtract(leave.Start_Date);
                    int number_of_days = 0;
                    number_of_days = number_of_day.Days + 1;
                    int d = 0;
                    VC_LMSEntities4 p = new VC_LMSEntities4();
                    for (DateTime i = leave.Start_Date; i <= leave.End_Date; i = i.AddDays(1))
                    {
                        if (i.DayOfWeek == DayOfWeek.Saturday || i.DayOfWeek == DayOfWeek.Sunday)
                        {
                            e = e + 1;
                        }
                        foreach (var item in p.holidays)
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
                                var body = "H, <br />" + employeename + " has applied for Leave for " + number_of_days + "Days";

                                //SendverificationLinkEmail(EmailID,body );
                                var emp_id = Session["userID"].ToString();
                                var apporvers = f.approvers.Where(y => y.Emplyoee_id == emp_id).FirstOrDefault();
                                if (apporvers != null)
                                {
                                    foreach (var item in f.approvers)
                                    {
                                        if (item.Emplyoee_id == emp_id)
                                        {
                                            var approvr = f.UsersTables.Where(q => q.EMPLOYEE_ID == item.approver_id).FirstOrDefault();
                                            //SendverificationLinkEmail(approvr.EMAIL, body);
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
                    //if (leave.Start_Date.DayOfWeek == DayOfWeek.Saturday || leave.Start_Date.DayOfWeek == DayOfWeek.Sunday)
                    //{
                    //    ViewBag.Message = "You are applying Leave on day which is already a Holiday";
                    //    return View();
                    //}
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
        public void SendverificationLinkEmail(string EmailID, string body )
        {
            var fromemailPassword = "Sbinarela@07";
            var fromemail = new MailAddress("sumit.kumar@valuecreed.com", "ValueCreed");
            var toemail = new MailAddress(EmailID);
            var subject = "Leave Application";
            //var body = "Hi,</br>" + employeename + " has applied for Leave for " + number_of_days + "Days";
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
    }
}
