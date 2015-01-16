using System;
using ActionStreetMap.Infrastructure.IO;

namespace Assets.Scripts.Demo
{
    class AndroidPathResolver: IPathResolver
    {
        private const string PathPrefix = "/sdcard/ASM";
        public string Resolve(string path)
        {
            if (path.StartsWith(PathPrefix))
                return path;
            return String.Format("{0}/{1}", PathPrefix, path.Replace(@"\", "/"));
        }
    }
}
