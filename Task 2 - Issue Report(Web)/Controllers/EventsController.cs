using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Task_2___Issue_Report_Web_.Data;

namespace Task_2___Issue_Report_Web_.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventStore _store;

        public EventsController(IWebHostEnvironment env)
        {
            _store = new EventStore(env.ContentRootPath);
        }

        [HttpGet]
        public IActionResult Index(string? category, string? date)
        {
            DateTime? parsedDate = null;
            if (!string.IsNullOrWhiteSpace(date) && DateTime.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var d))
            {
                parsedDate = d.Date;
            }

            var results = _store.Search(category, parsedDate);
            ViewBag.Categories = _store.Categories.OrderBy(c => c).ToArray();
            ViewBag.SelectedCategory = category ?? string.Empty;
            ViewBag.SelectedDate = parsedDate?.ToString("yyyy-MM-dd") ?? string.Empty;
            ViewBag.Recommendations = _store.GetRecommendations();
            return View(results);
        }
    }
}


