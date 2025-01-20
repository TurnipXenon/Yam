using Xunit.Abstractions;
using Yam.Core.Common;

namespace Yam.Core.Test.Utility;

public abstract class BaseTest
{
    protected readonly ITestOutputHelper XUnitLogger;

    protected BaseTest(ITestOutputHelper xUnitLogger)
    {
        XUnitLogger = xUnitLogger;
    }
}