using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using LMS.Models;

namespace LMS.DB
{
    public class CommonDB
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["LMSConnection"].ConnectionString;

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
                            PhoneNumber = reader["PhoneNumber"] == DBNull.Value
                                ? ""
                                : reader["PhoneNumber"].ToString(),
                            CreatedBy = reader["CreatedBy"] == DBNull.Value
                                ? ""
                                : reader["CreatedBy"].ToString(),
                            CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                        };
                    }
                }
            }

            return model;
        }

        public void UpdateProfile(
     int userId,
     string firstName,
     string lastName,
     string email,
     string phoneNumber)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_UpdateProfile", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue(
                    "@PhoneNumber",
                    string.IsNullOrEmpty(phoneNumber)
                        ? (object)DBNull.Value
                        : phoneNumber
                );

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}