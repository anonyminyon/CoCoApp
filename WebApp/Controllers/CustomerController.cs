using COCOApp.Helpers;
using COCOApp.Models;
using COCOApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using Org.BouncyCastle.Utilities;
using System.Diagnostics;

namespace COCOApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IHubContext<CustomerHub> _hubContext;
        private CustomerService _customerService;
        public CustomerController(CustomerService customerService, IHubContext<CustomerHub> hubContext)
        {
            _customerService = customerService;
            _hubContext = hubContext;
        }
        private const int PageSize = 10;

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult GetList(string nameQuery, int statusId, int pageNumber = 1)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            var customers = _customerService.GetCustomers(nameQuery, pageNumber, PageSize, user.Id, statusId);
            var totalCustomers = _customerService.GetTotalCustomers(nameQuery, user.Id, statusId);

            var response = new
            {
                customerResults = customers,
                pageNumber = pageNumber,
                totalPages = (int)Math.Ceiling(totalCustomers / (double)PageSize)
            };

            return Json(response);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult GetCustomer(int customerId, int pageNumber = 1)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            Customer model = _customerService.GetCustomerByIdForSeller(customerId, user.Id);
            ViewData["PageNumber"] = pageNumber;
            if (model != null)
            {
                return View("/Views/Customer/CustomerDetail.cshtml", model);
            }
            else
            {
                return View("/Views/Customer/ListCustomers.cshtml");
            }
        }
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult ViewEdit(int customerId, int pageNumber = 1)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            Customer model = _customerService.GetCustomerByIdForSeller(customerId, user.Id);
            ViewData["PageNumber"] = pageNumber;
            if (model != null)
            {
                return View("/Views/Customer/EditCustomer.cshtml", model);
            }
            else
            {
                return View("/Views/Customer/ListCustomers.cshtml");
            }
        }

        [Authorize(Roles = "Admin,Seller")]
        public IActionResult ViewAdd()
        {
            return View("/Views/Customer/AddCustomer.cshtml");
        }

        [Authorize(Roles = "Admin,Seller")]
        public IActionResult ViewList(int pageNumber = 1)
        {
            ViewData["PageNumber"] = pageNumber;
            return View("/Views/Customer/ListCustomers.cshtml");
        }
        [Authorize(Roles = "Admin,Seller")]
        public IActionResult ViewDetail()
        {
            return View("/Views/Customer/CustomerDetail.cshtml");
        }
        [Authorize(Roles = "Admin,Seller")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddCustomer(Customer model)
        {
            if (!ModelState.IsValid)
            {
                // Log the validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                string errorMessages = string.Join("; ", errors);

                Debug.WriteLine(errorMessages);
                // If the model state is not valid, return the same view with validation errors
                return View("/Views/Customer/AddCustomer.cshtml", model);
            }
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            int sellerId = user.Id;
            if (sellerId == 0)
            {
                sellerId = HttpContext.Session.GetCustomObjectFromSession<int>("sellerId");
            }
            // Convert the model to your domain entity
            var customer = new Customer
            {
                Name = model.Name,
                Phone = model.Phone,
                Address = model.Address,
                Note = model.Note,  // Note property is nullable
                Status = model.Status,
                SellerId = sellerId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Use the service to insert the customer
            _customerService.AddCustomer(customer);

            // Notify admins about changes to the customer
            await _hubContext.Clients.Group("Admin").SendAsync("CustomerUpdated", customer);

            HttpContext.Session.SetString("SuccessMsg", "Thêm khách hàng thành công!");
            // Redirect to the customer list or a success page
            return RedirectToAction("ViewList");
        }
        [Authorize(Roles = "Admin,Seller")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> EditCustomer(Customer model)
        {

            if (!ModelState.IsValid)
            {
                // Log the validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                string errorMessages = string.Join("; ", errors);

                Debug.WriteLine(errorMessages);
                // If the model state is not valid, return the same view with validation errors
                return View("/Views/Customer/EditCustomer.cshtml", model);
            }
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            Customer oldCustomer = _customerService.GetCustomerByIdForSeller(model.Id, user.Id);
            // Convert the model to your domain entity
            var customer = new Customer
            {
                Name = model.Name,
                Phone = model.Phone,
                Address = model.Address,
                Note = model.Note,  // Note property is nullable
                Status = model.Status,
                SellerId = oldCustomer.SellerId,
                UpdatedAt = DateTime.Now
            };

            // Use the service to edit the customer
            _customerService.EditCustomer(model.Id, customer);

            // Notify admins about changes to the customer
            await _hubContext.Clients.Group("Admin").SendAsync("CustomerUpdated", customer);

            HttpContext.Session.SetString("SuccessMsg", "Sửa khách hàng thành công!");
            // Redirect to the customer list or a success page
            return RedirectToAction("ViewList");
        }
        [HttpGet]
        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            int sellerId = user.Id;
            if (sellerId == 0)
            {
                sellerId = HttpContext.Session.GetCustomObjectFromSession<int>("sellerId");
            }
            var customers = _customerService.GetAllMyCustomers(sellerId);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Customers");

                // Adding Headers
                worksheet.Cells[1, 1].Value = "Mã khách";
                worksheet.Cells[1, 2].Value = "Tên khách hàng";
                worksheet.Cells[1, 3].Value = "Địa chỉ";
                worksheet.Cells[1, 4].Value = "Số điện thoại";
                worksheet.Cells[1, 5].Value = "Ghi chú";
                worksheet.Cells[1, 6].Value = "Trạng thái";

                // Adding Data
                for (int i = 0; i < customers.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = customers[i].Id;
                    worksheet.Cells[i + 2, 2].Value = customers[i].Name;
                    worksheet.Cells[i + 2, 3].Value = customers[i].Address;
                    worksheet.Cells[i + 2, 4].Value = customers[i].Phone;
                    worksheet.Cells[i + 2, 5].Value = customers[i].Note;
                    worksheet.Cells[i + 2, 6].Value = customers[i].Status ? "Đã kích hoạt" : "Chưa kích hoạt";
                }

                // Prepare the file to be downloaded
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"Danh_Sách_Khách-{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";

                // Return the Excel file
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
        // Import From Excel
        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile excelFile)
        {
            try
            {
                // Ensure a file is uploaded
                if (excelFile == null || excelFile.Length <= 0)
                {
                    TempData["Error"] = "Please upload a valid Excel file.";
                    return RedirectToAction("Index");
                }

                // Set license context for EPPlus (non-commercial)
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
                int sellerId = user.Id;
                if (sellerId == 0)
                {
                    sellerId = HttpContext.Session.GetCustomObjectFromSession<int>("sellerId");
                }
                // Load the uploaded Excel file
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0]; // Assuming the data is in the first sheet

                        var rowCount = worksheet.Dimension.Rows;
                        var customers = new List<Customer>();

                        for (int row = rowCount; row >= 2; row--) // Start from row 2 to skip header
                        {
                        int id = Convert.ToInt32(worksheet.Cells[row, 1].Value);
                            var customer = new Customer
                            {
                                Name = worksheet.Cells[row, 2].Text,
                                Address = worksheet.Cells[row, 3].Text,
                                Phone = worksheet.Cells[row, 4].Text,
                                Note = worksheet.Cells[row, 5].Text,
                                Status = worksheet.Cells[row, 6].Text == "Đã kích hoạt",
                                SellerId = sellerId,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };
                            if (_customerService.GetCustomerById(id) == null)
                            {
                                _customerService.AddCustomer(customer);
                            }
                        }
                    }
                }
                HttpContext.Session.SetString("SuccessMsg", "Nhập thành công!");
                return RedirectToAction("ViewList");
        }
            catch (Exception ex)
            {
                // Log the error (optional: log ex.Message to a file or logging system)
                ModelState.AddModelError("", "An error occurred while processing the file: " + ex.Message);
                HttpContext.Session.SetString("ErrorMsg", "File lỗi hoặc không đúng!");
                return RedirectToAction("ViewList");
    }
}
    }

}
