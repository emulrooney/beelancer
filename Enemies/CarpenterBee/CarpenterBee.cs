using Godot;
using System;

public class CarpenterBee : RigidBody2D
{
	[Export] private float BaseAccelerationForce { get; set; } = 1.5f;
	private float _additionalPower = 0f;
	
	private AnimationPlayer _animationPlayer;

	public void Setup(float rotation, float power = 0f)
	{
		Rotation = rotation;
		_additionalPower = power;
		ApplyCentralImpulse(10 * Transform.x * (BaseAccelerationForce + _additionalPower));

		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_animationPlayer.CurrentAnimation = "flying";
		_animationPlayer.Play();
	}
	
	public override void _IntegrateForces(Physics2DDirectBodyState state)
	{
		ApplyCentralImpulse(Transform.x * (BaseAccelerationForce + _additionalPower));
	}

	//Signalled
	private void OnCarpenterBodyCollision(object body)
	{
		if (body is Beelancer bee)
		{
			bee.ApplyCentralImpulse(Position.DirectionTo(bee.Position) * 30);
			bee.ShowImpact();
		}
		else
		{
			QueueFree();
		}
	}
	
	//Signalled
	private void OnDisappearTimerTimeout()
	{
		QueueFree();
	}
}
