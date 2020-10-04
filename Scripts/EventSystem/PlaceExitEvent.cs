using Godot;
using System;

namespace EventCallback
{
    public class PlaceExitEvent : Event<PlaceExitEvent>
    {
        //The direction of the new room that was placed
        public RoomDirection newLocation;
    }
}