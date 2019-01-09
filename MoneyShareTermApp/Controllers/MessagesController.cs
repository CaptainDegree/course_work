using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoneyShareTermApp.Models;

namespace MoneyShareTermApp.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly PostgresContext _context;

        public MessagesController(PostgresContext context)
        {
            _context = context;
        }

        public int UserId
        {
            get => int.Parse(User.Claims.FirstOrDefault(cl => cl.Type == ClaimTypes.PrimarySid).Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Write(int targetId, string msg, Currency cur) //TODO оплата за сообщение
        {
            decimal acc;
            decimal payment;
            CurrencySet userAcc = _context.Person
                .Include(p => p.Account)
                .FirstOrDefault(p => p.Id == UserId)
                .Account;

            Person target = _context.Person
                .Include(p => p.MessagePrice)
                .Include(p => p.Account)
                .FirstOrDefault(p => p.Id == targetId);

            switch (cur)
            {
                case Currency.Dollar:
                    acc = userAcc.Dollar;
                    payment = target.MessagePrice.Dollar;
                    break;
                case Currency.Euro:
                    acc = userAcc.Euro;
                    payment = target.MessagePrice.Euro;
                    break;
                case Currency.Ruble:
                    acc = userAcc.Ruble;
                    payment = target.MessagePrice.Ruble;
                    break;
                default:
                    throw new ArgumentException();
            }

            if (acc < payment)
                return new JsonResult(false); 

            switch (cur)
            {
                case Currency.Dollar:
                    userAcc.Dollar -= payment;
                    target.Account.Dollar += payment;
                    break;
                case Currency.Euro:
                    userAcc.Euro -= payment;
                    target.Account.Euro += payment;
                    break;
                case Currency.Ruble:
                    userAcc.Ruble -= payment;
                    target.Account.Ruble += payment;
                    break;
            }

            // обновление значения счета в бд
            _context.CurrencySet.Attach(userAcc); 
            _context.CurrencySet.Attach(target.Account); 

            var entryUser = _context.Entry(userAcc);
            var entryTarget = _context.Entry(target.Account);

            switch (cur)
            {
                case Currency.Dollar:
                    entryUser.Property(e => e.Dollar).IsModified = true;
                    entryTarget.Property(e => e.Dollar).IsModified = true;
                    break;
                case Currency.Euro:
                    entryUser.Property(e => e.Euro).IsModified = true;
                    entryTarget.Property(e => e.Euro).IsModified = true;
                    break;
                case Currency.Ruble:
                    entryUser.Property(e => e.Ruble).IsModified = true;
                    entryTarget.Property(e => e.Ruble).IsModified = true;
                    break;
            }

            var m = new Message {
                PersonId = UserId,
                TargetId = targetId,
                Mailer = new MoneyMailer(),
                Text = msg
            };

            _context.Add(m);
            await _context.SaveChangesAsync();

            return PartialView("_MsgTilePartial", m);
        }

        //// GET: Messages
        //public async Task<IActionResult> Index()
        //{
        //    var postgresContext = _context.Message.Include(m => m.Mailer).Include(m => m.Person).Include(m => m.Target);
        //    return View(await postgresContext.ToListAsync());
        //}

        //// GET: Messages/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var message = await _context.Message
        //        .Include(m => m.Mailer)
        //        .Include(m => m.Person)
        //        .Include(m => m.Target)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (message == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(message);
        //}

        //// GET: Messages/Create
        //public IActionResult Create()
        //{
        //    ViewData["MailerId"] = new SelectList(_context.MoneyMailer, "Id", "Id");
        //    ViewData["PersonId"] = new SelectList(_context.Person, "Id", "Email");
        //    ViewData["TargetId"] = new SelectList(_context.Person, "Id", "Email");
        //    return View();
        //}

        //// POST: Messages/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,PersonId,TargetId,MailerId,Time,Text")] Message message)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(message);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["MailerId"] = new SelectList(_context.MoneyMailer, "Id", "Id", message.MailerId);
        //    ViewData["PersonId"] = new SelectList(_context.Person, "Id", "Email", message.PersonId);
        //    ViewData["TargetId"] = new SelectList(_context.Person, "Id", "Email", message.TargetId);
        //    return View(message);
        //}

        //// GET: Messages/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var message = await _context.Message.FindAsync(id);
        //    if (message == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["MailerId"] = new SelectList(_context.MoneyMailer, "Id", "Id", message.MailerId);
        //    ViewData["PersonId"] = new SelectList(_context.Person, "Id", "Email", message.PersonId);
        //    ViewData["TargetId"] = new SelectList(_context.Person, "Id", "Email", message.TargetId);
        //    return View(message);
        //}

        //// POST: Messages/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,PersonId,TargetId,MailerId,Time,Text")] Message message)
        //{
        //    if (id != message.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(message);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!MessageExists(message.Id))
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
        //    ViewData["MailerId"] = new SelectList(_context.MoneyMailer, "Id", "Id", message.MailerId);
        //    ViewData["PersonId"] = new SelectList(_context.Person, "Id", "Email", message.PersonId);
        //    ViewData["TargetId"] = new SelectList(_context.Person, "Id", "Email", message.TargetId);
        //    return View(message);
        //}

        //// GET: Messages/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var message = await _context.Message
        //        .Include(m => m.Mailer)
        //        .Include(m => m.Person)
        //        .Include(m => m.Target)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (message == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(message);
        //}

        //// POST: Messages/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var message = await _context.Message.FindAsync(id);
        //    _context.Message.Remove(message);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool MessageExists(int id)
        //{
        //    return _context.Message.Any(e => e.Id == id);
        //}
    }
}
