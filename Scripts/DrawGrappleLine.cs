using Godot;
using System;

public class DrawGrappleLine : MeshInstance
{
    public void DrawLine(Vector3 _origin, Vector3 _target)
    {
    GD.Print("Calling grappling line");
        //Get the distance between the target and origin
        Vector3 midpoint = (_origin + _target) / 2;
        //Get the orientation of the line
        Vector3 orientation = (_target - _origin).Normalized();
        //The distance between the two points
        float distance = _origin.DistanceTo(_target);

        //Set the height of the cylender to half of the distance 
        ((CylinderMesh)Mesh).Height = distance / 2;
        //Should put the cylinder in the middle of the two vectors
        Transform = new Transform(Transform.basis, midpoint);
        //Or this could work
        //Transform = new Transform(Transform.basis, midpoint * orientation);
        Visible = true;
    }

    public void HideLine()
    {
        Visible = false;
    }



    public override void _Process(float delta)
    {

    }
}