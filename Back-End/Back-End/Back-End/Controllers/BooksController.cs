using Back_End.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Reflection.Metadata.BlobBuilder;

namespace Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly MyDbContext _myDbContext;

        public BooksController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }
        [HttpGet]
        public IActionResult GetAllBooks()
        {
            var Books = _myDbContext.Books.ToList();
            return Ok(Books);
        }
        [HttpGet("RandomBook")]
        public IActionResult GetAllBooks2()
        {
            int count = 8; 
            var randomBooks = _myDbContext.Books
                                          .OrderBy(b => Guid.NewGuid())
                                          .Take(count)
                                          .ToList();
            return Ok(randomBooks);
        }


        [HttpGet("byIDBooks/{id}")]
        public IActionResult GetBooksById(int id)
        {
            var Books = _myDbContext.Books.FirstOrDefault(a => a.Id == id);
            if (Books == null)
            {
                return NotFound();
            }
            return Ok(Books);
        }
        [HttpGet("topRatedBooks")]
        public IActionResult GetTopRatedBooks()
        {
            var Books = _myDbContext.Books
                .OrderByDescending(a => a.Rating)
                .Take(6)  
                .ToList();

            if (Books == null || Books.Count == 0)
            {
                return NotFound("No books found.");
            }

            return Ok(Books);
        }


        [HttpGet("bySaleBooks")]
        public IActionResult GetBooksBySale()
        {
            // جلب الكتب التي تحتوي على نسبة خصم أكبر من 0 وترتيبها عشوائيًا
            var Books = _myDbContext.Books
                .Where(a => a.DiscountPercentage > 0) // جلب الكتب التي عليها خصم فقط
                .OrderBy(b => Guid.NewGuid()) // ترتيب عشوائي
                .ToList();

            if (Books == null || Books.Count == 0)
            {
                return NotFound(); // إذا لم تكن هناك أي كتب عليها خصم
            }

            // تحديد عدد عشوائي بين 5 و 10
            Random random = new Random();
            int count = random.Next(4, Math.Min(10, Books.Count) + 1); // اختيار العدد بين 5 و 10 أو عدد الكتب إذا كان أقل

            // إرجاع عدد عشوائي من الكتب
            var randomBooks = Books.Take(count).ToList();

            return Ok(randomBooks);
        }




        [HttpGet("bynameBooks/{name}")]

        public IActionResult GetBooksByName(string name)
        {
            var Books = _myDbContext.Books.FirstOrDefault(a => a.Title == name);
            if (Books == null)
            {
                return NotFound();
            }
            return Ok(Books);
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var Books = _myDbContext.Books.Find(id);
            if (Books == null)
            {
                return NotFound();
            }
            _myDbContext.Books.Remove(Books);
            _myDbContext.SaveChanges();
            return Ok();
        }

    }
}
