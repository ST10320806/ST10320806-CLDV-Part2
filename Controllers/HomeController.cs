using Microsoft.AspNetCore.Mvc;
using ST10320806_Part1.Models;
using ST10320806_Part1.Services;
using System.Diagnostics;
using System.Threading.Tasks;
//Claude Ai used to correct all errors
namespace ST10320806_Part1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly CustomerService _customerService; // Inject CustomerService
        private readonly BlobService _blobService; // Inject BlobService
        private readonly OrderService _orderService; //Inject OrderService

        public HomeController(IHttpClientFactory httpClientFactory, ILogger<HomeController> logger, IConfiguration configuration, CustomerService customerService, BlobService blobService, OrderService orderService)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
            _customerService = customerService;
            _blobService = blobService;
            _orderService = orderService;
        }

        // Action for Index page
        public IActionResult Index()
        {
            return View();
        }
//------------------------------------------------------------------------------------------------//
//Code for the creation of a customer, calls CustomerService service
        public IActionResult CreateCustomer()
        {
            return View(new CustomerProfile());
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CustomerProfile profile)
        {
            if (ModelState.IsValid)
            {
                var isInserted = await _customerService.InsertCustomerAsync(profile);

                if (isInserted)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    _logger.LogError("Failed to insert customer data into the database.");
                    ModelState.AddModelError(string.Empty, "An error occurred while saving data.");
                }
            }

            return View(profile);
        }
//------------------------------------------------------------------------------------------------//
// Code for uploading to the Blob table, calls BlobService
        public IActionResult UploadBlob()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadBlob(IFormFile imageFile)
        {
            if (imageFile != null)
            {
                using var memoryStream = new MemoryStream();
                await imageFile.CopyToAsync(memoryStream);
                var imageData = memoryStream.ToArray();

                var isInserted = await _blobService.InsertBlobAsync(imageData);
                if (isInserted)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    _logger.LogError("Failed to insert blob data into the database.");
                    ModelState.AddModelError(string.Empty, "An error occurred while uploading the file.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "No file provided.");
            }

            return View();
        }
//------------------------------------------------------------------------------------------------//
//Code for creating an order, calls OrderService
        public IActionResult CreateOrder()
        {
            return View(new OrderProfile());
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderProfile order)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _orderService.InsertOrderAsync(order);
                    _logger.LogInformation("Order data inserted successfully.");
                    return View(Index);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to insert order data into the database: {ex.Message}");
                    ModelState.AddModelError("", "There was an error processing your request. Please try again later.");
                }
            }
            else
            {
                _logger.LogWarning("Invalid model state for order creation.");
            }

            // If there was an error or model state is invalid, return the same view with the order data and error message
            return View(order);
        }
    }
}

