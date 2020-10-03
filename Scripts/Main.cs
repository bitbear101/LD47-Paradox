using Godot;
using System;

public class Main : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
  string b = "Hello World";
        GD.Print(b);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
