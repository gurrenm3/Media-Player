using System.IO;
using System.Reflection;

namespace Media_Player.Extensions
{
    public static class AssemblyExt
    {
        public static string GetDirectory(this Assembly assembly)
        {
            return new FileInfo(assembly.Location).Directory.FullName;
        }
    }
}
