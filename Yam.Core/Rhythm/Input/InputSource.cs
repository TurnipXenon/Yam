namespace Yam.Core.Rhythm.Input;

public enum InputSource
{
    Game, // simulating that one _process game update has passed and ignores inputs
    Player, // a player did something that's mapped in the input system
    Unknown, // an input we don't care about, e.g. pressing a random key or gamepad button
}