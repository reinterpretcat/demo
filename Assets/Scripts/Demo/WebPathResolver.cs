using System.IO;
using ActionStreetMap.Infrastructure.IO;

namespace Assets.Scripts.Demo
{
    public class WebPathResolver: IPathResolver
    {
        public string Resolve(string path)
        {
            var lowerCase = path.ToLower();

            // WEB
            if (lowerCase.EndsWith(".mapcss"))
                path  += ".txt";

            if (lowerCase.EndsWith(".hgt"))
                path += ".bytes";

            return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)).Replace(@"\",@"/");
        }
    }
}
