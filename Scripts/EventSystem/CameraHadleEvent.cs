using Godot;
using System;

namespace EventCallback
{
    public class CameraHadleEvent : Event<CameraHadleEvent>
    {
        //The target the camera must track
        public Node2D target;
        //If the smoothing or drag margins on the camera track is enabled 
        public bool smoothing = false, dragMarginHorizontal = false, dragMarginVertical = false;
        //The smoothing speed
        public float smoothingSpeed = 0;
        //The drag margins margins
        public float drangMarginLeft = .2f, dragMarginRight = .2f, dragMarginTop = .2f, dragMarginBottom = .2f;
    }
}
