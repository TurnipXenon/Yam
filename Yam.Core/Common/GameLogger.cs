using System;
using Godot;
using Xunit.Abstractions;
using Environment = System.Environment;

namespace Yam.Core.Common;

public class GameLogger
{
    private static readonly bool IsGodot = Environment.GetEnvironmentVariable("GODOT_EDITOR_CUSTOM_FEATURES") != null;
    private static readonly bool IsLocalTest = Environment.GetEnvironmentVariable("RESHARPER_TESTRUNNER") != null;

    public ITestOutputHelper? XUnitLogger;

    public void Print(params string[] what)
    {
        if (XUnitLogger != null)
        {
            foreach (var key in Environment.GetEnvironmentVariables().Keys)
            {
                XUnitLogger.WriteLine($"{key}: {Environment.GetEnvironmentVariables()[key]}");
            }

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