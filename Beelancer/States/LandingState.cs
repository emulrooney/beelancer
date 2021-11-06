using Godot;
using System;

public class LandingState : BeeState
{
    public LandingState()
    {
        AnimationName = "landing";

        CanBeHitFromAir = false;
        CanBeHitFromGround = true;
        CanGatherPollen = false;
        CanRotate = false;
        CanLand = false;
        CanTakeoff = true;
        UseFlyingMovement = true;
    }
    
    public override void StartState()
    {
        
        
    }
}
