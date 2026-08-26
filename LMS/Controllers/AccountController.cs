using LMS.Models;
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

            if (model.Email == "adminlms12@gmail.com" && model.Password == "Admin@123")
            {
                Session["UserRole"] = "Admin";
                Session["UserEmail"] = model.Email;

                return RedirectToAction(
                    "Dashboard",
                    "Admin",
                    new { area = "Admin" }
                );
            }

            ViewBag.Error = "Invalid email or password.";   //I used viewbag so that temporaray error message to be send from  controller to view and not a hardcoded one
            return View(model);
        }
    }
}