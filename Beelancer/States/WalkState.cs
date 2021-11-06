using Godot;
using System;

public class WalkState : BeeState
{
    public WalkState()
    {
        AnimationName = "walking";
        
        CanBeHitFromAir = false;
        CanBeHitFromGround = true;
        CanGatherPollen = true;
        CanRotate = true;
        CanLand = false;
        CanTakeoff = true;
        UseFlyingMovement = false;
    }
    
    public override void StartState()
    {
        
        
    }
}
