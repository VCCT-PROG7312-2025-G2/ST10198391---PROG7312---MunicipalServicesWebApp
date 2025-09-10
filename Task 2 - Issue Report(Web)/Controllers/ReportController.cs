using Microsoft.AspNetCore.Mvc;
using Task_2___Issue_Report_Web_.Data;
using Task_2___Issue_Report_Web_.Models;

namespace Task_2___Issue_Report_Web_.Controllers
{
    public class ReportController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _uploadFolder;

        public ReportController(IWebHostEnvironment env)
        {
            _env = env;
            _uploadFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(_uploadFolder)) Directory.CreateDirectory(_uploadFolder);
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Shows report form
            ViewBag.Categories = new[] { "Sanitation", "Roads", "Water/Utilities", "Lighting", "Potholes", "Other" };
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(string location, string category, string description, List<IFormFile> files)
        {
            // Validates required fields
            if (string.IsNullOrWhiteSpace(location))
            {
                TempData["Error"] = "Location is required.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(category))
            {
                TempData["Error"] = "Category is required.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                TempData["Error"] = "Description is required.";
                return RedirectToAction("Index");
            }

            // Validates description length
            if (description.Length < 10)
            {
                TempData["Error"] = "Description must be at least 10 characters long.";
                return RedirectToAction("Index");
            }

            var issue = new Issue
            {
                Location = location,
                Category = category,
                Description = description
            };

            // Saves uploaded files
            if (files != null && files.Any())
            {
                foreach (var f in files)
                {
                    if (f.Length > 0)
                    {
                        var safeName = $"{Guid.NewGuid()}_{Path.GetFileName(f.FileName)}";
                        var savePath = Path.Combine(_uploadFolder, safeName);
                        using var stream = new FileStream(savePath, FileMode.Create);
                        f.CopyTo(stream);
                        issue.Attachments.Add($"/uploads/{safeName}");
                    }
                }
            }

            IssueStore.Add(issue);
            TempData["Success"] = "Report submitted successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult List()
        {
            return View(IssueStore.Issues.OrderByDescending(i => i.SubmittedAt).ToList());
        }
    }
}
