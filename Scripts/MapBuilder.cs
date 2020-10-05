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
    List<Vector3> roomPositions = new List<Vector3>();
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
        roomScene = ResourceLoader.Load("res://Scenes/BaseRoom.tscn") as PackedScene;
        //Place the first room
        GenerateRoom(new Vector3(-50, 0, 0));
        //Add the room to the room positions
        roomPositions.Add(new Vector3(-50, 0, 0));
        //We placed the room manual in the front so we set it manualy
        newRoomDirection = RoomDirection.FRONT;
        lastRoomDirection = newRoomDirection;
        //Randomly seed random number generator when starting the game
        rng.Seed = OS.GetTicksMsec();
    }
    //Take a step in the program so that we get the location of a room
    private void TakeStep(RoomDirection lastDirection)
    {

        //If we get a valid location we set the old location to the new lacation for the next interation
        lastRoomDirection = newRoomDirection;
        //Init the new location to zero
        Vector3 newLocation = Vector3.Zero;
        bool locationGood = false;
        do
        {
            //Stores the temp location until it verified it is open
            Vector3 tempLocation = new Vector3();
            //Loop through the random directions until one is chosen that is ok
            do
            {
                //Get the random direction from the random number generator
                newRoomDirection = (RoomDirection)rng.RandiRange(0, 5);
            } while (newRoomDirection == lastRoomDirection);
            //calculate the the new position depending on the placement 
            switch (newRoomDirection)
            {
                case RoomDirection.BACK:
                    tempLocation = lastLacation + new Vector3(50, 0, 0);
                    break;
                case RoomDirection.FRONT:
                    tempLocation = lastLacation + new Vector3(-50, 0, 0);
                    break;
                case RoomDirection.LEFT:
                    tempLocation = lastLacation + new Vector3(0, 0, 50);
                    break;
                case RoomDirection.RIGHT:
                    tempLocation = lastLacation + new Vector3(0, 0, -50);
                    break;
                case RoomDirection.TOP:
                    tempLocation = lastLacation + new Vector3(0, 50, 0);
                    break;
                case RoomDirection.BOTTOM:
                    tempLocation = lastLacation + new Vector3(0, -50, 0);
                    break;
            }
            //Loop through the locations and chehk if they are going to be adjacent
            //if they are not going to be adjecant then set locationGood to true

            //8 hours left and I'm doing this without optimization, what am I doing with my life!? Hope it work first time????????
            foreach (Vector3 location in roomPositions)
            {
                //We set the lacationGood to true if any collisions of rooms are present it will be set to false for the remainder of the loop
                locationGood = true;
                if ((tempLocation + new Vector3(0, 50, 0)) == location)
                {
                    locationGood = false;
                }
                if ((tempLocation + new Vector3(0, -50, 0)) == location)
                {
                    locationGood = false;
                }
                if ((tempLocation + new Vector3(50, 0, 0)) == location)
                {
                    locationGood = false;
                }
                if ((tempLocation + new Vector3(-50, 0, 0)) == location)
                {
                    locationGood = false;
                }
                if ((tempLocation + new Vector3(0, 0, 50)) == location)
                {
                    locationGood = false;
                }
                if ((tempLocation + new Vector3(0, 0, -50)) == location)
                {
                    locationGood = false;
                }
            }
            //If the location is good we set the new location to the tempLacation 
            if (locationGood) newLocation = tempLocation;
            //If the lacation is good we exit out of the loop, I hope
        } while (!locationGood);
        //Add the room locations so that the can be cros compared so we dont make rooms adjecent to each other
        roomPositions.Add(newLocation);
        //Set the last placed rooms ocation so when we place the next one we use this location to calculate the new position of the new room
        lastLacation = newLocation;
        GenerateRoom(newLocation);
    }
    private void GenerateRoom(Vector3 pos)
    {
        //Send signal to room of where the exit needs to go ==============
        PlaceExitEvent peei = new PlaceExitEvent();
        peei.newLocation = newRoomDirection;
        peei.FireEvent();
        //================================================================
        //Reseed the random number generator every time we have to place  room
        rng.Seed = OS.GetTicksMsec();
        //Instance the room and set the room spatial to it so we can edit the spawn position of the room
        room = (Spatial)roomScene.Instance();
        //Set the position of hte room
        room.Transform = new Transform(room.Transform.basis, pos);
        //Add the room to the room list
        roomList.Add(room);
        //Set the room as a child of the map builder
        AddChild(room);
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
            TakeStep(lastRoomDirection);
        }
    }
}
