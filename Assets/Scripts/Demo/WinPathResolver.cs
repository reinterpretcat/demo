using ActionStreetMap.Infrastructure.IO;

namespace Assets.Scripts.Demo
{
    /// <summary>
    ///     Resolves path on Windows OS
    /// </summary>
    public class WinPathResolver : IPathResolver
    {
        public string Resolve(string path)
        {           
            // WINDOWS
            if (path.StartsWith("Config") || path.StartsWith("Maps"))
                path = "Assets//Resources//" + path;

            return path;
        }
    }
}