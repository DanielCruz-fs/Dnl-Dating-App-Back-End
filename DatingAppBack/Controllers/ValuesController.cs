using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAppBack.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingAppBack.Controllers
{
    //main route from iis express http://localhost:44340/api/values
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        /*Inject DataContext*/
        private readonly DataContext context;

        public ValuesController(DataContext context)
        {
            this.context = context;
        }
        /**/

        // GET api/values
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var values = await this.context.Values.ToListAsync();
            return Ok(values);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue(int id)
        {
            var value = await this.context.Values.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(value);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
