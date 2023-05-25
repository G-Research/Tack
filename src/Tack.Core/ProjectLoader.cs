using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Construction;
using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Evaluation.Context;
using Microsoft.Extensions.Logging;

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
        private readonly Dictionary<string, Project> _projects;

        public ProjectLoader(ILogger<ProjectLoader> logger, IProjectLoaderOptions loaderOptions)
        {
            _logger = logger;
            _globalProperties = new Dictionary<string, string>()
            {
                {"Configuration", loaderOptions.Configuration},
            };

            _evaluationContext = EvaluationContext.Create(EvaluationContext.SharingPolicy.Shared);
            _projectCollection = new ProjectCollection(_globalProperties);

            _projects = new Dictionary<string, Project>();
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
                return false;
            }

            var projectOptions = new ProjectOptions
            {
                ProjectCollection = _projectCollection,
                LoadSettings = ProjectLoadSettings.IgnoreEmptyImports | ProjectLoadSettings.IgnoreInvalidImports |
                               ProjectLoadSettings.RecordDuplicateButNotCircularImports |
                               ProjectLoadSettings.IgnoreMissingImports,
                EvaluationContext = _evaluationContext,
            };

            project = Project.FromFile(normalisedPath, projectOptions);

            _projects.Add(project.FullPath, project);

            return true;
        }
    }
}