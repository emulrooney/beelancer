using Godot;
using System;

public class FlyingState : BeeState
{
    public FlyingState()
    {
        AnimationName = "flying";
        
        CanBeHitFromAir = true;
        CanBeHitFromGround = false;
        CanGatherPollen = false;
        CanRotate = true;
        CanAccelerate = true;
        CanLand = true;
        CanTakeoff = false;
        UseFlyingMovement = true;
    }

    public override void StartState()
    {
    }
}
    