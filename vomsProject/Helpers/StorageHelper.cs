using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using vomsProject.Data;

namespace vomsProject.Helpers
{
    public class StorageHelper
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public StorageHelper(string connectionString, string containerName)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerName = !string.IsNullOrWhiteSpace(containerName) ? containerName : "voms";
        }

        public BlobContainerClient GetBlobContainer()
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            return containerClient;
        }

        public async Task<bool> UploadToBlob(Stream file, ApplicationDbContext dbContext, Page Page, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var _context = dbContext;

            var imageGuid = Guid.NewGuid().ToString();

            try
            {
                var blob = await containerClient.UploadBlobAsync(imageGuid, file);

                await _context.Images.AddAsync(new Image
                {
                    ImageUrl = imageGuid,
                    Page = Page,
                    Solution = Page.Solution,
                    FriendlyName = fileName
                });

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("(UploadToBlob-method) failed with following exception: " + e);
                return false;
            }
        }

        public async Task<Stream> DownloadBlob(string imageName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blob = await containerClient.GetBlobClient(imageName).DownloadAsync();

            return blob.Value.Content;
        }
    }
}
