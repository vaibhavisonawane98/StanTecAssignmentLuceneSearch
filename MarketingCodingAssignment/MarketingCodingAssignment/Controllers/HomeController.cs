using MarketingCodingAssignment.Models;
using MarketingCodingAssignment.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;

namespace MarketingCodingAssignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private SearchEngine _searchEngine;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _searchEngine = new SearchEngine();
        }

        public IActionResult Index()
        {
            return View();
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

        public void PopulateIndex()
        {
            // Sample Data
            var films = new List<Film> {
                new Film {Id = "Film123", Title = "Test Title 1", Overview = "Test Desc 1" },
                new Film {Id = "Film456", Title = "Test Title 2", Overview = "Test Desc 2" },
                new Film {Id = "Film789", Title = "Test Title 3", Overview = "Test Desc 3" }
            };

            _searchEngine.PopulateIndex(films);
            return;
        }

        //[HttpPost]
        public JsonResult Search(String searchString)
        {
            return Json(_searchEngine.Search(searchString));
        }

        public void DeleteIndexContents()
        {
            _searchEngine.DeleteIndexContents();
            return;
        }

    }
}