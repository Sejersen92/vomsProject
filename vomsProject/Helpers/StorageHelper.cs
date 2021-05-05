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

        public async Task<bool> UploadToBlob(FileStream file, ApplicationDbContext dbContext, Page Page)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var _context = dbContext;

            var uniqueIdentifier = Guid.NewGuid().ToString();

            try
            {
                await containerClient.UploadBlobAsync(uniqueIdentifier, file);
                await _context.Images.AddAsync(new Image
                {
                    ImageUrl = uniqueIdentifier,
                    Page = Page,
                    Solution = Page.Solution
                });

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("(UploadToBlob-method) failed with following exception: " + e);
                return false;
            }
        }

        public string GetImageUrl(int pageId, ApplicationDbContext dbContext)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            try
            {
                var image = dbContext.Images.FirstOrDefault(x => x.Page.Id == pageId);
                if (image == null) return null;

                var blobName = image.ImageUrl;

                var blobClient = containerClient.GetBlobClient(blobName);
                var imageUri = blobClient.Uri;

                return imageUri.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("(GetImageUrl-method) failed with following exception: " + e);
                return null;
            }
        }

        public static IEnumerable<Solution> GetSolutions(string userId, ApplicationDbContext dbContext)
        {
            try
            {
                var solutions = dbContext.Solutions.Include(x => x.Users).Where(x => x.Users.FirstOrDefault().Id == userId).ToList();
                return solutions.Any() ? solutions : null;
            }
            catch (Exception e)
            {
                Console.WriteLine("(GetSolutions) caused an error: " + e);
                return null;
            }

        }
    }
}
