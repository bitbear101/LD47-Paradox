using Godot;
using System;

namespace EventCallback
{
    public class StartPressedEvent : Event<StartPressedEvent>
    {
        public bool startPressed = true;
    }

}
