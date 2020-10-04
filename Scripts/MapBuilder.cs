using Godot;
using System;
using System.Collections.Generic;
using EventCallback;


public enum RoomDirection
{
    FRONT,
    BACK,
    LEFT,
    RIGHT,
    TOP,
    BOTTOM
};

//When the room spawns the starting room already has an exit
public class MapBuilder : Spatial
{
    //List to keep rooms in
    List<Spatial> roomList = new List<Spatial>();
    //The reference to the room scene
    PackedScene roomScene = new PackedScene();
    //The node for the room
    Spatial room;
    RoomDirection lastRoomDirection;
    RoomDirection newRoomDirection;
    //Last rooms position, we set it to zero now because the starting room is always at 0,0,0 
    Vector3 lastLacation = Vector3.Zero;
    //A random number generator to place the rooms at a random order
    RandomNumberGenerator rng = new RandomNumberGenerator();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //Get the room scene
        roomScene = ResourceLoader.Load("res://Scene/BaseRoom.tscn") as PackedScene;
        //Place the first room
        GenerateRoom(new Vector3(-50, 0, 0));
        //We placed the room manual in the front so we set it manualy
        lastRoomDirection = RoomDirection.FRONT;
        //Randomly seed random number generator when starting the game
        rng.Seed = OS.GetTicksMsec();
    }
    //Take a step in the program so that we get the location of a room
    private void TakeStep(RoomDirection lastDirection)
    {
        do
        {
            //Get the random direction from the random number generator
            newRoomDirection = (RoomDirection)rng.RandiRange(0, 5);
        } while (newRoomDirection != lastRoomDirection);
        //If we get a valid location we set the old location to the new lacation for the next interation
        lastRoomDirection = newRoomDirection;

        //calculate the the new position depending on the placement 
        //Vector3 newLocation = lastLacation +
        //GenerateRoom()
    }
    private void GenerateRoom(Vector3 pos)
    {
        //Reseed the random number generator every time we have to place  room
        rng.Seed = OS.GetTicksMsec();
        //Instance the room and set the room spatial to it so we can edit the spawn position of the room
        room = (Spatial)roomScene.Instance();
        //Set the position of hte room
        //room.Transform = new Transform(room.Transform.basis, pos);
        //Add the room to the room list
        //roomList.Add(room);
        //Set the room as a child of the map builder
        AddChild(room);
        //Set the last placed rooms ocation so when we place the next one we use this location to calculate the new position of the new room
        lastLacation = pos;
    }

    private void ClearRooms()
    {
        foreach (Spatial room in roomList)
        {
            room.QueueFree();
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        //If the number of rooms drops below 5 then we generate a new one and place it
        if (roomList.Count < 5)
        {
            //TakeStep(lastRoomDirection);
        }
    }
}
