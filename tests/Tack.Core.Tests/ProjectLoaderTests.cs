using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Tack.Core.Tests
{
    public class ProjectLoaderTests
    {
        [Fact]
        public void TestCanIdentifyTestProjects()
        {
            var options = new TestOptions() { 
                Configuration = "Release", 
                SkipExistenceCheck = true, 
                Solution = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "test-cases", "test-cases.sln")
            };

            var projectLoader = new ProjectLoader(NullLogger<ProjectLoader>.Instance, options);
            var assemblyLoader = new AssemblyLoader(projectLoader, new FrameworkSelectorFactory(projectLoader));

            (bool errorsFound, List<string> testAssemblyPaths) = assemblyLoader.GetAssemblyList(options);

            errorsFound.Should().BeFalse();
            testAssemblyPaths.Count().Should().Be(2);
        }

    }
}
