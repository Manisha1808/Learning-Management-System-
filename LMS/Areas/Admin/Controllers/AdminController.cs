using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using BCrypt.Net;
using LMS.Areas.Admin.Models;

namespace LMS.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
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
            string passwordHash =
                BCrypt.Net.BCrypt.HashPassword(model.Password);

            // 2. Get connection string
            string connectionString =
                ConfigurationManager
                .ConnectionStrings["LMSConnection"]
                .ConnectionString;

            int userId;

            // 3. Create User
            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                    new SqlCommand("sp_CreateUser", con))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(
                        "@FirstName",
                        model.FirstName);

                    cmd.Parameters.AddWithValue(
                        "@LastName",
                        model.LastName);

                    cmd.Parameters.AddWithValue(
                        "@Email",
                        model.Email);

                    cmd.Parameters.AddWithValue(
                        "@PasswordHash",
                        passwordHash);

                    cmd.Parameters.AddWithValue(
                        "@RoleId",
                        Convert.ToInt32(model.Role));

                    cmd.Parameters.AddWithValue(
                        "@IsActive",
                        true);

                    con.Open();

                    userId = Convert.ToInt32(
                        cmd.ExecuteScalar());
                }
            }

            // 4. If Employee, assign Course
            if (Convert.ToInt32(model.Role) == 2)
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd =
                        new SqlCommand(
                            "sp_AssignUserCourse",
                            con))
                    {
                        cmd.CommandType =
                            CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue(
                            "@UserId",
                            userId);

                        cmd.Parameters.AddWithValue(
                            "@CourseId",
                            model.CourseId.Value);

                        cmd.Parameters.AddWithValue(
                            "@EnrollmentDate",
                            DateTime.Now);

                        cmd.Parameters.AddWithValue(
                            "@StartDate",
                            DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@EndDate",
                            DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@Status",
                            "Not Started");

                        cmd.Parameters.AddWithValue(
                            "@IsActive",
                            true);

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

    }
}