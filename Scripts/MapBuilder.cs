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
    Vector3 lastLocation = Vector3.Zero;
    //A random number generator to place the rooms at a random order
    RandomNumberGenerator rng = new RandomNumberGenerator();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //Add the start room to the list as well
        roomPositions.Add(new Vector3(0, 0, 0));
        //Get the room scene
        roomScene = ResourceLoader.Load("res://Scenes/BaseRoom.tscn") as PackedScene;
        //Place the first room
        GenerateRoom(new Vector3(-50, 0, 0));
        //Add the room to the room positions
        roomPositions.Add(new Vector3(-50, 0, 0));
        //Set the last location
        lastLocation = new Vector3(-50, 0, 0);
        //We placed the room manual in the front so we set it manualy
        newRoomDirection = RoomDirection.FRONT;
        lastRoomDirection = newRoomDirection;
        //Randomly seed random number generator when starting the game
        rng.Seed = OS.GetTicksMsec();
    }
    //Take a step in the program so that we get the location of a room
    private void TakeStep()
    {
        //Init the new location to zero
        Vector3 newLocation = Vector3.Zero;
        //If the new location is accepteble we set it to true after the check loops
        bool locationGood = false;

        do
        {
            //Stores the temp location until it verified it is open
            Vector3 tempLocation = new Vector3();
            //check to exit the direction finding loop
            bool directionGood = false;
            do
            {

                //Seed the random generator
                rng.Seed = OS.GetTicksUsec();
                //Get the random direction from the random number generator
                newRoomDirection = (RoomDirection)rng.RandiRange(0, 5);

                directionGood = true;
                if (lastRoomDirection == RoomDirection.FRONT && newRoomDirection == RoomDirection.BACK) directionGood = false;
                if (lastRoomDirection == RoomDirection.BACK && newRoomDirection == RoomDirection.FRONT) directionGood = false;
                if (lastRoomDirection == RoomDirection.TOP && newRoomDirection == RoomDirection.BOTTOM) directionGood = false;
                if (lastRoomDirection == RoomDirection.BOTTOM && newRoomDirection == RoomDirection.TOP) directionGood = false;
                if (lastRoomDirection == RoomDirection.LEFT && newRoomDirection == RoomDirection.RIGHT) directionGood = false;
                if (lastRoomDirection == RoomDirection.RIGHT && newRoomDirection == RoomDirection.LEFT) directionGood = false;

            } while (!directionGood);


            //calculate the the new position depending on the placement 
            switch (newRoomDirection)
            {
                case RoomDirection.BACK:
                    tempLocation = lastLocation + new Vector3(50, 0, 0);
                    break;
                case RoomDirection.FRONT:
                    tempLocation = lastLocation + new Vector3(-50, 0, 0);
                    break;
                case RoomDirection.LEFT:
                    tempLocation = lastLocation + new Vector3(0, 0, 50);
                    break;
                case RoomDirection.RIGHT:
                    tempLocation = lastLocation + new Vector3(0, 0, -50);
                    break;
                case RoomDirection.TOP:
                    tempLocation = lastLocation + new Vector3(0, 50, 0);
                    break;
                case RoomDirection.BOTTOM:
                    tempLocation = lastLocation + new Vector3(0, -50, 0);
                    break;
            }
            //Loop through the locations and chehk if they are going to be adjacent
            //if they are not going to be adjecant then set locationGood to true

            //8 hours left and I'm doing this without optimization, what am I doing with my life!? Hope it work first time????????
            //PS. It did not work the first time, mother F*CKER!!!!!!!!
            //Working halway only 6 hours left, sfgmalkanva;lsdnla jdnlskefnasl jnv
            for (int i = 0; i < roomPositions.Count; i++)
            {
                //We set the lacationGood to true if any collisions of rooms are present it will be set to false for the remainder of the loop
                locationGood = true;

                if (tempLocation == roomPositions[i])
                {
                    locationGood = false;
                }
                //If we allow the last position a room was spawend to be checked  there will be a false return
                if (lastLocation != roomPositions[i])
                {
                   // if ((tempLocation + new Vector3(0, 50, 0)) == roomPositions[i] || (tempLocation + new Vector3(0, 50, 0)) == lastLocation)
                    if ((tempLocation + new Vector3(0, 50, 0)) == roomPositions[i])
                    {
                        locationGood = false;
                    }
                    //if ((tempLocation + new Vector3(0, -50, 0)) == roomPositions[i] || (tempLocation + new Vector3(0, -50, 0)) == lastLocation)
                    if ((tempLocation + new Vector3(0, -50, 0)) == roomPositions[i])
                    {
                        locationGood = false;
                    }
                   // if ((tempLocation + new Vector3(50, 0, 0)) == roomPositions[i] || (tempLocation + new Vector3(50, 0, 0)) == lastLocation)
                    if ((tempLocation + new Vector3(50, 0, 0)) == roomPositions[i] )
                    {
                        locationGood = false;
                    }
                    //if ((tempLocation + new Vector3(-50, 0, 0)) == roomPositions[i] || (tempLocation + new Vector3(-50, 0, 0)) == lastLocation)
                    if ((tempLocation + new Vector3(-50, 0, 0)) == roomPositions[i])
                    {
                        locationGood = false;
                    }
                   // if ((tempLocation + new Vector3(0, 0, 50)) == roomPositions[i] || (tempLocation + new Vector3(0, 0, 50)) == lastLocation)
                    if ((tempLocation + new Vector3(0, 0, 50)) == roomPositions[i])
                    {
                        locationGood = false;
                    }
                   // if ((tempLocation + new Vector3(0, 0, -50)) == roomPositions[i] || (tempLocation + new Vector3(0, 0, -50)) == lastLocation)
                    if ((tempLocation + new Vector3(0, 0, -50)) == roomPositions[i])
                    {
                        locationGood = false;
                    }
                }
            }
            //If the location is good we set the new location to the tempLacation 
            if (locationGood)
            {
                //If we get a valid location we set the old location to the new lacation for the next interation
                lastRoomDirection = newRoomDirection;
                //Set the new locaion becuase it is now valid
                newLocation = tempLocation;
            }
            //If the lacation is good we exit out of the loop, I hope
        } while (!locationGood);
        //Add the room locations so that the can be cros compared so we dont make rooms adjecent to each other
        roomPositions.Add(newLocation);
        //Set the last placed rooms ocation so when we place the next one we use this location to calculate the new position of the new room
        lastLocation = newLocation;
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
            TakeStep();
        }
    }
}
