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

            folderMonitor.StorageProvider.DeleteBlob("unitTestFile");
            //folderMonitor.StorageProvider.PrintBlobFolder();
            //folderMonitor.Observe();

        }
    }
}
