using LMS.Areas.Employee.Models;
using LMS.DB;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
namespace LMS.Areas.Employee.Controllers
{
    public class EmployeeController : Controller
    {    // This filter allows the user(admin) to always land in the Lgin Page first  
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
        public ActionResult StartLearning(int id)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            EmployeeDB db = new EmployeeDB();
            db.StartCourse(userId, id);
            return RedirectToAction("Course", "Employee", new
            {
                area = "Employee",
                id = id
            });
        }
        //  This action method fetch the current user learning record 
        [HttpGet]
        public ActionResult MyLearning()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            EmployeeDB db = new EmployeeDB();
            List<EmployeeCourse> courses = db.GetMyLearning(userId);
            Dictionary<int, CourseProgress> progressList = new Dictionary<int, CourseProgress>(); // Stores all courses assigned to the logged-in employee
            Dictionary<int, List<VideoProgress>> videoProgressList = new Dictionary<int, List<VideoProgress>>(); // Stores overall progress for each course using CourseId as the key
            foreach (var course in courses)
            {
                progressList[course.CourseId] = db.GetCourseProgress(userId, course.CourseId);
                videoProgressList[course.CourseId] = db.GetCourseVideoProgress(userId, course.CourseId);
            }
            ViewBag.ProgressList = progressList;     
            ViewBag.VideoProgressList = videoProgressList;
            return View(courses);
        }
        //the Course action handles course access and deadline validation
        [HttpGet]
        public ActionResult Course(int id)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            EmployeeDB db = new EmployeeDB();
            EmployeeCourse course = db.GetEmployeeCourseAccess(userId, id); //Check the userid along with course id to verify access permitted
            if (course == null)
                return Content("You do not have access to this course.");
            if (course.EndDate.HasValue && course.EndDate.Value < DateTime.Now)
            {
                return Content("Course deadline has been crossed. Please contact admin."); // To check if access is expired 
            }
            return View(course);
        }
        // CourseContent is responsible for fetching and displaying the actual videos and learning progress
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
            if (course.EndDate.HasValue && course.EndDate.Value < DateTime.Now) //If end date exceeds currentdate, it will show deadline message
            {
                TempData["DeadlineMessage"] = "Course deadline has been crossed. Please contact admin.";
                return RedirectToAction("Dashboard", "Employee", new { area = "Employee" });
            }
            List<VideoProgress> progress = db.GetCourseVideoProgress(userId, id);
            VideoProgress nextVideo = progress.Find(v => !v.IsCompleted); //If the next video is not null, it will automatically switch to next once completed
            if (nextVideo != null)
            {
                return RedirectToAction("WatchVideo","Employee",new
                    {
                        area = "Employee",
                        id = nextVideo.VideoId
                    });
            }
            return RedirectToAction("CourseContent","Employee",new
                {
                    area = "Employee",
                    id = id
                });
        }
        //checking the video id if it's null then based on that video access is given 
        [HttpGet]
        public ActionResult WatchVideo(int id)
        {
            int userId = Convert.ToInt32(Session["UserId"]);   
            EmployeeDB db = new EmployeeDB();
            Video video = db.GetVideoById(id);
            if (video == null) 
            { return Content("Video not found."); }
            EmployeeCourse course = db.GetEmployeeCourseAccess(userId, video.CourseId);
            if (course == null)
            { return Content("You do not have access to this video."); }
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
        // Just to mark the videos as completed, later we use this to assign quiz
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
        // First check if course is completed, then quiz option appears
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
            List<QuizQuestion> questions = db.GetQuizQuestions(id);  //stored all the questions 
            return View(questions);
        }
        
        //User submits the answers, it checks from the db whether answers are correct or not
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
        // to check whether passed the quiz or not, based on that certificate will be generated 
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
        // To fetch the certificate details and display it
        [HttpGet]
        public ActionResult Certificate(int courseId)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            EmployeeDB db = new EmployeeDB();
            // Get certificate details
            Certificate certificate = db.GetCertificate(userId, courseId);
            if (certificate == null)
            {
                return Content("Certificate not found.");
            }
            return View(certificate);
        }
        // Just to get Employee Profile details 
        [HttpGet]
        public ActionResult ManageProfile()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            EmployeeDB db = new EmployeeDB();
            ManageProfile model = db.GetManageProfile(userId);
            return View(model);
        }
        //the updated is sent to db
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
//aditya@gmail.com
//Adi@1234
