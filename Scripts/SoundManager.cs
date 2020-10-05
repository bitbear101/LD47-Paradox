using Godot;
using System;
using EventCallback;

public class SoundManager : Node
{
    PackedScene musicScene = new PackedScene();
    Node music;
    PackedScene sfxScene = new PackedScene();
    Node sfx;

    public override void _Ready()
    {
        //Load the music scene that contains all the songs
        musicScene = ResourceLoader.Load("res://Scenes/Music.tscn") as PackedScene;
        music = musicScene.Instance();
        AddChild(music);
        //Load hte sfx scene that contains all the sfx
        sfxScene = ResourceLoader.Load("res://Scenes/Sfx.tscn") as PackedScene;
        sfx = sfxScene.Instance();
        AddChild(sfx);

        //Register the hit event for sound feedback on triggering
        HitEvent.RegisterListener(Hit);
        //Register to the rest of the events that could use sounds as quees
    }

    private void Hit(HitEvent hit)
    {
        //Play hit sound depending on who hit who
    }

    public override void _ExitTree()
    {
        HitEvent.UnregisterListener(Hit);
    }

}
