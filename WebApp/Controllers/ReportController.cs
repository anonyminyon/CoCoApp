using COCOApp.Models;
using COCOApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using COCOApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Layout.Borders;
using iText.Layout.Properties;

namespace COCOApp.Controllers
{
    public class ReportController : Controller
    {
        private readonly ExportOrderService _orderService;
        private readonly UserService _userService;
        private readonly ReportService _reportService;
        private readonly ExportOrderItemService _itemService;
        private readonly UserDetailsService _userDetailsService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReportController(ExportOrderService orderService, UserService userService, ReportService reportService, ExportOrderItemService itemService, UserDetailsService userDetailsService, IWebHostEnvironment webHostEnvironment)
        {
            _orderService = orderService;
            _userService = userService;
            _reportService = reportService;
            _itemService = itemService;
            _userDetailsService = userDetailsService;
            _webHostEnvironment = webHostEnvironment;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }
        [Authorize(Roles = "Admin,Seller")]
        public IActionResult ViewCreate()
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            ViewBag.Customers = _orderService.GetCustomersSelectList(user.Id);
            return View("/Views/Report/CreateReport.cshtml");
        }
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        public IActionResult GetOrders(int customerId, string daterange)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            ViewBag.Customers = _orderService.GetCustomersSelectList(user.Id);
            List<ExportOrder> orders = _orderService.GetExportOrders(daterange, customerId, user.Id);
            if (orders.Count > 0)
            {
                string dateRange = orders[orders.Count - 1].OrderDate.ToString("MM / dd / yyyy") + " - " + orders[0].OrderDate.ToString("MM / dd / yyyy");
            }
            HttpContext.Session.SetObjectInSession("dateRange", daterange);
            return View("/Views/Report/CreateReport.cshtml", orders);
        }
        [Authorize(Roles = "Admin,Seller")]
        [HttpPost]
        public IActionResult CreateSummary(List<int> orderIds)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            if (orderIds == null || orderIds.Count <= 0)
            {
                ViewBag.Customers = _orderService.GetCustomersSelectList(user.Id);
                return View("/Views/Report/CreateReport.cshtml");
            }
            // Assuming _orderService can fetch orders by their IDs
            List<ExportOrder> orders = _orderService.GetExportOrdersByIds(orderIds, user.Id);
            // Assuming _orderService can fetch orders by their IDs
            List<ExportOrderItem> orderItems = _itemService.GetExportOrderItemsByIds(orderIds, user.Id);
            int sellerId = user.Id;
            if (sellerId == 0)
            {
                sellerId = HttpContext.Session.GetCustomObjectFromSession<int>("sellerId");
            }
            Report report = new Report()
            {
                CustomerId = orders[0].CustomerId,
                TotalPrice = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                SellerId = sellerId,
            };
            _reportService.AddReport(report);
            foreach (var item in orderItems)
            {
                ReportDetail reportDetail = new ReportDetail()
                {
                    ReportId = report.Id,
                    ProductId = item.ProductId,
                    Volume = item.RealVolume,
                    TotalPrice = item.ProductPrice * item.Volume,
                    OrderDate = item.Order.OrderDate
                };
                _reportService.AddReportDetails(reportDetail);
            }
            List<ReportDetail> reportDetails = _reportService.GetReportDetails(report.Id);

            return View("/Views/Report/ReportSummary.cshtml", reportDetails);
        }
        [Authorize(Roles = "Admin,Seller")]
        [HttpPost]
        public IActionResult CreateInvoice(List<int> orderIds)
        {
            User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
            List<ExportOrder> orders = _orderService.GetExportOrdersByIds(orderIds, user.Id);
            if (orders == null || orders.Count == 0)
            {
                return View("/Views/Report/CreateReport.cshtml");
            }
            decimal ordersTotalCost = 0;
            for (int i = 0; i < orders.Count; i++)
            {
                ExportOrder order = orders[i];
                //order.OrderProductCost = costs[i];
                //order.OrderTotal = costs[i] * order.Volume;
                ordersTotalCost += order.OrderTotal;
            }
            ViewBag.totalCost = ordersTotalCost;
            // Pass orders to the view
            return View("/Views/Report/Invoice.cshtml", orders);
        }
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> Print(List<ReportDetail> reportDetails)
        {
            if (reportDetails == null || !reportDetails.Any())
            {
                ModelState.AddModelError("", "No details provided.");
                return RedirectToAction("ViewCreate");
            }

            try
            {
                User user = HttpContext.Session.GetCustomObjectFromSession<User>("user");
                int sellerId = user.Id == 0 ? HttpContext.Session.GetCustomObjectFromSession<int>("sellerId") : user.Id;

                user = _userService.GetUserById(sellerId);
                Report report = _reportService.GetReportById(reportDetails[0].ReportId, user.Id);
                string dateRange = HttpContext.Session.GetCustomObjectFromSession<string>("dateRange");
                List<ExportOrderItem> orderItems = _itemService.GetExportOrderItems(dateRange, report.Customer.Id, user.Id);

                int totalQuantity = orderItems.Sum(item => item.RealVolume);
                decimal totalCost = orderItems.Sum(item => item.Total);
                byte[] imageBytes = user.SellerDetail.ImageData;

                using (MemoryStream stream = new MemoryStream())
                {
                    PdfWriter writer = new PdfWriter(stream);
                    PdfDocument pdf = new PdfDocument(writer);
                    Document document = new Document(pdf, iText.Kernel.Geom.PageSize.A4);
                    document.SetMargins(20, 20, 20, 20);  // Improved margins

                    // Font Setup
                    string fontPath = Path.Combine(_webHostEnvironment.WebRootPath, "fonts", "arial-unicode-ms-regular.ttf");
                    PdfFont font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
                    PdfFont boldFont = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);

                    // Header
                    AddHeader(document, user, font, boldFont);
                    // Title: "Bảng tổng kết"
                    document.Add(new Paragraph("\nBảng tổng kết")
                        .SetFont(boldFont)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(16)  // Slightly larger font for title
                        .SetMarginBottom(10));  // Add spacing below the title
                    // Customer Information and Date Range
                    document.Add(new Paragraph($"Tên khách hàng: {report.Customer.Name}").SetFont(font));
                    document.Add(new Paragraph($"Địa chỉ: {report.Customer.Address}").SetFont(font));
                    document.Add(new Paragraph($"Từ ngày: {orderItems.First().Order.OrderDate:dd-MM-yyyy} đến ngày: {orderItems.Last().Order.OrderDate:dd-MM-yyyy}").SetFont(font));
                    document.Add(new Paragraph("\n"));

                    // Order Items Table
                    AddOrderItemsTable(document, orderItems, font, boldFont);

                    // Footer with Summary
                    AddFooter(document, totalQuantity, totalCost, font, boldFont);

                    // Payment Info and Signature
                    AddSignatureSection(document, imageBytes, font, boldFont);

                    document.Close();

                    byte[] pdfBytes = stream.ToArray();
                    return File(pdfBytes, "application/pdf", "OrderReport.pdf");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating report: {ex.Message}");
                ModelState.AddModelError("", "Failed to generate report.");
                return RedirectToAction("ViewCreate");
            }
        }

        // Header Section
        private void AddHeader(Document document, User user, PdfFont font, PdfFont boldFont)
        {
            Table headerTable = new Table(UnitValue.CreatePercentArray(new float[] { 70, 30 })).UseAllAvailableWidth();
            headerTable.AddCell(CreateCell("Cửa hàng:", boldFont, TextAlignment.LEFT, Border.NO_BORDER));
            headerTable.AddCell(CreateCell("Số điện thoại:", boldFont, TextAlignment.RIGHT, Border.NO_BORDER));
            headerTable.AddCell(CreateCell(user.SellerDetail.BusinessName, font, TextAlignment.LEFT, Border.NO_BORDER));
            headerTable.AddCell(CreateCell(user.UserDetail.Phone, font, TextAlignment.RIGHT, Border.NO_BORDER));
            headerTable.AddCell(CreateCell($"Địa chỉ: {user.SellerDetail.BusinessAddress}", font, TextAlignment.LEFT, Border.NO_BORDER, colspan: 2));
            document.Add(headerTable);
        }

        // Order Items Table
        private void AddOrderItemsTable(Document document, List<ExportOrderItem> orderItems, PdfFont font, PdfFont boldFont)
        {
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 15, 25, 15, 15, 15, 15 })).UseAllAvailableWidth();

            // Header Row
            string[] headers = { "Ngày đặt", "Sản phẩm", "Giá", "Đơn vị tính", "Số lượng", "Tổng tiền" };
            foreach (var header in headers)
            {
                table.AddHeaderCell(new Cell().Add(new Paragraph(header).SetFont(boldFont))
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER));
            }

            // Data Rows
            foreach (var item in orderItems)
            {
                table.AddCell(CreateCell(item.Order.OrderDate.ToString("dd-MM-yyyy"), font));
                table.AddCell(CreateCell(item.Product.ProductName, font));
                table.AddCell(CreateCell($"{item.Product.Cost} VND", font));
                table.AddCell(CreateCell(item.Product.MeasureUnit, font));
                table.AddCell(CreateCell(item.RealVolume.ToString(), font));
                table.AddCell(CreateCell($"{item.Total} VND", font));
            }

            document.Add(table);
        }

        // Footer with Summary
        private void AddFooter(Document document, int totalQuantity, decimal totalCost, PdfFont font, PdfFont boldFont)
        {
            Table footerTable = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 })).UseAllAvailableWidth();
            footerTable.AddCell(CreateCell($"Tổng số lượng: {totalQuantity}", boldFont, TextAlignment.LEFT));
            footerTable.AddCell(CreateCell($"Tổng giá: {totalCost} VND", boldFont, TextAlignment.RIGHT));
            document.Add(footerTable);
        }

        // Signature Section
        private void AddSignatureSection(Document document, byte[] imageBytes, PdfFont font, PdfFont boldFont)
        {
            Table signatureTable = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 })).UseAllAvailableWidth();
            Cell leftCell = new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Thông tin thanh toán").SetFont(boldFont));
            if (imageBytes != null)
            {
                Image image = new Image(ImageDataFactory.Create(imageBytes)).ScaleToFit(180, 150);
                leftCell.Add(image);
            }
            signatureTable.AddCell(leftCell);

            Cell rightCell = new Cell().SetBorder(Border.NO_BORDER)
                .Add(new Paragraph("Chữ ký xác nhận:.................................").SetFont(font))
                .SetTextAlignment(TextAlignment.RIGHT).SetPaddingTop(40);
            signatureTable.AddCell(rightCell);

            document.Add(signatureTable);
        }

        // Helper to Create Cells
        private Cell CreateCell(string content, PdfFont font, TextAlignment alignment = TextAlignment.LEFT, Border border = null, int colspan = 1)
        {
            var cell = new Cell(1, colspan).Add(new Paragraph(content).SetFont(font)).SetTextAlignment(alignment);
            if (border != null) cell.SetBorder(border);
            return cell;
        }
    }
}
