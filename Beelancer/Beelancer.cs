using Godot;
using System;
using System.Collections.Generic;
using System.Data;

public class Beelancer : RigidBody2D
{
	[Export] public float AccelerationForce = 200f;
	[Export] public float MaxVelocity = 200f;
	[Export] public float WalkSpeed = 50f;
	[Export] public float RotationSpeed = 100f;
	[Export] public float BrakePower = 20f;
	
	public static Beelancer Current { get; private set; }
	private Vector2 move;

	//Visuals
	private Label debugLabel; //temp
	private AnimationPlayer _animator;
	private Node2D _beeSprite;

	private BeeState _currentState;
	private Dictionary<PlayerStateEnum, BeeState> _states;

	private List<PollenDeposit> _activeDeposits = new List<PollenDeposit>();
	private Dictionary<ResourceTypeEnum, float> _collected;

	private Flower _landableFlower;
	
	private Timer _collectionTimer;

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

		_collected = new Dictionary<ResourceTypeEnum, float>
		{
			{ResourceTypeEnum.RedPollen, 0f},
			{ResourceTypeEnum.BluePollen, 0f},
			{ResourceTypeEnum.GreenPollen, 0f},
			// {ResourceTypeEnum.Honey, 0f},
			{ResourceTypeEnum.Nectar, 0f},
			// {ResourceTypeEnum.Energy, 0f},
		};
		
		SetupStates();
		SetState(PlayerStateEnum.Takeoff);
		_animator.CurrentAnimation = _currentState.AnimationName;
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

		if (Input.IsActionJustPressed("land"))
		{
			if (_currentState.CanLand)
			{
				Land();
			}
			else if (_currentState.CanTakeoff)
			{
				Takeoff();
			}
		}

		move = new Vector2(x, y);

		debugLabel.Text = $"{_currentState.AnimationName} {_landableFlower != null}";
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
				if (_currentState.UseFlyingMovement)
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
					SetState(PlayerStateEnum.Walking);
					LinearVelocity = (Transform.x * WalkSpeed);
				}
			}
			else
			{
				if (!_currentState.UseFlyingMovement)
				{
					SetState(PlayerStateEnum.Idle);
				}
			}
		}
	}

	public void Land()
	{
		GD.Print("Land");
		if (_currentState.CanLand && IsInstanceValid(_landableFlower))
		{
			GD.Print("Yep");
			SetState(PlayerStateEnum.Idle);
			_landableFlower.AcceptLander(this);
		}
	}

	public void Takeoff()
	{
		GD.Print("Takeoff");
		if (_currentState.CanTakeoff && IsInstanceValid(_landableFlower))
		{
			GD.Print("Yep");
			SetState(PlayerStateEnum.Takeoff);
			_landableFlower.RemoveLander(this);
		}
	}

	//AnimationPlayer
	private void SetState(PlayerStateEnum state)
	{
		GD.Print("Set State: " + state);
		_currentState = _states[state];
		_animator.CurrentAnimation = _currentState.AnimationName;
	}
	
	//Signalled
	private void OnPollenCollectorTimerTimeout()
	{
		if (!_currentState.CanGatherPollen) return;
		
		bool signalCollectionUpdate = false;
		
		//Probably inefficient but fine for now 
		foreach (var activeDeposit in _activeDeposits)
		{
			if (IsInstanceValid(activeDeposit))
			{
				activeDeposit.Harvest();
				_collected[activeDeposit.ResourceType] += activeDeposit.CollectionRate;
				signalCollectionUpdate = true;
			}
			else
			{
				_activeDeposits.Remove(activeDeposit);
			}
		}

		if (signalCollectionUpdate)
		{
			ResourceCounters.UpdatePlayerCollection(_collected);
		}
	}

	private void SetupStates()
	{
		_states = new Dictionary<PlayerStateEnum, BeeState>();
		
		_states.Add(PlayerStateEnum.Idle, new IdleState());
		_states.Add(PlayerStateEnum.Walking, new WalkState());
		_states.Add(PlayerStateEnum.Flying, new FlyingState());
		_states.Add(PlayerStateEnum.Takeoff, new TakeoffState());
		_states.Add(PlayerStateEnum.Landing, new LandingState());
	}
	
	/* SIGNALLED */

	//Signalled
	private void OnPollenCollectorAreaEntered(object area)
	{
		//TODO Only harvest when landed.

		//if cast fails, do nothing.
		if (area is PollenDeposit pollen)
		{
			_activeDeposits.Add(pollen);
		}
	}

	//Signalled
	private void OnPollenCollectorAreaExit(object area)
	{
		GD.Print("exit");
		//TODO On takeoff, remove _all_ areas
		
		//if cast fails, do nothing.
		if (area is PollenDeposit pollen)
		{
			_activeDeposits.Remove(pollen);
		}
	}
	
	//Signalled
	private void OnLandingShapeAreaEntered(object area)
	{
		if (area is Flower flower)
		{
			_landableFlower = flower;
		}	
	}
	
	//Signalled
	private void OnLandingShapeAreaExited(object area)
	{
		if (area is Flower flower)
		{
			if (!_currentState.UseFlyingMovement)
			{
				Takeoff();
			}
			
			_landableFlower = null;
		}
	}
}

