using MaraudersMap.Common.Models;
using MaraudersMap.Data.DbContexts;
using MaraudersMap.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MaraudersMap.MarauderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarauderUserController : ControllerBase
    {

        private readonly MarauderDbContext context;

        public MarauderUserController(MarauderDbContext context)
        {
            this.context = context;
        }

        // GET: api/<MarauderUserController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarauderDto>>> Get()
        {
            try
            {
                var marauderUsers = await context.MarauderUsers.ToListAsync();
                return Ok(marauderUsers);
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // GET api/<MarauderUserController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MarauderDto>> Get(int id)
        {
            try
            {
                var marauderUser = await context.MarauderUsers.FirstOrDefaultAsync(m => m.Id == id);
                return Ok(marauderUser);
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // POST api/<MarauderUserController>
        [HttpPost]
        public async Task<ActionResult<MarauderDto>> Post([FromBody] MarauderUser m)
        {
            try
            {
                if (context.MarauderUsers == null)
                {
                    return Problem("Course list is null.");
                }

                var userToAdd = new MarauderUser
                {
                    IsActivated = true,
                    Name = m.Name,
                    Coordinates = m.Coordinates,
                    LastUpdate = m.LastUpdate,
                };
                
                context.MarauderUsers.Add(userToAdd);
                await context.SaveChangesAsync();

                return CreatedAtAction(nameof(Get), new { id = userToAdd.Id }, userToAdd);

            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }

        // PUT api/<MarauderUserController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<MarauderDto>> Put(int id, [FromBody] MarauderUser m)
        {
            try
            {
                if (context.MarauderUsers == null)
                {
                    return Problem("Course list is null.");
                }

                var userToModify = await context.MarauderUsers.FindAsync(id);
                userToModify!.Coordinates = m.Coordinates;
                await context.SaveChangesAsync();
                return Ok(userToModify);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // DELETE api/<MarauderUserController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (context.MarauderUsers == null)
            {
                return NotFound();
            }
            var marauderUser = await context.MarauderUsers.FindAsync(id);
            if (marauderUser == null)
            {
                return NotFound();
            }

           context.MarauderUsers.Remove(marauderUser);
           await context.SaveChangesAsync();

           return NoContent();

        }
    }
}
