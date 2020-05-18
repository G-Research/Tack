using FakeItEasy;
using FluentAssertions;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Extensions.Logging.Abstractions;
using System.IO;
using System.Linq;
using System.Xml;
using Xunit;

namespace Tack.Core.Tests
{
    public class FrameworkSelectorTests
    {
        [Fact]
        public void CanCorrectIdentifyMaximumFrameworkInProject()
        {
            var projectContent = ProjectContent("netcoreapp3.1;net5.0;net6.0");

            var options = A.Fake<IProjectLoaderOptions>();
            A.CallTo(() => options.Configuration).Returns("Debug");
            ProjectLoader projectLoader = new ProjectLoader(NullLogger<ProjectLoader>.Instance, options);

            XmlReader xmlReader = XmlReader.Create(new StringReader(projectContent));
            Project project = projectLoader.LoadProjectFromProjectRootElement(ProjectRootElement.Create(xmlReader));

            MaxFrameworkSelector maxFrameworkSelector = new MaxFrameworkSelector();
            var result = maxFrameworkSelector.GetTargetFrameworks(project).ToList();
            result.Count.Should().Be(1);
            result[0].Should().Be("net6.0");
        }

        [Fact]
        public void CanCorrectlyFilterTestsBasedOnRegex()
        {
            var projectContent = ProjectContent("netcoreapp3.1;net5.0;net6.0");

            var options = A.Fake<IProjectLoaderOptions>();
            A.CallTo(() => options.Configuration).Returns("Debug");
            ProjectLoader projectLoader = new ProjectLoader(NullLogger<ProjectLoader>.Instance, options);

            XmlReader xmlReader = XmlReader.Create(new StringReader(projectContent));
            Project project = projectLoader.LoadProjectFromProjectRootElement(ProjectRootElement.Create(xmlReader));

            var frameworkSelector = new RegexFrameworkSelector("(net5.0|net6.0)");
            var result = frameworkSelector.GetTargetFrameworks(project).ToList();
            result.Count.Should().Be(2);
            result[0].Should().Be("net5.0");
            result[1].Should().Be("net6.0");
        }

        [Fact]
        public void MaxFrameworkSelector_CanIgnoreWindowsTargets()
        {
            var projectContent = ProjectContent("netcoreapp3.1;net5.0-windows");

            var options = A.Fake<IProjectLoaderOptions>();
            A.CallTo(() => options.Configuration).Returns("Debug");
            ProjectLoader projectLoader = new ProjectLoader(NullLogger<ProjectLoader>.Instance, options);

            XmlReader xmlReader = XmlReader.Create(new StringReader(projectContent));
            Project project = projectLoader.LoadProjectFromProjectRootElement(ProjectRootElement.Create(xmlReader));

            var frameworkSelector = new MaxFrameworkSelector(ignoreWindows: true);
            var result = frameworkSelector.GetTargetFrameworks(project).ToList();
            result.Count.Should().Be(1);
            result[0].Should().Be("netcoreapp3.1");
        }

        [Fact]
        public void MaxFrameworkSelector_WillIgnoreIncompatibleProject()
        {
            var projectContent = ProjectContent("net5.0-windows");

            var options = A.Fake<IProjectLoaderOptions>();
            A.CallTo(() => options.Configuration).Returns("Debug");
            ProjectLoader projectLoader = new ProjectLoader(NullLogger<ProjectLoader>.Instance, options);

            XmlReader xmlReader = XmlReader.Create(new StringReader(projectContent));
            Project project = projectLoader.LoadProjectFromProjectRootElement(ProjectRootElement.Create(xmlReader));

            var frameworkSelector = new MaxFrameworkSelector(ignoreWindows: true);
            var result = frameworkSelector.GetTargetFrameworks(project).ToList();
            result.Should().BeEmpty();
        }

        private static string ProjectContent(string frameworks)
        {
            return $@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFrameworks>{frameworks}</TargetFrameworks>
    <ToolCommandName>Tack</ToolCommandName>
    <PackAsTool>true</PackAsTool>
    <PackageRequireLicenseAcceptance>false</PackageRequireLicenseAcceptance>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
    <PackageId>Tack</PackageId>
    <LangVersion>8.0</LangVersion>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Bulldog"" Version=""0.3.10"" />
    <PackageReference Include=""Microsoft.Build"" Version=""16.8.0"" />
    <PackageReference Include=""Microsoft.Build.Framework"" Version=""16.8.0"" />
    <PackageReference Include=""Microsoft.Build.Utilities.Core"" Version=""16.8.0"" />
    <PackageReference Include=""System.Collections.Immutable"" Version=""6.0.0"" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include=""..\Tack.Core\Tack.Core.csproj"" />
  </ItemGroup>
</Project>";
        }
    }
}
