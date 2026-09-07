using LMS.Areas.Employee.Models;
using LMS.DB;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
namespace LMS.Areas.Employee.Controllers
{
    public class EmployeeController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "2")
            {
                filterContext.Result = RedirectToAction("Login", "Account", new { area = "" });
                return;
            }
            base.OnActionExecuting(filterContext);
        }
        public ActionResult Dashboard()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            EmployeeDB db = new EmployeeDB();
            var courses = db.GetEmployeeCourses(userId);
            return View(courses);
        }
        [HttpGet]
        public ActionResult MyLearning()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            EmployeeDB db = new EmployeeDB();
            List<EmployeeCourse> courses = db.GetMyLearning(userId);
            Dictionary<int, CourseProgress> progressList = new Dictionary<int, CourseProgress>();
            Dictionary<int, List<VideoProgress>> videoProgressList = new Dictionary<int, List<VideoProgress>>();
            foreach (var course in courses)
            {
                progressList[course.CourseId] = db.GetCourseProgress(userId, course.CourseId);
                videoProgressList[course.CourseId] = db.GetCourseVideoProgress(userId, course.CourseId);
            }
            ViewBag.ProgressList = progressList;
            ViewBag.VideoProgressList = videoProgressList;
            return View(courses);
        }
        [HttpGet]
        public ActionResult Course(int id)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            EmployeeDB db = new EmployeeDB();
            EmployeeCourse course = db.GetEmployeeCourseAccess(userId, id);
            if (course == null)
                return Content("You do not have access to this course.");
            if (course.EndDate.HasValue && course.EndDate.Value < DateTime.Now)
            {
                return Content("Course deadline has been crossed. Please contact admin.");
            }
            return View(course);
        }
        [HttpGet]
        public ActionResult CourseContent(int id)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            EmployeeDB db = new EmployeeDB();
            EmployeeCourse course = db.GetEmployeeCourseAccess(userId, id);
            if (course == null)
                return Content("You do not have access to this course.");
            List<Video> videos = db.GetCourseVideos(id);
            List<VideoProgress> progress = db.GetCourseVideoProgress(userId, id);
            CourseProgress courseProgress = db.GetCourseProgress(userId, id);
            ViewBag.Videos = videos;
            ViewBag.Progress = progress;
            ViewBag.CourseProgress = courseProgress;
            return View(course);
        }
        [HttpGet]
        public ActionResult ContinueLearning(int id)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            EmployeeDB db = new EmployeeDB();
            EmployeeCourse course = db.GetEmployeeCourseAccess(userId, id);
            if (course.EndDate.HasValue && course.EndDate.Value < DateTime.Now)
            {
                TempData["DeadlineMessage"] =
                    "Course deadline has been crossed. Please contact admin.";
                return RedirectToAction("Dashboard", "Employee", new { area = "Employee" });
            }
            List<VideoProgress> progress = db.GetCourseVideoProgress(userId, id);
            VideoProgress nextVideo = progress.Find(v => !v.IsCompleted);
            if (nextVideo != null)
            {
                return RedirectToAction(
                    "WatchVideo",
                    "Employee",
                    new
                    {
                        area = "Employee",
                        id = nextVideo.VideoId
                    });
            }
            return RedirectToAction(
                "CourseContent",
                "Employee",
                new
                {
                    area = "Employee",
                    id = id
                });
        }
        [HttpGet]
        public ActionResult ManageProfile()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            EmployeeDB db = new EmployeeDB();
            ManageProfile model = db.GetManageProfile(userId);
            return View(model);
        }
        [HttpGet]
        public ActionResult WatchVideo(int id)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            EmployeeDB db = new EmployeeDB();
            Video video = db.GetVideoById(id);

            if (video == null)
            {
                return Content("Video not found.");
            }

            EmployeeCourse course = db.GetEmployeeCourseAccess(userId, video.CourseId);

            if (course == null)
            {
                return Content("You do not have access to this video.");
            }

            List<Video> videos = db.GetCourseVideos(video.CourseId);

            for (int i = 0; i < videos.Count; i++)
            {
                if (videos[i].VideoId == video.VideoId && i + 1 < videos.Count)
                {
                    video.NextVideoId = videos[i + 1].VideoId;
                    break;
                }
            }

            return View(video);
        }

        [HttpGet]
        public ActionResult StartLearning(int id)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            EmployeeDB db = new EmployeeDB();
            db.StartCourse(userId, id);
            return RedirectToAction("Course", "Employee",
                new
                {
                    area = "Employee",
                    id = id
                });
        }
        [HttpGet]
        public ActionResult Quiz(int id)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            EmployeeDB db = new EmployeeDB();
            EmployeeCourse course = db.GetEmployeeCourseAccess(userId, id);
            if (course == null)
            {
                return Content("You do not have access to this course.");
            }
            CourseProgress courseProgress = db.GetCourseProgress(userId, id);
            if (courseProgress == null || courseProgress.TotalVideos == 0 || courseProgress.CompletedVideos != courseProgress.TotalVideos)
            {
                return Content("Please complete all course videos before attempting the quiz.");
            }
            List<QuizQuestion> questions = db.GetQuizQuestions(id);
            return View(questions);
        }
        [HttpGet]
        public ActionResult GetCertificate(int courseId)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            EmployeeDB db = new EmployeeDB();
            int quizResultId = db.GetLatestPassedQuizResult(userId, courseId);
            if (quizResultId == 0)
            {
                return Content("You have not passed the quiz.");
            }
            db.CreateCertificate(userId, courseId, quizResultId);
            return RedirectToAction("Certificate", "Employee", new { area = "Employee", courseId = courseId });
        }
        [HttpPost]
        public ActionResult Quiz(int courseId, FormCollection form)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            EmployeeDB db = new EmployeeDB();
            Dictionary<int, string> correctAnswers = db.GetQuizAnswers(courseId);
            int score = 0;
            foreach (var answer in correctAnswers)
            {
                string selectedAnswer = form["question_" + answer.Key];
                if (!string.IsNullOrEmpty(selectedAnswer) && selectedAnswer == answer.Value)
                {
                    score++;
                }
            }
            int totalQuestions = correctAnswers.Count;
            int percentage = 0;
            if (totalQuestions > 0)
            {
                percentage = (score * 100) / totalQuestions;
            }
            bool isPassed = percentage >= 60;
            // Save quiz result
            db.SaveQuizResult(userId, courseId, score, totalQuestions, percentage, isPassed);
            ViewBag.Score = score;
            ViewBag.TotalQuestions = totalQuestions;
            ViewBag.Percentage = percentage;
            ViewBag.CourseId = courseId;
            return View("QuizResult");
        }
        [HttpPost]
        public JsonResult MarkVideoCompleted(int videoId)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            EmployeeDB db = new EmployeeDB();
            db.MarkVideoCompleted(userId, videoId);
            return Json(new
            {
                success = true,
                message = "Video completed."
            });
        }
        [HttpGet]
        public ActionResult Certificate(int courseId)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            EmployeeDB db = new EmployeeDB();
            // Get certificate details
            Certificate certificate =
                db.GetCertificate(userId, courseId);
            if (certificate == null)
            {
                return Content("Certificate not found.");
            }
            return View(certificate);
        }
        [HttpPost]
        public JsonResult ManageProfile(ManageProfile model)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            EmployeeDB db = new EmployeeDB();
            db.UpdateProfile(userId, model.FirstName, model.LastName, model.Email, model.PhoneNumber);
            Session["UserEmail"] = model.Email;
            return Json(new
            {
                success = true,
                message = "Profile updated successfully."
            });
        }
    }
}
//For Testing Purpose:
//manisha1801@gmail.com
//Mani@1234
//roma801@gmail.com
//Roma@1234
