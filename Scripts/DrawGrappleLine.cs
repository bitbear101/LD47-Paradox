using Godot;
using System;

public class DrawGrappleLine : ImmediateGeometry
{
    /*
    Vector3 origin, target;

    public void DrawLine(Vector3 _origin, Vector3 _target)
    {
        origin = _origin;
        target = _target;
    }

    public override void _Process(float delta)
    {
        Clear();
        Begin(Mesh.PrimitiveType.Triangles);
        AddVertex(origin);
        AddVertex(target);
        End();
    }
    */

    [Export] public float length = 10.0f;
    [Export] public float maxRadius = 0.5f;
    [Export] public int densityLengthwise = 25;
    [Export] public int densityAround = 5;
    [Export] public float shape = 0;
    [Export] public Godot.Collections.Array<Vector3> points = new Godot.Collections.Array<Vector3>();
    [Export] public float segmentLength = 1.0f;

    public override void _Ready()
    {
        //The Length of the trail
        if (length <= 0) length = 2;
        //How many sides the trail will have
        if (densityAround < 3) densityAround = 3;
        //How many segments the trail will have
        if (densityLengthwise < 2) densityLengthwise = 2;
        //We get the segment length by deviding the lentghth of the trail by the amount of desired segments
        segmentLength = length / densityLengthwise;
        //We add the Transforms origin vector3 into the array for the amount of segements we want in the trail
        //Loop through the amount of desired segements
        for (int i = 0; i < densityLengthwise; i++)
        {
            //Add the global transforms origin vector3 as the value
            points.Add(GlobalTransform.origin);
        }
    }

    public override void _Process(float delta)
    {
        UpdateTrail();
        RenderTrail();
    }

    public void UpdateTrail()
    {
        int i = 0;
        Vector3 lastP = GlobalTransform.origin;
        foreach (Vector3 p in points)
        {
            Vector3 v = p; // We can't assign to foreach iterators in C#.
            //The distance from the the point p(now v) to the position of the last segment created
            float dis = v.DistanceTo(lastP);
            //We set this segments length that we calculated aobve in _Ready()
            float segLen = segmentLength;
            //If the are at the first segment of the trail
            if (i == 0)
            {
                //We set the defualt segment to 0.05f
                segLen = 0.05f;
            }
            //If the distance of the segments point from the previous point is more than the current segements length
            if (dis > segLen)
            {
                //We se the point v equal to the last segements position that is (added to the current point v position minus the last points position)
                //devided by the distacnce between the last and new segment times the segments length 
                v = lastP + (v - lastP) / dis * segLen;
            }
            //Set our last positions point to the current point before we restart teh loop
            lastP = v;
            //we set the old point inside the array to the new point of the segment 
            points[i] = v;
            //We manually interate i as it is outside the loop
            i += 1;
        }
    }

    public void RenderTrail()
    {
        //Clears all the drawn geometry the imediate draw node has drawn the previous frame
        Clear();
        //Begin the imidiate draw mode with the primative mode paased into it 
        Begin(Mesh.PrimitiveType.Triangles);
        //A godot array for the Vector3 point called local points
        Godot.Collections.Array<Vector3> localPoints = new Godot.Collections.Array<Vector3>();
        //Loop throuhgt hte points array again
        foreach (Vector3 p in points)
        {
            //We add the points members to the local points array after we subtracted the global transform from the point p
            localPoints.Add(p - GlobalTransform.origin);
        }
        //Create a new last point variable for use int this method
        Vector3 lastP = Vector3.Zero;
        //Create a new godot array for the verticies
        Godot.Collections.Array<Godot.Collections.Array<Vector3>> verts = new Godot.Collections.Array<Godot.Collections.Array<Vector3>>();

        int ind = 0;
        //If its the first interation of the loop below, here we just "reset" the firstInteration bool for use in the loop
        bool firstIteration = true;
        //Zero the lastFirstVec value
        Vector3 lastFirstVec = Vector3.Zero;
        //Loopthrough the localPoint array
        foreach (Vector3 p in localPoints)
        {
            //New godot array for the new last points
            Godot.Collections.Array<Vector3> newLastPoints = new Godot.Collections.Array<Vector3>();
            //Variable for the offset of the last point with the current point (Vector points from lastP to p)
            var offset = lastP - p;
            //If there is no offset
            if (offset == Vector3.Zero)
            {
                //We continue with the code
                continue;
            }
            Vector3 yVec = offset.Normalized(); // get vector pointing from this point to last point
            //Set het x Vector to zero
            Vector3 xVec = Vector3.Zero;
            //If this is the first ineration of the loop (First time the loop is run)
            if (firstIteration)
            {
                xVec = yVec.Cross(yVec.Rotated(Vector3.Right, 0.3f)); //cross product with random vector to get a perpendicular vector
            }
            else
            {
                
                xVec = yVec.Cross(lastFirstVec).Cross(yVec).Normalized(); // keep each loop at the same rotation as the previous
            }
            var width = maxRadius;
            if (shape != 0)
            {
                width = (1 - Mathf.Ease((ind + 1.0f) / densityLengthwise, shape)) * maxRadius;
            }
            Godot.Collections.Array<Vector3> segVerts = new Godot.Collections.Array<Vector3>();
            var fIter = true;
            for (int i = 0; i < densityAround; i++) // set up row of verts for each level
            {
                var newVert = p + width * xVec.Rotated(yVec, i * 2 * Mathf.Pi / densityAround).Normalized();
                if (fIter)
                {
                    lastFirstVec = newVert - p;
                    fIter = false;
                }
                segVerts.Add(newVert);
            }
            verts.Add(segVerts);
            lastP = p;
            ind += 1;
            firstIteration = false;
        }
        for (int j = 0; j < verts.Count - 1; j++)
        {

            var cur = verts[j];
            var nxt = verts[j + 1];
            for (int i = 0; i < densityAround; i++)
            {
                var nxtI = (i + 1) % densityAround;
                //order added affects normal
                AddVertex(cur[i]);
                AddVertex(cur[nxtI]);
                AddVertex(nxt[i]);
                AddVertex(cur[nxtI]);
                AddVertex(nxt[nxtI]);
                AddVertex(nxt[i]);
            }
        }
        if (verts.Count > 1)
        {
            for (int i = 0; i < densityAround; i++)
            {
                var nxt = (i + 1) % densityAround;
                AddVertex(verts[0][i]);
                AddVertex(Vector3.Zero);
                AddVertex(verts[0][nxt]);
            }

            for (int i = 0; i < densityAround; i++)
            {
                var nxt = (i + 1) % densityAround;
                AddVertex(verts[verts.Count - 1][i]);
                AddVertex(verts[verts.Count - 1][nxt]);
                AddVertex(lastP);
            }
        }
        End();
    }

}