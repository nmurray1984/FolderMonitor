using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ConsoleApplication1
{
    public interface IStorageProvider
    {
        //CloudStorageAccount StorageAccount { get; set; }
        //CloudBlobClient BlobClient { get; set; }
        //CloudBlobContainer BlobContainer { get; set; }
        //CloudBlockBlob BlockBlob { get; set; }

        void Initialize();
        void CreateBlob(string filepath, string filename);
        void DeleteBlob();
        void DeleteBlob(string name);
    }
}
