using System;
using System.IO;
using ActionStreetMap.Infrastructure.IO;
using UnityEngine;

namespace Assets.Scripts.Demo
{
    public class WebPathResolver: IPathResolver
    {
        public string Resolve(string path)
        {
            var lowerCase = path.ToLower();
            //if (lowerCase.StartsWith("config") || lowerCase.StartsWith("maps"))
            //    path = "Assets//Resources//" + path;

            // WEB
            if (lowerCase.EndsWith(".mapcss"))
                path  += ".txt";

           // return path.Replace(@"\", @"/");
            return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)).Replace(@"\",@"/");
        }
    }
}
