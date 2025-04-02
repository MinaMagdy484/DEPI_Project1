using DEPI_Project1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static DEPI_Project1.Controllers.AccountController;
namespace DEPI_Project1.Controllers
{
    //[Authorize(Roles = "Student")]
    public class StudentAdministrationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentAdministrationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Profile()
        {
            var studentEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(studentEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var student = _context.Students.FirstOrDefault(s => s.Email == studentEmail);
            if (student == null)
            {
                // Optionally handle the case where the student is not found
                return NotFound();
            }

            return View("Profile", student);
        }


       

        //=================================================================



    }
}


//[Authorize(Roles = "Student")]
//public IActionResult StudentProfile()
//{
//    var student = _context.Students.FirstOrDefault(s => s.Email == User.Identity.Name);
//    return View(student);
//}