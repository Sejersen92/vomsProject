﻿using Azure.Storage.Blobs;
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
        private string _connectionString;
        BlobServiceClient _blobServiceClient;
        private string _containerName;

        public StorageHelper(string connectionString, string containerName)
        {
            _connectionString = connectionString;
            _blobServiceClient = new BlobServiceClient(_connectionString);
            _containerName = !string.IsNullOrWhiteSpace(containerName) ? containerName : "voms";
        }

        public BlobContainerClient GetBlobContainer()
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            return containerClient;
        }

        public async Task<bool> UploadToBlob(FileStream file, ApplicationDbContext dbContext, Page Page)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
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
                Console.WriteLine("(UploadtoBlob-method) failed with following exception: " + e);
                return false;
            }
        }

        public string GetImageUrl (string name, ApplicationDbContext dbContext)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            
            try
            {
                var image = dbContext.Images.FirstOrDefault(x => x.ImageUrl == name);
                if (image != null)
                {
                    var imageUrl = image.ImageUrl;

                    BlobClient blobClient = containerClient.GetBlobClient(imageUrl);
                    var imageUri = blobClient.Uri;

                    return imageUri.ToString();
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("(GetImageUrl-method) failed with following exception: " + e);
                return null;
            }
        }
    }
}
