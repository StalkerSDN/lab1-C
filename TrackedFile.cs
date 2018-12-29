using System;
using System.IO;
using System.Text;

namespace version_control_system
{
    class TrackedFile
    {
        public readonly string fullName;
        public readonly string name;
        public long length;
        public long prevLength;
        public string creationTime;
        public string lastWriteTime;

        public Mark mark;

        private static string border = "|";

        public TrackedFile(string fullName)
        {
            this.fullName = fullName;
            this.mark = Mark.New;
            FileInfo item = new FileInfo(fullName);
            name = item.Name;
            length = item.Length;
            creationTime = item.CreationTime.ToString();
            lastWriteTime = item.LastWriteTime.ToString();
            prevLength = -1; ;
        }

        public TrackedFile(string fullName, string name, long length, string creationTime,string lastWriteTime)
        {
            this.fullName = fullName;
            this.name = name;
            this.length = length;
            this.creationTime = creationTime;
            this.lastWriteTime = lastWriteTime;
            mark = Mark.Deleted;
            prevLength = -1;
        }

        public bool Change()
        {
            if (mark == Mark.New || mark == Mark.Removed || mark == Mark.Deleted)
            {
                return (false);
            }

            if (prevLength == -1)
            {
                return (false);
            }

            if (length == prevLength)
            {
                return (false);
            }
            return (true);
        }

        public void Print()
        {
            if (mark == Mark.Deleted || mark == Mark.Removed || ((mark == Mark.Added || mark == Mark.Inited) && Change()))
            {
                Style.FileChangedColor();
            }
            else
            {
                Style.FileNotChangedColor();
            }
            Console.WriteLine();
            Console.Write("Файл: " + name);
            if (!Change() && mark != Mark.Inited)
            {
                Console.Write(" <<-- " + mark + "\n");               
            }
            else
            {
                Console.WriteLine();
            }

            Console.Write("      Размер: ");
            if (Change())
            {
                Console.Write(length + " байт <<-- " + prevLength + " байт");
            }
            else
            {
                Console.Write(length + " байт");
            }
            Console.WriteLine("\n      Создан: " + creationTime);
            Console.WriteLine("      Изменён: " + creationTime);
            Style.Default();
        }

        public void Save(string path)
        {
            File.AppendAllText(path, fullName + border + name + border + length + border + creationTime + border + lastWriteTime + border + mark +  Environment.NewLine, Encoding.Default);
        }
    }
    enum Mark
    {
        Inited,   // inited – файл добавлен с помощью команды init;
        Added,   // added – файл добавлен с помощью команды add;
        Removed, // removed – файл был убран из под версионного контроля командой remove;
        Deleted, // deleted – файл был либо удален, либо перемещен из папки, которая находится под версионным контролем;
        New,     // new – файл, который был либо создан, либо перемещен в папку, которая находится под версионным контролем.
    }
}