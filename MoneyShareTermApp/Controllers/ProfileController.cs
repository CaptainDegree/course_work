using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyShareTermApp.Models;
using MoneyShareTermApp.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MoneyShareTermApp.Controllers
{
    public class ProfileController : Controller
    {
        private readonly postgresContext _context = new postgresContext();

        // GET: Profile
        public async Task<IActionResult> Index()
        {
            var postgresContext = _context.Person.Include(p => p.Account).Include(p => p.CommentPrice).Include(p => p.Mailer).Include(p => p.MessagePrice).Include(p => p.Photo).Include(p => p.SubscriptionPrice);
            return View(await postgresContext.ToListAsync());
        }

        // GET: Profile/Details/5
        [Authorize] // TODO далее распределить по ролям: [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var person = await _context.Person
                .Include(p => p.Account)
                .Include(p => p.Photo)
                .Include(p => p.Post)
                .Include("Post.Mailer")
                .Include("Post.Mailer.MoneyTransferTarget")
                .Include("Post.File")
                .Include(p => p.SubscriptionPerson)
                .Include(p => p.SubscriptionSubscriber)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (person == null)
                return NotFound();

            ViewData["PersonId"] = person.Id;

            return View(person);
        }

        // GET: Profile/Register
        public IActionResult Register()
        {
            return View(); // добавить поддержку фото 
        }

        // GET: Profile/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Person user = await _context.Person.Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

                if (user != null)
                {
                    await Authenticate(user); // аутентификация

                    return RedirectToAction();
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        // POST: Profile/Register
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([ModelBinder(BinderType = typeof(PersonModelBinder))] Person person)
        {
            if (ModelState.IsValid)
            {
                Person user = await _context.Person.FirstOrDefaultAsync(u => u.Email == person.Email);

                if (user == null)
                {
                    Role userRole = await _context.Role.FirstOrDefaultAsync(r => r.Name == "user");
                    if (userRole != null)
                        person.Role = userRole; // для аутентификации нужно добавить роль

                    _context.Add(person);
                    await _context.SaveChangesAsync();

                    await Authenticate(person); // аутентификация

                    return RedirectToAction(nameof(Index));
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }

            return View(person);
        }

        private async Task Authenticate(Person user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Profile");
        }

        // GET: Profile/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var person = await _context.Person.FindAsync(id);
        //    if (person == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["AccountId"] = new SelectList(_context.CurrencySet, "Id", "Id", person.AccountId);
        //    ViewData["CommentPriceId"] = new SelectList(_context.CurrencySet, "Id", "Id", person.CommentPriceId);
        //    ViewData["MailerId"] = new SelectList(_context.MoneyMailer, "Id", "Id", person.MailerId);
        //    ViewData["MessagePriceId"] = new SelectList(_context.CurrencySet, "Id", "Id", person.MessagePriceId);
        //    ViewData["PhotoId"] = new SelectList(_context.File, "Id", "Link", person.PhotoId);
        //    ViewData["SubscriptionPriceId"] = new SelectList(_context.CurrencySet, "Id", "Id", person.SubscriptionPriceId);
        //    return View(person);
        //}

        // POST: Profile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,AccountId,MessagePriceId,CommentPriceId,SubscriptionPriceId,PhotoId,MailerId,Birthday,FirstName,MiddleName,SecondName,RegistrationTime,Password,Login,PhoneNumber,Email,Hidden")] Person person)
        //{
        //    if (id != person.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(person);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!PersonExists(person.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["AccountId"] = new SelectList(_context.CurrencySet, "Id", "Id", person.AccountId);
        //    ViewData["CommentPriceId"] = new SelectList(_context.CurrencySet, "Id", "Id", person.CommentPriceId);
        //    ViewData["MailerId"] = new SelectList(_context.MoneyMailer, "Id", "Id", person.MailerId);
        //    ViewData["MessagePriceId"] = new SelectList(_context.CurrencySet, "Id", "Id", person.MessagePriceId);
        //    ViewData["PhotoId"] = new SelectList(_context.File, "Id", "Link", person.PhotoId);
        //    ViewData["SubscriptionPriceId"] = new SelectList(_context.CurrencySet, "Id", "Id", person.SubscriptionPriceId);
        //    return View(person);
        //}

        // GET: Profile/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var person = await _context.Person
        //        .Include(p => p.Account)
        //        .Include(p => p.CommentPrice)
        //        .Include(p => p.Mailer)
        //        .Include(p => p.MessagePrice)
        //        .Include(p => p.Photo)
        //        .Include(p => p.SubscriptionPrice)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (person == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(person);
        //}

        // POST: Profile/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var person = await _context.Person.FindAsync(id);
        //    _context.Person.Remove(person);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool PersonExists(int id)
        //{
        //    return _context.Person.Any(e => e.Id == id);
        //}

        //  GET: Profile/Posts/5
        [Authorize]
        public async Task<IActionResult> Posts(int? id)
        {
            if (id == null)
                return NotFound();

            return View(await _context.Post
                .Where(p => p.PersonId == id ||
                p.Person.SubscriptionSubscriber.Any(s => s.Id == id))
                .Include(p => p.Mailer)
                    .ThenInclude(p => p.MoneyTransferTarget)
                .Include(p => p.File)
                .Include(p => p.Person)
                    .ThenInclude(p => p.Photo)
                .OrderBy(p => p.Mailer.CreationTime)
                .ToListAsync());
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
