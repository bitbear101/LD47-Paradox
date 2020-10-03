using Godot;
using System;
using EventCallback;

public class AreaExit : Area
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void OnAreaExitBodyEntered(Node body)
    {
        if(body.Name == "PlayerBody")
        {
        SetStateEvent ssei = new SetStateEvent();
        ssei.newState = ORB_STATE.CHASE;
        ssei.FireEvent();
        }
    }

    // public void body_entered()
    // {
    //      GD.Print("Body entered");
    // }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
