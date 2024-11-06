using COCOApp.Helpers;
using COCOApp.Hubs;
using COCOApp.Models;
using COCOApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace COCOApp.Controllers
{
    public class CategoryController : Controller
    {

        private readonly IHubContext<CategoryHub> _hubContext;
        private CategoryService _categoryService;
        public CategoryController(CategoryService categoryService, IHubContext<CategoryHub> hubContext)
        {
            _categoryService = categoryService;
            _hubContext = hubContext;
        }
        private const int PageSize = 10;

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult GetList(string nameQuery, int statusId, int pageNumber = 1)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            var categories = _categoryService.GetCategories(nameQuery, pageNumber, PageSize, user.Id, statusId);
            var totalCategories = _categoryService.GetTotalCategories(nameQuery, user.Id, statusId);

            var response = new
            {
                categoryResults = categories,
                pageNumber = pageNumber,
                totalPages = (int)Math.Ceiling(totalCategories / (double)PageSize)
            };

            return Json(response);
        }
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult GetCategory(int categoryId, int pageNumber = 1)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            Category model = _categoryService.GetCategoryById(categoryId, user.Id);
            ViewData["PageNumber"] = pageNumber;
            if (model != null)
            {
                return View("/Views/Category/CategoryDetail.cshtml", model);
            }
            else
            {
                return View("/Views/Category/ListCategorys.cshtml");
            }
        }
        [Authorize(Roles = "Admin,Seller")]
        public IActionResult ViewList(int pageNumber = 1)
        {
            ViewData["PageNumber"] = pageNumber;
            return View("/Views/Category/ListCategorys.cshtml");
        }
        [Authorize(Roles = "Admin,Seller")]
        public IActionResult ViewAdd(int pageNumber = 1)
        {
            ViewData["PageNumber"] = pageNumber;
            return View("/Views/Category/AddCategory.cshtml");
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult ViewEdit(int categoryId, int pageNumber = 1)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            Category model = _categoryService.GetCategoryById(categoryId, user.Id);
            ViewData["PageNumber"] = pageNumber;

            if (model != null)
            {
                return View("/Views/Category/EditCategory.cshtml", model);
            }
            else
            {
                return View("/Views/Category/ListCategorys.cshtml");
            }
        }

        [Authorize(Roles = "Admin,Seller")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddCategory(Category model)
        {
            

            if (!ModelState.IsValid)
            {
                // Log the validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                string errorMessages = string.Join("; ", errors);

                Debug.WriteLine(errorMessages);

                // Return the same view with validation errors
                return View("/Views/Category/AddCategory.cshtml", model);
            }
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            //model.SellerId = user.Id;
            int sellerId = user.Id;
            if (sellerId == 0)
            {
                sellerId = HttpContext.Session.GetCustomObjectFromSession<int>("sellerId");
            }

            // Convert the model to your domain entity
            var category = new Category
            {
                CategoryName = model.CategoryName,
                Description = model.Description,
                Status = model.Status,
                SellerId = sellerId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Use the service to insert the product
            _categoryService.AddCategory(category);

            // Notify admins about changes to the product
            await _hubContext.Clients.Group("Admin").SendAsync("CategoryUpdated", category);

            // On success
            HttpContext.Session.SetString("SuccessMsg", "Thêm danh mục thành công!");

            // Redirect to the product list or a success page
            return RedirectToAction("ViewList");
        }

        [Authorize(Roles = "Admin,Seller")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> EditCategory(Category model)
        {
            if (!ModelState.IsValid)
            {
                // Log the validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                string errorMessages = string.Join("; ", errors);

                Debug.WriteLine(errorMessages);

                // Return the same view with validation errors
                return View("/Views/Category/EditCategory.cshtml", model);
            }

            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            Category oldCategory = _categoryService.GetCategoryById(model.Id, user.Id);
            // Convert the model to your domain entity
            var category = new Category
            {
                CategoryName = model.CategoryName,
                Description = model.Description,
                Status = model.Status,
                SellerId = oldCategory.SellerId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Use the service to edit the product
            _categoryService.EditCategory(model.Id, category);

            // Notify admins about changes to the product
            await _hubContext.Clients.Group("Admin").SendAsync("CategoryUpdated", category);

            HttpContext.Session.SetString("SuccessMsg", "Sửa danh mục thành công!");
            // Redirect to the customer list or a success page
            return RedirectToAction("ViewList");
        }

    }
}
