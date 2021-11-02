using Godot;
using System;
using System.Collections.Generic;

public class Beelancer : RigidBody2D
{
	[Export] public float AccelerationForce = 10f;
	[Export] public float MaxVelocity = 200f;
	[Export] public float WalkSpeed = 50f;
	
	public static Beelancer Current { get; set; }
	private Vector2 move;

	private Label debugLabel; //temp
	private AnimationPlayer _animator; 
	
	private PlayerState _state = PlayerState.Takeoff;

	private Dictionary<PlayerState, string> _animationNames = new Dictionary<PlayerState, string>()
	{
		{PlayerState.Idle, "idle"},
		{PlayerState.Walking, "walking"},
		{PlayerState.Takeoff, "takeoff"},
		{PlayerState.Flying, "flying"},
};

	public override void _Ready()
	{
		if (IsInstanceValid(Current))
		{
			Current.QueueFree();
		}
		Current = this;

		debugLabel = GetNode<Label>("DebugLabel");
		_animator = GetNode<AnimationPlayer>("AnimationPlayer");

		_animator.CurrentAnimation = _animationNames[_state];
	}

	/**
	 * Handle inputs, animations, etc
	 */
	public override void _Process(float delta)
	{
		int x = 0;
		int y = 0;
		
		x -= Input.IsActionPressed("ui_left") ? 1 : 0;
		x += Input.IsActionPressed("ui_right") ? 1 : 0;
		y -= Input.IsActionPressed("ui_up") ? 1 : 0;
		y += Input.IsActionPressed("ui_down") ? 1 : 0;

		move = new Vector2(x, y);
	}
	
	/**
	 * Handle actual physics, movement, etc.
	 */
	public override void _PhysicsProcess(float delta)
	{
		if (move != Vector2.Zero)
		{
			if (_state == PlayerState.Takeoff || _state == PlayerState.Flying)
			{
				//Apply actual force
				ApplyCentralImpulse(move * AccelerationForce);
				//Apply velocity
				float vX = LinearVelocity.x;
				float vY = LinearVelocity.y;

				//reference:
				//https://www.reddit.com/r/godot/comments/mavqsv/how_do_i_set_speed_limit_for_rigidbody2d/
				if (Math.Abs(vX) > MaxVelocity || Math.Abs(vY) > MaxVelocity)
				{
					var terminalVelocity = LinearVelocity.Normalized();
					terminalVelocity *= MaxVelocity;
					LinearVelocity = terminalVelocity;
				}
			}
			else
			{
				LinearVelocity = move * WalkSpeed;

			}
		}
		
		
	}
	
}
