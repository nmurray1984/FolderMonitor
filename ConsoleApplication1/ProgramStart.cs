using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class ProgramStart
    {
        static void Main(string[] args)
        {
            FolderMonitor folderMonitor = new FolderMonitor("C://Users//dylan.parmley//Desktop//FindThis", new StorageProvider());
            //folderMonitor.StorageProvider.PrintBlobFolder();

            //folderMonitor.StorageProvider.DeleteBlob("unitTestFile");
            //folderMonitor.StorageProvider.PrintBlobFolder();
            folderMonitor.Observe();

            //folderMonitor.StorageProvider.
            //createTestFile(".pdf");
            //deleteTestFile(".pdf");


        }

        public static void createTestFile(string fileType)
        {
            string pathString = "C://Users//dylan.parmley//Desktop//FindThis";
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

        public static void deleteTestFile(string fileType)
        {
            string pathString = "C://Users//dylan.parmley//Desktop//FindThis";
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
