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
    public class GroupWordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GroupWordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GroupWords
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Groups.ToListAsync());
        //}


        public async Task<IActionResult> Index(string? search, string? searchType = "exact")
        {
            if (string.IsNullOrEmpty(search))
            {
                return View("Index", new List<GroupWord>());
            }

            // Normalize the search string
            search = NormalizeString(search);

            // Store the search text and type in ViewBag
            ViewBag.SearchText = search;
            ViewBag.SearchType = searchType;

            // Query the Words table
            var wordsQuery = _context.Groups.AsQueryable();

            var wordsList = await wordsQuery.ToListAsync();
            // Apply the search type
            switch (searchType)
            {
                case "exact":
                    // Exact match
                    wordsList = wordsList.Where(w => NormalizeString(w.Name) == search).ToList();
                    break;

                case "contain":
                    // Contains search term
                    wordsList = wordsList.Where(w => NormalizeString(w.Name).Contains(search)).ToList(); break;

                case "start":
                    // Starts with search term
                    wordsList = wordsList.Where(w => NormalizeString(w.Name).StartsWith(search)).ToList(); break;


                case "end":
                    // Ends with search term
                    wordsList = wordsList.Where(w => NormalizeString(w.Name).EndsWith(search)).ToList(); break;
                    break;

                default:
                    // Default to contains search if no valid search type is provided
                    wordsList = wordsList.Where(w => NormalizeString(w.Name).StartsWith(search)).ToList(); break;
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


        // GET: GroupWords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupWord = await _context.Groups
                .Include(m => m.Words)
                .Include(g => g.GroupExplanations)
                .Include(g => g.GroupParents) // Include parent group relations
                        .ThenInclude(gr => gr.ParentGroup) // Include parent groups
                            .ThenInclude(m => m.Words)
                .Include(g => g.GroupChilds) // Include child group relations
                        .ThenInclude(gr => gr.RelatedGroup) // Include child groups
                            .ThenInclude(m => m.Words)
                            .AsSplitQuery()
                            .FirstOrDefaultAsync(m => m.ID == id);
            if (groupWord == null)
            {
                return NotFound();
            }

            return View(groupWord);
        }

        // GET: GroupWords/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GroupWords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,OriginLanguage,EtymologyWord,Etymology,Notes,Pronunciation")] GroupWord groupWord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(groupWord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(groupWord);
        }

        // GET: GroupWords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupWord = await _context.Groups.FindAsync(id);
            if (groupWord == null)
            {
                return NotFound();
            }
            return View(groupWord);
        }

        // POST: GroupWords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,OriginLanguage,EtymologyWord,Etymology,Notes,Pronunciation")] GroupWord groupWord)
        {
            if (id != groupWord.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(groupWord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupWordExists(groupWord.ID))
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
            return View(groupWord);
        }

        // GET: GroupWords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupWord = await _context.Groups
                .FirstOrDefaultAsync(m => m.ID == id);
            if (groupWord == null)
            {
                return NotFound();
            }

            return View(groupWord);
        }

        // POST: GroupWords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var groupWord = await _context.Groups.FindAsync(id);
            if (groupWord != null)
            {
                _context.Groups.Remove(groupWord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupWordExists(int id)
        {
            return _context.Groups.Any(e => e.ID == id);
        }


        public IActionResult CreateGroupAsChild(int parentGroupID, int wordId)
        {
            var model = new CreateGroupViewModel
            {
                ParentGroupID = parentGroupID
            };
            ViewBag.WordId = wordId;
            return View(model);
        }

        // Action to handle form submission
        [HttpPost]
        public async Task<IActionResult> CreateGroupAsChild(CreateGroupViewModel model, int wordId)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Create a new GroupWord
            var newGroup = new GroupWord
            {
                Name = model.NewGroupName,
                OriginLanguage = model.OriginLanguage,
                EtymologyWord = model.EtymologyWord,
                Etymology = model.Etymology,
                Notes = model.Notes
            };

            // Add the new GroupWord to the context
            _context.Groups.Add(newGroup);
            await _context.SaveChangesAsync(); // Save to get the new Group ID

            // Create the GroupRelation
            var groupRelation = new GroupRelation
            {
                ParentGroupID = model.ParentGroupID,
                RelatedGroupID = newGroup.ID,
                IsCompound = model.IsCompound
            };

            // Add the GroupRelation to the context
            _context.GroupRelations.Add(groupRelation);
            await _context.SaveChangesAsync(); // Save the relation

            return RedirectToAction("Details", "GroupWords", new { id = model.ParentGroupID });
        }




        public IActionResult CreateGroupAsParent(int ChildGroupID, int wordId)
        {
            var model = new CreateGroupViewModel
            {
                ParentGroupID = ChildGroupID
            };
            ViewBag.wordId = wordId;
            return View(model);
        }

        // Action to handle form submission
        [HttpPost]
        public async Task<IActionResult> CreateGroupAsParent(CreateGroupViewModel model, int wordId)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Create a new GroupWord
            var newGroup = new GroupWord
            {
                Name = model.NewGroupName,
                OriginLanguage = model.OriginLanguage,
                EtymologyWord = model.EtymologyWord,
                Etymology = model.Etymology,
                Notes = model.Notes
            };

            // Add the new GroupWord to the context
            _context.Groups.Add(newGroup);
            await _context.SaveChangesAsync(); // Save to get the new Group ID

            // Create the GroupRelation
            var groupRelation = new GroupRelation
            {
                ParentGroupID = newGroup.ID,
                RelatedGroupID = model.ParentGroupID,
                IsCompound = model.IsCompound
            };

            // Add the GroupRelation to the context
            _context.GroupRelations.Add(groupRelation);
            await _context.SaveChangesAsync(); // Save the relation
            return RedirectToAction("Details", "GroupWords", new { id = model.ParentGroupID });

        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGroupRelation(int id, int wordId)
        {

            var relation = await _context.GroupRelations.FindAsync(id);

            if (relation == null)
            {
                Console.WriteLine($"relation with ID {id} was not found.");
                return NotFound();
            }

            try
            {
                _context.GroupRelations.Remove(relation);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting meaning with ID {id}: {ex.Message}");
                return BadRequest("Error occurred while deleting.");
            }

            return RedirectToAction("Details", "GroupWords", new { id = wordId });
        }




        public IActionResult AddMeaningToGroup(int groupId, int wordId)
        {
            var model = new AddMeaningToGroupViewModel
            {
                GroupId = groupId
            };
            ViewBag.wordId = wordId;

            return View(model);
        }

        // POST Action to handle the form submission
        [HttpPost]
        public async Task<IActionResult> AddMeaningToGroup(AddMeaningToGroupViewModel model, int wordId)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get all words in the specified group
            var words = await _context.Words
                .Where(w => w.GroupID == model.GroupId)
                .ToListAsync();

            if (words == null || !words.Any())
            {
                TempData["Error"] = "No words found in the specified group.";
                return RedirectToAction("Details", "GroupWords", new { id = model.GroupId });

            }

            // Create a new meaning
            var newMeaning = new Meaning
            {
                MeaningText = model.MeaningText,
                Language = model.Language,
                Notes = model.Notes
            };

            // Add the new meaning to each word in the group
            foreach (var word in words)
            {
                var wordMeaning = new WordMeaning
                {
                    WordID = word.WordId,
                    Meaning = newMeaning
                };
                _context.WordMeanings.Add(wordMeaning);
            }

            // Save the changes to the database
            await _context.SaveChangesAsync();

            TempData["Message"] = "Meaning added to all words in the group successfully.";
            return RedirectToAction("Details", "GroupWords", new { id = model.GroupId });
        }






        public IActionResult AddWordToGroup(int groupid)
        {
            // GroupID - Combine Name, OriginLanguage, and EtymologyWord for the display field
            ViewData["GroupID"] = new SelectList(_context.Groups.Select(g => new {
                g.ID,
                DisplayField = g.Name + " (" + g.OriginLanguage + ", " + g.EtymologyWord + ")"
            }), "ID", "DisplayField");

            // RootID - Only words that start with "C-"
            ViewData["RootID"] = new SelectList(_context.Words
                .Where(w => w.Language.StartsWith("C-"))
                .Select(w => new {
                    w.WordId,
                    DisplayField = w.Word_text + " (" + w.Language + ", " + w.Class + ")"
                }), "WordId", "DisplayField");

            ViewData["Languages"] = new SelectList(GetLanguagesList(), "Value", "Text");
            TempData["ReturnUrl"] = Request.Headers["Referer"].ToString();

            ViewBag.GroupID = groupid;
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWordToGroup(int groupid, [Bind("WordId,Word_text,Language,Class,notes,IPA,Pronunciation,IsDrevWord,RootID,GroupID")] Word word)
        {
            if (ModelState.IsValid)
            {
                _context.Add(word);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // GroupID - Combine Name, OriginLanguage, and EtymologyWord for the display field
            ViewData["GroupID"] = new SelectList(_context.Groups.Select(g => new {
                g.ID,
                DisplayField = g.Name + " (" + g.OriginLanguage + ", " + g.EtymologyWord + ")"
            }), "ID", "DisplayField", word.GroupID);

            // RootID - Only words that start with "C-"
            ViewData["RootID"] = new SelectList(_context.Words
                .Where(w => w.Language.StartsWith("C-"))
                .Select(w => new {
                    w.WordId,
                    DisplayField = w.Word_text + " (" + w.Language + ", " + w.Class + ")"
                }), "WordId", "DisplayField", word.RootID);

            ViewData["Languages"] = new SelectList(GetLanguagesList(), "Value", "Text", word.Language);
            var returnUrl = TempData["ReturnUrl"] as string;
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return View(word);
        }

        public async Task<IActionResult> AddExistingMeaningToGroup(int groupId)
        {
            var meanings = _context.Meanings
                .Include(m => m.WordMeanings) // Include related WordMeanings
                .ThenInclude(wm => wm.Word)   // Include related Word for each WordMeaning
                .ToList();

            // Create a list of SelectListItem with formatted text
            var selectListItems = meanings.Select(m => new SelectListItem
            {
                Value = m.ID.ToString(), // Use Meaning ID as the value
                Text = $"{m.MeaningText} - [{string.Join(", ", m.WordMeanings.Select(wm => wm.Word.Word_text))}]" // Format the text
            }).ToList();

            // Pass the formatted SelectList to the view
            ViewBag.Meanings = new SelectList(selectListItems, "Value", "Text");
            ViewBag.groupId = groupId; // Pass the WordId to the view
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddExistingMeaningToGroup( int meaningID, int groupId)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "The selected word does not exist.");
                ViewBag.Meanings = new SelectList(_context.Meanings.ToList(), "ID", "MeaningText");
                ViewBag.groupId = groupId;
                return View();
            }

            // Get all words in the specified group
            var words = await _context.Words
                .Where(w => w.GroupID == groupId)
                .ToListAsync();

            if (words == null || !words.Any())
            {
                TempData["Error"] = "No words found in the specified group.";
                return RedirectToAction("Details", "GroupWords", new { id = groupId });
            }

            // Add the existing meaning to each word in the group
            foreach (var word in words)
            {
                var wordMeaning = new WordMeaning
                {
                    WordID = word.WordId,
                    MeaningID = meaningID
                };
                _context.WordMeanings.Add(wordMeaning);
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = "Existing meaning added to all words in the group successfully.";
            return RedirectToAction("Details", "GroupWords", new { id = groupId });
        }

        private List<SelectListItem> GetLanguagesList()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem { Value = "AR", Text = "Arabic" },
                    new SelectListItem { Value = "FR", Text = "French" },
                    new SelectListItem { Value = "EN", Text = "English" },
                    new SelectListItem { Value = "RU", Text = "Russian" },
                    new SelectListItem { Value = "DE", Text = "German" },
                    new SelectListItem { Value = "IT", Text = "Italian" },
                    new SelectListItem { Value = "HE", Text = "Hebrew" },
                    new SelectListItem { Value = "GR", Text = "Greek" },
                    new SelectListItem { Value = "ARC", Text = "Aramaic" },
                    new SelectListItem { Value = "EG",  Text = "Egyptian" },
                    new SelectListItem { Value = "C-B" , Text = "Coptic - B" },
                    new SelectListItem { Value = "C-S",  Text = "Coptic - S" },
                    new SelectListItem { Value = "C-Sa", Text = "Coptic - Sa" },
                    new SelectListItem { Value = "C-Sf", Text = "Coptic - Sf" },
                    new SelectListItem { Value = "C-A",  Text = "Coptic - A" },
                    new SelectListItem { Value = "C-sA", Text = "Coptic - sA" },
                    new SelectListItem { Value = "C-F",  Text = "Coptic - F" },
                    new SelectListItem { Value = "C-Fb", Text = "Coptic - Fb" },
                    new SelectListItem { Value = "C-O",  Text = "Coptic - O" },
                    new SelectListItem { Value = "C-NH", Text = "Coptic - NH" }
                };
        }

    }
}
