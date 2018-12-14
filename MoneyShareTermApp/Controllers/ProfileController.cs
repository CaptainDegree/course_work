using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoneyShareTermApp.Models;

namespace MoneyShareTermApp.Controllers
{
    public class ProfileController : Controller
    {
        private readonly postgresContext _context;

        public ProfileController(postgresContext context)
        {
            _context = context;
        }
        
        // GET: Profile
        public async Task<IActionResult> Index()
        {
            var postgresContext = _context.Person.Include(p => p.Account).Include(p => p.CommentPrice).Include(p => p.Mailer).Include(p => p.MessagePrice).Include(p => p.Photo).Include(p => p.SubscriptionPrice);
            return View(await postgresContext.ToListAsync());
        }

        // GET: Profile/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Person
                .Include(p => p.Account)
                .Include(p => p.CommentPrice)
                .Include(p => p.Mailer)
                .Include(p => p.MessagePrice)
                .Include(p => p.Photo)
                .Include(p => p.SubscriptionPrice)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        //// GET: Profile/Create
        //public IActionResult Create()
        //{
        //    ViewData["AccountId"] = new SelectList(_context.CurrencySet, "Id", "Id");
        //    ViewData["CommentPriceId"] = new SelectList(_context.CurrencySet, "Id", "Id");
        //    ViewData["MailerId"] = new SelectList(_context.MoneyMailer, "Id", "Id");
        //    ViewData["MessagePriceId"] = new SelectList(_context.CurrencySet, "Id", "Id");
        //    ViewData["PhotoId"] = new SelectList(_context.File, "Id", "Link");
        //    ViewData["SubscriptionPriceId"] = new SelectList(_context.CurrencySet, "Id", "Id");
        //    return View();
        //}

        // POST: Profile/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,AccountId,MessagePriceId,CommentPriceId,SubscriptionPriceId,PhotoId,MailerId,Birthday,FirstName,MiddleName,SecondName,RegistrationTime,Password,Login,PhoneNumber,Email,Hidden")] Person person)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(person);
        //        await _context.SaveChangesAsync();
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
    }
}
