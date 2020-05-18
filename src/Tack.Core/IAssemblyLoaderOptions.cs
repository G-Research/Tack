using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Tack.Core
{
    public interface IAssemblyLoaderOptions
    {
        string Solution { get; }
        string TargetFramework { get; }
        IEnumerable<string> AssembliesToExclude { get; }
        IEnumerable<string> AssembliesToInclude { get; }
        bool GetPublishedOutput { get; }
        bool SkipExistenceCheck { get; }
        FrameworkSelector FrameworkSelector { get; }
        Regex TestAssemblyRegex => new Regex("(?i)test(s?)$");
    }
}
