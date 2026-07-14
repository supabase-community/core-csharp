using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Supabase.Core
{
    /// <summary>
    /// Shared utilities for Supabase client libraries.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Builds the <c>X-Client-Info</c> header value for the given client type.
        /// </summary>
        /// <remarks>
        /// Format: <c>name-csharp/version[; key=value ...]</c><br/>
        /// Appended metadata includes platform, platform version, runtime, runtime version,
        /// and framework (when detectable).
        /// </remarks>
        /// <param name="clientType">A type belonging to the client assembly, used to resolve the assembly name and version.</param>
        /// <returns>A structured header value identifying the client library and its host environment.</returns>
        public static string GetAssemblyVersion(Type clientType) =>
            $"{GetClientName(clientType)}-csharp/{GetClientVersion(clientType)}{BuildMetadata()}";

        private static string GetClientName(Type clientType) => clientType.Assembly.GetName().Name.ToLower();

        private static string? GetClientVersion(Type clientType) => GetInformationalVersion(clientType.Assembly);

        private static string BuildMetadata() => string.Concat(GetPlatformInfo().ToString(), GetRuntimeInfo().ToString(), GetFrameworkInfo().ToString());

        private sealed class MetadataEntry
        {
            private readonly string key;
            private readonly string value;
            private readonly string? version;

            internal MetadataEntry(string key, string value, string? version = null)
            {
                this.key = key;
                this.value = value;
                this.version = version;
            }

            public override string ToString() => string.IsNullOrEmpty(version)
                ? $"; {key}={value}"
                : $"; {key}={value}; {key}-version={version}";
            
            internal static MetadataEntry Unknown(string key) => new MetadataEntry(key, "unknown");
        }

        private static string GetPlatform()
        {
            if (RuntimeInformation.OSDescription == "Browser") return "browser";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return "Windows";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return "macOS";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return "Linux";
            return "unknown";
        }

        private static MetadataEntry GetPlatformInfo() => new MetadataEntry("platform", GetPlatform(), Environment.OSVersion.Version.ToString());

        private static MetadataEntry GetRuntimeInfo() => new MetadataEntry("runtime", "dotnet", Environment.Version.ToString());

        // Priority is explicit: MAUI wins over Blazor in hybrid apps where both assemblies are present.
        // Unity version uses GetCustomAttributesData() rather than member reflection — safe under IL2CPP.
        private static MetadataEntry GetFrameworkInfo()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .GroupBy(a => a.GetName().Name)
                .ToDictionary(g => g.Key, g => g.First());

            if (assemblies.TryGetValue("Microsoft.Maui", out var maui))
                return new MetadataEntry("framework", "maui", GetInformationalVersion(maui));
            if (assemblies.ContainsKey("UnityEngine.CoreModule"))
                return new MetadataEntry("framework", "unity", GetUnityVersion());
            if (assemblies.TryGetValue("Microsoft.AspNetCore.Components", out var blazor))
                return new MetadataEntry("framework", "blazor", GetInformationalVersion(blazor));
            return MetadataEntry.Unknown("framework");
        }

        private static string? GetInformationalVersion(Assembly assembly) =>
            assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        private static CustomAttributeData[] SafeGetCustomAttributesData(Assembly assembly)
        {
            try { return assembly.GetCustomAttributesData().ToArray(); }
            catch { return Array.Empty<CustomAttributeData>(); }
        }

        private static string? GetUnityVersion()
        {
            var attr = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(SafeGetCustomAttributesData)
                .FirstOrDefault(d => d.AttributeType.Name == "UnityAPICompatibilityVersionAttribute");
            return attr?.ConstructorArguments.Count > 0 ? attr.ConstructorArguments[0].Value as string : null;
        }
    }
}
