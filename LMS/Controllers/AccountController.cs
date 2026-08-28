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

        [HttpPost]
        public ActionResult Login(Login model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Email == "adminlms12@gmail.com" &&
                model.Password == "Admin@123")
            {
                int userId = 0;
                string connectionString = ConfigurationManager.ConnectionStrings["LMSConnection"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT UserId
                        FROM [User]
                        WHERE Email = @Email
                          AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Email",model.Email);
                        con.Open();
                        userId = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                Session["UserId"] = userId;
                Session["UserRole"] = "Admin";
                Session["UserEmail"] = model.Email;
                return RedirectToAction("Dashboard","Admin",new { area = "Admin" }
                );
            }
            ViewBag.Error = "Invalid email or password.";
            return View(model);
        }
    }
}