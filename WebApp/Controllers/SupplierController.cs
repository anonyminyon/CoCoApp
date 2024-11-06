using COCOApp.Helpers;
using COCOApp.Models;
using COCOApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using System.Diagnostics;

namespace COCOApp.Controllers
{
    public class SupplierController : Controller
    {
        private readonly IHubContext<SupplierHub> _hubContext;
        private SupplierService _supplierService;
        public SupplierController(SupplierService supplierService, IHubContext<SupplierHub> hubContext)
        {
            _supplierService = supplierService;
            _hubContext = hubContext;
        }
        private const int PageSize = 10;

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult GetList(string nameQuery, int statusId, int pageNumber = 1)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            var suppliers = _supplierService.GetSuppliers(nameQuery, pageNumber, PageSize, user.Id, statusId);
            var totalSuppliers = _supplierService.GetTotalSuppliers(nameQuery, user.Id, statusId);

            var response = new
            {
                supplierResults = suppliers,
                pageNumber = pageNumber,
                totalPages = (int)Math.Ceiling(totalSuppliers / (double)PageSize)
            };

            return Json(response);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult GetSupplier(int supplierId, int pageNumber = 1)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            Supplier model = _supplierService.GetSupplierById(supplierId, user.Id);
            ViewData["PageNumber"] = pageNumber;
            if (model != null)
            {
                return View("/Views/Supplier/SupplierDetail.cshtml", model);
            }
            else
            {
                return View("/Views/Supplier/ListSuppliers.cshtml");
            }
        }
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult ViewEdit(int supplierId, int pageNumber = 1)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            Supplier model = _supplierService.GetSupplierById(supplierId, user.Id);
            ViewData["PageNumber"] = pageNumber;
            if (model != null)
            {
                return View("/Views/Supplier/EditSupplier.cshtml", model);
            }
            else
            {
                return View("/Views/Supplier/ListSuppliers.cshtml");
            }
        }

        [Authorize(Roles = "Admin,Seller")]
        public IActionResult ViewAdd()
        {
            return View("/Views/Supplier/AddSupplier.cshtml");
        }

        [Authorize(Roles = "Admin,Seller")]
        public IActionResult ViewList(int pageNumber = 1)
        {
            ViewData["PageNumber"] = pageNumber;
            return View("/Views/Supplier/ListSuppliers.cshtml");
        }
        [Authorize(Roles = "Admin,Seller")]
        public IActionResult ViewDetail()
        {
            return View("/Views/Supplier/SupplierDetail.cshtml");
        }
        [Authorize(Roles = "Admin,Seller")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddSupplier(Supplier model)
        {
            if (!ModelState.IsValid)
            {
                // Log the validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                string errorMessages = string.Join("; ", errors);

                Debug.WriteLine(errorMessages);
                // If the model state is not valid, return the same view with validation errors
                return View("/Views/Supplier/AddSupplier.cshtml", model);
            }
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            int sellerId = user.Id;
            if (sellerId == 0)
            {
                sellerId = HttpContext.Session.GetCustomObjectFromSession<int>("sellerId");
            }
            // Convert the model to your domain entity
            var supplier = new Supplier
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

            // Use the service to insert the supplier
            _supplierService.AddSupplier(supplier);

            // Notify admins about changes to the supplier
            await _hubContext.Clients.Group("Admin").SendAsync("SupplierUpdated", supplier);

            HttpContext.Session.SetString("SuccessMsg", "Thêm nhà cung cấp thành công!");
            // Redirect to the supplier list or a success page
            return RedirectToAction("ViewList");
        }
        [Authorize(Roles = "Admin,Seller")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> EditSupplier(Supplier model)
        {

            if (!ModelState.IsValid)
            {
                // Log the validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                string errorMessages = string.Join("; ", errors);

                Debug.WriteLine(errorMessages);
                // If the model state is not valid, return the same view with validation errors
                return View("/Views/Supplier/EditSupplier.cshtml", model);
            }
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            Supplier oldSupplier = _supplierService.GetSupplierById(model.Id, user.Id);
            // Convert the model to your domain entity
            var supplier = new Supplier
            {
                Name = model.Name,
                Phone = model.Phone,
                Address = model.Address,
                Note = model.Note,  // Note property is nullable
                Status = model.Status,
                SellerId = oldSupplier.SellerId,
                UpdatedAt = DateTime.Now
            };

            // Use the service to edit the supplier
            _supplierService.EditSupplier(model.Id, supplier);

            // Notify admins about changes to the supplier
            await _hubContext.Clients.Group("Admin").SendAsync("SupplierUpdated", supplier);

            HttpContext.Session.SetString("SuccessMsg", "Sửa nhà cung cấp  thành công!");
            // Redirect to the supplier list or a success page
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
            var suppliers = _supplierService.GetAllMySuppliers(sellerId);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Suppliers");

                // Adding Headers
                worksheet.Cells[1, 1].Value = "Mã nhà cung cấp";
                worksheet.Cells[1, 2].Value = "Tên nhà cung cấp";
                worksheet.Cells[1, 3].Value = "Địa chỉ";
                worksheet.Cells[1, 4].Value = "Số điện thoại";
                worksheet.Cells[1, 5].Value = "Ghi chú";
                worksheet.Cells[1, 6].Value = "Trạng thái";

                // Adding Data
                for (int i = 0; i < suppliers.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = suppliers[i].Id;
                    worksheet.Cells[i + 2, 2].Value = suppliers[i].Name;
                    worksheet.Cells[i + 2, 3].Value = suppliers[i].Address;
                    worksheet.Cells[i + 2, 4].Value = suppliers[i].Phone;
                    worksheet.Cells[i + 2, 5].Value = suppliers[i].Note;
                    worksheet.Cells[i + 2, 6].Value = suppliers[i].Status ? "Đã kích hoạt" : "Chưa kích hoạt";
                }

                // Prepare the file to be downloaded
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"Danh_Sách_Nhà_Cung_cấp-{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";

                // Return the Excel file
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
        // Import From Excel
        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile excelFile)
        {
            //try
            //{
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
                    var suppliers = new List<Supplier>();

                    for (int row = rowCount; row >= 2; row--) // Start from row 2 to skip header
                    {
                        int id = Convert.ToInt32(worksheet.Cells[row, 1].Value);
                        var supplier = new Supplier
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
                        if (_supplierService.GetSupplierById(id) == null)
                        {
                            _supplierService.AddSupplier(supplier);
                        }
                    }
                }
            }
            HttpContext.Session.SetString("SuccessMsg", "Nhập thành công!");
            return RedirectToAction("ViewList");
        }
    }

}
