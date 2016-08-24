using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Permissions;


namespace ConsoleApplication1
{
    public class FolderMonitor : IFolderMonitor
    {
        public string FolderName { get; set; }
        public IStorageProvider StorageProvider { get; set; }
        public FileSystemWatcher Watcher { get; set; }
        public bool Running { get; set; }

        public FolderMonitor(string folderName, IStorageProvider storageProvider)
        {
            Initialize(folderName, storageProvider);

        }

        public void Initialize(string folderName, IStorageProvider storageProvider)
        {
            FolderName = folderName;
            StorageProvider = storageProvider;

            //Create a new FileSystemWatcher and set its properties.
            Watcher = new FileSystemWatcher();
            Watcher.Path = folderName;

            Watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName; ;
            // Only watch text files.
            Watcher.Filter = "*.pdf";

            // Add event handlers.
            //Watcher.Changed += new FileSystemEventHandler(OnChanged);
            Watcher.Created += new FileSystemEventHandler(OnChanged);
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Observe()
        {
            // Begin watching.
            Watcher.EnableRaisingEvents = true;
        }


        //create a blob of the file added to the folder we are watching in azure
        public void OnChanged(object source, FileSystemEventArgs e)
        {
            StorageProvider.CreateBlob(FolderName, e.Name);
        }



    }
}
