using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment2.Data;
using Assignment2.Models;
using Azure.Storage.Blobs;
using Assignment2.Models.ViewModels;
using Azure;

namespace Assignment2.Controllers
{
    public class NewsController : Controller
    {
        private readonly NewsDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        NewsViewModel viewModel;
        string containerName;

        public NewsController(NewsDbContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        // GET: News
        public async Task<IActionResult> Index(string id)
        {
            if(id == null)
            {
                return View("Error");
            }
            viewModel = new NewsViewModel
            {
                NewsBoards = await _context.NewsBoards
                .Include(x => x.News)
                .ToListAsync(),

            };

            viewModel.NewsBoard = viewModel.NewsBoards
                .Where(x => x.Id.Equals(id)).Single();

            List<News> news = await _context.News
                .Include(x => x.NewsBoard)
                .ToListAsync();

            viewModel.News = new();

            containerName = viewModel.NewsBoard.Title.ToLower();
            // Create a container for organizing blobs within the storage account.
            BlobContainerClient containerClient;
            try
            {
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName, Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch (RequestFailedException e)
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }


            foreach (var blob in containerClient.GetBlobs())
            {
                // Blob type will be BlobClient, CloudPageBlob or BlobClientDirectory
                // Use blob.GetType() and cast to appropriate type to gain access to properties specific to each type
                viewModel.News.Add(new News { FileName = blob.Name, Url = containerClient.GetBlobClient(blob.Name).Uri.AbsoluteUri });
            }
            if(viewModel.News.Count() > 0 && news.Count()>0)
            {
                foreach(var blobNews in viewModel.News)
                {
                    foreach(var contextNews in news)
                    {
                        if(contextNews.FileName.Equals(blobNews.FileName) && contextNews.Url.Equals(blobNews.Url))
                        {
                            blobNews.NewsId = contextNews.NewsId;
                        }
                        
                    }
                }
            }



            return View(viewModel);
        }

        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.NewsId == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: News/Create
        public IActionResult Create(string id)
        {
            if(id == null)
            {
                return View("Error");
            }

            viewModel = new NewsViewModel
            {
                NewsBoard = new NewsBoard { Id = id }
            };

            return View(viewModel);
        }

        // POST: News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id")] NewsBoard newsBoard, IFormFile file)
        {
            /*   if (ModelState.IsValid)
               {
                   _//context.Add(news);
                   await _context.SaveChangesAsync();
                   return RedirectToAction(nameof(Index));
               }*/

            viewModel = new NewsViewModel
            {
                NewsBoards = await _context.NewsBoards
                .Include(x => x.News)
                .ToListAsync()
            };

            viewModel.NewsBoard = viewModel.NewsBoards
                .Where(x => x.Id.Equals(newsBoard.Id)).Single();
            containerName = viewModel.NewsBoard.Title.ToLower(); 

            BlobContainerClient containerClient;
            // Create the container and return a container client object
            try
            {
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName);
                // Give access to public
                containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch (RequestFailedException)
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }

            try
            {
                string randomFileName = Path.GetRandomFileName();
                // create the blob to hold the data
                var blockBlob = containerClient.GetBlobClient(randomFileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                using (var memoryStream = new MemoryStream())
                {
                    // copy the file data into memory
                    await file.CopyToAsync(memoryStream);

                    // navigate back to the beginning of the memory stream
                    memoryStream.Position = 0;

                    // send the file to the cloud
                    await blockBlob.UploadAsync(memoryStream);
                    memoryStream.Close();
                }

                var news = new News
                {
                    FileName = randomFileName,
                    Url = blockBlob.Uri.ToString(),
                    NewsBoard = viewModel.NewsBoard
                };

                _context.News.Add(news);
                _context.SaveChanges();
            }
            catch (RequestFailedException)
            {
                RedirectToPage("Error");
            }

            return RedirectToAction("Index", new { id = newsBoard.Id });
        }

        // GET: News/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NewsId,FileName,Url")] News news)
        {
            if (id != news.NewsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.NewsId))
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
            return View(news);
        }

        // GET: News/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(x => x.NewsBoard)
                .FirstOrDefaultAsync(m => m.NewsId == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.News == null)
            {
                return Problem("Entity set 'NewsDbContext.News'  is null.");
            }
            viewModel = new NewsViewModel
               {
                   News = await _context.News
                   .Include(x => x.NewsBoard)
                   .ToListAsync()
               };


            var news = viewModel.News
                   .Where(x => x.NewsId.Equals(id)).Single();

            if (news == null)
            {
                return View("Error");
            }

            containerName = news.NewsBoard.Title.ToLower();

            BlobContainerClient containerClient;
            // Get the container and return a container client object
            try
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }
            catch (RequestFailedException)
            {
                return View("Error");
            }

            foreach (var blob in containerClient.GetBlobs())
            {
                try
                {
                    // Get the blob that holds the data
                    var blockBlob = containerClient.GetBlobClient(blob.Name);
                    if (await blockBlob.ExistsAsync())
                    {
                        if (blob.Name.Equals(news.FileName))
                        {
                            await blockBlob.DeleteAsync();

                            //delete from database
                            
                            if (news != null)
                            {
                                _context.News.Remove(news);
                            }
                            await _context.SaveChangesAsync();
                        }

                    }
                }
                catch (RequestFailedException)
                {
                    return View("Error");
                }
            }


            return RedirectToAction("Index", new { id = news.NewsBoard.Id });
        }

        private bool NewsExists(int id)
        {
          return (_context.News?.Any(e => e.NewsId == id)).GetValueOrDefault();
        }
    }
}
