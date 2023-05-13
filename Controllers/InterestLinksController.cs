using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Labb_3.Data;
using Labb_3.Models;

namespace Labb_3.Controllers
{
    public class InterestLinksController : Controller
    {
        private readonly InitialDbContext _context;

        public InterestLinksController(InitialDbContext context)
        {
            _context = context;
        }

        // GET: InterestLinks
        public async Task<IActionResult> Index()
        {
            var initialDbContext = _context.InterestLinks.Include(i => i.Interest);
            return View(await initialDbContext.ToListAsync());
        }

        // GET: InterestLinks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.InterestLinks == null)
            {
                return NotFound();
            }

            var interestLink = await _context.InterestLinks
                .Include(i => i.Interest)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (interestLink == null)
            {
                return NotFound();
            }

            return View(interestLink);
        }

        // GET: InterestLinks/Create
        public IActionResult Create()
        {
            ViewData["InterestId"] = new SelectList(_context.Interests, "Id", "Title"); // Change this line
            return View();
        }

        // POST: InterestLinks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Link,InterestId")] InterestLink interestLink)
        {
            ModelState.Remove("Interest");
            interestLink.Interest = _context.Interests.First(x => x.Id == interestLink.InterestId);
            if (ModelState.IsValid)
            {
                _context.Add(interestLink);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InterestId"] = new SelectList(_context.Interests, "Id", "Title", interestLink.Link);
            return View(interestLink);
        }

        // GET: InterestLinks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.InterestLinks == null)
            {
                return NotFound();
            }

            var interestLink = await _context.InterestLinks.FindAsync(id);
            if (interestLink == null)
            {
                return NotFound();
            }
            ViewData["InterestId"] = new SelectList(_context.Interests, "Id", "Id", interestLink.InterestId);
            return View(interestLink);
        }

        // POST: InterestLinks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Link,InterestId")] InterestLink interestLink)
        {
            if (id != interestLink.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(interestLink);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InterestLinkExists(interestLink.Id))
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
            ViewData["InterestId"] = new SelectList(_context.Interests, "Id", "Id", interestLink.InterestId);
            return View(interestLink);
        }

        // GET: InterestLinks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.InterestLinks == null)
            {
                return NotFound();
            }

            var interestLink = await _context.InterestLinks
                .Include(i => i.Interest)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (interestLink == null)
            {
                return NotFound();
            }

            return View(interestLink);
        }

        // POST: InterestLinks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.InterestLinks == null)
            {
                return Problem("Entity set 'InitialDbContext.InterestLinks'  is null.");
            }
            var interestLink = await _context.InterestLinks.FindAsync(id);
            if (interestLink != null)
            {
                _context.InterestLinks.Remove(interestLink);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InterestLinkExists(int id)
        {
          return (_context.InterestLinks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
