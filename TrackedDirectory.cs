using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace version_control_system
{
    class TrackedDirectory
    {
        public readonly string fullPath;
        public readonly string path;
        readonly string dataFile;
        readonly string pathFile;

        public List<TrackedFile> files = new List<TrackedFile>();

        public void Read()
        {
            DirectoryInfo dir = new DirectoryInfo(fullPath);
            foreach (FileInfo fileInDir in dir.GetFiles())
            {
                files.Add(new TrackedFile(fileInDir.FullName));
            }

            StreamReader sr = new StreamReader(dataFile, Encoding.Default);
            while (!sr.EndOfStream)
            {
                string[] words = sr.ReadLine().Split('|');
                TrackedFile fileData = Find(words[0]);
                if (fileData == null)
                {
                    files.Add(new TrackedFile(words[0], words[1], (long)Convert.ToDouble(words[2]), words[3], words[4]));
                }
                else
                {
                    fileData.prevLength = Int32.Parse(words[2]);
                    if (words[5] == "Inited")
                    {
                        fileData.mark = Mark.Inited;
                    }
                    else
                    {
                        fileData.mark = Mark.Added;
                    }
                }
            }
            sr.Close();
        }

        public void Update()
        {
            DirectoryInfo dir = new DirectoryInfo(fullPath);
            foreach (FileInfo fileInDir in dir.GetFiles())
            {
                TrackedFile fileData = Find(fileInDir.FullName);
                if (fileData == null)
                {
                    files.Add(new TrackedFile(fileInDir.FullName));
                }
                else
                {
                    if (fileData.prevLength == -1)
                    {
                        fileData.prevLength = fileData.length;
                    }
                    if (fileData.mark == Mark.Deleted)
                    {
                        fileData.mark = Mark.New;
                    }
                    FileInfo item = new FileInfo(fileInDir.FullName);
                    fileData.length = item.Length;
                    fileData.creationTime = item.CreationTime.ToString();
                    fileData.lastWriteTime = item.LastWriteTime.ToString();
                }
            }

            foreach (TrackedFile dataSet in files)
            {
                if (!File.Exists(dataSet.fullName))
                {
                    dataSet.mark = Mark.Deleted;
                }
            }
        }

        public TrackedDirectory(string fullPath)
        {           
            this.fullPath = fullPath;
            DirectoryInfo dir = new DirectoryInfo(fullPath);
            path = dir.Name;
            dataFile = VersionControlSystem.databasePath + path + ".txt";
            pathFile = VersionControlSystem.folderPathsPath + path + ".txt";
            Read();
        }

        public void Status()
        {
            Console.WriteLine("Папка: " + fullPath);
            Console.WriteLine("Файлов: " + files.Count );
            foreach (TrackedFile dataSet in files)
            {
                dataSet.Print();
            }
        }

        public void Save()
        {
            File.WriteAllText(pathFile, fullPath, Encoding.Default);
            File.WriteAllText(dataFile, String.Empty, Encoding.Default);
            foreach (TrackedFile dataSet in files)
            {
                if (dataSet.mark == Mark.Added || dataSet.mark == Mark.Inited)
                { 
                    dataSet.Save(dataFile);
                }
            }
        }

        public void Apply()
        {
            foreach (TrackedFile dataSet in files)
            {
                if (dataSet.mark == Mark.Added)
                {
                    dataSet.mark = Mark.Inited;
                    dataSet.Save(dataFile);
                }
            }
            Save();
            files = new List<TrackedFile>();
            Read();
        }

        public TrackedFile Find(string fullName)
        {
            foreach (TrackedFile dataSet in files)
            {
                if (dataSet.fullName == fullName)
                {
                    return (dataSet);
                }               
            }
            return null;
        }

        public TrackedFile FindAndReturn(string name)
        {
            foreach (TrackedFile dataSet in files)
            {
                if (dataSet.name == name)
                {
                    return (dataSet);
                }
            }
            return null;
        }

        public bool FindAndChangeMark(string name, Mark newMark)
        {
            foreach (TrackedFile dataSet in files)
            {
                if (dataSet.name == name)
                {
                    dataSet.mark = newMark;
                    return (true);
                }
            }
            return (false);
        }
    }
}