using Godot;
using System;

public class IdleState : BeeState
{
    public IdleState()
    {
        AnimationName = "idle";
        
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
