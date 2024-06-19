namespace Yam.Core.Rhythm.Models.Base;

public enum DirectionBit
{
    Up = 1<<0,
    Down = 1<<1,
    Left = 1<<2,
    Right = 1<<3,
    UpLeft = Up | Left,
    UpRight = Up | Right,
    DownLeft = Down | Left,
    DownRight = Down | Right,
}