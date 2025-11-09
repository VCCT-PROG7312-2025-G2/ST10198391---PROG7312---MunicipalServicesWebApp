using Microsoft.AspNetCore.Mvc;
using Task_2___Issue_Report_Web_.Data;
using Task_2___Issue_Report_Web_.Models;

namespace Task_2___Issue_Report_Web_.Controllers
{
    public class ServiceRequestStatusController : Controller
    {
        private readonly ServiceRequestStore _requestStore;

        public ServiceRequestStatusController(ServiceRequestStore requestStore)
        {
            _requestStore = requestStore;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var allRequests = _requestStore.GetAllSortedByDate();
            ViewBag.Statuses = Enum.GetValues<RequestStatus>();
            ViewBag.Categories = allRequests.Select(r => r.Category).Distinct().ToList();
            return View(allRequests);
        }

        [HttpGet]
        public IActionResult Search(string? requestId, string? category, RequestStatus? status)
        {
            var results = new List<ServiceRequest>();

            if (!string.IsNullOrWhiteSpace(requestId))
            {
                var request = _requestStore.GetByRequestId(requestId);
                if (request != null)
                {
                    results.Add(request);
                }
            }
            else
            {
                var allRequests = _requestStore.GetAll();

                if (!string.IsNullOrWhiteSpace(category))
                {
                    allRequests = allRequests.Where(r => r.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (status.HasValue)
                {
                    allRequests = allRequests.Where(r => r.Status == status.Value).ToList();
                }

                results = allRequests.OrderByDescending(r => r.SubmittedAt).ToList();
            }

            ViewBag.Statuses = Enum.GetValues<RequestStatus>();
            ViewBag.Categories = _requestStore.GetAll().Select(r => r.Category).Distinct().ToList();
            ViewBag.RequestId = requestId;
            ViewBag.SelectedCategory = category;
            ViewBag.SelectedStatus = status;

            return View("Index", results);
        }

        [HttpGet]
        public IActionResult Details(string requestId)
        {
            var request = _requestStore.GetByRequestId(requestId);
            if (request == null)
            {
                TempData["Error"] = "Service request not found.";
                return RedirectToAction("Index");
            }

            // Get related requests using graph
            var relatedRequests = _requestStore.GetRelatedRequests(request.Id, 5);
            ViewBag.RelatedRequests = relatedRequests;

            return View(request);
        }

        [HttpGet]
        public IActionResult Priority()
        {
            var topPriorityRequests = _requestStore.GetTopPriorityRequests(10);
            return View(topPriorityRequests);
        }

        [HttpGet]
        public IActionResult ByStatus(RequestStatus status)
        {
            var requests = _requestStore.GetRequestsByStatus(status);
            ViewBag.Status = status;
            ViewBag.Statuses = Enum.GetValues<RequestStatus>();
            return View(requests);
        }

        [HttpGet]
        public IActionResult ByCategory(string category)
        {
            var requests = _requestStore.GetRequestsByCategory(category);
            ViewBag.Category = category;
            ViewBag.Categories = _requestStore.GetAll().Select(r => r.Category).Distinct().ToList();
            return View(requests);
        }
    }
}

