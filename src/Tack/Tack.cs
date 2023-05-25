using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bulldog;
using Microsoft.Build.Locator;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Tack.Core;

namespace Tack
{
    public enum FileMode
    {
        Overwrite,
        Append,
    }

    public class Tack : ToolBase<Options>
    {
        protected override void ConfigureServices(IServiceCollection serviceCollection, Options options)
        {
            serviceCollection.AddSingleton<IProjectLoaderOptions>(options);
            serviceCollection.AddSingleton<ProjectLoader>();
            serviceCollection.AddSingleton<FrameworkSelectorFactory>();
            serviceCollection.AddTransient<AssemblyLoader>();
        }

        protected override Task<int> Run(Options options)
        {
            MSBuildLocator.RegisterDefaults();

            if (!File.Exists(options.Solution))
            {
                Log.Error("Unable to find specified solution file {Solution}", options.Solution);
                return Task.FromResult(1);
            }

            var assemblyLoader = ServiceProvider.GetService<AssemblyLoader>();

            (bool errorsFound, List<string> testAssemblyPaths) = assemblyLoader.GetAssemblyList(options);

            if (testAssemblyPaths.Count == 0)
            {
                Log.Error("Failed to find any matching test assemblies in {solution}.", options.Solution);
                return Task.FromResult(2);
            }

            var outputPathDir = Path.GetDirectoryName(options.OutputFile);
            if (!string.IsNullOrWhiteSpace(outputPathDir) && !Directory.Exists(outputPathDir))
            {
                Log.Information("Creating {outputPathDir} directory for output file.", outputPathDir);
                Directory.CreateDirectory(outputPathDir);
            }

            Log.Information("Writing list of test assemblies to output file {outfile}", options.OutputFile);
            if (options.FileMode == FileMode.Overwrite)
            {
                File.WriteAllLines(options.OutputFile, testAssemblyPaths);
            }
            else
            {
                File.AppendAllLines(options.OutputFile, testAssemblyPaths);
            }

            return errorsFound ? Task.FromResult(1) : Task.FromResult(0);
        }
    }
}