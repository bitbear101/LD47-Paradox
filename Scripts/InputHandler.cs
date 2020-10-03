using Godot;
using System;
using EventCallback;

public class InputHandler : Node
{
    InputHandleEvent ihei;

    public override void _UnhandledInput(Godot.InputEvent @event)
    {
        ihei = new InputHandleEvent();
        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.Pressed)
            {
                if (eventMouseButton.ButtonIndex == (int)ButtonList.Left) ihei.lmbPressed = true;
                if (eventMouseButton.ButtonIndex == (int)ButtonList.Right) ihei.rmbPressed = true;
            }
            else
            {
                if (eventMouseButton.ButtonIndex == (int)ButtonList.Left) ihei.lmbPressed = false;
                if (eventMouseButton.ButtonIndex == (int)ButtonList.Right) ihei.rmbPressed = false;
            }
        }

        if (Input.IsKeyPressed((int)KeyList.W)) ihei.upPressed = true; else ihei.upPressed = false;
        if (Input.IsKeyPressed((int)KeyList.S)) ihei.downPressed = true; else ihei.downPressed = false;
        if (Input.IsKeyPressed((int)KeyList.A)) ihei.leftPressed = true; else ihei.leftPressed = false;
        if (Input.IsKeyPressed((int)KeyList.D)) ihei.rightPressed = true; else ihei.rightPressed = false;
        if (Input.IsKeyPressed((int)KeyList.Space)) ihei.jumpPressed = true; else ihei.jumpPressed = false;
        if (Input.IsKeyPressed((int)KeyList.Control)) ihei.crouchPressed = true; else ihei.crouchPressed = false;
        if (Input.IsKeyPressed((int)KeyList.Shift)) ihei.sprintPressed = true; else ihei.sprintPressed = false;
        if (Input.IsKeyPressed((int)KeyList.E)) ihei.abilityPressed = true; else ihei.abilityPressed = false;
        if (Input.IsKeyPressed((int)KeyList.Q)) ihei.consolePressed = true; else ihei.consolePressed = false;
        if (Input.IsKeyPressed((int)KeyList.Escape)) ihei.escapePressed = true; else ihei.escapePressed = false;

        // if (@event is InputEventKey eventKey)
        // {
        //     if (eventKey.Pressed)
        //     {
        //         if (eventKey.Scancode == (int)KeyList.W) ihei.upPressed = true;
        //         if (eventKey.Scancode == (int)KeyList.S) ihei.downPressed = true;
        //         if (eventKey.Scancode == (int)KeyList.A) ihei.leftPressed = true;
        //         if (eventKey.Scancode == (int)KeyList.D) ihei.rightPressed = true;
        //         if (eventKey.Scancode == (int)KeyList.Space) ihei.jumpPressed = true;
        //         if (eventKey.Scancode == (int)KeyList.C) ihei.crouchPressed = true;
        //         if (eventKey.Scancode == (int)KeyList.Shift) ihei.sprintPressed = true;
        //         if (eventKey.Scancode == (int)KeyList.E) ihei.abilityPressed = true;
        //         if (eventKey.Scancode == (int)KeyList.Q) ihei.consolePressed = true;
        //     }
        //     else
        //     {
        //         if (eventKey.Scancode == (int)KeyList.W) ihei.upPressed = false;
        //         if (eventKey.Scancode == (int)KeyList.S) ihei.downPressed = false;
        //         if (eventKey.Scancode == (int)KeyList.A) ihei.leftPressed = false;
        //         if (eventKey.Scancode == (int)KeyList.D) ihei.rightPressed = false;
        //         if (eventKey.Scancode == (int)KeyList.Space) ihei.jumpPressed = false;
        //         if (eventKey.Scancode == (int)KeyList.C) ihei.crouchPressed = false;
        //         if (eventKey.Scancode == (int)KeyList.Shift) ihei.sprintPressed = false;
        //         if (eventKey.Scancode == (int)KeyList.E) ihei.abilityPressed = false;
        //         if (eventKey.Scancode == (int)KeyList.Q) ihei.consolePressed = false;
        //     }
        // }
        ihei.FireEvent();
    }
}
