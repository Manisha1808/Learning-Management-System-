using LMS.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using LMS.DB;

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
                                Session["UserId"] = userId;
                                Session["UserRole"] = roleId;
                                Session["UserEmail"] = model.Email;
                                if (roleId == 1)
                                {
                                    return RedirectToAction(
                                        "Dashboard",
                                        "Admin",
                                        new { area = "Admin" }
                                    );
                                }
                                else if (roleId == 2)
                                {
                                    return RedirectToAction(
                                        "Dashboard",
                                        "Employee",
                                        new { area = "Employee" }
                                    );
                                }
                                else if (roleId==3)
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
            int userId = Convert.ToInt32(Session["UserId"]);

            CommonDB db = new CommonDB();

            ManageProfile model = db.GetManageProfile(userId);

            return View(model);
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