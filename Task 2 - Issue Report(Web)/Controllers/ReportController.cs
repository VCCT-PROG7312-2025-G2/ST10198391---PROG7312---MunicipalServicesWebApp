using Microsoft.AspNetCore.Mvc;
using Task_2___Issue_Report_Web_.Data;
using Task_2___Issue_Report_Web_.Models;

namespace Task_2___Issue_Report_Web_.Controllers
{
    public class ReportController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _uploadFolder;
        private readonly ServiceRequestStore _requestStore;

        public ReportController(IWebHostEnvironment env, ServiceRequestStore requestStore)
        {
            _env = env;
            _uploadFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(_uploadFolder)) Directory.CreateDirectory(_uploadFolder);
            _requestStore = requestStore;
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

            var attachments = new List<string>();

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
                        attachments.Add($"/uploads/{safeName}");
                    }
                }
            }

            // Create ServiceRequest for tracking
            var serviceRequest = new ServiceRequest
            {
                Location = location,
                Category = category,
                Description = description,
                Attachments = attachments,
                Status = RequestStatus.Submitted,
                Priority = DeterminePriority(category, description),
                SubmittedAt = DateTime.UtcNow
            };

            _requestStore.Add(serviceRequest);

            // Also add to IssueStore for backward compatibility
            var issue = new Issue
            {
                Id = serviceRequest.Id,
                Location = location,
                Category = category,
                Description = description,
                Attachments = attachments,
                SubmittedAt = serviceRequest.SubmittedAt
            };
            IssueStore.Add(issue);

            TempData["Success"] = $"Report submitted successfully! Your Request ID is: {serviceRequest.RequestId}";
            TempData["RequestId"] = serviceRequest.RequestId;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult List()
        {
            return View(IssueStore.Issues.OrderByDescending(i => i.SubmittedAt).ToList());
        }

        private int DeterminePriority(string category, string description)
        {
            var cat = category.ToLower();
            var desc = description.ToLower();

            if (cat.Contains("water") || cat.Contains("utilities") || desc.Contains("emergency") || desc.Contains("urgent"))
            {
                return 1; // Highest priority
            }
            else if (cat.Contains("roads") || cat.Contains("potholes") || desc.Contains("dangerous") || desc.Contains("hazard"))
            {
                return 2;
            }
            else if (cat.Contains("lighting") && (desc.Contains("dark") || desc.Contains("unsafe")))
            {
                return 3;
            }
            else if (cat.Contains("sanitation"))
            {
                return 4;
            }
            return 5; // Default priority
        }
    }
}
