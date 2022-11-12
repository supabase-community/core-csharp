using System;
using System.Reflection;

namespace Supabase.Core
{
    public static class Util
    {
        public static string GetAssemblyVersion(Type clientType)
        {
            var assembly = clientType.Assembly;
            var informationVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            var name = assembly.GetName().Name;

            return $"{name.ToString().ToLower()}-csharp/{informationVersion}";
        }
    }
}
