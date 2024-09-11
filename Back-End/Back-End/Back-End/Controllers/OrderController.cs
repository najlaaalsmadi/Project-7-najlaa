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





        }
    }

