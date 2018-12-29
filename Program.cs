using System;

namespace version_control_system
{
    class Program
    {
        static void Main(string[] args)
        {
            VersionControlSystem.Read();
            while (true)
            {
                Console.Write("\nДля просмотра списка всех комманд введите help");
                Console.Write("\nВведите команду: ");
                string input = Console.ReadLine();
                Console.WriteLine();
                input = input.TrimStart(' ');
                input = input.TrimEnd(' ');
                VersionControlSystem.ExecuteCommand(input);
            }
        }
    }
}
