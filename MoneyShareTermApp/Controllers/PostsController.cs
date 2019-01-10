using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoneyShareTermApp.Models;

namespace MoneyShareTermApp.Controllers
{
    public class PostsController : Controller
    {
        private readonly PostgresContext _context;

        public PostsController(PostgresContext context)
        {
            _context = context;
        }

        public int UserId
        {
            get => int.Parse(User.Claims.FirstOrDefault(cl => cl.Type == ClaimTypes.PrimarySid).Value);
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var postgresContext = _context.Post.Include(p => p.Mailer).Include(p => p.Person).Include(p => p.PostNavigation);
            return View(await postgresContext.ToListAsync());
        }

        // GET: Posts/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var post = await _context.Post
        //        .Include(p => p.Mailer)
        //        .Include(p => p.Person)
        //        .Include(p => p.PostNavigation)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (post == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(post);
        //}

        // GET: Posts/Create

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PersonId,PostId,MailerId,Text")] Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["MailerId"] = new SelectList(_context.MoneyMailer, "Id", "Id", post.MailerId);
            ViewData["PersonId"] = new SelectList(_context.Person, "Id", "Email", post.PersonId);
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Id", post.PostId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PersonId,PostId,MailerId,Text")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
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
            ViewData["MailerId"] = new SelectList(_context.MoneyMailer, "Id", "Id", post.MailerId);
            ViewData["PersonId"] = new SelectList(_context.Person, "Id", "Email", post.PersonId);
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Id", post.PostId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }
    }
}
