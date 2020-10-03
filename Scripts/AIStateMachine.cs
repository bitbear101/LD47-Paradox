using Godot;
using System;
using EventCallback;

public enum ORB_STATE
{
    IDLE,
    CHASE
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
                break;
        }
    }

    private void SetState(SetStateEvent ssei)
    {
        //Set the new state of the orb
state = ssei.newState;
    }
}
