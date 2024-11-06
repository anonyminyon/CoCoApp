using COCOApp.Helpers;
using COCOApp.Models;
using COCOApp.Services;
using COCOApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace COCOApp.Controllers
{
    public class ImportOrderController : Controller
    {
        private readonly ImportOrderService _orderService;
        private readonly ProductService _productService;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly ImportOrderItemService _itemService;

        public ImportOrderController(ImportOrderService orderService, ProductService productService, IHubContext<OrderHub> hubContext, ImportOrderItemService itemService)
        {
            _orderService = orderService;
            _productService = productService;
            _hubContext = hubContext;
            _itemService = itemService;
        }
        private const int PageSize = 10;


        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult GetImportOrdersList(string nameQuery, int pageNumber = 1)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            var orders = _orderService.GetImportOrders(nameQuery, pageNumber, PageSize, user.Id);
            var totalOrders = _orderService.GetTotalImportOrders(nameQuery, user.Id);
            
            var response = new
            {
                orderResults = orders,
                pageNumber = pageNumber,
                totalPages = (int)Math.Ceiling(totalOrders / (double)PageSize)
            };

            return Json(response);
        }
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult GetOrderItemsList(int orderId, string nameQuery, int pageNumber = 1)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            var orders = _itemService.GetImportOrderItems(orderId, nameQuery, pageNumber, PageSize, user.Id);
            var totalOrders = _itemService.GetTotalImportOrderItems(orderId, nameQuery, user.Id);

            var response = new
            {
                orderResults = orders,
                pageNumber = pageNumber,
                totalPages = (int)Math.Ceiling(totalOrders / (double)PageSize)
            };

            return Json(response);
        }
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult GetOrderItemsListNoPaging(int orderId)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            var orders = _itemService.GetImportOrderItems(orderId, user.Id);

            var response = new
            {
                orderResults = orders,
            };
            return Json(response);
        }
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult ViewDetail(int orderId, int pageNumber = 1)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            ImportOrder model = _orderService.GetImportOrderById(orderId, user.Id); ;
            ViewData["PageNumber"] = pageNumber;
            if (model != null)
            {
                return View("/Views/ImportOrder/OrderDetail.cshtml", model);
            }
            else
            {
                return View("/Views/ImportOrder/ListOrders.cshtml");
            }
        }
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult ViewOrderItemDetail(int orderId, int productId, int pageNumber = 1)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            ImportOrderItem model = _itemService.GetImportOrderitemById(orderId, productId, user.Id); ;
            ViewData["PageNumber"] = pageNumber;
            if (model != null)
            {
                return View("/Views/ImportOrder/OrderItemDetail.cshtml", model);
            }
            else
            {
                return View("/Views/ImportOrder/ListOrders.cshtml");
            }
        }
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult ViewEdit(int orderId, int pageNumber = 1)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            ImportOrder model = _orderService.GetImportOrderById(orderId, user.Id); ;
            ViewBag.Customers = _orderService.GetSuppliersSelectList(user.Id);
            ViewBag.Products = _orderService.GetProductsSelectList(user.Id);
            ViewData["PageNumber"] = pageNumber;
            if (model != null)
            {
                return View("/Views/ImportOrder/EditOrder.cshtml", model);
            }
            else
            {
                return View("/Views/ImportOrder/ListOrders.cshtml");
            }
        }
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult ViewEditOrderItem(int orderId, int productId, int pageNumber = 1)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            ImportOrderItem model = _itemService.GetImportOrderitemById(orderId, productId, user.Id); ;
            ViewBag.Suppliers = _orderService.GetSuppliersSelectList(user.Id);
            ViewBag.Products = _orderService.GetProductsSelectList(user.Id);
            ViewData["PageNumber"] = pageNumber;
            if (model != null)
            {
                return View("/Views/ImportOrder/EditOrderItem.cshtml", model);
            }
            else
            {
                return View("/Views/ImportOrder/ListOrders.cshtml");
            }
        }
        [Authorize(Roles = "Admin,Seller")]
        public IActionResult ViewList(int pageNumber = 1)
        {
            ViewData["PageNumber"] = pageNumber;
            return View("/Views/ImportOrder/ListOrders.cshtml");
        }
        [Authorize(Roles = "Admin,Seller")]
        public IActionResult ViewOrderItemsList(int orderId, int pageNumber = 1)
        {
            ViewData["PageNumber"] = pageNumber;
            ViewData["OrderId"] = orderId;
            return View("/Views/ImportOrder/ListItems.cshtml");
        }
        [Authorize(Roles = "Admin,Seller")]
        public IActionResult Add()
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            ViewBag.Suppliers = _orderService.GetSuppliersSelectList(user.Id);
            ViewBag.Products = _orderService.GetProductsSelectList(user.Id);
            var viewModel = new MultiImportOrderViewModel();
            return View("/Views/ImportOrder/AddOrder.cshtml", viewModel);
        }
        [Authorize(Roles = "Admin,Seller")]
        // POST: ImportOrder/CreateMultiple
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateMultiple(MultiImportOrderViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the seller ID from session
                User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
                int sellerId = user?.Id ?? 0;
                if (sellerId == 0)
                {
                    sellerId = HttpContext.Session.GetCustomObjectFromSession<int>("sellerId");
                }

                // Sort Orders by SupplierId and OrderDate
                var sortedOrders = viewModel.ImportOrders
                                            .OrderBy(order => order.SupplierId)
                                            .ThenBy(order => order.OrderDate)
                                            .ToList();

                int i = 0;
                Debug.WriteLine(sortedOrders.Count);
                while (i < sortedOrders.Count)
                {
                    var order = sortedOrders[i];
                    ImportOrder importOrder = _orderService.GetImportOrderByCustomerAndDate(order.SupplierId, order.OrderDate);
                    if (importOrder == null)
                    {
                        importOrder = new ImportOrder
                        {
                            SupplierId = order.SupplierId,
                            OrderDate = order.OrderDate,
                            Complete = false,
                            OrderTotal = 0,
                            SellerId = sellerId,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        _orderService.AddImportOrder(importOrder);
                    }
                    // Add items for all orders with the same SupplierId and OrderDate
                    while (i < sortedOrders.Count && order.SupplierId == sortedOrders[i].SupplierId && order.OrderDate == sortedOrders[i].OrderDate)
                    {
                        var currentOrder = sortedOrders[i];
                        Product product = _productService.GetProductById(currentOrder.ProductId, user.Id);

                        var importOrderItem = new ImportOrderItem
                        {
                            OrderId = importOrder.Id,
                            ProductId = currentOrder.ProductId,
                            Volume = currentOrder.ProductVolume,
                            ProductCost = product.Cost,
                            //Total = currentOrder.ProductVolume * product.Cost,
                            //SellerId = sellerId,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        _itemService.AddImportOrderItem(importOrderItem);

                        i++; // Increment index to process the next order
                    }
                }

                HttpContext.Session.SetString("SuccessMsg", "Thêm đơn hàng thành công!");
                return RedirectToAction("ViewList"); // Redirect to action "ViewList" if model state is valid
            }
            // Log the validation errors if any
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            string errorMessages = string.Join("; ", errors);
            Debug.WriteLine(errorMessages);

            return RedirectToAction("Add"); // Redirect to action "Add" if model state is not valid
        }



        //[ValidateAntiForgeryToken]
        //[HttpPost]
        //public async Task<IActionResult> AddOrders(ImportOrder order,List<ImportOrderItem> orders)
        //{
        //    if (orders.Count == 0)
        //    {
        //        HttpContext.Session.SetString("ErrorMsg", "Không có đơn hàng nào!");
        //        return RedirectToAction("Add"); // Redirect to action "ViewList" if model state is not valid
        //    }
        //    User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
        //    foreach (var model in orders)
        //    {
        //        Product product = _productService.GetProductById(model.ProductId, user.Id);
        //        int sellerId = user.Id;
        //        if (sellerId == 0)
        //        {
        //            sellerId = HttpContext.Session.GetCustomObjectFromSession<int>("sellerId");
        //        }
        //        // Convert the model to your domain entity
        //        var order = new ImportOrder
        //        {
        //            SupplierId = model.SupplierId,
        //            ProductId = model.ProductId,
        //            Volume = model.Volume,
        //            OrderDate = model.OrderDate,
        //            Complete = false,
        //            OrderProductCost = product.Cost,
        //            OrderTotal = product.Cost * model.Volume,
        //            SellerId = sellerId,
        //            CreatedAt = DateTime.Now,
        //            UpdatedAt = DateTime.Now
        //        };

        //        _orderService.AddOrder(order);
        //        // Notify admins about changes to the order
        //        await _hubContext.Clients.Group("Admin").SendAsync("OrderUpdated", order);
        //    }

        //    HttpContext.Session.SetString("SuccessMsg", "Thêm đơn hàng thành công!");
        //    return RedirectToAction("ViewList"); // Redirect to action "ViewList" if model state is valid

        //}
        [Authorize(Roles = "Admin,Seller")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> EditOrder(ImportOrder model)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            if (!ModelState.IsValid)
            {
                // Log the validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                string errorMessages = string.Join("; ", errors);

                Debug.WriteLine(errorMessages);
                // If the model state is not valid, return the same view with validation errors
                ViewBag.Customers = _orderService.GetSuppliersSelectList(user.Id);
                ViewBag.Products = _orderService.GetProductsSelectList(user.Id);
                return View("/Views/ImportOrder/EditOrder.cshtml", model);
            }
            ImportOrder oldOrder = _orderService.GetImportOrderById(model.Id, user.Id);
            // Convert the model to your domain entity
            var order = new ImportOrder
            {
                SupplierId = model.SupplierId,
                OrderDate = model.OrderDate,
                SellerId = oldOrder.SellerId,
                UpdatedAt = DateTime.Now
            };

            // Use the service to edit the customer
            _orderService.EditImportOrder(model.Id, order);

            // Notify admins about changes to the order
            await _hubContext.Clients.Group("Admin").SendAsync("OrderUpdated", order);

            HttpContext.Session.SetString("SuccessMsg", "Sửa đơn hàng thành công!");
            // Redirect to the customer list or a success page
            return RedirectToAction("ViewList");
        }
        [Authorize(Roles = "Admin,Seller")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> EditOrderItem(ImportOrderItem model)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");

            if (ModelState.IsValid || model.Volume < model.RealVolume)
            {
                // Log the validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                string errorMessages = string.Join("; ", errors);

                Debug.WriteLine(errorMessages);
                // If the model state is not valid, return the same view with validation errors
                ViewBag.Customers = _orderService.GetSuppliersSelectList(user.Id);
                ViewBag.Products = _orderService.GetProductsSelectList(user.Id);
                return View("/Views/ImportOrder/EditOrderItem.cshtml", model);
            }
            ImportOrderItem oldOrder = _itemService.GetImportOrderitemById(model.OrderId, model.ProductId, user.Id);
            //if Real volumm = volum tức là supplier đã cung cấp đủ số lượng yêu cầu lúc này chuyển trạng thái của ImportOrderItem đó sang true
                // Convert the model to your domain entity
                var order = new ImportOrderItem
                {
                    OrderId = model.OrderId,
                    ProductId = model.ProductId,
                    Volume = model.Volume,
                    ProductCost = model.ProductCost,
                    RealVolume = model.RealVolume,
                    //Total = model.ProductCost * model.Volume,
                    //SellerId = oldOrder.Order.SellerId,
                    UpdatedAt = DateTime.Now,
                    Status = model.Volume == model.RealVolume ? true : false
                };
            
            
            // Use the service to edit the customer
            _itemService.EditImportOrderItem(model.OrderId, model.ProductId, order);

            // Notify admins about changes to the order
            await _hubContext.Clients.Group("Admin").SendAsync("OrderUpdated", order);

            HttpContext.Session.SetString("SuccessMsg", "Sửa đơn hàng thành công!");
            // Redirect to the customer list or a success page
            return RedirectToAction("ViewList");
        }
    }
}
