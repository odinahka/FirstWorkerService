using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment appEnvironment)
        {
            _logger = logger;
            _env = appEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(List<IFormFile> files)
        {
          if(files.Count<0)
            {
                ModelState.AddModelError(null, "Filled a file");
                return View();
            }
            string wwwPath = _env.WebRootPath;
            string path = Path.Combine(wwwPath, "files");

            //Rename filename as <guid>.ext
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(files[0].FileName);
            using(var stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                files[0].CopyTo(stream); 
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}