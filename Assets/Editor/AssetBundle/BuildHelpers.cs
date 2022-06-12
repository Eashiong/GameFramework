// ref: https://github.com/Saritasa/Unity3DTemplateProject
using System.IO;
using System;
using UnityEngine;
using System.Text;

namespace Utils
{
    /// <summary>
    /// This class contains Helper methods for other BuildProject class
    /// </summary>
    public static partial class BuildHelpers
    {
        /// <summary>
        /// Syntax sugar for Path.Combine
        /// </summary>
        /// <param name="path"></param>
        /// <param name="otherPath"></param>
        /// <returns></returns>
        public static string CombineAsPath(this string path, string otherPath)
        {
            return Path.Combine(path, otherPath);
        }

        /// <summary>
        /// Copy directory contents to other directory
        /// </summary>
        /// <param name="fromDirectory"></param>
        /// <param name="toDirectory"></param>
        public static void CopyDirectoryContents(string fromDirectory, string toDirectory)
        {
            var directoryInfo = new DirectoryInfo(fromDirectory);
            foreach (var file in directoryInfo.GetFiles())
            {
                File.Copy(file.FullName, toDirectory.CombineAsPath(file.Name));
            }

            foreach (var directory in directoryInfo.GetDirectories())
            {
                CopyDirectory(directory.FullName, toDirectory.CombineAsPath(directory.Name));
            }
        }

        /// <summary>
        /// Recursively copy one directory to other
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="targetDirectory"></param>
        public static void CopyDirectory(string sourceDirectory, string targetDirectory)
        {
            CopyDirectory(new DirectoryInfo(sourceDirectory), new DirectoryInfo(targetDirectory));
        }

        /// <summary>
        /// Recursively copy one directory to other
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void CopyDirectory(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirectory(diSourceSubDir, nextTargetSubDir);
            }
        }

        /// <summary>
        /// Recursively remove all directory content
        /// </summary>
        /// <param name="path"></param>
        public static void RemoveDirectoryContent(string path)
        {
            var directoryInfo = new DirectoryInfo(path);

            foreach (var file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (var directory in directoryInfo.GetDirectories())
            {
                DeleteDirectory(directory.FullName);
            }
        }

        /// <summary>
        /// Depth-first recursive delete, with handling for descendant 
        /// directories open in Windows Explorer.
        /// </summary>
        public static void DeleteDirectory(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }

        /// <summary>
        /// Handle compilation error message
        /// </summary>
        /// <param name="message"></param>
        public static void HandleCompilationError(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;
            UnityEngine.Debug.unityLogger.Log(message.Contains(" : warning ") ? LogType.Warning : LogType.Error, message);
        }

        /// <summary>
        /// Handles output of started process
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logType"></param>
        public static void HandleProcessOutput(string message, LogType logType)
        {
            if (string.IsNullOrEmpty(message))
                return;
            UnityEngine.Debug.unityLogger.Log(logType, message);
        }

        /// <summary>
        /// Appends sting as new line to accumulator if it is not empty
        /// </summary>
        /// <param name="accumulator"></param>
        /// <param name="message"></param>
        public static void AppendLine(StringBuilder accumulator, string message)
        {
            if (string.IsNullOrEmpty(message))
                return;
            accumulator.AppendLine(message);
        }

        /// <summary>
        /// Copy files from one folder to another by wildcard
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destinationFolder"></param>
        /// <param name="wildcard"></param>
        public static void CopyByWildcard(string sourceFolder, string destinationFolder, string wildcard)
        {
            foreach (var file in Directory.GetFiles(sourceFolder, wildcard))
            {
                File.Copy(file, destinationFolder.CombineAsPath(Path.GetFileName(file)));
            }
        }

        /// <summary>
        /// Get path to solution
        /// </summary>
        /// <returns></returns>
        public static string GetSolutionPath()
        {
            return Application.dataPath.CombineAsPath("..");
        }
    }
}
