using Godot;
using System;

public class FlyingState : BeeState
{
    public override void StartState()
    {
        AnimationName = "flying";
        
        CanBeHitFromAir = true;
        CanBeHitFromGround = false;
        CanGatherPollen = false;
        CanRotate = true;
    }
}
