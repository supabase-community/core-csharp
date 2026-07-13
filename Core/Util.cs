using System;
using System.Collections.Generic;
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

        private static string GetClientName(Type clientType) =>
            clientType.Assembly.GetName().Name!.ToLower();

        private static string GetClientVersion(Type clientType) =>
            clientType.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;

        private static string BuildMetadata() => string.Concat(GetPlatformInfo(), GetRuntimeInfo(), GetFrameworkInfo());

        private static string? GetPlatform()
        {
            if (RuntimeInformation.OSDescription == "Browser") return "browser";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return "Windows";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return "macOS";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return "Linux";
            return null;
        }

        private static string GetPlatformInfo()
        {
            var platform = GetPlatform();
            if (platform == null) return string.Empty;
            var platformVersion = Environment.OSVersion.Version.ToString();
            return string.IsNullOrEmpty(platformVersion)
                ? $"; platform={platform}"
                : $"; platform={platform}; platform-version={platformVersion}";
        }

        private static string GetRuntimeInfo()
        {
            var runtimeVersion = Environment.Version.ToString();
            return string.IsNullOrEmpty(runtimeVersion)
                ? "; runtime=dotnet"
                : $"; runtime=dotnet; runtime-version={runtimeVersion}";
        }

        // Scans loaded assemblies to detect the host framework. Priority is explicit: MAUI wins over Blazor
        // in hybrid apps where both assemblies are present. Returns null for plain console/desktop apps.
        private static string? GetFramework()
        {
            var names = new HashSet<string?>(GetAssemblies());
            if (names.Contains("Microsoft.Maui")) return "maui";
            if (names.Contains("UnityEngine.CoreModule")) return "unity";
            return names.Contains("Microsoft.AspNetCore.Components") ? "blazor" : null;
        }

        private static IEnumerable<string> GetAssemblies() => 
            AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetName().Name);

        private static string GetFrameworkInfo()
        {
            var framework = GetFramework();
            return framework != null ? $"; framework={framework}" : string.Empty;
        }
    }
}