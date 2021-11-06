using Godot;
using System;

public class TakeoffState : BeeState
{
    public TakeoffState()
    {
        AnimationName = "takeoff";
        
        CanBeHitFromAir = false;
        CanBeHitFromGround = true;
        CanGatherPollen = false;
        CanRotate = false;
        CanLand = true;
        CanTakeoff = false;
        UseFlyingMovement = true;
    }

    public override void StartState()
    {
        throw new NotImplementedException();
    }
}
