using LMS.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace LMS.Controllers
{
    public class AccountController : Controller
    {
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

            string connectionString =
                ConfigurationManager.ConnectionStrings["LMSConnection"].ConnectionString;

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

                            string passwordHash =
                                reader["PasswordHash"].ToString();

                            int roleId =
                                Convert.ToInt32(reader["RoleId"]);

                            bool isPasswordValid =
                                BCrypt.Net.BCrypt.Verify(
                                    model.Password,
                                    passwordHash
                                );

                            System.Diagnostics.Debug.WriteLine(
                                "USER ID = " + userId
                            );

                            System.Diagnostics.Debug.WriteLine(
                                "ROLE ID = " + roleId
                            );

                            System.Diagnostics.Debug.WriteLine(
                                "PASSWORD VALID = " + isPasswordValid
                            );

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
                            }
                        }
                    }
                }
            }

            ViewBag.Error = "Invalid email or password.";
            return View(model);
        }
    }
}