using COCOApp.Models;
using COCOApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using COCOApp.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace COCOApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IHubContext<ProductHub> _hubContext;
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private InventoryMangementService _inventoryMangementService;
        public ProductController(ProductService productService, CategoryService categoryService, IHubContext<ProductHub> hubContext, InventoryMangementService inventoryMangementService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _hubContext = hubContext;
            _inventoryMangementService = inventoryMangementService;
        }
        private const int PageSize = 10;
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult GetList(string nameQuery,int statusId, int pageNumber = 1)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            var products = _productService.GetProducts(nameQuery, pageNumber, PageSize,user.Id,statusId);
            var totalProducts = _productService.GetTotalProducts(nameQuery,user.Id,statusId);

            var response = new
            {
                productResults = products,
                pageNumber = pageNumber,
                totalPages = (int)Math.Ceiling(totalProducts / (double)PageSize)
            };

            return Json(response);
        }
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult GetProduct(int productId, int pageNumber = 1)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            Product model = _productService.GetProductById(productId, user.Id);
            ViewData["PageNumber"] = pageNumber;
            if (model != null)
            {
                // Get the categories and convert them to SelectListItems
                var categories = _categoryService.GetCategories(user.Id)
                                  .Select(c => new SelectListItem
                                  {
                                      Value = c.Id.ToString(),
                                      Text = c.CategoryName
                                  })
                                  .ToList();

                // Pass the categories to the view via ViewBag
                ViewBag.Categories = new SelectList(categories, "Value", "Text");
                return View("/Views/Products/ProductDetail.cshtml", model);
            }
            else
            {
                return View("/Views/Products/ListProducts.cshtml");
            }
        }
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult ViewEdit(int productId, int pageNumber = 1)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            Product model = _productService.GetProductById(productId, user.Id);;
            ViewData["PageNumber"] = pageNumber;
            if (model != null)
            {
                // Get the categories and convert them to SelectListItems
                var categories = _categoryService.GetCategories(user.Id)
                                  .Select(c => new SelectListItem
                                  {
                                      Value = c.Id.ToString(),
                                      Text = c.CategoryName
                                  })
                                  .ToList();

                // Pass the categories to the view via ViewBag
                ViewBag.Categories = new SelectList(categories, "Value", "Text");
                return View("/Views/Products/EditProduct.cshtml", model);
            }
            else
            {
                return View("/Views/Products/ListProducts.cshtml");
            }
        }
        [Authorize(Roles = "Admin,Seller")]
        public IActionResult ViewList(int pageNumber = 1)
        {
            ViewData["PageNumber"] = pageNumber;
            return View("/Views/Products/ListProducts.cshtml");
        }
        [Authorize(Roles = "Admin,Seller")]
        public IActionResult ViewAdd()
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            // Get the categories and convert them to SelectListItems
            var categories = _categoryService.GetCategories(user.Id)
                              .Select(c => new SelectListItem
                              {
                                  Value = c.Id.ToString(),
                                  Text = c.CategoryName
                              })
                              .ToList();

            // Pass the categories to the view via ViewBag
            ViewBag.Categories = new SelectList(categories, "Value", "Text");
            return View("/Views/Products/AddProduct.cshtml");
        }
        [Authorize(Roles = "Admin,Seller")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product model)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            model.SellerId = user.Id;

            if (!ModelState.IsValid)
            {
                // Log the validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                string errorMessages = string.Join("; ", errors);

                Debug.WriteLine(errorMessages);

                // Return the same view with validation errors
                return View("/Views/Products/AddProduct.cshtml", model);
            }
            int sellerId = user.Id;
            if (sellerId == 0)
            {
                sellerId = HttpContext.Session.GetCustomObjectFromSession<int>("sellerId");
            }

            // Convert the model to your domain entity
            var product = new Product
            {
                ProductName = model.ProductName,
                MeasureUnit = model.MeasureUnit,
                Cost = model.Cost,
                Status = model.Status,
                SellerId = sellerId,
                CategoryId = model.CategoryId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Use the service to insert the product
            _productService.AddProduct(product);

            var invenory = new InventoryManagement
            {
                ProductId = product.Id,
                RemainingVolume = 0,
                AllocatedVolume = 0,
                ShippedVolume = 0,
                Product= product,
            };
            _inventoryMangementService.AddInventory(invenory);

            // Notify admins about changes to the product
            await _hubContext.Clients.Group("Admin").SendAsync("ProductUpdated", product);

            // On success
            HttpContext.Session.SetString("SuccessMsg", "Thêm sản phẩm thành công!");

            // Redirect to the product list or a success page
            return RedirectToAction("ViewList");
        }

        [Authorize(Roles = "Admin,Seller")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> EditProduct(Product model)
        {
            if (!ModelState.IsValid)
            {
                // Log the validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                string errorMessages = string.Join("; ", errors);

                Debug.WriteLine(errorMessages);

                // Return the same view with validation errors
                return View("/Views/Products/EditProduct.cshtml", model);
            }

            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            Product oldProduct=_productService.GetProductById(model.Id, user.Id);
            // Convert the model to your domain entity
            var product = new Product
            {
                ProductName = model.ProductName,
                MeasureUnit = model.MeasureUnit,
                Cost = model.Cost,
                Status = model.Status,
                SellerId = oldProduct.SellerId,
                CategoryId = model.CategoryId,
                UpdatedAt = DateTime.Now
            };

            // Use the service to edit the product
            _productService.EditProduct(model.Id, product);

            // Notify admins about changes to the product
            await _hubContext.Clients.Group("Admin").SendAsync("ProductUpdated", product);

            HttpContext.Session.SetString("SuccessMsg", "Sửa hàng thành công!");
            // Redirect to the customer list or a success page
            return RedirectToAction("ViewList");
        }
    }
}
