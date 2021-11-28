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
			AudioManager.PlaySFX(SoundEffectEnum.Explore_CarpenterRobbery);

			var stolen = Game.Random.Next(0, 4);
			bee.ModifyResourceQuantity((ResourceTypeEnum)stolen, -0.5f);
			
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
		AudioManager.PlaySFX(SoundEffectEnum.Explore_Squish);
		QueueFree();
	}
}
