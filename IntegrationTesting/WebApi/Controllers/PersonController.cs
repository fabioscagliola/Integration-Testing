using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace com.fabioscagliola.IntegrationTesting.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        public const string NOTFOUND = "A person with the specified identifier could not be found!";

        readonly WebApiDbContext dbContext;

        public PersonController(WebApiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerResponse(200, type: typeof(Person))]
        public async Task<ActionResult> Create(PersonCreateData personCreateData)
        {
            Person person = new() { FName = personCreateData.FName, LName = personCreateData.LName, };
            dbContext.People.Add(person);
            await dbContext.SaveChangesAsync();
            return StatusCode(200, person);
        }

        [HttpGet]
        [Route("[action]/{id}")]
        [SwaggerResponse(200, type: typeof(Person))]
        public async Task<ActionResult> Read(int id)
        {
            Person? person = await dbContext.People.SingleOrDefaultAsync(x => x.Id == id);
            if (person == null)
                return BadRequest(NOTFOUND);
            return StatusCode(200, person);
        }

        [HttpGet]
        [Route("[action]")]
        [SwaggerResponse(200, type: typeof(List<Person>))]
        public async Task<ActionResult> ReadList()
        {
            List<Person> people = await dbContext.People.ToListAsync();
            return StatusCode(200, people);
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerResponse(200, type: typeof(Person))]
        public async Task<ActionResult> Update(Person person)
        {
            Person? existing = await dbContext.People.SingleOrDefaultAsync(x => x.Id == person.Id);
            if (existing == null)
                return BadRequest(NOTFOUND);
            existing.FName = person.FName;
            existing.LName = person.LName;
            await dbContext.SaveChangesAsync();
            return StatusCode(200, existing);
        }

        [HttpGet]
        [Route("[action]/{id}")]
        [SwaggerResponse(200)]
        public async Task<ActionResult> Delete(int id)
        {
            Person? person = await dbContext.People.SingleOrDefaultAsync(x => x.Id == id);
            if (person == null)
                return BadRequest(NOTFOUND);
            dbContext.People.Remove(person);
            await dbContext.SaveChangesAsync();
            return StatusCode(200);
        }
    }
}
