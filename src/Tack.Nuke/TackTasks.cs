using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

namespace Tack.Nuke;

internal static class ArgumentBuilderExtensions
{
    public static Arguments AddArgumentIfNotNull<T>(this Arguments arguments, string argumentName, T value)
    {
        if (value != null)
        {
            arguments.Add(argumentName, value);
        }

        return arguments;
    }

    public static Arguments AddArgumentIfNotNullOrDefault<T>(this Arguments arguments, string argumentName, T? value, T defaultValue) where T : IEquatable<T>
    {
        if (value != null && (defaultValue == null || !value.Equals(defaultValue)))
        {
            arguments.Add(argumentName);
            arguments.Add(value.ToString());
        }

        return arguments;
    }

    public static Arguments AddIfNotEmptyOrNull(this Arguments arguments, string argumentName, IEnumerable<string>? value, char separator = ' ')
    {
        if (value != null && value.Any())
        {
            arguments.Add(argumentName, string.Join(separator, value)); // With some escaping??
        }

        return arguments;
    }
}

[Serializable]
public class TackSettings : ToolSettings
{
    public TackSettings(string solutionFile, string outFile)
    {
        SolutionFile = solutionFile;
        OutFile = outFile;
    }
    //
    // Summary:
    //     Path to the DotNet executable. TODO: This should try to pick Tack.exe if available but lets assume that we've done DotNetToolRestore
    public override string ProcessToolPath => base.ProcessToolPath ?? DotNetTasks.DotNetPath;

    public static string TackPath =>
    ToolPathResolver.GetPackageExecutable("tack", "Tack.dll",
#if NET6_0
                framework: "net6.0"
#elif NET5_0
        framework: "net5.0"
#endif
        ) ?? "tack";

    public override Action<OutputType, string> ProcessCustomLogger => DotNetTasks.DotNetLogger;

    public string SolutionFile { get; }
    public string? Framework { get; set; }

    public IEnumerable<string>? ExcludeAssemblies { get; set; }
    public string? Configuration { get; set; }
    public string? OutFile { get; set; }
    public bool GetPublishedOutput { get; set; }

    public FrameworkSelector? FrameworkPicker { get; set; }

    protected override Arguments ConfigureProcessArguments(Arguments arguments)
    {
        arguments.Add(TackPath).Add("get-test-assemblies")
            .Add("--solution").Add(SolutionFile)
            .Add("--outfile").Add(OutFile)
            .AddArgumentIfNotNullOrDefault("--framework", Framework, "")
            .AddArgumentIfNotNullOrDefault("--configuration", Configuration, "")
            .AddIfNotEmptyOrNull("--exclude-assemblies", ExcludeAssemblies)
            .AddArgumentIfNotNull("--framework-selector", FrameworkPicker)
            .AddArgumentIfNotNullOrDefault("--get-published-outputs", GetPublishedOutput, false);

        return arguments;
    }
}
public enum FrameworkSelector
{
    All,
    Max,
    App,
    Regex,
    MaxNoWindows
}

public class TackTasks
{
    public static IReadOnlyCollection<Output> Tack(TackSettings tackSettings)
    {
        using var process = ProcessTasks.StartProcess(tackSettings);
        process.AssertZeroExitCode();
        return process.Output;
    }
}
