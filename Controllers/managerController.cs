using LeaveManagementSystemValueCreed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace leavemangementValueCreed.Controllers
{
    public class managerController : Controller
    {
        // GET: HR
        public ActionResult Index()
        {
            var developer = "Sumit K";
            return View();
        }

        // GET: HR/Details/5
        public ActionResult Details(int id)
        {
            var developer = "Sumit K";
            return View();
        }

        // GET: HR/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HR/Create
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

        // GET: HR/Edit/5
        public ActionResult Edit(int id)
        {
            VC_LMSEntities d = new VC_LMSEntities();
            return View(d.Leaves.Where(x => x.ID == id).FirstOrDefault());
        }

        // POST: HR/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Leave leave)
        {
            try
            {
                using (VC_LMSEntities db = new VC_LMSEntities())
                {

                    db.Entry(leave).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return View();
            }
        }

        // GET: HR/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HR/Delete/5
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
        public ActionResult Leave()
        {
            VC_LMSEntities d = new VC_LMSEntities();
            List<Leave> leavelist = new List<Leave>();
            //foreach (var item in d.UsersTables)
            //{
            //    if (item.manager_id == (Session["userID"].ToString()))
            //    {

            //        foreach (var emplist in d.Leaves.ToList())
            //        {
            //            if ((item.EMPLOYEE_ID == emplist.Employee_Id) && (emplist.Status == "Pending on Manager"))
            //            {
            //                leavelist.Add(emplist);
            //            }
            //        }

            //    }
            //}
            return View(leavelist);
        }
        public ActionResult Edit_now(int id)
        {
            VC_LMSEntities d = new VC_LMSEntities();
            var row = d.Leaves.Where(x => x.ID == id).FirstOrDefault();
            if (row.Status == "Cancelled" || row.Status == "Approved By Manager")
            {
                ViewBag.Message = "Cannot Edit this Leave";
                return View();
            }
            else
            {

                return View(d.Leaves.Where(x => x.ID == id).FirstOrDefault());
            }
        }
        [HttpPost]
        public ActionResult Edit_now(int id, Leave leave)
        {

            VC_LMSEntities l = new VC_LMSEntities();
            if (leave.Status == "Cancel")
            {
                leave.Status = "Cancelled";
                try
                {
                    l.Entry(leave).State = System.Data.Entity.EntityState.Modified;
                    l.SaveChanges();
                    return RedirectToAction("Leave", "Employee");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Invalid Values has been Passed";
                    return View();
                }
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
                            else if (number_of_days < account.leaves_alloted)
                            {
                                try
                                {

                                    leave.Start_Date = leave.Start_Date.Date;
                                    leave.End_Date = leave.End_Date.Date;


                                    leave.Leave_apply_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.Date, indian_zone);
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
                                        return RedirectToAction("Index", "manager");

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
                            leave.Leave_apply_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.Date, indian_zone);
                            leave.Status = "Pending on Manager";
                            leave.Employe_name = Session["userName"].ToString();
                            leave.Employee_Id = (Session["userID"].ToString());
                            using (VC_LMSEntities f = new VC_LMSEntities())
                            {

                                f.Entry(leave).State = System.Data.Entity.EntityState.Modified;
                                f.SaveChanges();
                                message = "Successfully Applied";
                                ViewBag.Status = status;
                                return RedirectToAction("Index", "manager");

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
                    if ((Session["userID"].ToString()) == item.Employee_Id)
                    {
                        if ((item.Start_Date < leave.Start_Date) && (leave.End_Date < item.End_Date) && (item.Status != "Cancelled" || item.Status == "Approved By Approver" || item.Status == "Approved By Manager"))
                        {
                            ViewBag.Message = "Leave range falling under Another Leave Range ";
                            ViewBag.Status = false;
                            ModelState.Clear();
                            return View();
                        }
                        else if ((item.Start_Date < leave.Start_Date) && (leave.End_Date < item.End_Date) && (item.Status != "Cancelled" || item.Status == "Approved By Approver" || item.Status == "Approved By Manager"))
                        {
                            ViewBag.Message = "Leave date conflict,Please Apply after previous leave Ends ";
                            ViewBag.Status = false;
                            ModelState.Clear();
                            return View();
                        }
                        else if ((item.Start_Date < leave.Start_Date) && (leave.Start_Date < item.End_Date) && (item.Status != "Cancelled" || item.Status == "Approved By Approver" || item.Status == "Approved By Manager"))
                        {
                            ViewBag.Message = "Leave date conflict,Please Apply after previous leave Ends,or Edit ";
                            ViewBag.Status = false;
                            ModelState.Clear();
                            return View();
                        }
                        if ((leave.Start_Date == item.Start_Date || leave.End_Date == item.End_Date) && item.Status != "Cancelled" || item.Status == "Approved By Approver" || item.Status == "Approved By Manager")
                        {
                            ViewBag.Message = "Matching Date For Already Applied Leave Found,Please Edit that Leave,if approved,then applya new Leave ";
                            ViewBag.Status = false;
                            ModelState.Clear();
                            return View();
                        }
                    }
                    if (item.Employee_Id == (Session["userID"].ToString()) && (item.Status == "Pending on Manager"||item.Status=="Pending on Approver"||item.Status=="Approved By Approver"||item.Status=="Approved By Manager"))
                    {
                        c += item.Number_of_Days;
                    }
                }
                int x = Convert.ToInt32(Session["userID"]);
                var account = dd.UsersTables.Where(a => a.EMPLOYEE_ID == x.ToString()).FirstOrDefault();

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
                    number_of_days = number_of_day.Days;
                    int d = 0;
                    for (DateTime i = leave.Start_Date; i < leave.End_Date;i= i.AddDays(1))
                    {
                        if (i.DayOfWeek.ToString() == "Saturday" || i.DayOfWeek.ToString() == "Sunday")
                        {
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

                            leave.Number_of_Days = number_of_days+1;
                            leave.Leave_apply_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.Date, indian_zone);
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
                    leave.Start_Date = leave.Start_Date.Date;
                    leave.End_Date = leave.End_Date.Date;

                    leave.Number_of_Days = 1;
                    leave.Leave_apply_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.Date, indian_zone);
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


        public ActionResult myleave()
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
        public ActionResult Editmyleave(int id)
        {
            VC_LMSEntities d = new VC_LMSEntities();
            return View(d.Leaves.Where(x => x.ID == id).FirstOrDefault());

        }
        [HttpPost]
        public ActionResult Editmyleave(int id, LeaveManagementSystemValueCreed.Models.Leave leave)
        {
            try
            {
                using (VC_LMSEntities db = new VC_LMSEntities())
                {

                    db.Entry(leave).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return View();
            }
        }
  
    }
}
