using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Security.Permissions;

namespace ConsoleApplication1
{

    public class StorageProvider : IStorageProvider
    {

        public CloudStorageAccount StorageAccount { get; set; }
        public CloudBlobClient BlobClient { get; set; }
        public CloudBlobContainer BlobContainer { get; set; }
        public CloudBlockBlob BlockBlob { get; set; }

        public StorageProvider()
        {
            Initialize();
        }

        public void Initialize()
        {
            StorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            BlobClient = StorageAccount.CreateCloudBlobClient();
            //TODO: Change to appropriate container name
            BlobContainer = BlobClient.GetContainerReference("mycontainer");
            //BlobContainer = BlobClient.GetContainerReference("pdflobs");
            BlobContainer.CreateIfNotExists();
        }

        //naming conventions? 
        public void CreateBlob(string filePath, string fileName)
        {
            string fullFilePath = System.IO.Path.Combine(filePath, fileName);
            BlockBlob = BlobContainer.GetBlockBlobReference(fileName);


            if (!FileExists(fileName))
            {
                using (var fileStream = System.IO.File.OpenRead(fullFilePath))
                {
                    BlockBlob.UploadFromStream(fileStream);
                }
            }
            else
            {
                //file already exists on cloud
                //throw (new Exception("File: "+fileName +" already exists in the container: "+BlobContainer.Name));
            }

        }

        public void DeleteBlob()
        {
            DeleteBlob(BlockBlob.Name);
        }

        public void DeleteBlob(string fileName)
        {
            if (FileExists(fileName))
            {
                BlockBlob = BlobContainer.GetBlockBlobReference(fileName);
                BlockBlob.Delete();
            }
            else
            {
                //file dne
                //throw (new Exception("File: " + fileName + " does not exist in the directory " + BlobContainer.Uri));
            }


        }

        public bool FileExists(string fileName)
        {
            if (0 < BlobContainer.ListBlobs().OfType<CloudBlockBlob>().Where(e => e.Name.Equals(fileName)).Count())
                return true;
            else
                return false;
        }
    }
}
