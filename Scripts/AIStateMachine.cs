using Godot;
using System;
using EventCallback;

public enum ORB_STATE
{
    //When the orb is just staring at you and not moving
    IDLE,
    //Chases the player around
    CHASE,
    //Teleport the player back to the starting point
    TELPORT,
    //Scans the player in an ominus way, ill impliment if i get time
    SCAN
};
public class AIStateMachine : KinematicBody
{
    int speed = 1;
    KinematicBody player;
    //Set the orbs state to idle when creating it
    ORB_STATE state = ORB_STATE.IDLE;
    //The point where the player will be teleported back to
    Spatial teleportPoint;
    //The timer for the teleporter, if the countdown reaches zero the player is teleported to the start
    Timer teleportTimer, winTimer;

    public override void _Ready()
    {
        teleportPoint = GetNode<Spatial>("../StartRoom/TeleportPoint");
        teleportTimer = GetNode<Timer>("TeleportTimer");
        winTimer = GetNode<Timer>("WinTimer");
        player = GetNode<KinematicBody>("../PlayerBody");
        SetStateEvent.RegisterListener(SetState);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        switch (state)
        {
            case ORB_STATE.IDLE:
                //Looks at the player
                LookAt(player.GlobalTransform.origin, Vector3.Up);
                //Start the win timer and after 2 min the player will win the game if he doesn't exit the room
                winTimer.Start();
                if (winTimer.TimeLeft < 0.1)
                {
                    //Send win message to winEvent
                    WinEvent wei = new WinEvent();
                    wei.win = true;
                    wei.FireEvent();
                }
                break;

            case ORB_STATE.CHASE:
                //Stop the win timer, the game can never be won now
                winTimer.Stop();
                //Looks at the player
                LookAt(player.GlobalTransform.origin, Vector3.Up);
                //Check if the distance from the player is more than 4 units
                if (Transform.origin.DistanceTo(player.Transform.origin) > 3f)
                {
                    //velocity = (player.GlobalTransform.origin - GlobalTransform.origin).Normalized() * speed * delta;
                    Transform = new Transform(Transform.basis, Transform.origin.LinearInterpolate(player.Transform.origin, speed * delta));
                }
                //Check if the orb location is further than 20 units from the player
                if (Transform.origin.DistanceTo(player.Transform.origin) > 40f)
                {
                    //Teleport the orb to the players location is it is further than 20 units, this stops the orb getting stuck problem
                    //what can I say there is nothing like a good brute force solution to game jam games!
                    Transform = new Transform(Transform.basis, player.Transform.origin + (Vector3.Up * 3));
                }
                if (Transform.origin.DistanceTo(player.Transform.origin) < 5)
                {
                    if (teleportTimer.IsStopped())
                    {
                        teleportTimer.Start();
                    }
                    if (teleportTimer.TimeLeft < 0.1)
                    {
                        player.Transform = new Transform(player.Transform.basis, teleportPoint.Transform.origin);
                    }
                }
                else
                {
                    teleportTimer.Stop();
                }
                break;

            case ORB_STATE.SCAN:
                //just some eye candy I want to apply later if there is time, all I need is more time!
                break;
            case ORB_STATE.TELPORT:
                //We will just animate the orb shooting the player with a teleport lazer or something later
                break;
        }
    }
    private void SetState(SetStateEvent ssei)
    {
        //Set the new state of the orb
        state = ssei.newState;
    }
}
