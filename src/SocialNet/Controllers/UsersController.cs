using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using SocialNet.Models;
using SocialNet.ViewModels.Users;
using System.Security.Claims;
using Microsoft.AspNet.Identity;

namespace SocialNet.Controllers
{
    [Produces("application/json")]
    [Route("api/Users")]
    public class UsersController : Controller
    {
        private ApplicationDbContext context;
        private UserManager<ApplicationUser> userManager;

        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        // GET: api/Users
        [HttpGet]
        public IEnumerable<UserViewModel> GetApplicationUsers()
        {
            return context.ApplicationUsers.Select(u => new UserViewModel(u.UserName, u.Description));
        }

        // GET: api/Users/5
        [HttpGet("{id}", Name = "GetApplicationUser")]
        public async Task<IActionResult> GetApplicationUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            UserViewModel applicationUser = await context.ApplicationUsers.Where(m => m.Id == id)
                                                .Select(u => new UserViewModel(u.UserName, u.Description))
                                                .SingleOrDefaultAsync();

            if (applicationUser == null)
            {
                return this.HttpNotFound();
            }

            return Ok(applicationUser);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplicationUser([FromRoute] string id, [FromBody] UserViewModel applicationUser)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (HttpContext.User.GetUserId() != id)
            {
                return this.HttpUnauthorized();
            }

            var user = await this.userManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            context.Entry(user).State = EntityState.Modified;

            await context.SaveChangesAsync();

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }
    }
}