using Godot;
using System;

namespace EventCallback
{
    public class WinEvent : Event<WinEvent>
    {
        //Send true if the player wins the game
        public bool win = false;
    }

}
