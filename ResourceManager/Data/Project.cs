using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceManager.Data
{
    internal class Project
    {

        public string Name;
        public OpenMethod OpenMethod;

        // Local Folder Data
        public string FolderPath;

        // Server Data
        public string Host;
        public ushort Port;
        public string FTPHost;
        public string FTPUser;
        public string FTPPassword;

        public Project()
        {
            Name = string.Empty;
            OpenMethod = OpenMethod.LocalFolder;
            FolderPath = string.Empty;
            Host = string.Empty;
            Port = ushort.MaxValue;
            FTPHost = string.Empty;
            FTPUser = string.Empty;
            FTPPassword = string.Empty;
        }

    }

    internal enum OpenMethod
    {
        LocalFolder,
        Server
    }

}
