using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment2.Data;
using Assignment2.Models;
using Assignment2.Models.ViewModels;

namespace Assignment2.Controllers
{
    public class NewsBoardsController : Controller
    {
        private readonly NewsDbContext _context;

        public NewsBoardsController(NewsDbContext context)
        {
            _context = context;
        }

        // GET: NewsBoards
        public async Task<IActionResult> Index(string Id)
        {
            NewsBoardViewModel viewModel = new NewsBoardViewModel
            {
                NewsBoards = await _context.NewsBoards
             .Include(i => i.Subscriptions)
             .ThenInclude(i => i.Client) //Includes the client data in the subscription
             .ToListAsync()
            };

            if (Id != null)
            {
                ViewData["NewsBoardId"] = Id;
                viewModel.Subscriptions = viewModel.NewsBoards
                    .Where(x => x.Id.Equals(Id)).Single().Subscriptions;
            }

            return View(viewModel);
        }

        // GET: NewsBoards/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.NewsBoards == null)
            {
                return NotFound();
            }

            var newsBoard = await _context.NewsBoards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (newsBoard == null)
            {
                return NotFound();
            }

            return View(newsBoard);
        }

        // GET: NewsBoards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NewsBoards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Fee")] NewsBoard newsBoard)
        {
            if (newsBoard.Id != null)
            {
                _context.Add(newsBoard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(newsBoard);
        }

        // GET: NewsBoards/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.NewsBoards == null)
            {
                return NotFound();
            }

            var newsBoard = await _context.NewsBoards.FindAsync(id);
            if (newsBoard == null)
            {
                return NotFound();
            }
            return View(newsBoard);
        }

        // POST: NewsBoards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Fee")] NewsBoard newsBoard)
        {
            if (id != newsBoard.Id)
            {
                return NotFound();
            }

            if (newsBoard.Id != null)
            {
                try
                {
                    _context.Update(newsBoard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsBoardExists(newsBoard.Id))
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
            return View(newsBoard);
        }

        // GET: NewsBoards/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.NewsBoards == null)
            {
                return NotFound();
            }

            var newsBoard = await _context.NewsBoards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (newsBoard == null)
            {
                return NotFound();
            }

            return View(newsBoard);
        }

        // POST: NewsBoards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.NewsBoards == null)
            {
                return Problem("Entity set 'NewsDbContext.NewsBoards'  is null.");
            }
            var newsBoard = await _context.NewsBoards.FindAsync(id);
            if (newsBoard != null)
            {
                _context.NewsBoards.Remove(newsBoard);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsBoardExists(string id)
        {
          return (_context.NewsBoards?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
