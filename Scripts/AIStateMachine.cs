using Godot;
using System;

public enum STATE
{
    IDLE,
    CHASE
};
public class AIStateMachine : KinematicBody
{

    KinematicBody player;
    //Set the orbs state to idle when creating it
    STATE state = STATE.IDLE;

    public override void _Ready()
    {
        player = GetNode<KinematicBody>("../PlayerBody");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        switch (state)
        {
            case STATE.IDLE:
                break;

            case STATE.CHASE:

                //Check the distance from the players position to the hook point of the grapple
                //GD.Print("Distance to hook point = " + Transform.origin.DistanceTo(hookPoint));
                if (Transform.origin.DistanceTo(player.Transform.Position) < 1.5f)
                {
                    Transform = new Transform(Transform.basis, Transform.origin.LinearInterpolate(player.Transform.Position, maxGrappleSpeed * delta));
                }
                break;
        }
    }
}
