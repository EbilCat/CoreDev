using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CoreDev.Utils
{
    public class FileUtils
    {
        /// <summary>
        /// Sets the provided path into the actual path of the directory in the filesystem
        /// </summary>
        /// <param name="dirPath">Sets to full path to the file if one can be found</param>
        /// <returns>True if the directory path was successfully found and set</returns>
        public static bool SetActualDirectoryPath(ref string dirPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            if (dirInfo.Exists)
            {
                dirPath = dirInfo.FullName;
            }
            return dirInfo.Exists;
        }

        /// <summary>
        /// Searches up several folder levels to find a file.
        /// </summary>
        /// <param name="filePath">Sets to full path to the file if one can be found</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="folderName">Name of folder containing the file</param>
        /// <param name="numOfLevelsToSearch">Number of folder levels to search</param>
        /// <returns>True if the file is found</returns>
        public static bool GetFullFilePath(out string filePath, string fileName, string folderName = "", int numOfLevelsToSearch = 5)
        {
            bool exists = GetFilePath(out filePath, fileName, folderName, numOfLevelsToSearch, true);
            filePath = string.IsNullOrEmpty(filePath) ? "" : Path.GetFullPath(filePath);
            return exists;
        }

        /// <summary>
        /// Searches up several folder levels to find a file.
        /// </summary>
        /// <param name="filePath">Sets to relative path to the file if one can be found</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="folderName">Name of folder containing the file</param>
        /// <param name="numOfLevelsToSearch">Number of folder levels to search</param>
        /// <returns>True if the file is found</returns>
        public static bool GetRelativeFilePath(out string filePath, string fileName, string folderName = "", int numOfLevelsToSearch = 5)
        {
            return GetFilePath(out filePath, fileName, folderName, numOfLevelsToSearch, false);
        }

        private static bool GetFilePath(out string filePath, string fileName, string folderName, int numOfLevelsToSearch, bool isFullPath)
        {
            string projectFolder = Application.dataPath;
            string folderLevel = string.Empty;

            for (int i = 0; i < numOfLevelsToSearch; i++)
            {
                string fullPathToFile = string.Format("{0}/{1}{2}/{3}", projectFolder, folderLevel, folderName, fileName);
                if (File.Exists(fullPathToFile))
                {
                    string relativePathToFile = string.Format("{0}{1}/{2}", folderLevel, folderName, fileName);
                    if (isFullPath) { filePath = fullPathToFile; }
                    else { filePath = relativePathToFile; }
                    return true;
                }
                folderLevel += "../";
            }
            filePath = string.Empty;
            return false;
        }

        /// <summary>
        /// Searches up several folder levels to find a directory.
        /// </summary>
        /// <param name="directoryPath">Sets to full path to the directory if one can be found</param>
        /// <param name="directoryName">Name of directory</param>
        /// <param name="numOfLevelsToSearch">Number of folder levels to search</param>
        /// <returns>True if the directory is found</returns>
        public static bool GetFullDirectoryPath(out string directoryPath, string directoryName, int numOfLevelsToSearch = 5)
        {
            bool exists = GetDirectoryPath(out directoryPath, directoryName, numOfLevelsToSearch, true);
            directoryPath = string.IsNullOrEmpty(directoryPath) ? "" : Path.GetFullPath(directoryPath);
            return exists;
        }

        /// <summary>
        /// Searches up several folder levels to find a directory.
        /// </summary>
        /// <param name="directoryPath">Sets to relative path to the directory if one can be found</param>
        /// <param name="directoryName">Name of directory</param>
        /// <param name="numOfLevelsToSearch">Number of folder levels to search</param>
        /// <returns>True if the directory is found</returns>
        public static bool GetRelativeDirectoryPath(out string directoryPath, string directoryName, int numOfLevelsToSearch = 5)
        {
            return GetDirectoryPath(out directoryPath, directoryName, numOfLevelsToSearch, false);
        }

        private static bool GetDirectoryPath(out string directoryPath, string directoryName, int numOfLevelsToSearch, bool isFullPath)
        {
            string projectFolder = Application.dataPath;
            string folderLevel = string.Empty;

            for (int i = 0; i < numOfLevelsToSearch; i++)
            {
                string fullPathToFile = string.Format("{0}/{1}{2}", projectFolder, folderLevel, directoryName);
                if (Directory.Exists(fullPathToFile))
                {
                    string relativePathToDirectory = string.Format("{0}{1}", folderLevel, directoryName);
                    if (isFullPath) { directoryPath = fullPathToFile; }
                    else { directoryPath = relativePathToDirectory; }
                    return true;
                }
                folderLevel += "../";
            }
            directoryPath = string.Empty;
            return false;
        }

        public static T GetFirstAssetInstance<T>() where T : UnityEngine.Object
        {
            T instance = default(T);
            int count = 0;
#if UNITY_EDITOR
            // If we're running the game in the editor, the "Preload Assets" array will be ignored.
            // So get all the Constant files using AssetDatabase
            string[] objsGUID = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name);
            count = objsGUID.Length;
            if (count > 0)
            {
                instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(UnityEditor.AssetDatabase.GUIDToAssetPath(objsGUID[0]));
            }
#else
            // Get all asset of type T from Resources or loaded assets
            T[] instances = Resources.FindObjectsOfTypeAll<T>();
            count = instances.Length;
            if(count > 0) {
                instance = instances[0];
            }

#endif
            // No asset of type T was found
            if (count == 0) { Debug.LogWarning(string.Format("{0}: No asset of type \"{1}\" found in loaded resources. Create a new one and add it to the \"Preloaded Assets\" array in Edit > Project Settings > Player > Other Settings.", typeof(FileUtils), typeof(T).Name)); }

            // More than one asset of type T was found
            if (count > 1) { Debug.LogWarning(string.Format("{0}: More than one asset of type \"{1}\" found in loaded resources. If it was intended to be a Singleton, remove other assets of that type from this project.", typeof(FileUtils), typeof(T).Name)); }

            return instance;
        }
    }

    public static class FileUtilsExtensions
    {
        public static bool Empty(this string directoryPath)
        {
            bool success = directoryPath.DeleteFiles();
            success &= directoryPath.DeleteFolders();
            return success;
        }

        public static bool DeleteFiles(this string directoryPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
            if (dirInfo.Exists)
            {
                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    file.Delete();
                }
                return true;
            }
            return false;
        }

        public static bool DeleteFolders(this string directoryPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
            if (dirInfo.Exists)
            {
                foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                {
                    dir.Delete(true);
                }
                return true;
            }
            return false;
        }

        public static void DirectoryCopy(this string sourceDirectoryPath, string destDirectoryPath, bool copySubDirs)
        {
            DirectoryInfo source = new DirectoryInfo(sourceDirectoryPath);
            if (!source.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirectoryPath);
            }

            DirectoryInfo destination = Directory.Exists(destDirectoryPath) ? new DirectoryInfo(destDirectoryPath) : Directory.CreateDirectory(destDirectoryPath);

            foreach (FileInfo file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(destination.FullName, file.Name));
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subDir in source.GetDirectories())
                {
                    DirectoryCopy(subDir.FullName, destination.CreateSubdirectory(subDir.Name).FullName, copySubDirs);
                }
            }
        }
    }
}
