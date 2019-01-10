﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyShareTermApp.Models;
using MoneyShareTermApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace MoneyShareTermApp.Controllers //TODO добавлять посты
{
    public class ProfileController : Controller
    {
        private readonly PostgresContext _context;
        private IHostingEnvironment _appEnvironment;

        public ProfileController(PostgresContext context, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public int UserId
        {
            get => int.Parse(User.Claims.FirstOrDefault(cl => cl.Type == ClaimTypes.PrimarySid).Value);
        }

        // GET: Profile
        [Authorize (Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            var postgresContext = _context.Person.Include(p => p.Account).Include(p => p.CommentPrice).Include(p => p.Mailer).Include(p => p.MessagePrice).Include(p => p.Photo).Include(p => p.SubscriptionPrice);
            return View(await postgresContext.ToListAsync());
        }

        // GET: Profile/Details/5
        // TODO далее распределить по ролям: [Authorize(Roles = "admin, user")], частично доступно всем (User.FindFirst(ClaimTypes.NameIdentifier).Value)
        [Authorize]
        public async Task<IActionResult> Details(int? id)  // TODO можно ли войти на чужую страницу ?
        {
            if (id == null)
                id = UserId;

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

                    return RedirectToAction("Details", "Profile", user.Id);
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
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name),
                new Claim(ClaimTypes.PrimarySid, user.Id.ToString())
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
                .OrderByDescending(p => p.Mailer.CreationTime)
                .ToListAsync());
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Chats() // TODO отправка денег - тоже сообщение ?, добавить еще в сущность
        {
            List<Message> msgs = new List<Message>();
            int id = UserId;

            await _context.Message
                .Include(m => m.Mailer)
                    .ThenInclude(m => m.MoneyTransferTarget)
                .Include(m => m.Target)
                    .ThenInclude(t => t.Photo)
                .Where(m => m.TargetId == UserId || m.PersonId == UserId) // только мои или те, что мне 
                .GroupBy(m => m.Target)
                .ForEachAsync(tm => msgs.Add(tm.Select(m => m).Max()));

            foreach (var m in msgs)
                foreach (var mtt in m.Mailer.MoneyTransferTarget)
                    mtt.Account = _context.CurrencySet.FirstOrDefault(cs => cs.Id == mtt.AccountId);

            return View(msgs);
        }

        [Authorize]
        public async Task<IActionResult> Chat(int id) // TODO проверять id
        {
            var msgs = _context.Message
                .Include(m => m.Mailer)
                    .ThenInclude(m => m.MoneyTransferTarget)
                .Include(m => m.Person)
                .Include("Person.Photo")
                .Include("Person.Account")
                .Include("Person.Mailer")
                .Where(m => m.PersonId.Equals(UserId) && m.TargetId.Equals(id))
                .OrderBy(m => m.Mailer.CreationTime);

            foreach (var m in msgs)
                foreach (var mtt in m.Mailer.MoneyTransferTarget)
                    mtt.Account = _context.CurrencySet.FirstOrDefault(cs => cs.Id == mtt.AccountId);

            ViewData["TargetId"] = id;

            return View(await msgs.ToListAsync());
        }

        [Authorize]
        public async Task<IActionResult> People(string searchString)
        {
            var people = _context.Person
                        .Include(p => p.Photo)
                        .Include(p => p.Account)
                        .Select(p => p);

            if (!String.IsNullOrEmpty(searchString))
                people = people.Where(s => s.Name().Contains(searchString));

            return View(await people.ToListAsync());
        }

        [Authorize]
        public IActionResult Settings(int id)
        {
            if (UserId != id)
                return NotFound();

            var person = _context.Person
                .Include(p => p.Account)
                .Include(p => p.CommentPrice)
                .Include(p => p.Mailer)
                .Include(p => p.MessagePrice)
                .Include(p => p.Photo)
                .Include(p => p.SubscriptionPrice)
                .FirstOrDefault(p => p.Id == id);

            return View(person);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([ModelBinder(BinderType = typeof(PersonSettingsBinder))] Person updPerson, IFormFile uploadedFile)
        {
            if (UserId != updPerson.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(updPerson);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details");
            }
            return RedirectToAction("Setting", UserId);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                // добавляю файл
                string path = "/images/" + uploadedFile.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                var user = _context.Person.FirstOrDefault(p => p.Id == UserId);
                user.Photo = new Models.File
                {
                    Link = uploadedFile.FileName
                };

                await _context.SaveChangesAsync();
                return RedirectToAction("Details");
            }

            return RedirectToAction("Setting", UserId);
        }
    }
}
