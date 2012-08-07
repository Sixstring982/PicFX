using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PicFX
{
    class FileBrowser
    {
        private string currentPath = Application.StartupPath;
        public string GetPath()
        {
            return currentPath;
        }
        public string GetDirectory()
        {
            return CurrentDirName(currentPath);
        }

        public string[] GetFolderItems(string relativePath)
        {
            string[] files = null;
            string[] dirs = null;
            try
            {
                files = Directory.GetFiles(currentPath + "\\" + relativePath);
                dirs = Directory.GetDirectories(currentPath + "\\" + relativePath);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            string[] finalStrs = new string[files.Length + dirs.Length];
            for (int i = 0; i < finalStrs.Length; i++)
            {
                if (i >= files.Length)
                    finalStrs[i] = "(" + CurrentDirName(dirs[i - files.Length]) + ")";
                else
                    finalStrs[i] = CurrentDirName(files[i]);
            }
            return finalStrs;
        }

        private string CurrentDirName(string directory)
        {
            if (directory.Length == 0)
                return "";
            int i;
            for (i = directory.Length - 1; i >= 0; i--)
            {
                if (directory[i] == '\\')
                        break;
            }
            return directory.Remove(0, i);
        }

        private string ParentPath(string path)
        {
            int i;
            for (i = path.Length - 1; i >= 0; i--)
            {
                if (path[i] == '\\')
                    break;
            }
            if (i == 0) return path;
            return path.Remove(i);
        }

        public bool ChangeDir(string[] relativePath)
        {
            string finalPath = Util.PutTogether(relativePath);
            if (Directory.Exists(currentPath + "\\" + finalPath))
            {
                if (finalPath == "..")
                    currentPath = ParentPath(currentPath);
                else
                    currentPath += "\\" + finalPath;
                return true;
            }
            Console.WriteLine("[FileBrowser]Directory does not exist.");
            return false;
        }
    }
}
