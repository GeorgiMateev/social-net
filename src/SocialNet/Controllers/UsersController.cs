using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using SocialNet.Models;

namespace SocialNet.Controllers
{
    [Produces("application/json")]
    [Route("api/Users")]
    public class UsersController : Controller
    {
        private ApplicationDbContext context;

        public UsersController(ApplicationDbContext context)
        {
            this.context = context;
        }

        // GET: api/Users
        [HttpGet]
        public IEnumerable<ApplicationUser> GetApplicationUser()
        {
            return context.ApplicationUsers;
        }

        // GET: api/Users/5
        [HttpGet("{id}", Name = "GetApplicationUser")]
        public async Task<IActionResult> GetApplicationUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            ApplicationUser applicationUser = await context.ApplicationUsers.SingleAsync(m => m.Id == id);

            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            return Ok(applicationUser);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplicationUser([FromRoute] string id, [FromBody] ApplicationUser applicationUser)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != applicationUser.Id)
            {
                return HttpBadRequest();
            }

            context.Entry(applicationUser).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationUserExists(id))
                {
                    return HttpNotFound();
                }
                else
                {
                    throw;
                }
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> PostApplicationUser([FromBody] ApplicationUser applicationUser)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            context.ApplicationUsers.Add(applicationUser);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ApplicationUserExists(applicationUser.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("GetApplicationUser", new { id = applicationUser.Id }, applicationUser);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplicationUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            ApplicationUser applicationUser = await context.ApplicationUsers.SingleAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            context.ApplicationUsers.Remove(applicationUser);
            await context.SaveChangesAsync();

            return Ok(applicationUser);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ApplicationUserExists(string id)
        {
            return context.ApplicationUsers.Count(e => e.Id == id) > 0;
        }
    }
}