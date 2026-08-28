using LMS.Areas.Admin.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace LMS.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        protected override void OnActionExecuting(
         ActionExecutingContext filterContext)
        {
            if (Session["UserRole"] == null ||
                Session["UserRole"].ToString() != "Admin")
            {
                filterContext.Result = RedirectToAction(
                    "Login",
                    "Account",
                    new {area = ""}
                );

                return;
            }

            base.OnActionExecuting(filterContext);
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public ActionResult RegisterUser()
        {
            return View();
        }
        [HttpGet]
        public ActionResult UserList()
        {
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

            // Employee must have a course
            if (Convert.ToInt32(model.Role) == 2 &&
                !model.CourseId.HasValue)
            {
                return Json(new
                {
                    success = false,
                    message = "Please select a course."
                });
            }

            // 1. Hash password  
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            // 2. Get connection string
            string connectionString = ConfigurationManager.ConnectionStrings["LMSConnection"].ConnectionString;

            int userId;

            // 3. Create User
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_CreateUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FirstName",model.FirstName);
                    cmd.Parameters.AddWithValue("@LastName",model.LastName);
                    cmd.Parameters.AddWithValue("@Email",model.Email);
                    cmd.Parameters.AddWithValue("@PasswordHash",passwordHash);
                    cmd.Parameters.AddWithValue("@RoleId",Convert.ToInt32(model.Role));
                    cmd.Parameters.AddWithValue("@IsActive", true);
                    cmd.Parameters.AddWithValue("@CreatedBy", Convert.ToInt32(Session["UserId"]));

                    con.Open();
                    userId = Convert.ToInt32(
                        cmd.ExecuteScalar());
                }
            }

            // 4. If Employee, assign Course
            if (Convert.ToInt32(model.Role) == 2)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_AssignUserCourse",con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserId",userId);
                        cmd.Parameters.AddWithValue("@CourseId",model.CourseId.Value);
                        cmd.Parameters.AddWithValue("@EnrollmentDate",DateTime.Now);
                        cmd.Parameters.AddWithValue("@StartDate",DBNull.Value);
                        cmd.Parameters.AddWithValue("@EndDate",DBNull.Value);
                        cmd.Parameters.AddWithValue("@Status","Not Started");
                        cmd.Parameters.AddWithValue("@IsActive",true);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            // 5. Return success response
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
            string connectionString = ConfigurationManager.ConnectionStrings["LMSConnection"].ConnectionString;       
            int userid = Convert.ToInt32(Session["UserId"]);
            var users = new System.Collections.Generic.List<object>();
            
            using (SqlConnection con = new SqlConnection(connectionString))
            {   using (SqlCommand cmd = new SqlCommand("sp_GetUserList", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LoggedInUserId",userid);
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {   while (reader.Read())
                        {
                            users.Add(new
                            {
                                UserId = Convert.ToInt32(reader["UserId"]),
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Role = reader["Role"].ToString(),
                                Course = reader["Course"] == DBNull.Value? "": reader["Course"].ToString(),
                                EnrollmentDate = reader["EnrollmentDate"] == DBNull.Value? "": Convert.ToDateTime(reader["EnrollmentDate"]).ToString("dd-MMM-yyyy"),
                                Status = reader["Status"] == DBNull.Value? "" : reader["Status"].ToString(),
                                CreatedDate = reader["CreatedDate"] == DBNull.Value? "": Convert.ToDateTime(reader["CreatedDate"]).ToString("dd-MMM-yyyy"),
                                CreatedBy = reader["CreatedBy"] == DBNull.Value? "": reader["CreatedBy"].ToString()
                            });
                        }
                    }
                }
            }
            return Json(
                users,
                JsonRequestBehavior.AllowGet
            );
        }
        
        [HttpGet]
        public JsonResult GetUserById(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LMSConnection"].ConnectionString;
            object user = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetUserById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId",id);
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new
                            {
                                UserId = Convert.ToInt32(reader["UserId"]),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Email = reader["Email"].ToString(),
                                RoleId = Convert.ToInt32(reader["RoleId"]),
                                CourseId = reader["CourseId"] == DBNull.Value? (int?)null: 
                                Convert.ToInt32(reader["CourseId"]),
                                EnrollmentDate = reader["EnrollmentDate"] == DBNull.Value? "":
                                Convert.ToDateTime(reader["EnrollmentDate"]).ToString("yyyy-MM-dd"),
                                Status = reader["Status"] == DBNull.Value? "": reader["Status"].ToString()
                            };
                        }
                    }
                }
            }
            return Json(
                user,
                JsonRequestBehavior.AllowGet
            );
        }

        [HttpPost]
        public JsonResult UpdateUser(
        int UserId,string FirstName,string LastName,string Email,int RoleId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LMSConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_UpdateUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@FirstName", FirstName);
                    cmd.Parameters.AddWithValue("@LastName", LastName);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@RoleId", RoleId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return Json(new
            {
                success = true,
                message = "User updated successfully."
            });
        }

        [HttpPost]
        public JsonResult DeleteUser(int UserId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["LMSConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DeleteUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return Json(new
            {
                success = true,
                message = "User deleted successfully."
            });
        }

    }
}