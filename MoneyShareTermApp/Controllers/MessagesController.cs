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
        public async Task<IActionResult> Write(int targetId, string msg, Currency cur, MsgType type)
        {
            Person sender = _context.Person
                .Include(p => p.Account)
                .Include(p => p.Mailer)
                .SingleOrDefault(p => p.Id == UserId);

            Person target = _context.Person
                .Include(p => p.MessagePrice)
                .Include(p => p.Mailer)
                .SingleOrDefault(p => p.Id == targetId);

            CurrencySet payment = new CurrencySet // плата
            {
                Euro = 0,
                Dollar = 0,
                Ruble = 0
            };

            Message m; // или сообщение пользователя, или о переводе

            if (type == MsgType.Message) // если - сообщение, то отправить 
            {
                int charAmount = msg.Length; // оплата за знаки
                switch (cur)
                {
                    case Currency.Dollar:
                        payment.Dollar = target.MessagePrice.Dollar * charAmount;
                        break;
                    case Currency.Euro:
                        payment.Euro = target.MessagePrice.Euro * charAmount;
                        break;
                    case Currency.Ruble:
                        payment.Ruble = target.MessagePrice.Ruble * charAmount;
                        break;
                    default:
                        throw new ArgumentException();
                }

                m = new Message
                {
                    PersonId = UserId,
                    TargetId = targetId,
                    Mailer = new MoneyMailer(),
                    Text = msg
                };
            } else // если просто перевод 
            {
                decimal val = decimal.Parse(msg);

                if (val == 0)
                    return new JsonResult(false);

                switch (cur)
                {
                    case Currency.Dollar:
                        payment.Dollar = val;
                        break;
                    case Currency.Euro:
                        payment.Euro = val;
                        break;
                    case Currency.Ruble:
                        payment.Ruble = val;
                        break;
                    default:
                        throw new ArgumentException();
                }

                m = new Message
                {
                    PersonId = UserId,
                    TargetId = targetId,
                    Mailer = new MoneyMailer(),
                    Text = Message.MsgTransfer + payment.GetOne().Item2 + payment.GetOne().Item1
                };
            }

            _context.Add(m); // добавим сообщение, чтобы сослаться на него при отправке
            _context.SaveChanges();

            try
            {
                switch (type)
                {
                    case MsgType.Message:
                        MoneyTransfer.TransMoney(sender, m.Mailer, payment, TransferType.Message, _context);
                        break;
                    case MsgType.Money:
                        MoneyTransfer.TransMoney(sender, m.Mailer, payment, TransferType.Trans, _context);
                        break;
                    default:
                        throw new ArgumentException();
                }
            } catch
            {
                _context.Remove(m);
                await _context.SaveChangesAsync();
                return new JsonResult(false);
            }

            return PartialView("_MsgTilePartial", m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendMsgs(int targetId, int msgId)
        {
            var msgs = _context.Message
                .Include(m => m.Mailer)
                    .ThenInclude(m => m.MoneyTransferTarget)
                .Include(m => m.Person)
                .Include("Person.Photo")
                .Include("Person.Account")
                .Include("Person.Mailer")
                .Where(m => m.PersonId.Equals(UserId) && m.TargetId.Equals(targetId) && m.Id > msgId)
                .OrderBy(m => m.Mailer.CreationTime);

            foreach (var m in msgs)
                foreach (var mtt in m.Mailer.MoneyTransferTarget)
                    mtt.Account = _context.CurrencySet.FirstOrDefault(cs => cs.Id == mtt.AccountId);

            if (msgs.Any())
                return PartialView("_MsgTilePartial", msgs.First());

            return new JsonResult(false);
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
