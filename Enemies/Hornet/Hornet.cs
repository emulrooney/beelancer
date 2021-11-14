using Godot;
using System;

public class Hornet : RigidBody2D
{
	public override void _Process(float delta)
	{
		
	}

	private void OnDetectPlayerAreaBodyEntered(object body)
	{
		// Replace with function body.
	}

	private void OnDetectPlayerAreaBodyExited(object body)
	{
		// Replace with function body.
	}
	
	private void OnHornetBodyCollision(object body)
	{
		if (body is Beelancer bee)
		{
			bee.ApplyCentralImpulse(Position.DirectionTo(bee.Position) * 50);	
		}
	}


}

