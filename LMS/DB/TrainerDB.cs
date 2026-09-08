using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using LMS.Areas.Trainer.Models;

namespace LMS.DB
{
    public class TrainerDB
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["LMSConnection"].ConnectionString;

        public DataTable GetEmployeeList(EmployeeProgress model)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(
                "sp_GetEmployeeListForTrainer", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@SearchText",
                    string.IsNullOrEmpty(model.SearchText)
                        ? (object)DBNull.Value
                        : model.SearchText
                );

                cmd.Parameters.AddWithValue(
                    "@Status",
                    string.IsNullOrEmpty(model.StatusFilter)
                        ? (object)DBNull.Value
                        : model.StatusFilter
                );

                cmd.Parameters.AddWithValue(
                    "@EnrollmentFrom",
                    model.EnrollmentFrom.HasValue
                        ? (object)model.EnrollmentFrom.Value
                        : DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "@EnrollmentTo",
                    model.EnrollmentTo.HasValue
                        ? (object)model.EnrollmentTo.Value
                        : DBNull.Value
                );

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }
    }
}