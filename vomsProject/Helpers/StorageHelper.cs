using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
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
        private readonly IConfiguration Configuration;
        private readonly CloudBlobContainer _blobContainer;

        public StorageHelper(IConfiguration configuration)
        {
            _containerName = "voms";
            _containerName = !string.IsNullOrWhiteSpace(_containerName) ? _containerName : "voms";
            Configuration = configuration;

            _blobServiceClient = new BlobServiceClient(Configuration.GetValue<string>("ConnectionStrings:BlobStorageConnection"));

            _blobContainer = new CloudBlobContainer(new Uri("https://sejersenstorageaccount.blob.core.windows.net/voms"),
                new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                    Configuration.GetValue<string>("AccountName"), 
                    Configuration.GetValue<string>("AccountKey")));
        }

        /// <summary>
        /// Uploads a blob to the storageaccount (and a specific container) assigned in appsettings.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="dbContext"></param>
        /// <param name="Page"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Deletes a blob from the same storageaccount (and its specific container).
        /// </summary>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public async Task DeleteBlob(string blobName)
        {
            var blob = _blobContainer.GetBlobReference(blobName);
            await blob.DeleteIfExistsAsync();
        }
    }
}
