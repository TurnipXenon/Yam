using Godot;

namespace Yam.Game.Scenes.Twenty48;

public partial class NumberTile : ColorRect
{
    [Export] public Label label;
    [Export] public Color endingColor;
    
    private int step = 1;

    public override void _Ready()
    {
        label.Text = (2 ^ 1).ToString();
    }
}