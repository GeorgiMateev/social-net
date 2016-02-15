using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using SocialNet.Models;
using SocialNet.ViewModels.Message;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using System.Security.Claims;

namespace SocialNet.Controllers
{
    [Produces("application/json")]
    [Route("api/Messages")]
    public class MessagesController : Controller
    {
        private ApplicationDbContext context;

        public MessagesController(ApplicationDbContext context)
        {
            this.context = context;
        }

        // GET: api/Messages/user/5
        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetMessage([FromRoute] string id)
        {
            var subscriber = await this.context.ApplicationUsers
                .Include(u => u.Publishers)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (subscriber == null)
            {
                return this.HttpNotFound();
            }

            if (HttpContext.User.GetUserId() != subscriber.Id)
            {
                return this.HttpUnauthorized();
            }

            var publishersIds = subscriber.Publishers.Select(p => p.PublisherId);

            return this.Ok(await context.Messages
                                .Include(m => m.Author)
                                .Where(m => publishersIds.Contains(m.Author.Id))
                                .ToListAsync());
        }

        // PUT: api/Messages/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage([FromRoute] string id, [FromBody] MessageViewModel messageViewModel)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Message message = await this.context.Messages.Include(m => m.Author).SingleOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return this.HttpNotFound();
            }

            if (HttpContext.User.GetUserId() != message.Author.Id)
            {
                return this.HttpUnauthorized();
            }

            message.Text = messageViewModel.Text;

            await context.SaveChangesAsync();

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // POST: api/Messages
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] MessageViewModel messageVM)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var author = await this.context.ApplicationUsers.SingleOrDefaultAsync(a => a.Id == messageVM.AuthorId);
            if (author == null)
            {
                return this.HttpBadRequest();
            }

            if (HttpContext.User.GetUserId() != author.Id)
            {
                return this.HttpUnauthorized();
            }

            var message = new Message();
            message.Text = messageVM.Text;
            message.Author = author;

            context.Messages.Add(message);
           
            await context.SaveChangesAsync();

            return this.Ok();
        }

        // DELETE: api/Messages/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Message message = await context.Messages.Include(m => m.Author).SingleAsync(m => m.Id == id);
            if (message == null)
            {
                return HttpNotFound();
            }

            if (HttpContext.User.GetUserId() != message.Author.Id)
            {
                return this.HttpUnauthorized();
            }

            context.Messages.Remove(message);
            await context.SaveChangesAsync();

            return Ok(message);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}