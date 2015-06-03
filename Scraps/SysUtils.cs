using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace Scraps
{
    public static class SysUtils
    {
        public static void DeleteFileIfExists(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        public static void DeleteDirectoryIfExists(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (Directory.Exists(path))
            {
                //Debug.Print("Deleting directory {0}", path);
                Directory.Delete(path, true);
            }
        }

        public static void CopyFileToFolder(string fileName, string outputPath)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");
            File.Copy(fileName, Path.Combine(outputPath, Path.GetFileName(fileName)), true);
        }

        public static void EnsureDirectoryExists(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Will set the environment variable, but only if the value is different
        /// because SetEnvironmentVariable seems to be slow.
        /// </summary>
        public static void EnsureEnvironmentVariableExist(string variableName, string value)
        {
            if (variableName == null) throw new ArgumentNullException("variableName");
            if (value != Environment.GetEnvironmentVariable(variableName))
            {
                Environment.SetEnvironmentVariable(variableName, value, EnvironmentVariableTarget.User);
            }
        }

        public static string RelativePath(string absolutePath, string relativeTo)
        {
            if (absolutePath == null) throw new ArgumentNullException("absolutePath");
            if (relativeTo == null) throw new ArgumentNullException("relativeTo");
            var absoluteDirectories = absolutePath.Split('\\');
            var relativeDirectories = relativeTo.Split('\\');
            //Get the shortest of the two paths       
            int length = absoluteDirectories.Length < relativeDirectories.Length
                             ? absoluteDirectories.Length
                             : relativeDirectories.Length;
            //Use to determine where in the loop we exited   
            int lastCommonRoot = -1;
            int index;
            //Find common root         
            for (index = 0; index < length; index++)
                if (absoluteDirectories[index] == relativeDirectories[index])
                    lastCommonRoot = index;
                else
                    break;
            //If we didn't find a common prefix then throw     
            if (lastCommonRoot == -1)
                throw new ArgumentException("Paths do not have a common base");
            //Build up the relative path          
            var relativePath = new StringBuilder();
            //Add on the ..     
            for (index = lastCommonRoot + 1; index < absoluteDirectories.Length; index++)
                if (absoluteDirectories[index].Length > 0)
                    relativePath.Append("..\\");
            //Add on the folders          
            for (index = lastCommonRoot + 1; index < relativeDirectories.Length - 1; index++)
                relativePath.Append(relativeDirectories[index] + "\\");
            relativePath.Append(relativeDirectories[relativeDirectories.Length - 1]);
            return relativePath.ToString();
        }


        public static void AssociateFile(string extension, string application, string description, string icon,
                                         string executablePath)
        {
            using (var keyClassesRoot = Registry.ClassesRoot)
            {
                using (var keyApplication = keyClassesRoot.CreateSubKey(application))
                {
                    if (keyApplication != null)
                    {
                        keyApplication.SetValue(null, description);

                        keyApplication.SetValue("BrowserFlags", 8, RegistryValueKind.DWord);

                        keyApplication.SetValue("EditFlags", 0, RegistryValueKind.DWord);


                        using (var keyShell = keyApplication.CreateSubKey("shell"))
                        {
                            if (keyShell != null)
                            {
                                keyShell.SetValue(null, "open"); // default action


                                using (var keyOpen = keyShell.CreateSubKey("open"))
                                {
                                    if (keyOpen != null)

                                        using (var keyCommand = keyOpen.CreateSubKey("command"))
                                        {
                                            if (keyCommand != null)
                                            {
                                                keyCommand.SetValue(null,
                                                                    String.Format("\"{0}\" \"%1\"", executablePath));
                                            }
                                        }
                                }
                            }
                        }
                    }
                }


                using (var keyExtension = keyClassesRoot.CreateSubKey(extension))
                {
                    if (keyExtension != null)

                        keyExtension.SetValue(null, application);
                } 
            }
        }

        public static void CopyDirectory(string source, string destnation)
        {
            if (string.IsNullOrEmpty(source)) throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(destnation)) throw new ArgumentNullException("destnation");
            var sourceDirectoryInfo = new DirectoryInfo(source);
            var destnationDirectoryInfo = new DirectoryInfo(destnation);

            if (!sourceDirectoryInfo.Exists) throw new DirectoryNotFoundException("Directory does not exists \"" + sourceDirectoryInfo.FullName + "\"");
            destnationDirectoryInfo.Create();
            CopyFilesToDirectory(sourceDirectoryInfo, destnationDirectoryInfo);
        }

        private static void CopyFilesToDirectory(DirectoryInfo sourceDirectoryInfo, DirectoryInfo destnationDirectoryInfo)
        {
            foreach (var sourceFileInfo in sourceDirectoryInfo.GetFiles())
            {
                var destFileName = Path.Combine(destnationDirectoryInfo.FullName, sourceFileInfo.Name);
                //Debug.Print("Copying {0} -> {1}", sourceFileInfo, destFileName);
                sourceFileInfo.CopyTo(destFileName);
            }

            foreach (var subSourceDirectoryInfo in sourceDirectoryInfo.GetDirectories())
            {
                var subDestnationDirectoryInfo = destnationDirectoryInfo.CreateSubdirectory(subSourceDirectoryInfo.Name);
                CopyFilesToDirectory(subSourceDirectoryInfo, subDestnationDirectoryInfo);
            }
        }

        public static string GetFolderNameFromStackTrace(string format = "{0}-{1}")
        {
            var stackTrace = new StackTrace();
            var stackFrame = stackTrace.GetFrame(1);
            var method = stackFrame.GetMethod();
            var declaringType = method.DeclaringType;
            if (declaringType == null) throw new NullReferenceException("Unable to get DeclaringType from " + method);
            var folder =string.Format(format,
                declaringType.Name, 
                method.Name);
            return folder.ToLower();
            //return folder;
        }

        public static void ThrowIfFileNotExists(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException(string.Format(
                    "File \"{0}\" not found.", fileName), fileName);
        }
    }
}