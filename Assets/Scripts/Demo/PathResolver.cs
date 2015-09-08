using System.IO;
using ActionStreetMap.Infrastructure.IO;

namespace Assets.Scripts.Demo
{
    class PathResolver: IPathResolver
    {
        public string Resolve(string path)
        {
#if UNITY_ANDROID
            if (path.StartsWith(PathPrefix))
                return path;
            return String.Format("{0}/{1}", PathPrefix, path.Replace(@"\", "/"));

#elif UNITY_WEBPLAYER
            var lowerCase = path.ToLower();

            // WEB
            if (lowerCase.EndsWith(".mapcss"))
                path  += ".txt";

            if (lowerCase.EndsWith(".hgt"))
                path += ".bytes";

            return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)).Replace(@"\",@"/");
#else
            // Tested on Windows
            if (path.StartsWith("Config") || path.StartsWith("Maps"))
                path = "Assets//Resources//" + path;

            return path;
#endif
        }
    }
}
