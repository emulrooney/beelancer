using Godot;
using System;

public abstract  class BeeState
{
    public string AnimationName { get; protected set; }
    public bool CanBeHitFromAir { get; protected set; }
    public bool CanBeHitFromGround { get; protected set; }
    public bool CanGatherPollen { get; protected set; }
    public bool CanRotate { get; protected set; }

    public bool CanTakeoff { get; protected set; }
    public bool CanLand { get; protected set; }
    public bool UseFlyingMovement { get; protected set; }

    public abstract void StartState();
}
