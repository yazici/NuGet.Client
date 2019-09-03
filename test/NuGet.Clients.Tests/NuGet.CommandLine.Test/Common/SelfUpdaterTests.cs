// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Moq;
using NuGet.Configuration;
using NuGet.Test.Utility;
using NuGet.Versioning;
using Xunit;

namespace NuGet.CommandLine.Test
{
    public class SelfUpdaterTests
    {
        [Theory]
        [InlineData("1.1.1", true, false)]
        [InlineData("1.1.1", false, false)]
        [InlineData("1.1.1-beta", true, false)]
        [InlineData("1.1.1-beta", false, false)]
        [InlineData("99.99.99", true, true)]
        [InlineData("99.99.99", false, true)]
        [InlineData("99.99.99-beta", true, true)]
        [InlineData("99.99.99-beta", false, false)]
        public async Task SelfUpdater_WithArbitraryVersions_UpdateSelf(string version, bool prerelease, bool replaced)
        {
            // Arrange
            using (var testDirectory = TestDirectory.Create())
            {
                var tc = new TestContext(testDirectory);

                System.Console.Write(version);

                // Act
                await tc.Target.UpdateSelfAsync(prerelease, NuGetConstants.V3FeedUrl);

                // Assert
                tc.VerifyReplacedState(replaced);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SelfUpdater_WithCurrentVersion(bool prerelease)
        {
            // Arrange
            using (var testDirectory = TestDirectory.Create())
            {
                var tc = new TestContext(testDirectory);
                // TODO make the latest available be current version

                // Act
                await tc.Target.UpdateSelfAsync(prerelease, NuGetConstants.V3FeedUrl);

                // Assert
                tc.VerifyReplacedState(replaced: false);
            }
        }

        private class TestContext
        {
            public TestContext(TestDirectory directory)
            {
                Directory = directory;
                Console = new Mock<IConsole>();
                OriginalContent = new byte[] { 0 };
                NewContent = new byte[] { 1 };

                var clientVersion = typeof(SelfUpdater)
                    .Assembly
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;
                ClientVersion = new SemanticVersion(clientVersion);
                IsClientPrerelease = NuGetVersion
                    .Parse(clientVersion)
                    .IsPrerelease;

                Console = new Mock<IConsole>();
                Target = new SelfUpdater(Console.Object);

                Target.AssemblyLocation = Path.Combine(Directory, "nuget.exe");

                File.WriteAllBytes(Target.AssemblyLocation, OriginalContent);
            }

            public Mock<IConsole> Console { get; }
            public SelfUpdater Target { get; }
            public Mock<IPackage> Package { get; set; }
            public TestDirectory Directory { get; }
            public byte[] NewContent { get; set; }
            public byte[] OriginalContent { get; }
            public SemanticVersion ClientVersion { get; }
            public bool IsClientPrerelease { get; }

            public void VerifyReplacedState(bool replaced)
            {
                Assert.True(File.Exists(Target.AssemblyLocation), "nuget.exe should still exist.");
                var actualContent = File.ReadAllBytes(Target.AssemblyLocation);

                Assert.Equal(replaced ? NewContent : OriginalContent, actualContent);
            }
        }
    }
}
