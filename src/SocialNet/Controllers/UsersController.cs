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
using Microsoft.AspNet.Authorization;

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
            return context.ApplicationUsers.Select(u => new UserViewModel(u.Id, u.UserName, u.Description));
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
                                                .Select(u => new UserViewModel(u.Id, u.UserName, u.Description))
                                                .SingleOrDefaultAsync();

            if (applicationUser == null)
            {
                return this.HttpNotFound();
            }

            return Ok(applicationUser);
        }

        // PUT: api/Users/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplicationUser([FromRoute] string id, [FromBody] UserViewModel userViewModel)
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

        // GET: api/users/5/subscriptions
        [Authorize]
        [HttpGet("{id}/subscriptions")]
        public async Task<IActionResult> Subscriptions([FromRoute] string id)
        {
            var subscriber = await this.context.ApplicationUsers
                .Include(u => u.Publishers)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (subscriber == null)
            {
                return this.HttpBadRequest("Subscriber is not found");
            }

            return this.Ok(subscriber.Publishers.Select(s =>
            {
                return new UserViewModel(this.context.ApplicationUsers.FirstOrDefault(u => u.Id == s.PublisherId));
            }));
        }

        // POST: api/Users/subscribe
        [Authorize]
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscriptionViewModel subscriptionVM)
        {
            var subscriber = await this.context.ApplicationUsers
                .Include(u => u.Publishers)
                .FirstOrDefaultAsync(u => u.Id == subscriptionVM.SubscriberId);

            if (subscriber == null)
            {
                return this.HttpBadRequest("Subscriber is not found");
            }

            var publisher = await this.context.ApplicationUsers
                .Include(u => u.Subscribers)
                .FirstOrDefaultAsync(u => u.Id == subscriptionVM.PublisherId);

            if (publisher == null)
            {
                return this.HttpBadRequest("Publisher is not found");
            }

            if(subscriber.Publishers.FirstOrDefault(p => p.PublisherId == publisher.Id) != null)
            {
                return this.HttpBadRequest("Already subscribed");
            }

            var subscription = new Subscription();
            subscription.Publisher = publisher;
            subscription.PublisherId = publisher.Id;
            subscription.Subscriber = subscriber;
            subscription.SubscriberId = subscriber.Id;

            subscriber.Publishers.Add(subscription);
            publisher.Subscribers.Add(subscription);
            this.context.Add(subscription);
            await this.context.SaveChangesAsync();

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // POST: api/Users/unsubscribe
        [Authorize]
        [HttpPost("unsubscribe")]
        public async Task<IActionResult> Unsubscribe([FromBody] SubscriptionViewModel subscription)
        {
            var subscriber = await this.context.ApplicationUsers
                .Include(u => u.Publishers)
                .FirstOrDefaultAsync(u => u.Id == subscription.SubscriberId);

            if (subscriber == null)
            {
                return this.HttpBadRequest("Subscriber is not found");
            }

            var publisher = await this.context.ApplicationUsers
                .Include(u => u.Subscribers)
                .FirstOrDefaultAsync(u => u.Id == subscription.PublisherId);

            if (publisher == null)
            {
                return this.HttpBadRequest("Publisher is not found");
            }

            if (subscriber.Publishers.FirstOrDefault(p => p.PublisherId == publisher.Id) == null)
            {
                return this.HttpBadRequest("Not subscribed");
            }

            subscriber.Publishers.RemoveAll(p => p.PublisherId == publisher.Id);
            subscriber.Subscribers.RemoveAll(s => s.SubscriberId == subscriber.Id);

            await this.context.SaveChangesAsync();

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }
    }
}