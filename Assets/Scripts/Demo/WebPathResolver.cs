using System;
using System.IO;
using ActionStreetMap.Infrastructure.IO;

namespace Assets.Scripts.Demo
{
    public class WebPathResolver: IPathResolver
    {
        public string Resolve(string path)
        {
            // WEB
            if (path.EndsWith(".hgt") || path.EndsWith(".pbf") || path.EndsWith(".mapcss"))
                 return path.Replace(@"\", @"/");
             return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)).Replace(@"\",@"/");
        }
    }
}
