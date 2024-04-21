using System;
using System.Reflection;

namespace Supabase.Core
{
    /// <summary>
    /// A shared utilities class
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Returns the Current Assembly version - this is usually appended into the headers of each request.
        /// </summary>
        /// <param name="clientType"></param>
        /// <returns></returns>
        public static string GetAssemblyVersion(Type clientType)
        {
            var assembly = clientType.Assembly;
            var informationVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            var name = assembly.GetName().Name;

            return $"{name.ToString().ToLower()}-csharp/{informationVersion}";
        }
    }
}
