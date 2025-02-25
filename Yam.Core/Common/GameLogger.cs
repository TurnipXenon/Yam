using System;
using Godot;
using Xunit.Abstractions;
using Environment = System.Environment;

namespace Yam.Core.Common;

public class GameLogger
{
    
    /// <summary>
    /// The higher, the noisier
    /// </summary>
    public enum Noise
    {
        AlwaysOn = 0,
        Log = 1,
        Debug = 2,
        Verbose = 3,
        TooVerbose = 4,
    }

    private const int AllowedNoise = (int)Noise.Log;

    private static readonly bool IsGodot = Environment.GetEnvironmentVariable("GODOT_EDITOR_CUSTOM_FEATURES") != null;
    private static readonly bool IsLocalTest = Environment.GetEnvironmentVariable("RESHARPER_TESTRUNNER") != null;

    public ITestOutputHelper? XUnitLogger;

    public void Print(params string[] what)
    {
        Print(Noise.Log, what);
    }
    
    public void Print(Noise noise, params string[] what)
    {
        if ((int)noise > AllowedNoise)
        {
            return;
        }
        
        if (XUnitLogger != null)
        {
            XUnitLogger.WriteLine(what.Join(""));
        }
        else if (IsGodot)
        {
            GD.Print(what);
        }
        else if (IsLocalTest && XUnitLogger == null)
        {
            throw new Exception("Missing XUnitLogger");
        }
#if DEBUG
        else
        {
            Console.WriteLine(what.Join(" "));
        }
#endif // DEBUG
    }

    public void PrintErr(string what)
    {
        if (XUnitLogger != null)
        {
            XUnitLogger.WriteLine($"Godot.PrintErr: {what}");
        }
        else if (IsGodot)
        {
            GD.PrintErr(what);
        }
        else if (IsLocalTest && XUnitLogger == null)
        {
            throw new Exception("Missing XUnitLogger");
        }
    }
}