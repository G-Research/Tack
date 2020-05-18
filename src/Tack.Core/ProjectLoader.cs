using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Construction;
using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Evaluation.Context;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;

namespace Tack.Core
{
    public interface IProjectLoaderOptions
    {
        string Configuration { get; }
    }

    public class ProjectLoader
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, string> _globalProperties;
        private readonly EvaluationContext _evaluationContext;
        private readonly ProjectCollection _projectCollection;
        private readonly ProjectLoadSettings _loadSettings;
        private readonly Dictionary<string, Project> _projects;

        public ProjectLoader(ILogger<ProjectLoader> logger, IProjectLoaderOptions loaderOptions)
        {
            _logger = logger;
            (Version sdkVersion, string sdkDirectory) = GetSdkPath();
            _globalProperties = GetGlobalProperties(sdkVersion, sdkDirectory, loaderOptions.Configuration);
            _evaluationContext = EvaluationContext.Create(EvaluationContext.SharingPolicy.Shared);
            _projectCollection = new ProjectCollection(_globalProperties);

            Toolset toolSet = new Toolset("Current", sdkDirectory, _projectCollection, sdkDirectory);

            _projectCollection.AddToolset(toolSet);
            _loadSettings = ProjectLoadSettings.RejectCircularImports;
            _projects = new Dictionary<string, Project>();
        }

        public static Dictionary<string, string> GetGlobalProperties(Version sdkVersion, string sdkDirectory, string configuration)
        {
            var globalProperties = new Dictionary<string, string>()
            {
                {"RoslynTargetsPath", Path.Combine(sdkDirectory, "Roslyn")},
                {"MSBuildSDKsPath", Path.Combine(sdkDirectory, "Sdks")},
                {"MSBuildExtensionsPath", sdkDirectory},
                {"Configuration", configuration}
            };

            if (sdkVersion >= new Version(6, 0))
            {
#if NET6_0_OR_GREATER
                Environment.SetEnvironmentVariable("MSBUILDADDITIONALSDKRESOLVERSFOLDER", Path.Combine(sdkDirectory, "SdkResolvers"));
                Environment.SetEnvironmentVariable("MSBuildEnableWorkloadResolver", "true");
#else
                Environment.SetEnvironmentVariable("MSBuildEnableWorkloadResolver", "false");
#endif
            }

            Environment.SetEnvironmentVariable("MSBuildSDKsPath", Path.Combine(sdkDirectory, "Sdks"));
            Environment.SetEnvironmentVariable("MSBuildExtensionsPath", sdkDirectory);
            Environment.SetEnvironmentVariable("MSBUILD_NUGET_PATH", sdkDirectory);
            Environment.SetEnvironmentVariable("MSBuildToolsPath", sdkDirectory);
            return globalProperties;
        }

        private (Version SdkVersion, string SdkPath) GetSdkPath()
        {
            using var process = Process.Start(new ProcessStartInfo("dotnet", "--info")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = false,
                CreateNoWindow = true,
                UseShellExecute = false
            });

            try
            {
                string path = null;
                Version version = null;
                while (!process.StandardOutput.EndOfStream)
                {
                    var line = process.StandardOutput.ReadLine();
                    if (line.Contains(" Version: ") && line.TrimStart().StartsWith("Version: "))
                    {
                        if (!Version.TryParse(line.Replace("Version: ", "").Trim(), out version))
                        {
                            new ApplicationException("Unable to parse SDK version from dotnet --info.");
                        }
                    }
                    else if (line.Contains("Base Path"))
                    {
                        if (version == null)
                        {
                            new ApplicationException("Unable to find SDK version from dotnet --info");
                        }
                        path = line.Replace("Base Path: ", "").Trim();
                        _logger.LogInformation("Found SDK Path: {path}", path);
                        return (version, path);
                    }
                }
            }
            finally
            {
                process.StandardOutput.ReadToEnd();
                if (!process.WaitForExit(60 * 1000))
                {
                    throw new ApplicationException("Timeout waiting for dotnet process to exit");
                }
            }

            throw new ApplicationException("Unable to find SDK path for project loading.");
        }

        public bool TryGetProject(string fullPath, out Project project)
        {
            string normalisedPath = Path.GetFullPath(fullPath);
            if (_projects.TryGetValue(normalisedPath, out project))
            {
                return true;
            }

            if (!File.Exists(normalisedPath))
            {
                _logger.LogWarning("Unable to find project file at: {fullPath}. Returning false", normalisedPath);

                project = null;
                return false;
            }

            project = Project.FromFile(normalisedPath,
                new ProjectOptions
                {
                    ProjectCollection = _projectCollection,
                    LoadSettings = _loadSettings,
                    EvaluationContext = _evaluationContext,
                    GlobalProperties = _globalProperties,
                });

            _projects.Add(project.FullPath, project);

            return true;
        }

        internal Project LoadProjectFromProjectRootElement(ProjectRootElement projectRootElement)
        {
            var project = Project.FromProjectRootElement(projectRootElement,
                new ProjectOptions
                {
                    ProjectCollection = _projectCollection,
                    LoadSettings = _loadSettings,
                    EvaluationContext = _evaluationContext,
                    GlobalProperties = _globalProperties,
                });

            return project;
        }
    }
}
