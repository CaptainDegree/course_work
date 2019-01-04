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
    public class MessagesController : Controller
    {
        private readonly postgresContext _context;

        public MessagesController(postgresContext context)
        {
            _context = context;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            var postgresContext = _context.Message.Include(m => m.Mailer).Include(m => m.Person).Include(m => m.Target);
            return View(await postgresContext.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                .Include(m => m.Mailer)
                .Include(m => m.Person)
                .Include(m => m.Target)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            ViewData["MailerId"] = new SelectList(_context.MoneyMailer, "Id", "Id");
            ViewData["PersonId"] = new SelectList(_context.Person, "Id", "Email");
            ViewData["TargetId"] = new SelectList(_context.Person, "Id", "Email");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PersonId,TargetId,MailerId,Time,Text")] Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MailerId"] = new SelectList(_context.MoneyMailer, "Id", "Id", message.MailerId);
            ViewData["PersonId"] = new SelectList(_context.Person, "Id", "Email", message.PersonId);
            ViewData["TargetId"] = new SelectList(_context.Person, "Id", "Email", message.TargetId);
            return View(message);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["MailerId"] = new SelectList(_context.MoneyMailer, "Id", "Id", message.MailerId);
            ViewData["PersonId"] = new SelectList(_context.Person, "Id", "Email", message.PersonId);
            ViewData["TargetId"] = new SelectList(_context.Person, "Id", "Email", message.TargetId);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PersonId,TargetId,MailerId,Time,Text")] Message message)
        {
            if (id != message.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MailerId"] = new SelectList(_context.MoneyMailer, "Id", "Id", message.MailerId);
            ViewData["PersonId"] = new SelectList(_context.Person, "Id", "Email", message.PersonId);
            ViewData["TargetId"] = new SelectList(_context.Person, "Id", "Email", message.TargetId);
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                .Include(m => m.Mailer)
                .Include(m => m.Person)
                .Include(m => m.Target)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Message.FindAsync(id);
            _context.Message.Remove(message);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Message.Any(e => e.Id == id);
        }
    }
}
