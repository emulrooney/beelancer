using Godot;
using System;

public class CarpenterBee : RigidBody2D
{
	[Export] private float BaseAccelerationForce { get; set; } = 20f;
	private float _additionalPower;

	public void Setup(float rotation, float power = 0f)
	{
		Rotation = rotation;
		_additionalPower = power;
	}
	
	public override void _IntegrateForces(Physics2DDirectBodyState state)
	{
		

	}

	private void OnDetectPlayerAreaBodyEntered(object body)
	{
		
	}

	private void OnDetectPlayerAreaBodyExited(object body)
	{
		// Replace with function body.
	}
	
	private void OnCarpenterBodyCollision(object body)
	{
		if (body is Beelancer bee)
		{
			bee.ApplyCentralImpulse(Position.DirectionTo(bee.Position) * 30);	
		}
	}
}
