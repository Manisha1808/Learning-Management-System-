using LMS.Areas.Trainer.Models;
using LMS.DB;
using System.Data;
using System.Linq;
using System.Web.Mvc;
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

        [HttpGet]
        public ActionResult Dashboard()
        {
            return View();
        }
        // Fetching the employee list 
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
                {
                    rows = rows
                },
                JsonRequestBehavior.AllowGet
            );
        }
    }
}