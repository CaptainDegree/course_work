using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyShareTermApp.Models;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MoneyShareTermApp.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly PostgresContext _context;
        private IHostingEnvironment _appEnvironment;

        public PostsController(PostgresContext context, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public int UserId
        {
            get => int.Parse(User.Claims.FirstOrDefault(cl => cl.Type == ClaimTypes.PrimarySid).Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Text")] Post post, IFormFile uploadedFile)
        {
            if (ModelState.IsValid && uploadedFile != null)
            {
                // добавляю файл
                string path = "/images/" + uploadedFile.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                post.File.Add(new Models.File
                {
                    Link = uploadedFile.FileName
                });
                post.Person = _context.Person
                    .Include(p => p.SubscriptionPrice)
                    .Include(p => p.Mailer)
                    .FirstOrDefault(p => p.Id == UserId);                

                _context.Add(post);

                // оплата подписчиков // TODO проверить, ловить ошибку
                foreach (var sub in _context.Subscription.Include(s => s.Person).ThenInclude(p => p.SubscriptionPrice).Include(s => s.Subscriber).ThenInclude(s => s.Mailer).Where(s => s.PersonId == UserId))
                {
                    MoneyTransfer.TransMoney(sub.Subscriber, sub.Person, sub.Person.SubscriptionPrice, TransferType.Subscription, _context);

                    var m = new Message
                    {
                        PersonId = sub.Subscriber.Id,
                        TargetId = sub.Person.Id,
                        Mailer = new MoneyMailer(),
                        Text = Message.SubTransfer + sub.Person.SubscriptionPrice.GetAll()
                    };

                    _context.Add(m); 
                    _context.SaveChanges();
                }
                    

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Profile");
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id) // TODO сделать 
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Mailer)
                .Include(p => p.Person)
                .Include(p => p.PostNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        public IActionResult Like(CurrencySet like, int postId)
        {
            int id = UserId;

            if (like.CheckPositive())
            {
                // отправка денег
                Person sender = _context.Person
                    .Include(p => p.Mailer)
                    .FirstOrDefault(p => p.Id == UserId);
                Post postTarget = _context.Post
                    .Include(p => p.Mailer)
                    .Include(p => p.Person)
                    .FirstOrDefault(p => p.Id == postId);

                id = postTarget.PersonId;

                MoneyTransfer.TransMoney(sender, postTarget.Mailer, like, TransferType.Like, _context);

                // отправка сообщения 
                var m = new Message
                {
                    PersonId = sender.Id,
                    TargetId = postTarget.Person.Id,
                    Mailer = new MoneyMailer(),
                    Text = Message.LikeTransfer + like.GetAll() + " Id поста: " + postId
                };

                _context.Add(m);
                _context.SaveChanges();
            }

            return RedirectToAction("Details", "Profile", new { id });
        }
    }
}
