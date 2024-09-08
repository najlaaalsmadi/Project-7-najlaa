using Back_End.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _myDbContext;

        public UserController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }
        // Get all categories
        [HttpGet]
        public IActionResult GetAllUser()
        {
            var User = _myDbContext.Users.ToList();
            return Ok(User);
        }

        // Get category by ID
        [HttpGet("byIDUser/{id}")]
        public IActionResult GetUserById(int id)
        {
            var User = _myDbContext.Users.FirstOrDefault(a => a.Id == id);
            if (User == null)
            {
                return NotFound();
            }
            return Ok(User);
        }

        // Get category by name
        [HttpGet("bynameUser/{name}")]

        public IActionResult GetUserByName(string name)
        {
            var User = _myDbContext.Users.FirstOrDefault(a => a.Name == name);
            if (User == null)
            {
                return NotFound();
            }
            return Ok(User);
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var User = _myDbContext.Users.Find(id);
            if (User == null)
            {
                return NotFound();
            }
            _myDbContext.Users.Remove(User);
            _myDbContext.SaveChanges();
            return Ok();
        }

    }
}
