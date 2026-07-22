using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supabase.Core.Diagnostics;

namespace CoreTests.Diagnostics
{
    [TestClass]
    public class InstrumentationTests
    {
        [TestMethod]
        public void GetVersion_ReturnsTheAssemblyVersion() =>
            Instrumentation.GetVersion(typeof(Instrumentation).Assembly).Should().MatchRegex(@"^\d+\.\d+\.\d+");

        [TestMethod]
        public void GetVersion_StripsBuildMetadata() =>
            Instrumentation.GetVersion(typeof(Instrumentation).Assembly).Should().NotContain("+");

        [TestMethod]
        public void CreateActivitySource_UsesTheGivenNameAndAssemblyVersion()
        {
            using var source = Instrumentation.CreateActivitySource(typeof(Instrumentation).Assembly, "Supabase.Test");
            source.Name.Should().Be("Supabase.Test");
            source.Version.Should().Be(Instrumentation.GetVersion(typeof(Instrumentation).Assembly));
        }

        [TestMethod]
        public void CreateMeter_UsesTheGivenNameAndAssemblyVersion()
        {
            using var meter = Instrumentation.CreateMeter(typeof(Instrumentation).Assembly, "Supabase.Test");
            meter.Name.Should().Be("Supabase.Test");
            meter.Version.Should().Be(Instrumentation.GetVersion(typeof(Instrumentation).Assembly));
        }
    }
}
