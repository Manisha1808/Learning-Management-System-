using LMS.DB;
using LMS.Areas.Admin.Models;
using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Data;

namespace LMS.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {   // This filter allows the user(admin) to always land in the Lgin Page first  
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "1")
            {
                filterContext.Result = RedirectToAction("Login", "Account", new { area = "" });
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        public ActionResult Dashboard()
        {
            AdminDB db = new AdminDB();
            DataTable dt = db.GetUserRoleCounts();

            if (dt.Rows.Count > 0)
            {
                ViewBag.AdminCount = dt.Rows[0]["AdminCount"];
                ViewBag.EmployeeCount = dt.Rows[0]["EmployeeCount"];
                ViewBag.TrainerCount = dt.Rows[0]["TrainerCount"];
            }
            return View();
        }

        [HttpGet]
        public ActionResult RegisterUser()
        {
            AdminDB db = new AdminDB();
            ViewBag.Roles = db.GetRoles();
            ViewBag.Courses = db.GetCourses();  //Viewbag is used so that, we are not hardcoding the data and any updates in db will directly added here and visible on view
            return View();
        }
        [HttpGet]
        public ActionResult UserList()
        {   AdminDB db = new AdminDB();
            ViewBag.Roles = db.GetRoles();
            ViewBag.Courses = db.GetCourses();
            return View();
        }
        [HttpGet]
        public ActionResult SearchUser()
        {
            AdminDB db = new AdminDB();
            ViewBag.Roles = db.GetRoles();
            ViewBag.Courses = db.GetCourses();
            ViewBag.Statuses = new List<SelectListItem>
            {
                new SelectListItem { Text = "Not Started", Value = "Not Started" }, //Directly save the list here, rather than creating a seperate SP because there wont beany changes in this
                new SelectListItem { Text = "In Progress", Value = "In Progress" },
                new SelectListItem { Text = "Completed", Value = "Completed" }
            };

            return View();
        }

        [HttpPost]
        public JsonResult RegisterUser(RegisterUser model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Please correct the validation errors."
                });
            }
            int roleId = Convert.ToInt32(model.Role);
            if (roleId == 2 && !model.CourseId.HasValue)   //Choosing course became mandatory, if the role is of Employee
            {
                return Json(new
                {
                    success = false,
                    message = "Please select a course."
                });
            }
            // Password Hashing
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            AdminDB db = new AdminDB();
            int loggedInUserId = Convert.ToInt32(Session["UserId"]);
            int userId = db.CreateUser(model, passwordHash, loggedInUserId);
            // sp_CreateUser returns -1 when email already exists
            if (userId == -1)
            {
                return Json(new
                {
                    success = false,
                    message = "Email already exists."
                });
            }
            if (roleId == 2)   // If the role is of Employee, course is assigned 
            {
                db.AssignUserCourse(userId, model.CourseId.Value);
            }
            return Json(new
            {
                success = true,
                message = "User created successfully.",
                userId = userId
            });
        }
        [HttpGet]
        public JsonResult GetUsers()
        {
            AdminDB db = new AdminDB();
            int loggedInUserId = Convert.ToInt32(Session["UserId"]);   //Fetching  session wise user Id
            var users = db.GetUsers(loggedInUserId);
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        //Fetching User By Id so that we can later use it to know what user was assigned with which role and further details
        [HttpGet]
        public JsonResult GetUserById(int id)
        {
            AdminDB db = new AdminDB();
            var user = db.GetUserById(id); 
            return Json(user, JsonRequestBehavior.AllowGet);
        }
        // Searching the user based on theirId
        [HttpGet]
        public JsonResult SearchUsers(string searchText, int roleId)
        {
            AdminDB db = new AdminDB();
            int loggedInUserId = Convert.ToInt32(Session["UserId"]);
            var users = db.SearchUsers(searchText, roleId, loggedInUserId);

            return Json(users, JsonRequestBehavior.AllowGet);
        }
        // The updated data is sent to the db 
        [HttpPost]
        public JsonResult UpdateUser(int UserId, string FirstName, string LastName, string Email, int RoleId, DateTime? StartDate, DateTime? EndDate, bool CourseIsActive)
        {
            AdminDB db = new AdminDB();
            db.UpdateUser(UserId, FirstName, LastName, Email, RoleId, StartDate, EndDate, CourseIsActive);

            return Json(new
            {
                success = true,
                message = "User updated successfully."
            });
        }
        //Whether the user is active or not is sent to the db
        [HttpPost]
        public JsonResult UpdateUserStatus(int UserId, bool IsActive)
        {
            AdminDB db = new AdminDB();
            db.UpdateUserStatus(UserId, IsActive);

            return Json(new
            {
                success = true,
                message = IsActive ? "User activated successfully." : "User deactivated successfully."  //this is for the dialog box 
            });
        }
    }
}
