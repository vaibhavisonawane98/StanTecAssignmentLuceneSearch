using MarketingCodingAssignment.Models;
using MarketingCodingAssignment.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.VisualBasic.FileIO;
using System.Text.Json;

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
            PopulateIndex();
            return View();
        }

        [HttpPost]
        public IActionResult Index(string searchString)
        {
            var result = Search(searchString);
            return View(result);
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
        public List<Film> GetData()
        {
            var path = "C:\\StanTecAssignment\\movies.csv";
            string json;

            using (var parser = new TextFieldParser(path))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                string[] headers = parser.ReadFields();

                var records = new List<Dictionary<string, string>>();

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    var record = new Dictionary<string, string>();

                    for (int i = 0; i < headers.Length; i++)
                    {
                        record[headers[i]] = fields[i];
                    }

                    records.Add(record);
                }

                json = JsonConvert.SerializeObject(records, Formatting.Indented);
            }

            var ListOfRecords = System.Text.Json.JsonSerializer.Deserialize<List<Film>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IgnoreNullValues = true
            });
            return ListOfRecords;

        }
        
        public void PopulateIndex()
        {
            // Sample Data
            var films = GetData();

            _searchEngine.PopulateIndex(films);
            return;
        }

        public List<Film> Search(String searchString)
        {
            return _searchEngine.Search(searchString).ToList();
        }

        public void DeleteIndexContents()
        {
            _searchEngine.DeleteIndexContents();
            return;
        }

    }
}