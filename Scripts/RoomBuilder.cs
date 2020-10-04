using Godot;
using System;

public class RoomBuilder : Spatial
{

    KinematicBody frontWall;
    KinematicBody backWall;
    KinematicBody leftWall;
    KinematicBody rightWall;
    KinematicBody topWall;
    KinematicBody bottomWall;
    //Theme pak for the scene
    PackedScene exitWallPack = new PackedScene();
    //The body for the wall
    KinematicBody exitWall;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LoadResources();
        InitResources();
    }

    private void LoadResources()
    {
        frontWall = GetNode<KinematicBody>("FrontWall");
        backWall = GetNode<KinematicBody>("BackWall");
        leftWall = GetNode<KinematicBody>("LeftWall");
        rightWall = GetNode<KinematicBody>("RightWall");
        topWall = GetNode<KinematicBody>("Roof");
        bottomWall = GetNode<KinematicBody>("Floor");
        exitWallPack = ResourceLoader.Load("res://Scenes/ExitWall.tscn") as PackedScene;
    }

    private void InitResources()
    {
        exitWall = (KinematicBody)exitWallPack.Instance();
        AddChild(exitWall);
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }
}
