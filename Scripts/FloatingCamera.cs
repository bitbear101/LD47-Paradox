using Godot;
using System;

public class FloatingCamera : Spatial
{
    //The sensitivity of the mouse, how fast it turns
    float mouseSensitivity = 0.1f;
    //The instance of the camera
    Camera camera;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //Grab the refference to the camera
        camera = GetNode<Camera>("Camera");
        Input.SetMouseMode(Input.MouseMode.Captured);

    }

    public override void _Input(InputEvent @event)
    {

        if (Input.IsKeyPressed((int)KeyList.Escape))
        {
            if (Input.GetMouseMode() == Input.MouseMode.Visible)
                Input.SetMouseMode(Input.MouseMode.Captured);
            else
                Input.SetMouseMode(Input.MouseMode.Visible);
        }

        if (@event is InputEventMouseMotion eventMouseMotion && Input.GetMouseMode() == Input.MouseMode.Captured)
        {

            RotateX(Mathf.Deg2Rad(-eventMouseMotion.Relative.y * mouseSensitivity));
            RotateY(Mathf.Deg2Rad(-eventMouseMotion.Relative.x * mouseSensitivity));

            Vector3 camRotation = RotationDegrees;
            camRotation.x = Mathf.Clamp(camRotation.x, -80f, 80f);
            RotationDegrees = camRotation;
        }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
