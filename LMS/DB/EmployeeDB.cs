using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using LMS.Areas.Employee.Models;
namespace LMS.DB
{
    public class EmployeeDB
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["LMSConnection"].ConnectionString;
        public List<EmployeeCourse> GetEmployeeCourses(int userId)
        {
            List<EmployeeCourse> courses = new List<EmployeeCourse>();
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetEmployeeCourses", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        courses.Add(new EmployeeCourse
                        {
                            UserCourseId = Convert.ToInt32(reader["UserCourseId"]),
                            UserId = Convert.ToInt32(reader["UserId"]),
                            CourseId = Convert.ToInt32(reader["CourseId"]),
                            CourseName = reader["CourseName"].ToString(),
                            Description = reader["Description"].ToString(),
                            EnrollmentDate = Convert.ToDateTime(reader["EnrollmentDate"]),
                            Status = reader["Status"].ToString(),
                            StartDate = reader["StartDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["StartDate"]),
                            EndDate = reader["EndDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EndDate"])
                        });
                    }
                }
            }
            return courses;
        }
        public List<EmployeeCourse> GetMyLearning(int userId)
        {
            List<EmployeeCourse> courses = new List<EmployeeCourse>();
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetMyLearning", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        courses.Add(new EmployeeCourse
                        {
                            UserCourseId = Convert.ToInt32(reader["UserCourseId"]),
                            UserId = Convert.ToInt32(reader["UserId"]),
                            CourseId = Convert.ToInt32(reader["CourseId"]),
                            CourseName = reader["CourseName"].ToString(),
                            Description = reader["Description"].ToString(),
                            EnrollmentDate = Convert.ToDateTime(reader["EnrollmentDate"]),
                            StartDate = reader["StartDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["StartDate"]),
                            EndDate = reader["EndDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EndDate"]),
                            Status = reader["Status"].ToString()
                        });
                    }
                }
            }
            return courses;
        }
        public EmployeeCourse GetEmployeeCourseAccess(int userId, int courseId)
        {
            EmployeeCourse course = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetEmployeeCourseAccess", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@CourseId", courseId);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        course = new EmployeeCourse
                        {
                            UserCourseId = Convert.ToInt32(reader["UserCourseId"]),
                            UserId = Convert.ToInt32(reader["UserId"]),
                            CourseId = Convert.ToInt32(reader["CourseId"]),
                            CourseName = reader["CourseName"].ToString(),
                            Description = reader["Description"].ToString(),
                            EnrollmentDate = Convert.ToDateTime(reader["EnrollmentDate"]),
                            StartDate = reader["StartDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["StartDate"]),
                            EndDate = reader["EndDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EndDate"]),
                            Status = reader["Status"].ToString()
                        };
                    }
                }
            }
            return course;
        }
        public List<Video> GetCourseVideos(int courseId)
        {
            List<Video> videos = new List<Video>();
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetCourseVideos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CourseId", courseId);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        videos.Add(new Video
                        {
                            VideoId = Convert.ToInt32(reader["VideoId"]),
                            CourseId = Convert.ToInt32(reader["CourseId"]),
                            VideoTitle = reader["VideoTitle"].ToString(),
                            VideoUrl = reader["VideoUrl"].ToString(),
                            Description = reader["Description"].ToString(),
                            SequenceNo = Convert.ToInt32(reader["SequenceNo"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        });
                    }
                }
            }
            return videos;
        }
        public Video GetVideoById(int videoId)
        {
            Video video = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetVideoById", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VideoId", videoId);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        video = new Video
                        {
                            VideoId = Convert.ToInt32(reader["VideoId"]),
                            CourseId = Convert.ToInt32(reader["CourseId"]),
                            VideoTitle = reader["VideoTitle"].ToString(),
                            VideoUrl = reader["VideoUrl"].ToString(),
                            Description = reader["Description"].ToString(),
                            SequenceNo = Convert.ToInt32(reader["SequenceNo"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        };
                    }
                }
            }
            return video;
        }
        public void MarkVideoCompleted(int userId, int videoId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_MarkVideoCompleted", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@VideoId", videoId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public List<VideoProgress> GetCourseVideoProgress(int userId, int courseId)
        {
            List<VideoProgress> progressList = new List<VideoProgress>();

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetCourseVideoProgress", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@CourseId", courseId);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        progressList.Add(new VideoProgress
                        {
                            VideoId = Convert.ToInt32(reader["VideoId"]),
                            VideoTitle = reader["VideoTitle"].ToString(),
                            SequenceNo = Convert.ToInt32(reader["SequenceNo"]),
                            IsCompleted = Convert.ToBoolean(reader["IsCompleted"]),
                            CompletedDate = reader["CompletedDate"] == DBNull.Value? (DateTime?)null: Convert.ToDateTime(reader["CompletedDate"])
                        });
                    }
                }
            }
            return progressList;
        }
        public CourseProgress GetCourseProgress(int userId, int courseId)
        {
            CourseProgress progress = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetCourseProgress", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@CourseId", courseId);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        progress = new CourseProgress
                        {
                            TotalVideos = Convert.ToInt32(reader["TotalVideos"]),
                            CompletedVideos = Convert.ToInt32(reader["CompletedVideos"])
                        };
                    }
                }
            }
            return progress;
        }
        public List<QuizQuestion> GetQuizQuestions(int courseId)
        {
            List<QuizQuestion> questions = new List<QuizQuestion>();
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetQuizQuestions", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CourseId", courseId);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        questions.Add(new QuizQuestion
                        {
                            QuestionId = Convert.ToInt32(reader["QuestionId"]),
                            CourseId = Convert.ToInt32(reader["CourseId"]),
                            QuestionText = reader["QuestionText"].ToString(),
                            OptionA = reader["OptionA"].ToString(),
                            OptionB = reader["OptionB"].ToString(),
                            OptionC = reader["OptionC"].ToString(),
                            OptionD = reader["OptionD"].ToString()
                        });
                    }
                }
            }
            return questions;
        }
        public void StartCourse(int userId, int courseId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_StartCourse", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@CourseId", courseId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public Dictionary<int, string> GetQuizAnswers(int courseId)
        {
            Dictionary<int, string> answers = new Dictionary<int, string>();
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetQuizAnswers", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CourseId", courseId);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int questionId = Convert.ToInt32(reader["QuestionId"]);
                        string correctOption = reader["CorrectOption"].ToString();
                        answers.Add(questionId, correctOption);
                    }
                }
            }
            return answers;
        }
        public void SaveQuizResult(int userId, int courseId, int score, int totalQuestions, int percentage, bool isPassed)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SaveQuizResult", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@CourseId", courseId);
                    cmd.Parameters.AddWithValue("@Score", score);
                    cmd.Parameters.AddWithValue("@TotalQuestions", totalQuestions);
                    cmd.Parameters.AddWithValue("@Percentage", percentage);
                    cmd.Parameters.AddWithValue("@IsPassed", isPassed);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void CreateCertificate(int userId, int courseId, int quizResultId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_CreateCertificate", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@CourseId", courseId);
                    cmd.Parameters.AddWithValue("@QuizResultId", quizResultId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int GetLatestPassedQuizResult(int userId, int courseId)
        {
            int quizResultId = 0;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(@"
            SELECT TOP 1 QuizResultId
            FROM QuizResult
            WHERE UserId = @UserId
              AND CourseId = @CourseId
              AND IsPassed = 1
            ORDER BY AttemptDate DESC", con))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@CourseId", courseId);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        quizResultId = Convert.ToInt32(result);
                    }
                }
            }
            return quizResultId;
        }
        public Certificate GetCertificate(int userId, int courseId)
        {
            Certificate certificate = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetCertificate", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@CourseId", courseId);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        certificate = new Certificate
                        {
                            CertificateId = Convert.ToInt32(reader["CertificateId"]),
                            UserId = Convert.ToInt32(reader["UserId"]),
                            CourseId = Convert.ToInt32(reader["CourseId"]),
                            QuizResultId = Convert.ToInt32(reader["QuizResultId"]),
                            CertificateNumber = reader["CertificateNumber"].ToString(),
                            EmployeeName = reader["EmployeeName"].ToString(),
                            CourseName = reader["CourseName"].ToString(),
                            Score = Convert.ToInt32(reader["Score"]),
                            TotalQuestions = Convert.ToInt32(reader["TotalQuestions"]),
                            Percentage = Convert.ToInt32(reader["Percentage"]),
                            IsPassed = Convert.ToBoolean(reader["IsPassed"]),
                            IssueDate = Convert.ToDateTime(reader["IssueDate"])
                        };
                    }
                }
            }
            return certificate;
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
                cmd.Parameters.AddWithValue("@RoleId", 2);
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