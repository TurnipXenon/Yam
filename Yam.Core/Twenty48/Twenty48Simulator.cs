using System;
using System.Collections.Generic;
using Yam.Core.Common;

namespace Yam.Core.Twenty48;

public class Twenty48Node
{
    // Step is always a multiple of two
    public int Step { get; private set; }
    public int PosX { get; private set; }
    public int PosY { get; private set; }

    public Dictionary<BitwiseDirection, Twenty48Node> neighbors = new();

    public Twenty48Node(int startingStep, int posX, int posY)
    {
        Step = startingStep;
        PosX = posX;
        PosY = posY;
    }

    public void Move(BitwiseDirection direction)
    {
        switch (direction)
        {
            case BitwiseDirection.Up:
                // todo: move
                // todo: check neighbors
                throw new NotImplementedException();
            
            case BitwiseDirection.Down:
                // todo: move
                // todo: check neighbors
                throw new NotImplementedException();
            
            case BitwiseDirection.Left:
                // todo: move
                // todo: check neighbors
                PosX = 0;
                break;
            
            case BitwiseDirection.Right:
                // todo: move
                // todo: check neighbors
                throw new NotImplementedException();
            
            case BitwiseDirection.None:
            case BitwiseDirection.UpLeft:
            case BitwiseDirection.UpRight:
            case BitwiseDirection.DownLeft:
            case BitwiseDirection.DownRight:
                throw new Exception($"Unimplemented direction {nameof(direction)}");

            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    public override string ToString()
    {
        return $"{{Step: {Step}; PosX: {PosX}; PosY: {PosY}}}";
    }
}

public class Twenty48Simulator
{
    public List<Twenty48Node> NodeList = [];
    public int Width = 4;
    public int Height = 4;
    public int generationStep = 0;
    public Random rand;

    public Twenty48Simulator()
    {
        rand = new Random();
    }

    public Twenty48Simulator(int seed)
    {
        rand = new Random(seed);
    }

    public void NewGame()
    {
        NodeList.Clear();

        var positionIndex = rand.Next(0, 4);
        var posX = 0;
        var posY = 0;
        switch (positionIndex)
        {
            case 0:
                posX = 0;
                posY = 0;
                break;
            case 1:
                posX = Width;
                posY = 0;
                break;
            case 2:
                posX = Width;
                posY = Height;
                break;
            case 3:
                posX = 0;
                posY = Height;
                break;
        }

        NodeList.Add(new Twenty48Node(2, posX, posY));
    }

    public bool Move(BitwiseDirection direction)
    {
        switch (direction)
        {
            case BitwiseDirection.Up:
            case BitwiseDirection.Down:
            case BitwiseDirection.Left:
            case BitwiseDirection.Right:
                NodeList.ForEach(node => node.Move(direction));

                // todo: generate a new node in one area
                // todo: generate a new node based on next corner we want
                return true;

            case BitwiseDirection.None:
            case BitwiseDirection.UpLeft:
            case BitwiseDirection.UpRight:
            case BitwiseDirection.DownLeft:
            case BitwiseDirection.DownRight:
                return false;

            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }
}