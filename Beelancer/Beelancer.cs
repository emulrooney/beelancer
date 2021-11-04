using Godot;
using System;
using System.Collections.Generic;

public class Beelancer : RigidBody2D
{
	[Export] public float AccelerationForce = 200f;
	[Export] public float MaxVelocity = 200f;
	[Export] public float WalkSpeed = 50f;
	[Export] public float RotationSpeed = 100f;
	[Export] public float BrakePower = 20f;
	
	public static Beelancer Current { get; set; }
	private Vector2 move;

	private Label debugLabel; //temp
	private AnimationPlayer _animator;

	private float baseLinearDamping;

	private Node2D _beeSprite;
	private PlayerState _state = PlayerState.Takeoff;

	private List<PollenDeposit> activeDeposits = new List<PollenDeposit>();
	private Timer _collectionTimer;

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
		_beeSprite = GetNode<Node2D>("BeeSprite");

		_collectionTimer = GetNode<Timer>("PollenCollector/Timer");
		_collectionTimer.Start();
		
		_animator.CurrentAnimation = _animationNames[_state];
		baseLinearDamping = LinearDamp;
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
		y += Input.IsActionPressed("ui_up") ? 1 : 0;
		y -= Input.IsActionPressed("ui_down") ? 1 : 0;

		move = new Vector2(x, y);
	}

	/**
	 * Handle actual physics, movement, etc.
	 */
	public override void _IntegrateForces(Physics2DDirectBodyState state)
	{
		if (move != Vector2.Zero)
		{
			if (move.x != 0)
			{
				//Rotation
				state.AngularVelocity = move.x * RotationSpeed;
			}

			if (move.y > 0)
			{
				if (_state == PlayerState.Takeoff || _state == PlayerState.Flying)
				{
					//Apply actual force
					ApplyCentralImpulse(Transform.x * AccelerationForce);
					
					float vX = AppliedForce.x;
					float vY = AppliedForce.y;

					//reference:
					//https://www.reddit.com/r/godot/comments/mavqsv/how_do_i_set_speed_limit_for_rigidbody2d/
					if (Math.Abs(vX) > MaxVelocity || Math.Abs(vY) > MaxVelocity)
					{
						var terminalVelocity = AppliedForce.Normalized();
						terminalVelocity *= MaxVelocity;
						AppliedForce = terminalVelocity;
					}
				}
				else
				{
					LinearVelocity = move * WalkSpeed;
				}
			}
		}
	}

	//Signalled
	private void OnPollenCollectorTimerTimeout()
	{
		GD.Print("tick");
		//Probably inefficient but fine for now
		foreach (var activeDeposit in activeDeposits)
		{
			if (IsInstanceValid(activeDeposit))
			{
				activeDeposit.Harvest();
			}
			else
			{
				activeDeposits.Remove(activeDeposit);
			}
		}
	}

	//Signalled
	private void OnPollenCollectorAreaEntered(object area)
	{
		//TODO Only harvest when landed.

		GD.Print("Enter");
		
		//if cast fails, do nothing.
		var pollen = area as PollenDeposit;
		if (pollen != null)
		{
			activeDeposits.Add(pollen);
		}
	}


	//Signalled
	private void OnPollenCollectorAreaExit(object area)
	{
		GD.Print("exit");
		//TODO On takeoff, remove _all_ areas
		
		//if cast fails, do nothing.
		var pollen = area as PollenDeposit;
		if (pollen != null)
		{
			activeDeposits.Remove(pollen);
		}
	}
}
