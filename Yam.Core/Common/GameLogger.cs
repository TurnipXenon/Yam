using Godot;
using Xunit.Abstractions;

namespace Yam.Core.Common;

// todo(turnip): improve GameLogger by detecting that we're being run in xUnit
// and ignore any calls to Godot. Or check if Godot's GD global function specific
// to Print is available for use
public class GameLogger
{
    public ITestOutputHelper? XUnitLogger;

    public void Print(params string[] what)
    {
        if (XUnitLogger != null)
        {
            XUnitLogger.WriteLine(what.Join(""));
        }
        else
        {
            GD.Print(what);
        }
    }
    
    public void PrintErr(string what)
    {
        if (XUnitLogger != null)
        {
            XUnitLogger.WriteLine($"Godot.PrintErr: {what}");
        }
        else
        {
            GD.PrintErr(what);
        }
    }
}