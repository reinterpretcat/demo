using System;
using System.IO;
using ActionStreetMap.Infrastructure.IO;
using UnityEngine;

namespace Assets.Scripts.Demo
{
    /// <summary>
    ///  This is web file system implementation only for demo web player build:
    ///  it contains hardcoded paths to demo files as web player doesn't support 
    ///  reading from disk
    /// </summary>
    public class WebFileSystemService : IFileSystemService
    {
        private readonly IPathResolver _pathResolver;

        public WebFileSystemService(IPathResolver pathResolver)
        {
            _pathResolver = pathResolver;
        }

        #region
        public Stream ReadStream(string path)
        {
            var file = Resources.Load<TextAsset>(_pathResolver.Resolve(path));
            return new MemoryStream(file.bytes);
        }

        public string ReadText(string path)
        {
            var file = Resources.Load<TextAsset>(_pathResolver.Resolve(path));
            return file.text;
        }

        public byte[] ReadBytes(string path)
        {
            var file = Resources.Load<TextAsset>(_pathResolver.Resolve(path));
            return file.bytes;
        }

        public bool Exists(string path)
        {
            return true;
        }

        public string[] GetFiles(string path, string searchPattern)
        {
            // NOTE hardcode default one so far as we can't list files in web player
            if (searchPattern == "*.list")
                return new[] {@"Maps/osm/berlin/areas.list.txt"};

            return new string[0];
        }

        public string[] GetDirectories(string path, string searchPattern)
        {
            return new string[0];
        }

        #endregion
    }
}
