# Troubleshooting

## Inconclusive: Exit code is -1073741819 (Output is too long. Showing the last 100 lines:

You would usually encounter this as an exit code during unit testing. This is caused by `GD.Print()`. Right now, we don't have any mechanisms to check if the code is running in Godot's editor or run under xUnit. The only way for our logger to detect if you're in either is by setting the `output` property in GameLogger. If it's null, we use `GD.Print`. Otherwise, use xUnit's `ITestOutputHelper`.

```c#
public abstract class BeatTest
{
        public class SimulateSingleBeat
    {
        public SimulateSingleBeat(ITestOutputHelper output)
        {
            GameLogger.Output = output;
        }
        
        // tests below with function calls that interact with GameLogger
    }
}
```

For example, if you remove the code above in `BeatTest.SimulateSingleBeat`, the error would appear. The code above does what we mentioned previously.
