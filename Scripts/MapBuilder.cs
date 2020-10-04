using Godot;
using System;
using System.Collections.Generic;
using EventCallback;

//When the room spawns the starting room already has an exit
public class MapBuilder : Spatial
{
    //List to keep rooms in
    List<KinematicBody> roomList = new List<KinematicBody>();
    //The reference to the room scene
    PackedScene roomScene = new PackedScene();
    //The node for the room
    KinematicBody room;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    private void GenerateRoom()
    {

    }

    private void ClearRooms()
    {
        // for()
        // {
        //     rooms[i].QueueFree();
        // }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
