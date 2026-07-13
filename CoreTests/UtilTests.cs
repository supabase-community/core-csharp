using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supabase.Core;

namespace CoreTests
{
    [TestClass]
    public class UtilTests
    {
        private static string Result => Util.GetAssemblyVersion(typeof(Util));

        [TestMethod]
        public void GetAssemblyVersion_BaseFormatIsCorrect()
        {
            Result.Should().MatchRegex(@"^[\w.]+-csharp/[\w.+\-]+");
        }

        [TestMethod]
        public void GetAssemblyVersion_ShouldContainRuntime()
        {
            Result.Should().Contain("; runtime=dotnet");
        }

        [TestMethod]
        public void GetAssemblyVersion_ShouldContainRuntimeVersion()
        {
            Result.Should().MatchRegex(@"; runtime-version=\S+");
        }

        [TestMethod]
        public void GetAssemblyVersion_ContainsKnownPlatform()
        {
            Result.Should().MatchRegex(@"; platform=(Windows|Linux|macOS|browser)");
        }

        [TestMethod]
        public void GetAssemblyVersion_IncludesPlatformVersion()
        {
            Result.Should().MatchRegex(@"; platform-version=\S+");
        }

        [TestMethod]
        public void GetAssemblyVersion_HasNoEmptyOrUnknownValues()
        {
            Result.Should().NotMatchRegex(@"=\s*;");
            Result.Should().NotMatchRegex(@"=$");
            Result.Should().NotContain("=unknown");
        }

        [TestMethod]
        public void GetAssemblyVersion_DoesNotIncludeFrameworkForPlainDotnet()
        {
            Result.Should().NotContain("; framework=");
        }
    }
}
