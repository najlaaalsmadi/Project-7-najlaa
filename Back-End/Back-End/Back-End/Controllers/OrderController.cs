using Back_End.DTO;
using Back_End.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using OfficeOpenXml;

namespace Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly MyDbContext _myDbContext;

        public OrderController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }

        [HttpPost("MoveItemsToOrder/{userId}")]
        public IActionResult MoveItemsToOrder(List<OrderItemsDTO> order, int userId)
        {
            // check if cart items are empty
            if (order == null || order.Count == 0)
            {
                return BadRequest("The cart is empty");
            }


            // find total
            decimal? total = 0;
            foreach (var Item in order)
            {
                if (Item.Format.Contains("Copy"))
                {
                    total += (Item.Price * Item.Quantity);
                }
                if (Item.Format.Contains("PDF"))
                {
                    total += (Item.Price * 50 / 100);
                }
                if (Item.Format.Contains("Audio"))
                {
                    total += (Item.Price * 60 / 100);
                }
                else
                {
                    total += Item.Price;
                }

            }


            // add new order
            var newOrder = new Order
            {
                UserId = userId,
                TotalAmount = total,
                Status = "Pending",
            };
            _myDbContext.Orders.Add(newOrder);
            _myDbContext.SaveChanges();


            // add order items
            foreach (var Item in order)
            {
                var newItem = new OrderItem
                {
                    OrderId = newOrder.Id,
                    BookId = Item.BookId,
                    Quantity = Item.Quantity,
                    Price = Item.Price,
                    Format = Item.Format,
                };
                _myDbContext.OrderItems.Add(newItem);
            }
            _myDbContext.SaveChanges();


            return Ok(new { id = newOrder.Id });
        }


        [HttpGet]
        public IActionResult DownloadPdf(int id)
        {
            var order = _myDbContext.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // قم بإنشاء مستند PDF
            using (var memoryStream = new MemoryStream())
            {
                var document = new Document();
                PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // إضافة البيانات من DTO إلى الـ PDF
                document.Add(new Paragraph($"Name: {order.User.Name}"));
                document.Add(new Paragraph($"Email: {order.User.Email}"));
                document.Add(new Paragraph($"Phone Number: {order.User.PhoneNumber}"));
                document.Add(new Paragraph("Order Items:"));

                foreach (var item in order.OrderItems)
                {
                    document.Add(new Paragraph($"- Book ID: {item.BookId}, Price: {item.Price}, Quantity: {item.Quantity}"));
                }

                document.Close();

                // إعداد ملف PDF للتحميل
                var fileBytes = memoryStream.ToArray();
                var fileName = "order_details.pdf";

                return File(fileBytes, "application/pdf", fileName);
            }



        }

        
            [HttpGet("Excel/{id}")]

            public IActionResult DownloadExcel(int id)
            {
                var order = _myDbContext.Orders
                    .Include(o => o.OrderItems)
                    .Include(o => o.User)
                    .FirstOrDefault(o => o.Id == id);

                if (order == null)
                {
                    return NotFound();
                }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // إنشاء مستند Excel
            using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Order Details");

                    // إضافة بيانات المستخدم
                    worksheet.Cells[1, 1].Value = "Name";
                    worksheet.Cells[1, 2].Value = order.User.Name;
                    worksheet.Cells[2, 1].Value = "Email";
                    worksheet.Cells[2, 2].Value = order.User.Email;

                    worksheet.Cells[4, 1].Value = "Order Items";
                    worksheet.Cells[5, 1].Value = "Book ID";
                    worksheet.Cells[5, 2].Value = "Price";
                    worksheet.Cells[5, 3].Value = "Quantity";

                    // إضافة عناصر الطلب
                    int row = 6;
                    foreach (var item in order.OrderItems)
                    {
                        worksheet.Cells[row, 1].Value = item.BookId;
                        worksheet.Cells[row, 2].Value = item.Price;
                        worksheet.Cells[row, 3].Value = item.Quantity;
                        row++;
                    }

                    // إعداد ملف Excel للتحميل
                    var fileBytes = package.GetAsByteArray();
                    var fileName = "order_details.xlsx";

                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        [HttpGet("alluser")]
        public IActionResult DownloadPdf()
        {
            // Retrieve all users and their orders
            var users = _myDbContext.Users
                .Include(u => u.Orders)
                .ThenInclude(o => o.OrderItems)
                .ToList();

            if (users == null || !users.Any())
            {
                return NotFound();
            }

            // Create a PDF document
            using (var memoryStream = new MemoryStream())
            {
                var document = new Document();
                PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                foreach (var user in users)
                {
                    // Add user details
                    document.Add(new Paragraph($"User Name: {user.Name}"));
                    document.Add(new Paragraph($"Email: {user.Email}"));
                    document.Add(new Paragraph($"Phone Number: {user.PhoneNumber}"));
                    document.Add(new Paragraph("Order Items:"));

                    foreach (var order in user.Orders)
                    {
                        foreach (var item in order.OrderItems)
                        {
                            document.Add(new Paragraph($"- Order ID: {order.Id}, Book ID: {item.BookId}, Price: {item.Price}, Quantity: {item.Quantity}"));
                        }
                    }

                    // Add a page break after each user
                    document.Add(new Paragraph("\n"));
                }

                document.Close();

                // Prepare PDF file for download
                var fileBytes = memoryStream.ToArray();
                var fileName = "all_orders_details.pdf";

                return File(fileBytes, "application/pdf", fileName);
            }
        }



        [HttpGet("ExcelAll")]
        public IActionResult DownloadExcelall()
        {
            var users = _myDbContext.Users
                .Include(u => u.Orders)
                .ThenInclude(o => o.OrderItems)
                .ToList();

            if (users == null || !users.Any())
            {
                return NotFound();
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Create an Excel package
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Order Details");

                // Set headers
                worksheet.Cells[1, 1].Value = "User Name";
                worksheet.Cells[1, 2].Value = "Email";
                worksheet.Cells[1, 3].Value = "Phone Number";
                worksheet.Cells[1, 4].Value = "Order ID";
                worksheet.Cells[1, 5].Value = "Book ID";
                worksheet.Cells[1, 6].Value = "Price";
                worksheet.Cells[1, 7].Value = "Quantity";

                int row = 2; // Starting row for data

                foreach (var user in users)
                {
                    foreach (var order in user.Orders)
                    {
                        foreach (var item in order.OrderItems)
                        {
                            worksheet.Cells[row, 1].Value = user.Name;
                            worksheet.Cells[row, 2].Value = user.Email;
                            worksheet.Cells[row, 3].Value = user.PhoneNumber;
                            worksheet.Cells[row, 4].Value = order.Id;
                            worksheet.Cells[row, 5].Value = item.BookId;
                            worksheet.Cells[row, 6].Value = item.Price;
                            worksheet.Cells[row, 7].Value = item.Quantity;
                            row++;
                        }
                    }
                }

                // Prepare Excel file for download
                var fileBytes = package.GetAsByteArray();
                var fileName = "all_orders_details.xlsx";

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

    }
}

