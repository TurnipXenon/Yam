using Godot;
using Xunit.Abstractions;

namespace Yam.Core.Common;

// todo(turnip): improve GameLogger by detecting that we're being run in xUnit
// and ignore any calls to Godot. Or check if Godot's GD global function specific
// to Print is available for use
public static class GameLogger
{
    public static ITestOutputHelper? Output;

    public static void Print(params string[] what)
    {
        if (Output != null)
        {
            Output.WriteLine(what.Join(""));
        }
        else
        {
            GD.Print(what);
        }
    }
    
    public static void PrintErr(string what)
    {
        if (Output != null)
        {
            Output.WriteLine($"Godot.PrintErr: {what}");
        }
        else
        {
            GD.PrintErr(what);
        }
    }
}