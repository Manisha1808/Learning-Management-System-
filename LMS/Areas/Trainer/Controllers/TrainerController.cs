using LMS.Areas.Trainer.Models;
using LMS.DB;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
namespace LMS.Areas.Trainer.Controllers
{
    public class TrainerController : Controller
    {  // This filter allows the user(admin) to always land in the Lgin Page first  
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserRole"] == null ||
                Session["UserRole"].ToString() != "3")
            {
                filterContext.Result = RedirectToAction("Login","Account",new { area = "" });
                return;
            }
            base.OnActionExecuting(filterContext);
        }
        private TrainerDB db = new TrainerDB();
        [HttpGet]
        public ActionResult Dashboard()
        {
            ViewBag.Statuses = new List<SelectListItem>
            {
                new SelectListItem { Text = "Not Started", Value = "Not Started" }, //Directly save the list here, rather than creating a seperate SP because there wont beany changes in this
                new SelectListItem { Text = "In Progress", Value = "In Progress" },
                new SelectListItem { Text = "Completed", Value = "Completed" }
            };
            return View();
        }
        //The employee data is fetched and displayed
        [HttpGet]
        public JsonResult GetEmployeeList(EmployeeProgress model)
            {  
                DataTable employeeList = db.GetEmployeeList(model);
                var rows = employeeList.AsEnumerable().Select(row => new
                    {
                        UserId = row["UserId"],
                        UserName = row["UserName"].ToString(),
                        Email = row["Email"].ToString(),
                        CourseName = row["CourseName"].ToString(),
                        Status = row["Status"].ToString(),
                        EnrollmentDate = row["EnrollmentDate"]
                    }).ToList();
                return Json(
                    new
                    { rows = rows},
                    JsonRequestBehavior.AllowGet
                );
            }
        }
}

