using Microsoft.Build.Locator;

namespace Tack.Core.Tests
{
    public abstract class BaseFixture
    {
        static BaseFixture()
        {
            MSBuildLocator.RegisterDefaults();
        }
    }
}
