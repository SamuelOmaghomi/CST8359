using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lab5.Data;
using Lab5.Models;
using Azure.Storage.Blobs;
using Azure;

namespace Lab5.Pages.Predictions
{
    public class DeleteModel : PageModel
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string earthContainerName = "earthimages";
        private readonly string computerContainerName = "computerimages";

        private readonly Lab5.Data.PredictionDataContext _context;

        public DeleteModel(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        [BindProperty]
        public Prediction Prediction { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string Id)
        {
            /*            if (id == null || _context.Predictions == null)
                        {
                            return NotFound();
                        }

                        var prediction = await _context.Predictions.FirstOrDefaultAsync(m => m.PredictionId == id);

                        if (prediction == null)
                        {
                            return NotFound();
                        }
                        else 
                        {
                            Prediction = prediction;
                        }*/

            BlobContainerClient earthContainerClient;
            BlobContainerClient computerContainerClient;
            try
            {
                earthContainerClient = await _blobServiceClient.CreateBlobContainerAsync(earthContainerName, Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
                computerContainerClient = await _blobServiceClient.CreateBlobContainerAsync(computerContainerName, Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch (Exception e)
            {
                earthContainerClient = _blobServiceClient.GetBlobContainerClient(earthContainerName);
                computerContainerClient = _blobServiceClient.GetBlobContainerClient(computerContainerName);
            }

            //List<Smiley> smilies = new();
            if (earthContainerClient.Exists())      //checks if the containercliennt esists
            {
                foreach (var blob in earthContainerClient.GetBlobs())
                {
                    var blockBlob = earthContainerClient.GetBlobClient(blob.Name);
                    if (await blockBlob.ExistsAsync())
                    {
                        if (blob.Name.Equals(Id))
                        {
                            Prediction = new Prediction { FileName = blob.Name, Url = earthContainerClient.GetBlobClient(blob.Name).Uri.AbsoluteUri, question = Models.Prediction.Question.Earth };
                        }
                    }
                }
            }

            if (computerContainerClient.Exists())
            {
                foreach (var blob in computerContainerClient.GetBlobs())
                {
                    var blockBlob = computerContainerClient.GetBlobClient(blob.Name);
                    if (await blockBlob.ExistsAsync())
                    {
                        if (blob.Name.Equals(Id))
                        {
                            Prediction = new Prediction { FileName = blob.Name, Url = computerContainerClient.GetBlobClient(blob.Name).Uri.AbsoluteUri, question = Models.Prediction.Question.Computer };
                        }
                    }
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string Id)
        {
            BlobContainerClient containerClient;
            // Get the container and return a container client object
            string containerName;
            if (Prediction.question == Prediction.Question.Earth)
            {
                containerName = earthContainerName;
            }
            else
            {
                containerName = computerContainerName;
            }

            try
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }
            catch (RequestFailedException)
            {
                return RedirectToPage("Error");
            }

            foreach (var blob in containerClient.GetBlobs())
            {
                try
                {
                    // Get the blob that holds the data
                    var blockBlob = containerClient.GetBlobClient(blob.Name);
                    if (await blockBlob.ExistsAsync())
                    {
                        if (blob.Name.Equals(Id))
                        {
                            await blockBlob.DeleteAsync();
                        }
                    }
                }
                catch (RequestFailedException)
                {
                    return RedirectToPage("Error");
                }
            }
            /*     if (id == null || _context.Predictions == null)
                 {
                     return NotFound();
                 }
                 var prediction = await _context.Predictions.FindAsync(id);

                 if (prediction != null)
                 {
                     Prediction = prediction;
                     _context.Predictions.Remove(Prediction);
                     await _context.SaveChangesAsync();
                 }*/

            return RedirectToPage("./Index");
        }
    }
}
