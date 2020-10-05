using Godot;
using System;
using EventCallback;

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

    //Room openings to detirmine the platforms that are needed for the room
    bool roofOpen = false, floorOpen = false, leftOpen = false, rightOpen = false, frontOpen = false, backOpen = false;
    //The rays to detect the the wall of the adjecent rooms
    RayCast floorRay, roofRay, frontRay, backRay, rightRay, leftRay;
    //The walls for the room
    KinematicBody frontWall, backWall, leftWall, rightWall, roofWall, floorWall;
    //The platforms that make up the interior
    KinematicBody platformSet1, platformSet2, platformSet3, platformSet4, platformSet5, platformSet6, platformSet7;
    //The scenes for the platform
    PackedScene platformSet1Scene = new PackedScene();
    PackedScene platformSet2Scene = new PackedScene();
    PackedScene platformSet3Scene = new PackedScene();
    PackedScene platformSet4Scene = new PackedScene();
    PackedScene platformSet5Scene = new PackedScene();
    PackedScene platformSet6Scene = new PackedScene();
    PackedScene platformSet7Scene = new PackedScene();
    //Theme pak for the scene
    PackedScene exitWallScene = new PackedScene();
    //The body for the wall
    KinematicBody exitWall;
    //Scheck for the exit placement
    bool checkedForExits = false, exitPlaced = false;
    //Random number generator to choose the platform set for the room
    RandomNumberGenerator rng = new RandomNumberGenerator();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LoadResources();
        GeneratePlatformSet();
    }
    private void LoadResources()
    {
        //Regestir for the event callback for the placing of the rooms
        PlaceExitEvent.RegisterListener(InitExitWall);
        //Init he raycasts of the room walls used for creating the room
        roofRay = GetNode<RayCast>("Roof/RoofRay");
        floorRay = GetNode<RayCast>("Floor/FloorRay");
        frontRay = GetNode<RayCast>("FrontWall/FrontRay");
        backRay = GetNode<RayCast>("BackWall/BackRay");
        leftRay = GetNode<RayCast>("LeftWall/LeftRay");
        rightRay = GetNode<RayCast>("RightWall/RightRay");
        //Get the reference to the walls
        frontWall = GetNode<KinematicBody>("FrontWall");
        backWall = GetNode<KinematicBody>("BackWall");
        leftWall = GetNode<KinematicBody>("LeftWall");
        rightWall = GetNode<KinematicBody>("RightWall");
        roofWall = GetNode<KinematicBody>("Roof");
        floorWall = GetNode<KinematicBody>("Floor");
        //Get the refferences to the platform sets
        platformSet1Scene = ResourceLoader.Load("res://Scenes/PlatformSet1.tscn") as PackedScene;
        platformSet2Scene = ResourceLoader.Load("res://Scenes/PlatformSet2.tscn") as PackedScene;
        platformSet3Scene = ResourceLoader.Load("res://Scenes/PlatformSet3.tscn") as PackedScene;
        platformSet4Scene = ResourceLoader.Load("res://Scenes/PlatformSet4.tscn") as PackedScene;
        platformSet5Scene = ResourceLoader.Load("res://Scenes/PlatformSet5.tscn") as PackedScene;
        platformSet6Scene = ResourceLoader.Load("res://Scenes/PlatformSet6.tscn") as PackedScene;
        platformSet7Scene = ResourceLoader.Load("res://Scenes/PlatformSet7.tscn") as PackedScene;
        //Grab the reference to the exit walls scene
        exitWallScene = ResourceLoader.Load("res://Scenes/ExitWall.tscn") as PackedScene;
    }
    private void GeneratePlatformSet()
    {
        //3hours and 31min left and this decides to work perfectly the first time round, why not hte loos 4 hours ago WHYYYYYYY!?!??!1?!!
        //Seed the random number generator
        rng.Seed = OS.GetTicksUsec();
        int platformSet = rng.RandiRange(1, 7);

        switch (platformSet)
        {
            case 1:
                platformSet1 = (KinematicBody)platformSet1Scene.Instance();
                AddChild(platformSet1);
                break;
            case 2:
                platformSet2 = (KinematicBody)platformSet2Scene.Instance();
                AddChild(platformSet2);
                break;
            case 3:
                platformSet3 = (KinematicBody)platformSet3Scene.Instance();
                AddChild(platformSet3);
                break;
            case 4:
                platformSet4 = (KinematicBody)platformSet4Scene.Instance();
                AddChild(platformSet4);
                break;
            case 5:
                platformSet5 = (KinematicBody)platformSet5Scene.Instance();
                AddChild(platformSet5);
                break;
            case 6:
                platformSet6 = (KinematicBody)platformSet6Scene.Instance();
                AddChild(platformSet6);
                break;
            case 7:
                platformSet7 = (KinematicBody)platformSet7Scene.Instance();
                AddChild(platformSet7);
                break;
        }
    }
    private void CheckForExits()
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
    }

    private void PlaceExitWall(RoomDirection location)
    {
        //Swing the directions back should become forward and so on
        if (exitPlaced) return;
        switch (location)
        {
            case RoomDirection.BACK:
                //Create the exit wall
                exitWall = (KinematicBody)exitWallScene.Instance();
                exitWall.Transform = new Transform(exitWall.Transform.basis, backWall.Transform.origin);
                exitWall.RotateY(Mathf.Deg2Rad(90));
                AddChild(exitWall);
                backWall.QueueFree();
                //Instance the exit wall her and position it to align
                break;
            case RoomDirection.FRONT:
                //Create the exit wall
                exitWall = (KinematicBody)exitWallScene.Instance();
                exitWall.Transform = new Transform(exitWall.Transform.basis, frontWall.Transform.origin);
                exitWall.RotateY(Mathf.Deg2Rad(-90));
                AddChild(exitWall);
                frontWall.QueueFree();
                break;
            case RoomDirection.LEFT:
                //Create the exit wall
                exitWall = (KinematicBody)exitWallScene.Instance();
                exitWall.Transform = new Transform(exitWall.Transform.basis, leftWall.Transform.origin);
                AddChild(exitWall);
                leftWall.QueueFree();
                break;
            case RoomDirection.RIGHT:
                //Create the exit wall
                exitWall = (KinematicBody)exitWallScene.Instance();
                exitWall.Transform = new Transform(exitWall.Transform.basis, rightWall.Transform.origin);
                AddChild(exitWall);
                rightWall.QueueFree();
                break;
            case RoomDirection.TOP:
                //Create the exit wall
                exitWall = (KinematicBody)exitWallScene.Instance();
                exitWall.Transform = new Transform(exitWall.Transform.basis, roofWall.Transform.origin);
                exitWall.RotateX(Mathf.Deg2Rad(90));
                AddChild(exitWall);
                roofWall.QueueFree();
                break;
            case RoomDirection.BOTTOM:
                //Create the exit wall
                exitWall = (KinematicBody)exitWallScene.Instance();
                exitWall.Transform = new Transform(exitWall.Transform.basis, floorWall.Transform.origin);
                exitWall.RotateX(Mathf.Deg2Rad(-90));
                AddChild(exitWall);
                floorWall.QueueFree();
                break;
        }
    }

    private void InitExitWall(PlaceExitEvent peei)
    {
        //Where the exit needs to go
        PlaceExitWall(peei.newLocation);
        //We have to do this so that if the message to place a exit is sent to the newest room that this room does not also place antother exit util the whole room is just exits
        //To bad the player will need to figure out there is no exit ;)
        exitPlaced = true;
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        //Run the function only once, yes I know there are better ways to do this but I can't think of one now and my bag of fucks is empty so I cant gave any
        if (!checkedForExits)
        {
            //Set the checked bool to true so it only runs once in its life, sorry function
            checkedForExits = true;
            //Check for adjacent rooms
            CheckForExits();
        }
    }
}
