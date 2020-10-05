using Godot;
using System;
using EventCallback;

public class MenuOutput : Control
{
    public void OnStartPressed()
    {
        StartPressedEvent spei = new StartPressedEvent();
        spei.startPressed = true;
        spei.FireEvent();
        GD.Print("Start Pressed");
        Visible = false;
    }

    public void OnExitPressed()
    {
        GetTree().Quit();
    }

}
