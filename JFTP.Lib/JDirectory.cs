using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFTP.Lib
{
    public class JDirectory
    {
        public string Path { get; set; }

        public IEnumerable<JDirectory> Directories { get; set; }
        public IEnumerable<JFile> Files { get; set; }

        /// <summary>
        /// Parses a directory structure as a JDirectory object.
        /// </summary>
        /// <param name="path">The top level directory to be parsed.</param>
        public JDirectory(string path)
            : this(path, false)
        { }

        /// <summary>
        /// Parses a directory structure as a JDirectory object.
        /// </summary>
        /// <param name="path">The top level directory to be parsed.</param>
        /// <param name="recurse">Whether or not to descend into child directories.</param>
        public JDirectory(string path, bool recurse)
        {
            // TODO:
            /* Currently, the recurse flag doesn't do a damn thing to stop this routine 
             * from descending into child directories. I should probably fix that at some 
             * point.
             * 
             * That said, it is entirely possible that this whole process could take place
             * instantaneously: EnumerateDirectories() streams its output, so this is all 
             * deferred. That is, unfortunately, not true when you go to enumerating the 
             * file tokens associated with every one of these files.
             * */

            Path = path;
            Directories = Directory.EnumerateDirectories(path).Select(dir => new JDirectory(dir));
            Files = Directory.EnumerateFiles(path).Select(file => new JFile(file));
        }

        public IEnumerable<JFile> EnumerateFiles()
        {
            foreach (var file in Files)
                yield return file;

            foreach (var directory in Directories)
                foreach (var file in directory.EnumerateFiles())
                    yield return file;
        }

        public Dictionary<string, string> AsLibrary()
        {
            return EnumerateFiles().ToDictionary(
                file => file.Token,
                file => file.Path);
        }
    }
}