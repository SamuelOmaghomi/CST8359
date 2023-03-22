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
using System.IO;

namespace Lab5.Pages.Predictions
{


    public class IndexModel : PageModel
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string earthContainerName = "earthimages";
        private readonly string computerContainerName = "computerimages";
        private readonly Lab5.Data.PredictionDataContext _context;
        public List<Prediction> Prediction { get; set; } = new();
        public IndexModel(BlobServiceClient blobServiceClient)
        {
             _blobServiceClient = blobServiceClient;
        }

        

        public async Task OnGetAsync()
        {
            // Create a container for organizing blobs within the storage account.
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
            if(earthContainerClient.Exists())
            {
                foreach (var blob in earthContainerClient.GetBlobs())
                {
                    // Blob type will be BlobClient, CloudPageBlob or BlobClientDirectory
                    // Use blob.GetType() and cast to appropriate type to gain access to properties specific to each type
                    Prediction.Add(new Prediction { FileName = blob.Name, Url = earthContainerClient.GetBlobClient(blob.Name).Uri.AbsoluteUri, question = Models.Prediction.Question.Earth });
                }
            }

            if (computerContainerClient.Exists())
            {
                foreach (var blob in computerContainerClient.GetBlobs())
                {
                    // Blob type will be BlobClient, CloudPageBlob or BlobClientDirectory
                    // Use blob.GetType() and cast to appropriate type to gain access to properties specific to each type
                    Prediction.Add(new Prediction { FileName = blob.Name, Url = computerContainerClient.GetBlobClient(blob.Name).Uri.AbsoluteUri, question = Models.Prediction.Question.Computer });
                }
            }

            

            /*if (_context.Predictions != null)
            {
                Prediction = await _context.Predictions.ToListAsync();
            }*/
        }
    }
}
