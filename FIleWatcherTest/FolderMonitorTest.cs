using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApplication1;
using System.IO;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Linq;
using Moq;

namespace FileWatcherTest
{
    [TestClass]
    public class FolderMonitorTest
    {
        public string filePath = "C://Users//dylan.parmley//Desktop//FindThis";

        [TestMethod]
        public void MOQ_Watcher_Filter_FindsPDF()
        {
            Mock<IStorageProvider> mockStorageProvider = new Mock<IStorageProvider>();
            
            FolderMonitor folderMonitor = new FolderMonitor(filePath, mockStorageProvider.Object);
            folderMonitor.Observe();

            createTestFile(".pdf");
            deleteTestFile(".pdf");

            mockStorageProvider.Verify(t => t.CreateBlob(filePath, "unitTestFile.pdf"));
            //Assert.IsTrue(folderMonitor.Running);

        }

        [TestMethod]
        public void Watcher_Filter_FindsPDF()
        {

            FolderMonitorDebug folderMonitor = new FolderMonitorDebug(filePath, new StorageProvider());
            folderMonitor.Observe();

            createTestFile(".pdf");
            deleteTestFile(".pdf");

            Assert.IsTrue(folderMonitor.Running);

        }

        [TestMethod]
        public void MOQ_Watcher_Filter_IgnoresTxt()
        {
            Mock<IStorageProvider> mockStorageProvider = new Mock<IStorageProvider>();
            FolderMonitor folderMonitor = new FolderMonitor(filePath, mockStorageProvider.Object);
            folderMonitor.Observe();

            createTestFile(".txt");
            deleteTestFile(".txt");

            mockStorageProvider.Verify(t => t.CreateBlob(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            //Assert.IsFalse(folderMonitor.Running);
        }

        [TestMethod]
        public void Watcher_Filter_IgnoresTxt()
        {
            Mock<IStorageProvider> mockStorageProvider = new Mock<IStorageProvider>();
            FolderMonitorDebug folderMonitor = new FolderMonitorDebug(filePath, new StorageProviderDebug());
            folderMonitor.Observe();

            createTestFile(".txt");
            deleteTestFile(".txt");

            
            Assert.IsFalse(folderMonitor.Running);
        }

        [TestMethod]
        public void StorageProvider_CreateBlob_CreatesBlob()
        {

            StorageProvider storageProvider = new StorageProvider();

            //verify the file does not exist in the cloud already

            storageProvider.DeleteBlob("cloudCreatedUnitTest");

            //initial number of blobs in the directory
            int numBlobs = storageProvider.BlobContainer.ListBlobs().OfType<CloudBlockBlob>().Count();

            string fileName = "cloudCreatedUnitTest.txt";

            storageProvider.CreateBlob(filePath, fileName);

            numBlobs -= storageProvider.BlobContainer.ListBlobs().OfType<CloudBlockBlob>().Count();

            storageProvider.DeleteBlob();
            //verify the size has increased by 1
            Assert.AreEqual(numBlobs, -1);


        }

        //verify file size matches
        [TestMethod]
        public void StorageProvider_CreateBlob_BlobContentMatchesPDFContent()
        {
            StorageProvider storageProvider = new StorageProvider();

            //initial number of blobs in the directory
            //string filePath = "C://Users//dylan.parmley//Desktop//FindThis";
            string fileName = "contentMatches.txt";
            string fullPath = System.IO.Path.Combine(filePath, fileName);
            DirectoryInfo directory = new DirectoryInfo(filePath);

            //ensure there is no file that already exists in cloud with the same name

            storageProvider.DeleteBlob("contentMatches");

            storageProvider.CreateBlob(filePath, fileName);

            long fileSize = directory.GetFiles().Single(b => b.Name.Equals(fileName)).Length;
            //long blobSize = storageProvider.BlobContainer.ListBlobs().OfType<CloudBlockBlob>().Single(b => b.Uri.ToString().Equals(fileName) ).Properties.Length;
            long blobSize = storageProvider.BlobContainer.ListBlobs().OfType<CloudBlockBlob>().Single(b => b.Name.Equals(fileName)).Properties.Length;

            storageProvider.DeleteBlob();
            //verify the size has increased by 1
            Assert.AreEqual(blobSize, fileSize);
        }

        [ExpectedException(typeof(System.IO.FileNotFoundException),
            "Parameter index is out of range.")]
        [TestMethod]
        public void StorageProvider_BlobCreation_CatchesFileNotFoundException()
        {
            StorageProvider storageProvider = new StorageProvider();

            //ensure our test file doesnt already exist in the cloud
            storageProvider.DeleteBlob("cloudCreatedUnitTest");

            //initial number of blobs in the directory
            //string pathName = "C://Users//dylan.parmley//Desktop//FindThis";
            string fileName = "aFileThatDoesntExist.txt";

            storageProvider.CreateBlob(filePath, fileName);
        }


        [TestMethod]
        public void StorageProvider_BlobDelete_FilePathArg_CatchesFileNotFoundException()
        {
            StorageProviderDebug storageProvider = new StorageProviderDebug();

            //ensure our test file doesnt already exist in the cloud
            storageProvider.DeleteBlob("aFileThatDoesntExist");

            Assert.IsTrue(storageProvider.DeleteFailed);

        }


        [ExpectedException(typeof(System.NullReferenceException),
            "Parameter index is out of range.")]
        [TestMethod]
        public void StorageProvider_BlobDelete_NoArgs_CatchesNullRefException()
        {
            StorageProviderDebug storageProvider = new StorageProviderDebug();

            //ensure our test file doesnt already exist in the cloud
            storageProvider.DeleteBlob();

            //Assert.IsTrue(storageProvider.DeleteFailed);


        }

        public void createTestFile(string fileType)
        {
            string pathString = filePath;
            string fileName = "unitTestFile" + fileType;

            pathString = System.IO.Path.Combine(pathString, fileName);


            if (!System.IO.File.Exists(pathString))
            {
                using (System.IO.FileStream fs = System.IO.File.Create(pathString))
                {

                }
            }
            else
            {
                Console.WriteLine("File \"{0}\" already exists.", fileName);
                return;
            }

        }

        public void deleteTestFile(string fileType)
        {
            string pathString = filePath;
            string fileName = "unitTestFile" + fileType;
            pathString = System.IO.Path.Combine(pathString, fileName);


            System.IO.FileInfo fi = new System.IO.FileInfo(pathString);
            try
            {
                fi.Delete();
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }




    }
}
