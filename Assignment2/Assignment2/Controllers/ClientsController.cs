using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment2.Data;

using Assignment2.Models.ViewModels;
using Assignment2.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Assignment2.Controllers
{
    public class ClientsController : Controller
    {
        private readonly NewsDbContext _context;

        public ClientsController(NewsDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index(int? Id)
        {
            var viewModel = new ClientSubscriptionsViewModel
            {
                Clients = await _context.Clients
                  .Include(i => i.Subscriptions)
                  .ThenInclude(i => i.NewsBoard)
                  .AsNoTracking()
                  .ToListAsync()
            };

            //gets newsboards from the context
            var newsboardModel = new NewsBoardViewModel
            {
                NewsBoards = await _context.NewsBoards
                  .Include(i => i.Subscriptions)
                  .AsNoTracking()
                  .ToListAsync()
            };

            

            if (Id != null)
            {
                viewModel.Client = viewModel.Clients
                             .Where(x => x.Id == Id).Single();

                viewModel.Subscriptions = new List<NewsBoardSubscriptionsViewModel>();

                foreach (var newsboard in newsboardModel.NewsBoards)
                {
                    viewModel.Subscriptions.Add(new NewsBoardSubscriptionsViewModel { NewsBoardId = newsboard.Id, Title = newsboard.Title, IsMember = false });
                }

                foreach (var subcription in viewModel.Client.Subscriptions)
                {
                    foreach (var newsboard in viewModel.Subscriptions)
                    {
                        if(subcription.NewsBoard.Id == newsboard.NewsBoardId)
                        {
                            newsboard.IsMember = true;
                        }
                    }
                }
                
            }

            return View(viewModel);
        }

        public async Task<IActionResult> EditSubscriptions(int? Id, string? newsboardId)
        {
            var viewModel = new ClientSubscriptionsViewModel
            {
                Clients = await _context.Clients
                             .Include(i => i.Subscriptions)
                             .ThenInclude(i => i.NewsBoard)
                             .AsNoTracking()
                             .ToListAsync()
            };

            var newsboardModel = new NewsBoardViewModel
            {
                NewsBoards = await _context.NewsBoards
                  .Include(i => i.Subscriptions)
                  .AsNoTracking()
                  .ToListAsync()
            };



            if (Id != null)
            {
                viewModel.Client = viewModel.Clients
                             .Where(x => x.Id == Id).Single();

                viewModel.Subscriptions = new List<NewsBoardSubscriptionsViewModel>();

                foreach (var newsboard in newsboardModel.NewsBoards)
                {
                    viewModel.Subscriptions.Add(new NewsBoardSubscriptionsViewModel { NewsBoardId = newsboard.Id, Title = newsboard.Title, IsMember = false });
                }

                foreach (var subcription in viewModel.Client.Subscriptions)
                {
                    foreach (var newsboard in viewModel.Subscriptions)
                    {
                        if (subcription.NewsBoard.Id.Equals(newsboard.NewsBoardId))
                        {
                            newsboard.IsMember = true;                         
                        }
                    }
                }

                if (newsboardId != null)
                {
                    foreach (var newsboard in viewModel.Subscriptions)
                    {
                        if (newsboardId.Equals(newsboard.NewsBoardId))
                        {

                            if (newsboard.IsMember == true)
                            {
                                var clientSubscription = await _context.Subscriptions.FindAsync(viewModel.Client.Id, newsboard.NewsBoardId);
                                if (clientSubscription != null)
                                {
                                    _context.Subscriptions.Remove(clientSubscription);
                                }

                            }
                            else
                            {
                                var clientSubscription = new Subscription { ClientId = viewModel.Client.Id, NewsBoardId = newsboard.NewsBoardId };
                                _context.Add(clientSubscription);

                            }
                            await _context.SaveChangesAsync();

                            newsboard.IsMember = !newsboard.IsMember;
                        }
                    }
                }

                viewModel.Subscriptions = viewModel.Subscriptions.OrderBy(s => s.IsMember).ThenBy(s => s.Title).ToList();

            }

            return View(viewModel);
        }


        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastName,FirstName,BirthDate")] Client client)
        {
            if (client.FirstName != null)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,BirthDate")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (client.FirstName != null)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Clients == null)
            {
                return Problem("Entity set 'NewsDbContext.Clients'  is null.");
            }
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
          return (_context.Clients?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
