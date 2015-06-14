using System;
using System.IO;
using ActionStreetMap.Infrastructure.Dependencies;
using ActionStreetMap.Infrastructure.Diagnostic;
using ActionStreetMap.Infrastructure.IO;
using ActionStreetMap.Unity.IO;

namespace Assets.Scripts.Demo
{
    internal class WebFileSystemService: FileSystemService
    {
        [Dependency]
        public WebFileSystemService(IPathResolver pathResolver, ITrace trace) : base(pathResolver, trace)
        {
        }

        public override string[] GetDirectories(string path, string searchPattern)
        {
            return new string[0];
        }

        public override string[] GetFiles(string path, string searchPattern)
        {
            return new string[]
            {
                PathResolver.Resolve(String.Format("{0}{1}moscow{1}header.txt", path, Path.DirectorySeparatorChar))
            };
        }

        public override void CreateDirectory(string path)
        {
            Trace.Warn(LogTag, "Skip create directory for {0}", path);
        }
    }
}
