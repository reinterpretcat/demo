using System;
using ActionStreetMap.Infrastructure.IO;

namespace Assets.Scripts.Demo
{
    class AndroidPathResolver: IPathResolver
    {
        public string Resolve(string path)
        {
            // ANDROID
            return String.Format("/sdcard/Mercraft/{0}", path.Replace(@"\", "/"));
        }
    }
}
