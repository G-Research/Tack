using FluentAssertions;
using Xunit;
using FakeItEasy;

namespace Tack.Core.Tests
{
    public class AssemblyLoaderTests
    {
        [Fact]
        public void ExcludeCategoriesCorrectlyExcludeProjects()
        {
            var assembliesToInclude = new string[0];
            var assembliesToExclude = new[] { "^MyTest" };
            var assemblyLoader = new AssemblyLoader(A.Fake<ProjectLoader>(), A.Fake<FrameworkSelectorFactory>());
            assemblyLoader.ExcludeAssembly(assembliesToInclude, assembliesToExclude, "MyTestProject.Test").Should().BeTrue("MyTest should match this project name");
            assemblyLoader.ExcludeAssembly(assembliesToInclude, assembliesToExclude, "AnotherTestProject.Test").Should().BeFalse();
            assemblyLoader.ExcludeAssembly(assembliesToInclude, assembliesToExclude, "NotMyTestProject.Test").Should().BeFalse();
        }

        [Fact]
        public void IncludeCategoriesCorrectlyExcludeProjects()
        {
            var assembliesToInclude = new[] { "^MyTest" };
            var assembliesToExclude = new string[0];
            var assemblyLoader = new AssemblyLoader(A.Fake<ProjectLoader>(), A.Fake<FrameworkSelectorFactory>());
            assemblyLoader.ExcludeAssembly(assembliesToInclude, assembliesToExclude, "MyTestProject.Test").Should().BeFalse("MyTest should match this project name");
            assemblyLoader.ExcludeAssembly(assembliesToInclude, assembliesToExclude, "AnotherTestProject.Test").Should().BeTrue();
            assemblyLoader.ExcludeAssembly(assembliesToInclude, assembliesToExclude, "NotMyTestProject.Test").Should().BeTrue();
        }

        [Fact]
        public void IncludeAndExcludeCategoriesCorrectlyExcludeProjects()
        {
            var assembliesToInclude = new[] { "Test" };
            var assembliesToExclude = new[] { "^MyTest" };
            var assemblyLoader = new AssemblyLoader(A.Fake<ProjectLoader>(), A.Fake<FrameworkSelectorFactory>());
            assemblyLoader.ExcludeAssembly(assembliesToInclude, assembliesToExclude, "MyTestProject.Test").Should().BeTrue("");
            assemblyLoader.ExcludeAssembly(assembliesToInclude, assembliesToExclude, "AnotherTestProject.Test").Should().BeFalse();
            assemblyLoader.ExcludeAssembly(assembliesToInclude, assembliesToExclude, "NotMyTestProject.Test").Should().BeFalse();
        }
    }
}
