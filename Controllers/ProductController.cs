
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1/products")]
    public class ProductController : ControllerBase
    {

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Product>>> Get(
            [FromServices] DataContext context
        )
        {
            var products = await context
                .Products
                .Include(x => x.Category)
                .AsNoTracking()
                .ToListAsync();
            return products;
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById(
            int id,
            [FromServices] DataContext context
        )
        {
            var product = await context
                .Products
                .Include(x => x.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            return product;
        }

        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get(
            int id,
            [FromServices] DataContext context
        )
        {
            var products = await context
                .Products
                .Include(x => x.Category)
                .AsNoTracking()
                .Where(x => x.CategoryId == id)
                .ToListAsync();
            return products;
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employer")]
        public async Task<ActionResult<Product>> Post(
           [FromBody] Product model,
           [FromServices] DataContext context
       )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel criar o produto" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Product>> Put(
            int id,
            [FromBody] Product model,
           [FromServices] DataContext context
)
        {
            if (id != model.Id)
                return NotFound(new { message = "Este produto não foi encontrado!" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Product>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Não foi possivel atualizar o produto" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel atualizar o produto" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Product>> Delete(
            int id,
            [FromServices] DataContext context
        )
        {
            var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
                return NotFound(new { message = "Produto não encontrado" });
            try
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return Ok(new { message = "Produto removido com sucesso" });
            }
            catch (Exception)
            {
                return Ok(new { message = "Não foi possivel deletar o produto" });
            }
        }
    }
}