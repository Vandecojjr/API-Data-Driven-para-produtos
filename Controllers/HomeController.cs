using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1")]
    public class HomeController : ControllerBase
    {
        [Route("")]
        [HttpGet]
        public async Task<ActionResult<dynamic>> Get([FromServices] DataContext context)
        {
            var employer = new User { Id = 1, Username = "Robin", Password = "robin123", Role = "employer" };
            var manager = new User { Id = 2, Username = "Batman", Password = "batman123", Role = "manager" };
            var category = new Category { Id = 1, Title = "Roupas" };
            var product = new Product { Id = 1, Title = "Camiseta", Description = "Tanto faz", Price = 50 };

            context.Users.Add(employer);
            context.Users.Add(manager);
            context.Categories.Add(category);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            return Ok(new { message = "dados configurados" });
        }

    }
}