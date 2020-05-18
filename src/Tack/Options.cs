using System.Collections.Generic;
using System.Text.RegularExpressions;
using Bulldog;
using CommandLine;
using Tack.Core;

namespace Tack
{
    [Verb("get-test-assemblies")]
    public class Options : OptionsBase, IProjectLoaderOptions, IAssemblyLoaderOptions
    {
        [Option("solution", Required = true, HelpText = "The solution to search in.")]
        public string Solution { get; set; }

        [Option("configuration", Required = false, Default = "Debug", HelpText = "Project build configuration")]
        public string Configuration { get; set; }

        [Option("framework", Required = false, Default = ".", HelpText = "Regex used to match target frameworks.")]
        public string TargetFramework { get; set; }

        [Option("exclude-assemblies", HelpText = "Comma separated list of assembly name patterns to exclude.", Separator = ',')]
        public IEnumerable<string> AssembliesToExclude { get; set; } = new string[0];

        // Initialised as Non-null IEnumerable<string> by CommandLineParser
        [Option("include-assemblies", HelpText = "Comma separated list of assembly name patterns to filter by.", Separator = ',')]
        public IEnumerable<string> AssembliesToInclude { get; set; } = new string[0];

        [Option("outfile", Required = true, HelpText = "Output file to output.")]
        public string OutputFile { get; set; }

        [Option("get-published-output", Required = false, HelpText = "Get the published output of the tests rather than the build output")]
        public bool GetPublishedOutput { get; set; }

        [Option("skip-existence-check", Required = false, HelpText = "Skips validation that the test assemblies we have found exist on disk")]
        public bool SkipExistenceCheck { get; set; }

        [Option("file-mode", Required = false, Default = FileMode.Overwrite, HelpText = "Indicates whether to append or overwrite output file")]
        public FileMode FileMode { get; set; }

        [Option("framework-selector", Required = false, Default = FrameworkSelector.All, HelpText = "Indicates how to determine the set of target frameworks. Whether to pick All, the maximum framework or the framework of top level application.")]
        public FrameworkSelector FrameworkSelector { get; set; }

        [Option("test-regex", Required = false, Default = "(?i)test(s?)$", HelpText = "Regex for determining set of test assemblies.")]
        public string TestRegex
        {
            get { return _testAssemblyRegex.ToString(); }
            set { _testAssemblyRegex = new Regex(value); }
        }

        public Regex _testAssemblyRegex = new Regex("(?i)test(s?)$");
        public Regex TestAssemblyRegex => _testAssemblyRegex;
    }
}
