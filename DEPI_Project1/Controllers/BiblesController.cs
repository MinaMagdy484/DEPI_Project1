using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CopticDictionarynew1.Controllers
{
    [Authorize(Roles = "Student,Instructor,Admin")]
    public class BiblesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BiblesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bibles
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Bibles.ToListAsync());
        //}

        
        public async Task<IActionResult> Index(string? search, string? searchType = "exact")
        {
            if (string.IsNullOrEmpty(search))
            {
                return View("Index", new List<Bible>());
            }

            // Normalize the search string
            search = NormalizeString(search);

            // Store the search text and type in ViewBag
            ViewBag.SearchText = search;
            ViewBag.SearchType = searchType;

            // Query the Words table
            var wordsQuery = _context.Bibles.AsQueryable();

            var wordsList = await wordsQuery.ToListAsync();
            // Apply the search type
            switch (searchType)
            {
                case "exact":
                    // Exact match
                    wordsList = wordsList.Where(w => NormalizeString(w.Text) == search).ToList();
                    break;

                case "contain":
                    // Contains search term
                    wordsList = wordsList.Where(w => NormalizeString(w.Text).Contains(search)).ToList(); break;

                case "start":
                    // Starts with search term
                    wordsList = wordsList.Where(w => NormalizeString(w.Text).StartsWith(search)).ToList(); break;


                case "end":
                    // Ends with search term
                    wordsList = wordsList.Where(w => NormalizeString(w.Text).EndsWith(search)).ToList(); break;
                    break;

                default:
                    // Default to contains search if no valid search type is provided
                    wordsList = wordsList.Where(w => NormalizeString(w.Text).StartsWith(search)).ToList(); break;
                    break;
            }

            // Fetch the results
            return View("Index", wordsList);
        }


        private string NormalizeString(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            // Convert to lowercase
            input = input.ToLowerInvariant();

            // Optional: Remove diacritics (accents)
            input = RemoveDiacritics(input);

            return input;
        }


        private string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // Normalize to form D (decomposed characters), remove non-spacing marks
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        // GET: Bibles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bible = await _context.Bibles
                .FirstOrDefaultAsync(m => m.BibleID == id);
            if (bible == null)
            {
                return NotFound();
            }

            return View(bible);
        }

        // GET: Bibles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bibles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BibleID,Book,Chapter,Verse,Language,Edition,Text,Pronunciation,Notes")] Bible bible)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bible);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bible);
        }

        // GET: Bibles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bible = await _context.Bibles.FindAsync(id);
            if (bible == null)
            {
                return NotFound();
            }
            return View(bible);
        }

        // POST: Bibles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BibleID,Book,Chapter,Verse,Language,Edition,Text,Pronunciation,Notes")] Bible bible)
        {
            if (id != bible.BibleID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bible);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BibleExists(bible.BibleID))
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
            return View(bible);
        }

        // GET: Bibles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bible = await _context.Bibles
                .FirstOrDefaultAsync(m => m.BibleID == id);
            if (bible == null)
            {
                return NotFound();
            }

            return View(bible);
        }

        // POST: Bibles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bible = await _context.Bibles.FindAsync(id);
            if (bible != null)
            {
                _context.Bibles.Remove(bible);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BibleExists(int id)
        {
            return _context.Bibles.Any(e => e.BibleID == id);
        }
    }
}
