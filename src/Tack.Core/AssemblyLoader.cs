using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Serilog;

[assembly: InternalsVisibleTo("Tack.Core.Tests")]
namespace Tack.Core
{
    public class AssemblyLoader
    {
        private ProjectLoader _projectLoader;
        private FrameworkSelectorFactory _frameworkSelectorFactory;

        public AssemblyLoader(ProjectLoader projectLoader, FrameworkSelectorFactory frameworkSelectorFactory)
        {
            _projectLoader = projectLoader;
            _frameworkSelectorFactory = frameworkSelectorFactory;
        }

        public (bool errorsFound, List<string> testAssemblyPaths) GetAssemblyList(IAssemblyLoaderOptions options)
        {
            SolutionFile solutionFile = SolutionFile.Parse(Path.GetFullPath(options.Solution));
            List<string> testAssemblyPaths = new List<string>();
            bool errorsFound = false;
            var frameworkSelector = _frameworkSelectorFactory.Get(options, solutionFile);

            foreach (var testProject in solutionFile.ProjectsInOrder.Where(p => p.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat && options.TestAssemblyRegex.IsMatch(p.ProjectName)))
            {
                if (ExcludeAssembly(options.AssembliesToInclude, options.AssembliesToExclude, projectName: testProject.ProjectName))
                {
                    Log.Information("TestAssembly {projectName} does not match assembly filters. Skipping.", testProject.ProjectName);
                    continue;
                }

                if (!_projectLoader.TryGetProject(testProject.AbsolutePath, out Project project))
                {
                    throw new Exception($"Unable to load project {testProject.AbsolutePath}");
                }

                foreach (var targetFramework in frameworkSelector.GetTargetFrameworks(project))
                {
                    var directory = Path.GetDirectoryName(Path.GetFullPath(testProject.AbsolutePath));

                    string targetExtension = project.GetPropertyValue("TargetExt");

                    if (string.IsNullOrEmpty(targetExtension))
                    {
                        targetExtension = ".dll";
                        if (targetFramework.StartsWith("net4"))
                        {
                            var outputType = project.GetPropertyValue("OutputType").ToLower();
                            if (outputType != "library")
                            {
                                targetExtension = ".exe";
                            }
                        }
                    }

                    string outputPath = project.GetPropertyValue("OutputPath");

                    if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                    {
                        outputPath = outputPath.Replace('\\', '/');
                    }

                    if (!outputPath.Contains(targetFramework))
                    {
                        outputPath = Path.Combine(outputPath, targetFramework);
                    }

                    if (options.GetPublishedOutput)
                    {
                        outputPath = Path.Combine(outputPath, "publish");
                    }

                    var testAssemblyFullPath = Path.Combine(directory, outputPath, project.GetPropertyValue("AssemblyName") + targetExtension);

                    if (!File.Exists(testAssemblyFullPath))
                    {
                        if (options.SkipExistenceCheck)
                        {
                            Log.Warning("Unable to find test assembly file: {testAssembly}", testAssemblyFullPath);
                        }
                        else
                        {
                            Log.Error("Unable to find test assembly file: {testAssembly}", testAssemblyFullPath);
                            errorsFound = true;
                        }
                    }

                    Log.Information("Adding {framework} for {project} to output list.", targetFramework, testProject.ProjectName);
                    testAssemblyPaths.Add(testAssemblyFullPath);
                }
            }

            return (errorsFound, testAssemblyPaths);
        }

        public bool ExcludeAssembly(IEnumerable<string> assembliesToInclude, IEnumerable<string> assembliesToExclude, string projectName)
        {
            foreach (var excludePattern in assembliesToExclude)
            {
                if (!string.IsNullOrWhiteSpace(excludePattern) && Regex.IsMatch(projectName, excludePattern))
                {
                    return true;
                }
            }

            if (!assembliesToInclude.Any() || assembliesToInclude.All(x => string.IsNullOrWhiteSpace(x)))
            {
                return false;
            }

            bool exclude = true;

            foreach (var includePattern in assembliesToInclude)
            {
                if (!string.IsNullOrWhiteSpace(includePattern) && Regex.IsMatch(projectName, includePattern))
                {
                    exclude = false;
                    break;
                }
            }

            return exclude;
        }
    }
}
