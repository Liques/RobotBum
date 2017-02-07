using System;

namespace RobotBumFoundationCore
{
    
    /// <summary>
    /// Object that refers to airports runways
    /// </summary>
    public static class MultiOSFileSupport
    {

        private static string resourcesFolder;
            public static string ResourcesFolder
            {
                get { 
                    if(!String.IsNullOrEmpty(resourcesFolder))
                        return resourcesFolder;
                   else {
                        return resourcesFolder = "Resources" + Splitter;
                        }
                    }
                set { resourcesFolder = value;}
            }

            private static string splitter;
            public static string Splitter
            {
                get { 
                     if(!String.IsNullOrEmpty(splitter))
                        return splitter;

                     if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                 System.Runtime.InteropServices.OSPlatform.Windows))
                 return splitter = "\\";
                 else
                 return splitter = "/";
                 
                    }
            } 
    

        public static string FolderAddress(string folderAddress) {

             if(System.IO.Directory.Exists(folderAddress))
                return folderAddress;

            string[] fileParts = folderAddress.Split('/');

            string currentPath = String.Empty;
            string splitChar = "/";
           

             if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                 System.Runtime.InteropServices.OSPlatform.Windows))
                 splitChar = "\\";

            foreach(var item in fileParts) {
                if(String.IsNullOrEmpty(item))
                    continue;
                if(!String.IsNullOrEmpty(currentPath) ||
                String.IsNullOrEmpty(currentPath) && System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                System.Runtime.InteropServices.OSPlatform.Linux))
                    currentPath = currentPath + splitChar;

                currentPath = currentPath + item;

                if(!System.IO.Directory.Exists(folderAddress)){
                    System.IO.Directory.CreateDirectory(currentPath);
                }

            }

            currentPath = currentPath + splitChar;

            return currentPath;
        }
        
    }
}
