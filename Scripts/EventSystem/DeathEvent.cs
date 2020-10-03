using Godot;
using System;

namespace EventCallback
{
    public class DeathEvent : Event<DeathEvent>
    {
        public Node2D target;
    }
}
