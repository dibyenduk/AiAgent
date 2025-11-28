using System;
using System.IO;

namespace AgentService
{
    internal class FileSystemTools
    {
        public void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void AddFile(string directoryPath, string fileName, string content)
        {
            string filePath = Path.Combine(directoryPath, fileName);
            File.WriteAllText(filePath, content);
        }

        public void MoveFile(string sourceFilePath, string destinationFilePath)
        {
            if (File.Exists(sourceFilePath))
            {
                File.Move(sourceFilePath, destinationFilePath);
            }
        }

        public void RenameFile(string filePath, string newFileName)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            string newFilePath = Path.Combine(directoryPath, newFileName);
            MoveFile(filePath, newFilePath);
        }

        public int CountFilesInDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                return Directory.GetFiles(directoryPath).Length;
            }
            return 0;
        }

        public void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
