using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace com.fabioscagliola.IntegrationTesting.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        public const string FNAMEORLNAMEARENULLOREMPTY = "You must indicate the first name and the last name!";
        public const string NOTFOUND = "A person with the specified identifier could not be found!";

        readonly WebApiDbContext dbContext;

        public PersonController(WebApiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Creates a person.
        /// </summary>
        [HttpPost]
        [Route("[action]")]
        [SwaggerResponse(200, Type = typeof(Person))]
        [SwaggerResponse(400)]
        public async Task<IActionResult> Create(PersonCreateData personCreateData)
        {
            if (string.IsNullOrEmpty(personCreateData.FName) || string.IsNullOrEmpty(personCreateData.LName))
                return BadRequest(FNAMEORLNAMEARENULLOREMPTY);
            Person person = new() { FName = personCreateData.FName, LName = personCreateData.LName, };
            dbContext.People.Add(person);
            await dbContext.SaveChangesAsync();
            return Ok(person);
        }

        /// <summary>
        /// Retrieves a person.
        /// </summary>
        [HttpGet]
        [Route("[action]/{id}")]
        [SwaggerResponse(200, Type = typeof(Person))]
        [SwaggerResponse(400)]
        public async Task<IActionResult> Read(int id)
        {
            Person? person = await dbContext.People.SingleOrDefaultAsync(x => x.Id == id);
            if (person == null)
                return BadRequest(NOTFOUND);
            return Ok(person);
        }

        /// <summary>
        /// Retrieves all the people.
        /// </summary>
        [HttpGet]
        [Route("[action]")]
        [SwaggerResponse(200, Type = typeof(List<Person>))]
        public async Task<IActionResult> ReadList()
        {
            List<Person> people = await dbContext.People.ToListAsync();
            return Ok(people);
        }

        /// <summary>
        /// Updates a person.
        /// </summary>
        [HttpPost]
        [Route("[action]")]
        [SwaggerResponse(200, Type = typeof(Person))]
        [SwaggerResponse(400)]
        public async Task<IActionResult> Update(Person person)
        {
            Person? existing = await dbContext.People.SingleOrDefaultAsync(x => x.Id == person.Id);
            if (existing == null)
                return BadRequest(NOTFOUND);
            if (string.IsNullOrEmpty(person.FName) || string.IsNullOrEmpty(person.LName))
                return BadRequest(FNAMEORLNAMEARENULLOREMPTY);
            existing.FName = person.FName;
            existing.LName = person.LName;
            await dbContext.SaveChangesAsync();
            return Ok(existing);
        }

        /// <summary>
        /// Deletes a person.
        /// </summary>
        [HttpGet]
        [Route("[action]/{id}")]
        [SwaggerResponse(200, "Deletes a person.")]
        [SwaggerResponse(400)]
        public async Task<IActionResult> Delete(int id)
        {
            Person? person = await dbContext.People.SingleOrDefaultAsync(x => x.Id == id);
            if (person == null)
                return BadRequest(NOTFOUND);
            dbContext.People.Remove(person);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
