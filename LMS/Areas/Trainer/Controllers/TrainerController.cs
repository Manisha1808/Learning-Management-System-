using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using LMS.DB;
using LMS.Areas.Trainer.Models;
namespace LMS.Areas.Trainer.Controllers
{
    public class TrainerController : Controller
    {
        protected override void OnActionExecuting(
            ActionExecutingContext filterContext)
        {
            if (Session["UserRole"] == null ||
                Session["UserRole"].ToString() != "3")
            {
                filterContext.Result = RedirectToAction(
                    "Login",
                    "Account",
                    new { area = "" }
                );
                return;
            }
            base.OnActionExecuting(filterContext);
        }
        private TrainerDB db = new TrainerDB();
        public ActionResult Dashboard()
        {
            return View();
        }
        public JsonResult GetEmployeeList(EmployeeProgress model)
        {
            DataTable employeeList = db.GetEmployeeList(model);
            var rows = employeeList.AsEnumerable()
                .Select(row => new
                {
                    UserId = row["UserId"],
                    UserName = row["UserName"].ToString(),
                    Email = row["Email"].ToString(),
                    CourseName = row["CourseName"].ToString(),
                    Status = row["Status"].ToString(),
                    EnrollmentDate = row["EnrollmentDate"]
                }).https://github.com/Manisha1808/Learning-Management-System-.git
                .ToList();
            return Json(
                new
                {
                    rows = rows
                },
                JsonRequestBehavior.AllowGet
            );
        }
    }    
}
