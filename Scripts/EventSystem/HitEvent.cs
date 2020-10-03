using Godot;
using System;

namespace EventCallback
{
    public class HitEvent : Event<HitEvent>
    {
        //The nodes to pass to the hit function in the health class
        public Node2D target, attacker;
        //The damage the attacker does
        public int damage;
    }
}
