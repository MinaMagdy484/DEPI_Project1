using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CopticDictionarynew1.Controllers
{
    [Authorize(Roles = "Student,Instructor,Admin")]
    public class DrevWordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DrevWordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DrevWords
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DrevWords.Include(d => d.Word1).Include(d => d.Word2);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DrevWords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drevWord = await _context.DrevWords
                .Include(d => d.Word1)
                .Include(d => d.Word2)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (drevWord == null)
            {
                return NotFound();
            }

            return View(drevWord);
        }

        // GET: DrevWords/Create
        public IActionResult Create()
        {
            ViewData["WordID"] = new SelectList(_context.Words, "WordId", "Class");
            ViewData["RelatedWordID"] = new SelectList(_context.Words, "WordId", "Class");
            return View();
        }

        // POST: DrevWords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,WordID,RelatedWordID")] DrevWord drevWord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(drevWord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WordID"] = new SelectList(_context.Words, "WordId", "Class", drevWord.WordID);
            ViewData["RelatedWordID"] = new SelectList(_context.Words, "WordId", "Class", drevWord.RelatedWordID);
            return View(drevWord);
        }

        // GET: DrevWords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drevWord = await _context.DrevWords.FindAsync(id);
            if (drevWord == null)
            {
                return NotFound();
            }
            ViewData["WordID"] = new SelectList(_context.Words, "WordId", "Class", drevWord.WordID);
            ViewData["RelatedWordID"] = new SelectList(_context.Words, "WordId", "Class", drevWord.RelatedWordID);
            return View(drevWord);
        }

        // POST: DrevWords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,WordID,RelatedWordID")] DrevWord drevWord)
        {
            if (id != drevWord.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(drevWord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DrevWordExists(drevWord.ID))
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
            ViewData["WordID"] = new SelectList(_context.Words, "WordId", "Class", drevWord.WordID);
            ViewData["RelatedWordID"] = new SelectList(_context.Words, "WordId", "Class", drevWord.RelatedWordID);
            return View(drevWord);
        }

        // GET: DrevWords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drevWord = await _context.DrevWords
                .Include(d => d.Word1)
                .Include(d => d.Word2)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (drevWord == null)
            {
                return NotFound();
            }

            return View(drevWord);
        }

        // POST: DrevWords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var drevWord = await _context.DrevWords.FindAsync(id);
            if (drevWord != null)
            {
                _context.DrevWords.Remove(drevWord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DrevWordExists(int id)
        {
            return _context.DrevWords.Any(e => e.ID == id);
        }
    }
}
