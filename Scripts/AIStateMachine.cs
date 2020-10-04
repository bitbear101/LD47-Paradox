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
    [Export]
    int speed = 125;
    KinematicBody player;
    //Set the orbs state to idle when creating it
    ORB_STATE state = ORB_STATE.IDLE;

    public override void _Ready()
    {
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
                break;

            case ORB_STATE.CHASE:
            //Looks at the player
             LookAt(player.GlobalTransform.origin, Vector3.Up);
            //Check if the distance from the player is more than 4 units
                if (Transform.origin.DistanceTo(player.Transform.origin) > 3f)
                {
                    //If the distance from the player is more than 4 units we declare velocity and start moving towards the player
                    Vector3 velocity = Vector3.Zero;
                    //velocity = (player.GlobalTransform.origin - GlobalTransform.origin).Normalized() * speed * delta;
                    velocity = GlobalTransform.origin.LinearInterpolate(player.GlobalTransform.origin, speed * delta);
                    MoveAndSlide(velocity, Vector3.Up);
                }
                //Check if the orb location is further than 20 units from the player
                if(Transform.origin.DistanceTo(player.Transform.origin) > 30f)
                {
                    //Teleport the orb to the players location is it is further than 20 units, this stops the orb getting stuck problem
                    //what can I say there is nothing like a good brute force solution to game jam games!
                    Transform = new Transform(Transform.basis, player.Transform.origin + (Vector3.Up * 3)); 
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
