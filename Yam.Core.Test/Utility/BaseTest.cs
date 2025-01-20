using Xunit.Abstractions;
using Yam.Core.Common;

namespace Yam.Core.Test.Utility;

public abstract class BaseTest
{
    protected BaseTest(ITestOutputHelper output)
    {
        GameLogger.Output = output;
    }
}