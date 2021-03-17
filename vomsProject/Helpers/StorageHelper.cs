using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<bool> UploadToBlob(FileStream file)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            string uniqueIdentifier = Guid.NewGuid().ToString();

            try
            {
                await containerClient.UploadBlobAsync(uniqueIdentifier, file);
                //Create ImageTable
                // - Id(PK), Name, Url(FK to solution), AltText, possible link to pages which uses this element.

                //Add to database /textstring (UniqueIdentifier,www.urlimage.com)
                //--getImage(Id) - Returns string(Url). 

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
