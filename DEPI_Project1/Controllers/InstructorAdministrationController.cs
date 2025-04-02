using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DEPI_Project1.Controllers
{
    public class InstructorAdministrationController : Controller
    {


        private readonly ApplicationDbContext _context;
        public InstructorAdministrationController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        //[Authorize(Roles = "Instructor")]
        //public IActionResult InstructorProfile()
        //{
        //    var instructor = _context.Instructors.FirstOrDefault(i => i.Email == User.Identity.Name);
        //    return View(instructor);
        //}

        //[Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Profile()
        {
            var instructorEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(instructorEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(i => i.Email == instructorEmail);

            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);  // Use the default View name "Profile"
        }






    

        // GET: InstructorAdministration/Courses


        // GET: InstructorAdministration/Courses/Create
      
        // POST: InstructorAdministration/Courses/Create
        

        // GET: InstructorAdministration/Courses/Edit/5




    }
}
