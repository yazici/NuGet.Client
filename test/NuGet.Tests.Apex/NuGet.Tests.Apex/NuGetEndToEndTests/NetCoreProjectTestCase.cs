using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Apex.VisualStudio.Solution;
using NuGet.StaFact;
using Xunit;
using Xunit.Abstractions;

namespace NuGet.Tests.Apex
{
    public class NetCoreProjectTestCase : SharedVisualStudioHostTestClass, IClassFixture<VisualStudioHostFixtureFactory>
    {
        public NetCoreProjectTestCase(VisualStudioHostFixtureFactory visualStudioHostFixtureFactory, ITestOutputHelper output)
            : base(visualStudioHostFixtureFactory, output)
        {
        }

        // basic create for .net core template
        [NuGetWpfTheory]
        [MemberData(nameof(GetNetCoreTemplates))]
        public void CreateNetCoreProject_RestoresNewProject(ProjectTemplate projectTemplate)
        {
            // Arrange
            EnsureVisualStudioHost();

            using (var testContext = new ApexTestContext(VisualStudio, projectTemplate, XunitLogger))
            {
                VisualStudio.AssertNoErrors();
            }
        }

        // basic create for .net core template
        [NuGetWpfTheory]
        [MemberData(nameof(GetNetCoreTemplates))]
        public void CreateNetCoreProject_AddProjectReference(ProjectTemplate projectTemplate)
        {
            // Arrange
            EnsureVisualStudioHost();

            using (var testContext = new ApexTestContext(VisualStudio, projectTemplate, XunitLogger))
            {
                var project2 = testContext.SolutionService.AddProject(ProjectLanguage.CSharp, projectTemplate, ProjectTargetFramework.V46, "TestProject2");
                project2.Build();

                testContext.Project.References.Dte.AddProjectReference(project2);
                testContext.SolutionService.SaveAll();

                testContext.SolutionService.Build();
                testContext.NuGetApexTestService.WaitForAutoRestore();

                VisualStudio.AssertNoErrors();
                CommonUtility.AssertPackageInAssetsFile(VisualStudio, testContext.Project, "TestProject2", "1.0.0", XunitLogger);
            }
        }

        // make sure ClearWindows in ApexTestContext constructor will only clear OutputWindow at the beginning
        [NuGetWpfTheory]
        [MemberData(nameof(GetNetCoreTemplates))]
        public void CreateNetCoreProject_InstallNonExistingPackageFail(ProjectTemplate projectTemplate)
        {
            // Arrange
            EnsureVisualStudioHost();

            using (var testContext = new ApexTestContext(VisualStudio, projectTemplate, XunitLogger))
            {
                VisualStudio.AssertNoErrors();

                var project3 = testContext.SolutionService.AddProject(ProjectLanguage.CSharp, projectTemplate, ProjectTargetFramework.V46, "TestProject3");
                project3.Build();

                var nugetConsole = GetConsole(project3);

                var packageName = "nonexistPackage";
                var packageVersion = "1.0.1";

                nugetConsole.InstallPackageFromPMC(packageName, packageVersion);

                var outputList = VisualStudio.GetOutputWindowsLines();
                StringBuilder sb = new StringBuilder();
                foreach (var line in outputList)
                {
                    sb.AppendLine(line);
                }
  
                Assert.True(outputList.Count > 0, "install a nonexist package failed, the outputlist of PMC should not be empty.");
                Assert.True(sb.ToString().Contains("Unable to find package"), sb.ToString());

            }
        }
        // There  is a bug with VS or Apex where NetCoreConsoleApp and NetCoreClassLib create netcore 2.1 projects that are not supported by the sdk
        // Commenting out any NetCoreConsoleApp or NetCoreClassLib template and swapping it for NetStandardClassLib as both are package ref.

        public static IEnumerable<object[]> GetNetCoreTemplates()
        {
            yield return new object[] { ProjectTemplate.NetStandardClassLib };
        }
    }
}
