using Godot;
using System;
using EventCallback;

public enum ExitLoction
{
    FRONT,
    BACK,
    LEFT,
    RIGHT,
    ROOF,
    FLOOR
};

public class RoomBuilder : Spatial
{
    //BUILD NOTES:
    //1. The starting room already has an exit wall -- Done 
    //2. When this room spawn detect witch side is adjasent to the exit wall
    //3. The room then deletes its own wall on the exit walls side
    //4. Then a random direction is chosen and this rooms exit wall is built on that side
    //4.1 The wall side with the already adjecent exit wall direction is removed so we don''t spawn an exit to an exit
    //5. The exit and entrance is then taken into acount and the room is then filled with the apropriate platforms
    //5.1 Room openings (exit and entrance) bool topOpen, bottomOpen, leftOpen

    //The exit location for this room
    ExitLoction myExit;
    //Room openings to detirmine the platforms that are needed for the room
    bool roofOpen = false, floorOpen = false, leftOpen = false, rightOpen = false, frontOpen = false, backOpen = false;
    //The rays to detect the the wall of the adjecent rooms
    RayCast floorRay, roofRay, frontRay, backRay, rightRay, leftRay;
    //The walls for the room
    KinematicBody frontWall, backWall, leftWall, rightWall, roofWall, floorWall;
    //Theme pak for the scene
    PackedScene exitWallPack = new PackedScene();
    //The body for the wall
    KinematicBody exitWall;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LoadResources();
        InitResources();
        AdjacentRoom();
    }

    private void LoadResources()
    {
        //Init he raycasts of the room walls used for creating the room
        roofRay = GetNode<RayCast>("RoofRay");
        floorRay = GetNode<RayCast>("FloorRay");
        frontRay = GetNode<RayCast>("FrontRay");
        backRay = GetNode<RayCast>("BackRay");
        leftRay = GetNode<RayCast>("LeftRay");
        rightRay = GetNode<RayCast>("RightRay");
        //Get the reference to the walls
        frontWall = GetNode<KinematicBody>("FrontWall");
        backWall = GetNode<KinematicBody>("BackWall");
        leftWall = GetNode<KinematicBody>("LeftWall");
        rightWall = GetNode<KinematicBody>("RightWall");
        roofWall = GetNode<KinematicBody>("Roof");
        floorWall = GetNode<KinematicBody>("Floor");
        //Grab the reference to the exit walls scene
        exitWallPack = ResourceLoader.Load("res://Scenes/ExitWall.tscn") as PackedScene;
    }

    private void InitResources()
    {
        //exitWall = (KinematicBody)exitWallPack.Instance();
        //AddChild(exitWall);
    }

    private void AdjacentRoom()
    {
        if (roofRay.IsColliding())
        {
            //Set the open flag on the wall making contact with the previous room 
            roofOpen = true;
            //Delete the wall to open it up to the adjacent rooms exit
            roofWall.QueueFree();
        }
        if (floorRay.IsColliding())
        {
            //Set the open flag on the wall making contact with the previous room 
            floorOpen = true;
            //Delete the wall to open it up to the adjacent rooms exit
            floorWall.QueueFree();
        }
        if (frontRay.IsColliding())
        {
            //Set the open flag on the wall making contact with the previous room 
            frontOpen = true;
            //Delete the wall to open it up to the adjacent rooms exit
            frontWall.QueueFree();
        }
        if (backRay.IsColliding())
        {
            //Set the open flag on the wall making contact with the previous room 
            backOpen = true;
            //Delete the wall to open it up to the adjacent rooms exit
            backWall.QueueFree();
        }
        if (leftRay.IsColliding())
        {
            //Set the open flag on the wall making contact with the previous room 
            leftOpen = true;
            //Delete the wall to open it up to the adjacent rooms exit
            leftWall.QueueFree();
        }
        if (rightRay.IsColliding())
        {
            //Set the open flag on the wall making contact with the previous room 
            rightOpen = true;
            //Delete the wall to open it up to the adjacent rooms exit
            rightWall.QueueFree();
        }

        GD.Print(roofOpen +" "+ floorOpen +" "+ leftOpen +" "+ rightOpen +" "+ frontOpen +" "+ backOpen);
    }

    private void PlaceExitWall(ExitLoction location)
    {
        switch (myExit)
        {
            case ExitLoction.BACK:
                backWall.QueueFree();
                //Instance the exit wall her and position it to align
                break;
            case ExitLoction.FRONT:
                frontWall.QueueFree();
                break;
            case ExitLoction.LEFT:
                leftWall.QueueFree();
                break;
            case ExitLoction.RIGHT:
                rightWall.QueueFree();
                break;
            case ExitLoction.ROOF:
                roofWall.QueueFree();
                break;
            case ExitLoction.FLOOR:
                floorWall.QueueFree();
                break;
        }

    }

    private void InitExitWall()
    {

    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }
}
