using Microsoft.Build.Evaluation;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog;
using System;
using Microsoft.Build.Construction;
using System.IO;

namespace Tack.Core
{
    public enum FrameworkSelector
    {
        All,
        Max,
        App,
        Regex,
        MaxNoWindows
    }

    public interface IFrameworkSelector
    {
        IEnumerable<string> GetTargetFrameworks(Project project);
    }

    public class FrameworkSelectorFactory
    {
        public FrameworkSelectorFactory(ProjectLoader projectLoader)
        {
            ProjectLoader = projectLoader;
        }

        public ProjectLoader ProjectLoader { get; }

        public IFrameworkSelector Get(IAssemblyLoaderOptions options, SolutionFile solutionFile)
        {
            switch (options.FrameworkSelector)
            {
                case FrameworkSelector.All:
                    {
                        if (string.IsNullOrEmpty(options.TargetFramework))
                        {
                            return new AllFrameworkSelector();
                        }
                        else
                        {
                            return new RegexFrameworkSelector(options.TargetFramework);
                        }
                    }
                case FrameworkSelector.Regex:
                    {
                        return new RegexFrameworkSelector(options.TargetFramework);
                    }
                case FrameworkSelector.Max:
                    {
                        return new MaxFrameworkSelector();
                    }
                case FrameworkSelector.MaxNoWindows:
                    {
                        return new MaxFrameworkSelector(ignoreWindows: true);
                    }
                case FrameworkSelector.App:
                    {
                        string solutionName = Path.GetFileNameWithoutExtension(options.Solution);
                        var appProject = solutionFile.ProjectsInOrder.SingleOrDefault(p => p.ProjectName.ToLower() == solutionName.ToLower());
                        if (appProject == null)
                        {
                            throw new Exception($"Unable to find base application for {solutionName}. No project with that matching name exists. Consider different FrameworkSelector type.");
                        }

                        if (!ProjectLoader.TryGetProject(appProject.AbsolutePath, out Project project))
                        {
                            throw new Exception($"Error loading project {appProject.AbsolutePath}.");
                        }

                        return new AppFrameworkSelector(project.TargetFrameworks());
                    }
                default:
                    {
                        throw new ArgumentException("Unsupported FrameworkSelector type.", nameof(options.FrameworkSelector));
                    }
            }
        }
    }

    internal class AppFrameworkSelector : IFrameworkSelector
    {
        List<TargetFramework> _targetFrameworks;
        public AppFrameworkSelector(IEnumerable<string> targetFrameworks)
        {
            _targetFrameworks = targetFrameworks.Select(t=>new TargetFramework(t)).ToList();
        }

        public IEnumerable<string> GetTargetFrameworks(Project testProject)
        {
            var targetFrameworks = new HashSet<string>();
            var testFrameworks = testProject.TargetFrameworks().Select(t => new TargetFramework(t)).ToArray();

            foreach (var appFramework in _targetFrameworks)
            {
                if (appFramework.GetMatchingTestFramework(testFrameworks, out TargetFramework testFramework))
                {
                    targetFrameworks.Add(testFramework.Framework);
                }
                else
                {
                    throw new Exception($"Unable to find matching test framework for {appFramework} in {testProject.ProjectName()}.");
                }
            }

            return targetFrameworks;
        }
    }

    public sealed class MaxFrameworkSelector : IFrameworkSelector
    {
        private readonly bool _ignoreWindows;

        public MaxFrameworkSelector(bool ignoreWindows = false)
        {
            _ignoreWindows = ignoreWindows;
        }

        public IEnumerable<string> GetTargetFrameworks(Project project)
        {
            var matchingFrameworks = project.TargetFrameworks()
                .Select(t => new TargetFramework(t))
                .Where(t => _ignoreWindows == false || t.TargetPlatformIdentifier != TargetPlatformIdentifier.Windows)
                .ToArray();

            if (!matchingFrameworks.Any())
            {
                Log.Information("Skipping {project} as it does not match framework filter.", project.ProjectName());
                return Array.Empty<string>();
            }

            var maxTargetFramework = matchingFrameworks.Max();
            return new[] { maxTargetFramework.Framework };
        }
    }

    public sealed class AllFrameworkSelector : IFrameworkSelector
    {
        public IEnumerable<string> GetTargetFrameworks(Project project) => project.TargetFrameworks();
    }

    public sealed class RegexFrameworkSelector : IFrameworkSelector
    {
        private Regex _targetFrameworkRegex;

        public RegexFrameworkSelector(string targetFrameworkRegex)
        {
            _targetFrameworkRegex = new Regex(targetFrameworkRegex);
        }

        public IEnumerable<string> GetTargetFrameworks(Project project)
        {
            foreach (var targetFramework in project.TargetFrameworks())
            {
                if (_targetFrameworkRegex.IsMatch(targetFramework))
                {
                    yield return targetFramework;
                }
                else
                {
                    Log.Information("Skipping {framework} for {project} as it does not match framework filter.", targetFramework, project.ProjectName());
                }
            }
        }
    }
}
