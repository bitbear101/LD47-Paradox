
using Godot;
using System;

namespace EventCallback
{
    public class SetStateEvent : Event<SetStateEvent>
    {
        //The new state that needsto sent to the orb
        public ORB_STATE newState;
    }
}
