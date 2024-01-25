using System;
using System.Collections.Generic;
using Microsoft.Build.Evaluation;

namespace Tack.Core
{
    public static class ProjectExtensions
    {
        public static string ProjectName(this Project project) => project.GetPropertyValue("ProjectName");

        public static IEnumerable<string> TargetFrameworks(this Project project)
        {
            var targetFramework = project.GetPropertyValue("TargetFramework");
            if (!string.IsNullOrEmpty(targetFramework))
            {
                return new[] { targetFramework };
            }
            else
            {
                var targetFrameworks = project.GetPropertyValue("TargetFrameworks");

                if (string.IsNullOrEmpty(targetFrameworks))
                {
                    throw new Exception($"Unable to determine target framework for project '{project.GetPropertyValue("ProjectName")}'");
                }
                return targetFrameworks.Split(";");
            }
        }
    }
}
