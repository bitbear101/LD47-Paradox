using Godot;
using System;

public class DrawGrappleLine : ImmediateGeometry
{
    [Export]
    Vector3 origin, target;

    public void DrawLine(Vector3 _origin, Vector3 _target)
    {
        origin = _origin;
        target = _target;
    }

    public override void _Process(float delta)
    {
        Clear();
        Begin(Mesh.PrimitiveType.Lines);
        SetColor(new Color(1,0,1));
        AddVertex(origin);
        AddVertex(target);
        End();
    }
}