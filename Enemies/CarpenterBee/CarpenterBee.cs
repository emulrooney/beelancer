using Godot;
using System;

public class CarpenterBee : RigidBody2D
{
	[Export] private float BaseAccelerationForce { get; set; } = 1.5f;
	private float _additionalPower = 0f;

	public void Setup(float rotation, float power = 0f)
	{
		Rotation = rotation;
		_additionalPower = power;
		ApplyCentralImpulse(10 * Transform.x * (BaseAccelerationForce + _additionalPower));
	}
	
	public override void _IntegrateForces(Physics2DDirectBodyState state)
	{
		ApplyCentralImpulse(Transform.x * (BaseAccelerationForce + _additionalPower));
	}

	private void OnCarpenterBodyCollision(object body)
	{
		if (body is Beelancer bee)
		{
			bee.ApplyCentralImpulse(Position.DirectionTo(bee.Position) * 30);	
		}
	}
	
	private void OnDisappearTimerTimeout()
	{
		QueueFree();
	}
}
