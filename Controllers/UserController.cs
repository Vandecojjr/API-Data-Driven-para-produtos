
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;

namespace Shop.Controllers
{
    [Route("v1/users")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get(
            [FromServices] DataContext context)
        {
            var users = await context
            .Users
            .AsNoTracking()
            .ToListAsync();
            return users;
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        //[Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Post(
                    [FromBody] User model,
                    [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                model.Role = "employer";
                context.Users.Add(model);
                await context.SaveChangesAsync();

                model.Password = "";
                return model;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel criar o usuario" });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate(
            [FromBody] User model,
            [FromServices] DataContext context)
        {
            var user = await context
            .Users
            .AsNoTracking()
            .Where(x => x.Username == model.Username && x.Password == model.Password)
            .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválidos" });
            var token = TokenService.GenerateToken(user);
            return new
            {
                token = token
            };
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put(
                    int id,
                    [FromBody] User model,
                    [FromServices] DataContext context)
        {
            if (id != model.Id)
                return NotFound(new { message = "Este usuário não foi encontrado!" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<User>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel atualizar o usuario" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Delete(
            int id,
            [FromServices] DataContext context)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return BadRequest(new { message = "O usuário não foi encontrado" });
            try
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return Ok(new { message = "Usuário removido com sucesso" });
            }
            catch (Exception)
            {
                return Ok(new { message = "Não foi possivel deletar o usuário" });
            }
        }

    }
}