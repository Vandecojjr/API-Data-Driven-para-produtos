
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Category>>> Get(
            [FromServices] DataContext context
        )
        {
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            return categories;
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(
            int id,
            [FromServices] DataContext context
        )
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return category;
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employer")]
        public async Task<ActionResult<Category>> Post(
            [FromBody] Category model,
            [FromServices] DataContext context
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Categories.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel criar a categoria" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "employer")]
        public async Task<ActionResult<Category>> Put(
            int id,
            [FromBody] Category model,
            [FromServices] DataContext context
        )
        {
            if (id != model.Id)
                return NotFound(new { message = "Esta categoria não foi encontrada!" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Não foi possivel atualizar a categoria" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel atualizar a categoria" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Category>> Delete(
            int id,
            [FromServices] DataContext context
        )
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
                return NotFound(new { message = "Categoria não encontrada" });
            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(new { message = "Categoria removida com sucesso" });
            }
            catch (Exception)
            {
                return Ok(new { message = "Não foi possivel deletar a categoria" });
            }
        }
    }
}