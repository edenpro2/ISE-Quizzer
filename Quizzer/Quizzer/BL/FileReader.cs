#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QuizApp.BL
{
    public static class FileReader
    {
        private static readonly string ProjectDirectory;

        public enum PathOption
        {
            CreateDirectory,
            SearchOnly
        }

        static FileReader()
        {
            var baseDir = @"..\..";
            string projectDirectory;

            do
            {
                projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, baseDir));
                baseDir += @"\..";
            } while (!projectDirectory.EndsWith("ISE-Quizzer"));

            ProjectDirectory = projectDirectory;
        }

        public static string? GetFilePath(string filename, List<string>? extensions = null)
        {
            var projectPath = ProjectDirectory;

            if (extensions == null)
            {
                var nameOnly = System.IO.Path.GetFileNameWithoutExtension(filename);
                // extract extension from filename
                extensions = new List<string> { filename[nameOnly.Length..] };
            }

            return
                Directory
                    .GetFiles(projectPath, "*.*", SearchOption.AllDirectories)
                    .Where(f => extensions.IndexOf(Path.GetExtension(f)) >= 0)
                    .FirstOrDefault(f => f.Contains(Path.GetFileNameWithoutExtension(filename)));
        }

        /// <summary>
        /// Return full path of folder (folder parameter must start without \\)
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="pathOption"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static string GetFolderPath(string folder, PathOption pathOption = PathOption.SearchOnly, SearchOption searchOption = SearchOption.AllDirectories)
        {
            var dir = Directory
                .GetDirectories(ProjectDirectory, "*", searchOption)
                .FirstOrDefault(sub => sub.EndsWith(folder));

            if (dir == null)
            {
                dir = ProjectDirectory + $"\\{folder}";
                Directory.CreateDirectory(dir);
            }

            return dir;
        }

        public static IList<string?> GetFileNames(string directory, List<string> extensions, SearchOption option)
        {
            var dir = GetFolderPath(directory);

            return Directory
                .GetFiles(dir, "*.*", option)
                .Where(f => extensions.IndexOf(Path.GetExtension(f)) >= 0)
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();
        }

        public static IEnumerable<string> LoadTxt(string filename)
        {
            var path = GetFilePath(filename, new List<string> { ".txt" });
            return path == null ? new List<string>() : File.ReadAllLines(path).ToList();
        }
    }
}
