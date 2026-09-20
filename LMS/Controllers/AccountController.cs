using LMS.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using LMS.DB;
using LMS.Models.Enums;
namespace LMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["LMSConnection"].ConnectionString;
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public ActionResult Login(Login model)
        {
            if (!ModelState.IsValid)
                return View(model);
            int userId = 0;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT UserId, PasswordHash, RoleId
                FROM [User]
                WHERE Email = @Email
                  AND IsActive = 1";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", model.Email);

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userId = Convert.ToInt32(reader["UserId"]);
                            string passwordHash = reader["PasswordHash"].ToString();
                            int roleId = Convert.ToInt32(reader["RoleId"]);
                            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, passwordHash);
                            if (isPasswordValid)
                            {
                                // Store user details in session
                                Session["UserId"] = userId;
                                Session["UserRole"] = roleId;
                                Session["UserEmail"] = model.Email;

                                // Convert RoleId from database to UserRole enum
                                UserRole role = (UserRole)roleId;
                                if (role == UserRole.Admin)
                                {
                                    return RedirectToAction(
                                        "Dashboard",
                                        "Admin",
                                        new { area = "Admin" }
                                    );
                                }
                                else if (role == UserRole.Employee)
                                {
                                    return RedirectToAction(
                                        "Dashboard",
                                        "Employee",
                                        new { area = "Employee" }
                                    );
                                }
                                else if (role == UserRole.Trainer)
                                {
                                    return RedirectToAction(
                                        "Dashboard",
                                        "Trainer",
                                        new { area = "Trainer" }
                                    );
                                }
                            }
                        }
                    }
                }
            }

            ViewBag.Error = "Invalid email or password.";
            return View(model);
        }

        [HttpGet]
        public ActionResult ManageProfile()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            CommonDB db = new CommonDB();

            ManageProfile model = db.GetManageProfile(userId);

            if (model == null)
            {
                return Content("Profile not found for UserId: " + userId);
            }

            return View(model);
        }
        [HttpGet]
        public JsonResult GetManageProfile()
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            CommonDB db = new CommonDB();

            ManageProfile model = db.GetManageProfile(userId);

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ManageProfile(ManageProfile model)
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            CommonDB db = new CommonDB();

            db.UpdateProfile(
                userId,
                model.FirstName,
                model.LastName,
                model.Email,
                model.PhoneNumber
            );

            Session["UserEmail"] = model.Email;

            return Json(new
            {
                success = true,
                message = "Profile updated successfully."
            });
        }
    }
}