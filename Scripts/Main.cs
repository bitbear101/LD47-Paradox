using Godot;
using System;
using EventCallback;

public class Main : Spatial
{
    //scene for the menu
    PackedScene menuScene = new PackedScene();
    Control menu;
    PackedScene inputHandlerScene = new PackedScene();
    Node inputHandler;
    //Scene for the soundmanager
    PackedScene soundManagerScene = new PackedScene();
    Node soundManager;
    PackedScene playerScene = new PackedScene();
    KinematicBody player;
    PackedScene OrbScene = new PackedScene();
    KinematicBody orb;
    PackedScene StartRoomScene = new PackedScene();
    Spatial startRoom;
    PackedScene mapBuilderScene = new PackedScene();
    Spatial mapBuilder;

    PackedScene winScene = new PackedScene();
    Spatial winScreen;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        StartPressedEvent.RegisterListener(StartListener);

        menuScene = ResourceLoader.Load("res://Scenes/Menu.tscn") as PackedScene;
        inputHandlerScene = ResourceLoader.Load("res://Scenes/InputHandler.tscn") as PackedScene;
        soundManagerScene = ResourceLoader.Load("res://Scenes/SoundManager.tscn") as PackedScene;
        playerScene = ResourceLoader.Load("res://Scenes/Player.tscn") as PackedScene;
        OrbScene = ResourceLoader.Load("res://Scenes/Orb.tscn") as PackedScene;
        StartRoomScene = ResourceLoader.Load("res://Scenes/StartRoom.tscn") as PackedScene;
        mapBuilderScene = ResourceLoader.Load("res://Scenes/MapBuilder.tscn") as PackedScene;
        winScene = ResourceLoader.Load("res://Scenes/WinScene.tscn") as PackedScene;
        Init();
    }

    private void Init()
    {
        menu = (Control)menuScene.Instance();
        soundManager = soundManagerScene.Instance();
        inputHandler = inputHandlerScene.Instance();
    }

    private void StartListener(StartPressedEvent spei)
    {
        InitGame();
    }

    private void InitGame()
    {
        player = (KinematicBody)playerScene.Instance();
        orb = (KinematicBody)OrbScene.Instance();
        startRoom = (Spatial)StartRoomScene.Instance();
        mapBuilder = (Spatial)mapBuilderScene.Instance();

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
