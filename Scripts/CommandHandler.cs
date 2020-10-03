using Godot;
using System;
using System.Collections.Generic;

public class CommandHandler : Node
{
    public enum Arguments
    {
        ARG_INT,
        ARG_STRING,
        ARG_BOOL,
        ARG_FLOAT
    };

    public Dictionary<String, List<Arguments>> ValidCommands = new Dictionary<String, List<Arguments>>()
    {
        {"SetSpeed", new List<Arguments>() {Arguments.ARG_INT}},
        {"SetTest2Arg", new List<Arguments>() {Arguments.ARG_INT, Arguments.ARG_STRING}},
        {"BoolTest", new List<Arguments>() {Arguments.ARG_INT, Arguments.ARG_BOOL}},
        {"exit", new List<Arguments>() {}}
    };

    public void SetSpeed(int newSpeed)
    {
        GD.Print("New speed is set to ->" + newSpeed);
    }

    public void SetTest2Arg(int newSpeed, string myString)
    {
        GD.Print("SetTest2Arg int = " + newSpeed + " and the string is = " + myString);
    }

    public void BoolTest(int newSpeed, bool myBool)
    {
        GD.Print("BoolTest int = " + newSpeed + " and the string is = " + myBool);
    }

    public void exit()
    {
        GetTree().Quit();
    }

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
