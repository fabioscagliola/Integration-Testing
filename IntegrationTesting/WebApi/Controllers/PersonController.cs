using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace com.fabioscagliola.IntegrationTesting.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly WebApiDbContext dbContext;

        public PersonController(WebApiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<List<Person>> List()
        {
            return await dbContext.People.ToListAsync();
        }
    }
}
