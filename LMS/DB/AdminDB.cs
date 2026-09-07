using LMS.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LMS.DB
{
    public class AdminDB
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["LMSConnection"].ConnectionString;
        public int CreateUser(RegisterUser model, string passwordHash, int createdBy)
        {
            int userId;
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_CreateUser", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                cmd.Parameters.AddWithValue("@LastName", model.LastName);
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                cmd.Parameters.AddWithValue("@RoleId", Convert.ToInt32(model.Role));
                cmd.Parameters.AddWithValue("@IsActive", true);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                con.Open();
                userId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return userId;
        }

        public void AssignUserCourse(int userId, int courseId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_AssignUserCourse", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@CourseId", courseId);
                cmd.Parameters.AddWithValue("@EnrollmentDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@StartDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@EndDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", "Not Started");
                cmd.Parameters.AddWithValue("@IsActive", true);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<UserList> GetUsers(int loggedInUserId)
        {
            List<UserList> users = new List<UserList>();

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetUserList", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LoggedInUserId", loggedInUserId);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserList
                        {
                            UserId = Convert.ToInt32(reader["UserId"]),
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Role = reader["Role"].ToString(),
                            Course = reader["Course"] == DBNull.Value ? "NA" : reader["Course"].ToString(),
                            EnrollmentDate = reader["EnrollmentDate"] == DBNull.Value ? "NA" : Convert.ToDateTime(reader["EnrollmentDate"]).ToString("dd-MMM-yyyy"),
                            Status = reader["Status"] == DBNull.Value ? "" : reader["Status"].ToString(),
                            UserStatus = reader["UserStatus"] == DBNull.Value ? "" : reader["UserStatus"].ToString(),
                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CreatedDate"]).ToString("dd-MMM-yyyy"),
                            CreatedBy = reader["CreatedBy"] == DBNull.Value ? "" : reader["CreatedBy"].ToString()
                        });
                    }
                }
            }

            return users;
        }
        public List<UserList> SearchUsers(string searchText, int? roleId, int loggedInUserId)
        {
            List<UserList> users = new List<UserList>();
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_SearchUsers", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SearchText", string.IsNullOrEmpty(searchText) ? (object)DBNull.Value : searchText);
                cmd.Parameters.AddWithValue("@RoleId", roleId.HasValue ? (object)roleId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@loggedInUserId", loggedInUserId);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserList
                        {
                            UserId = Convert.ToInt32(reader["UserId"]),
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Role = reader["Role"].ToString(),
                            Course = reader["Course"] == DBNull.Value ? "" : reader["Course"].ToString(),
                            EnrollmentDate = reader["EnrollmentDate"] == DBNull.Value ? "" : Convert.ToDateTime(reader["EnrollmentDate"]).ToString("dd-MMM-yyyy"),
                            Status = reader["Status"] == DBNull.Value ? "" : reader["Status"].ToString(),
                            UserStatus = reader["UserStatus"].ToString(),
                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CreatedDate"]).ToString("dd-MMM-yyyy"),
                            CreatedBy = reader["CreatedBy"] == DBNull.Value ? "" : reader["CreatedBy"].ToString()
                        });
                    }
                }
            }

            return users;
        }
        public List<UserList> SearchEmployees(EmployeeSearch model)
        {
            List<UserList> users = new List<UserList>();
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_SearchEmployees", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmployeeName", string.IsNullOrWhiteSpace(model.EmployeeName) ? (object)DBNull.Value : model.EmployeeName);
                cmd.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(model.Email) ? (object)DBNull.Value : model.Email);
                cmd.Parameters.AddWithValue("@CourseId", model.CourseId.HasValue ? (object)model.CourseId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", string.IsNullOrWhiteSpace(model.Status) ? (object)DBNull.Value : model.Status);
                cmd.Parameters.AddWithValue("@FromDate", model.FromDate.HasValue ? (object)model.FromDate.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@ToDate", model.ToDate.HasValue ? (object)model.ToDate.Value : DBNull.Value);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserList
                        {
                            UserId = Convert.ToInt32(reader["UserId"]),
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Role = reader["Role"].ToString(),
                            Course = reader["Course"] == DBNull.Value ? "" : reader["Course"].ToString(),
                            EnrollmentDate = reader["EnrollmentDate"] == DBNull.Value ? "" : Convert.ToDateTime(reader["EnrollmentDate"]).ToString("dd-MMM-yyyy"),
                            Status = reader["Status"] == DBNull.Value ? "" : reader["Status"].ToString(),
                            UserStatus = reader["UserStatus"] == DBNull.Value ? "" : reader["UserStatus"].ToString(),
                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CreatedDate"]).ToString("dd-MMM-yyyy"),
                            CreatedBy = reader["CreatedBy"] == DBNull.Value ? "" : reader["CreatedBy"].ToString()
                        });
                    }
                }
            }
            return users;
        }
        public object GetUserById(int id)
        {
            object user = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetUserById", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", id);
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
                            CourseId = reader["CourseId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["CourseId"]),
                            EnrollmentDate = reader["EnrollmentDate"] == DBNull.Value ? "" : Convert.ToDateTime(reader["EnrollmentDate"]).ToString("yyyy-MM-dd"),
                            Status = reader["Status"] == DBNull.Value ? "" : reader["Status"].ToString(),
                            StartDate = reader["StartDate"] == DBNull.Value ? "" : Convert.ToDateTime(reader["StartDate"]).ToString("yyyy-MM-dd"),
                            EndDate = reader["EndDate"] == DBNull.Value ? "" : Convert.ToDateTime(reader["EndDate"]).ToString("yyyy-MM-dd"),
                            CourseIsActive = reader["CourseIsActive"] == DBNull.Value ? false : Convert.ToBoolean(reader["CourseIsActive"]),
                        };
                    }
                }
            }
            return user;
        }
        public void UpdateUser(int userId, string firstName, string lastName, string email, int roleId, DateTime? startDate, DateTime? endDate, bool courseIsActive)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_UpdateUser", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@RoleId", roleId);
                cmd.Parameters.AddWithValue("@StartDate", (object)startDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EndDate", (object)endDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CourseIsActive", courseIsActive);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void UpdateUserStatus(int userId, bool isActive)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_UpdateUserStatus", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@IsActive", isActive);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public ManageProfile GetManageProfile(int userId)
        {
            ManageProfile model = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetUserById", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model = new ManageProfile
                        {
                            UserId = Convert.ToInt32(reader["UserId"]),
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Email = reader["Email"].ToString(),
                            PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? "" : reader["PhoneNumber"].ToString(),
                            CreatedBy = reader["CreatedBy"] == DBNull.Value ? "" : reader["CreatedBy"].ToString()
                        };
                    }
                }
            }
            return model;
        }
        public void UpdateProfile(int userId, string firstName, string lastName, string email, string phoneNumber)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_UpdateUser", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@RoleId", 1);
                cmd.Parameters.AddWithValue("@StartDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@EndDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@CourseIsActive", true);
                cmd.Parameters.AddWithValue("@PhoneNumber", (object)phoneNumber ?? DBNull.Value);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}