using MaraudersMap.Common.Models;
using MaraudersMap.Data.DbContexts;
using MaraudersMap.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MaraudersMap.MarauderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarauderUserController : ControllerBase
    {

        private readonly MarauderDbContext context;
        private readonly int port = 1111;
        private readonly string address = "127.0.0.1";

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
                    Name = m.Name,
                    Coordinates = m.Coordinates,
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

        // GET api/connect
        [HttpGet("connect")]
        public ActionResult Connect()
        {
            try
            {
                //Process.Start("TcpMarauderServer.exe");
                // post build event => áthelyezni a .exe -t a megfelelő helyre
               
                List<string> walls = new()
                {
                    "-13;1;6,2=13;1;6,2",
                    "-13;1;-6,2=13;1;-6,2",
                    "13;1;6,2=13;1;-6,2",
                    "-13;1;6,2=-13;1;-6,2",
                    "-5,35;1;6,2=-5,35;1;2,48",
                    "-5,35;1;2,48=-10,97;1;2,48",
                    "-0,6;1;2,24=4,6;1;2,24",
                    "4,6;1;2,24=4,6;1;6,2",
                    "8,6;1;2,46=8,6;1;6,2"
                };

                return Ok(new { port, address, walls });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // GET api/connect
        [HttpGet("blueprint")]
        public ActionResult GetBlueprint()
        {
            try
            {
                string json = "{\"walls\":[\"-13;1;6.2=13;1;6.2\",\"-13;1;-6.2=13;1;-6.2\",\"13;1;6.2=13;1;-6.2\",\"-13;1;6.2=-13;1;-6.2\",\"-5.35;1;6.2=-5.35;1;2.48\",\"-5.35;1;2.48=-10.97;1;2.48\",\"-0.6;1;2.24=4.6;1;2.24\",\"4.6;1;2.24=5.3;1;6.2\",\"8.6;1;2.46=8.6;1;6.2\"]}";

                return Ok(json);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
