using System.Collections.Generic;

namespace Tack.Core.Tests
{
    public class TestOptions : IProjectLoaderOptions, IAssemblyLoaderOptions
    {
        public string Configuration { get; set; }

        public string Solution { get; set; }

        public string TargetFramework { get; set; }

        public IEnumerable<string> AssembliesToExclude { get; set; } = new string[0];

        public IEnumerable<string> AssembliesToInclude { get; set; } = new string[0];

        public bool GetPublishedOutput { get; set; }

        public bool SkipExistenceCheck { get; set; }

        public FrameworkSelector FrameworkSelector { get; set; } = FrameworkSelector.All;
    }
}
