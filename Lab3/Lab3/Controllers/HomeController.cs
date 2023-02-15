using Microsoft.AspNetCore.Mvc;
using Lab3.Models;

namespace Lab3.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SongForm() => View();

        [HttpPost]
        public IActionResult Sing()
        {
            //sets the users input for number of bottles
            HttpContext.Session.SetString("countBottles", Request.Form["countBottles"]);

            return View();
        }

        public IActionResult CreateStudent() => View();

        [HttpPost]
       public IActionResult DisplayStudent(Student student)
        {
            return View(student);
        }
        public IActionResult Error()
        {
            return View();
        }

    }
}
