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

    //NM - Rename this to CloudStorageRepository and IStorageRepository
    // I steered you the wrong direction before.  The Repository Pattern (google it) is a standard
    // pattern for interacting with data sources.
    public class StorageProvider : IStorageProvider
    {

        public CloudStorageAccount StorageAccount { get; set; }
        public CloudBlobClient BlobClient { get; set; }
        
    
        public CloudBlobContainer BlobContainer { get; set; }
        
        //NM - In general having a data object as part of a Repository class is bad practice.  If two different parts of the code
        // are interacting with one instance of the class, you could have some strange bugs pop up (one user seeing another user's files, for instance)
        public CloudBlockBlob BlockBlob { get; set; }
        
        //NM - A parameterless constructor shouldn't set the state of the object.  For example, there needs to be a way for 
        // a calling method to get a new instance of the object without creating a blob container.
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
            //NM - if you add the statement 'using System.IO' to the top of the code file, you won't have to use it here
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
                //NM - make sure this is uncommented
                //file already exists on cloud
                //throw (new Exception("File: "+fileName +" already exists in the container: "+BlobContainer.Name));
            }

        }

        //NM - If you remove the BlobBlob property, this isn't needed
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
                //NM - When you are making comments, make sure they are expressive.  It will be common for 
                // people who do not know English well to be reading your code and making modifications.
                // They might not understand what 'file dne' means.
                //file dne
                //throw (new Exception("File: " + fileName + " does not exist in the directory " + BlobContainer.Uri));
            }


        }

        public bool FileExists(string fileName)
        {
            //NM - It's good practice to 1) assign large expressions to a variable - the name of the variable can explain what 
            // you are trying to do in the code, and may remove the need for a comment.  2) The 0 should go after the expression, to
            // improve readbility.  3) You can use one return statement since this is already a bool.
            //
            // Here's an example:
            // var countOfFileNames = BlobContainer.ListBlobs().OfType<CloudBlockBlob>().Where(e => e.Name.Equals(fileName)).Count();
            // return countOfFileNames > 0;
            
            //NM - Also, make sure that this isn't returning a full list of Blobs over the network.  The Where clause
            // may be against local memory, instead of being passed onto Azure.
            if (0 < BlobContainer.ListBlobs().OfType<CloudBlockBlob>().Where(e => e.Name.Equals(fileName)).Count())
                return true;
            else
                return false;
        }
    }
}
