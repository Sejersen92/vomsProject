using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
        static readonly string _connectionString = "DefaultEndpointsProtocol=https;AccountName=sejersenstorageaccount;AccountKey=JJ1SzAPaRZ1G+Tu3Sz/3rhsxqeu63pdZCs1nVfB00nLm9agANyf86WKx22e1/zIzTw9DCQBarXJpoxOoxcmRVg==;EndpointSuffix=core.windows.net";
        static BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
        static readonly string containerName = "voms";

        public BlobContainerClient GetBlobContainer()
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            return containerClient;
        }

        public async Task<bool> UploadToBlob(FileStream file, ApplicationDbContext dbContext, Page Page)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var _context = dbContext;

            string uniqueIdentifier = Guid.NewGuid().ToString();

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
                Console.WriteLine("(UploadtoBlob-method) The exception is: " + e);
                return false;
            }
        }
    }
}
