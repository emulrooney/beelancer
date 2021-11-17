using Godot;
using System;
using System.Collections.Generic;
using System.Data;

public class Beelancer : RigidBody2D
{
	[Export] public float AccelerationForce = 100f;
	[Export] public float MaxVelocity = 150f;
	[Export] public float WalkSpeed = 50f;
	[Export] public float RotationSpeed = 100f;

	[Export] public float AccelerationBonusPerUpgrade = 1f;
	[Export] public float MaxVelocityBonusPerUpgrade = 5f;
	[Export] public float WalkSpeedBonusPerUpgrade = 4f;
	[Export] public float MaxGatherBonusPerUpgrade = .01f;


	public static Beelancer Current { get; private set; }
	private Vector2 move;

	//Visuals
	private AnimationPlayer _animator;

	private BeeState _currentState;
	private Dictionary<PlayerStateEnum, BeeState> _states;

	private List<PollenDeposit> _activeDeposits = new List<PollenDeposit>();
	private Dictionary<ResourceTypeEnum, float> _collected;
	
	private float _pollenWeight = 0f;

	private Flower _landableFlower;
	
	private Timer _collectionTimer;
	
	public bool InCover { get; set; }

	public Dictionary<UpgradeTypeEnum, int> Levels { get; private set; }

	public override void _Ready()
	{
		if (IsInstanceValid(Current))
		{
			Current.QueueFree();
		}
		Current = this;
		_animator = GetNode<AnimationPlayer>("AnimationPlayer");
		// _beeSprite = GetNode<Node2D>("BeeSprite");

		_collectionTimer = GetNode<Timer>("PollenCollector/Timer");
		_collectionTimer.Start();

		_collected = new Dictionary<ResourceTypeEnum, float>
		{
			{ResourceTypeEnum.RedPollen, 0f},
			{ResourceTypeEnum.BluePollen, 0f},
			{ResourceTypeEnum.GreenPollen, 0f},
			{ResourceTypeEnum.Honey, 0f}
		};
		
		SetupStates();
		SetState(PlayerStateEnum.Takeoff);
		_animator.CurrentAnimation = _currentState.AnimationName;

		Levels = new Dictionary<UpgradeTypeEnum, int>();
		
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
		
		if (Input.IsKeyPressed((int)KeyList.P)) {
			GUIManager.SetGameState(GameStateEnum.HiveMenu);
		}

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
	}

	/**
	 * Handle actual physics, movement, etc.
	 */
	public override void _IntegrateForces(Physics2DDirectBodyState state)
	{
		if (move != Vector2.Zero)
		{
			if (move.x != 0 && _currentState.CanRotate)
			{
				//Rotation
				state.AngularVelocity = move.x * RotationSpeed;
			}

			if (move.y > 0)
			{
				if (_currentState.UseFlyingMovement)
				{
					//Apply actual force
					var acceleration = AccelerationForce + GetBonus(UpgradeTypeEnum.Accelerate);
					ApplyCentralImpulse(Transform.x * acceleration);
					
					float vX = AppliedForce.x;
					float vY = AppliedForce.y;

					//reference:
					//https://www.reddit.com/r/godot/comments/mavqsv/how_do_i_set_speed_limit_for_rigidbody2d/
					if (Math.Abs(vX) > MaxVelocity || Math.Abs(vY) > MaxVelocity)
					{
						var terminalVelocity = AppliedForce.Normalized();
						terminalVelocity *= MaxVelocity + GetBonus(UpgradeTypeEnum.Speed);
						AppliedForce = terminalVelocity;
					}
				}
				else
				{
					SetState(PlayerStateEnum.Walking);
					LinearVelocity = (Transform.x * (WalkSpeed + GetBonus(UpgradeTypeEnum.Speed) / 10));
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
		if (_currentState.CanLand && IsInstanceValid(_landableFlower) && _landableFlower.Landable)
		{
			SetState(PlayerStateEnum.Idle);
			Game.SetLandedFlower(_landableFlower);
		}
	}

	public void Takeoff()
	{
		if (_currentState.CanTakeoff)
		{
			_activeDeposits = new List<PollenDeposit>();
			SetState(PlayerStateEnum.Takeoff);
			Game.SetLandedFlower(null);

			_pollenWeight = UpdatePollenWeight();
		}
	}

	//AnimationPlayer
	private void SetState(PlayerStateEnum state)
	{
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
				_collected[activeDeposit.ResourceType] += activeDeposit.CollectionRate + GetBonus(UpgradeTypeEnum.Gather);
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

	public float GetResourceQuantity(ResourceTypeEnum resource)
	{
		return _collected[resource];
	}

	public void ModifyResourceQuantity(ResourceTypeEnum resource, float value)
	{
		_collected[resource] += value;
		ResourceCounters.UpdatePlayerCollection(_collected);
	}

	public float UpdatePollenWeight()
	{
		return _collected[ResourceTypeEnum.RedPollen]
			+ _collected[ResourceTypeEnum.BluePollen]
			+ _collected[ResourceTypeEnum.GreenPollen];
	}

	public float GetBonus(UpgradeTypeEnum upgrade)
	{
		var level = Game.CurrentLevels[upgrade];
		
		switch (upgrade)
		{
			case UpgradeTypeEnum.Carry:
				
				break;
			case UpgradeTypeEnum.Accelerate:
				return level * AccelerationBonusPerUpgrade;
			case UpgradeTypeEnum.Sneak:
				break;
			case UpgradeTypeEnum.Trade:
				break;
			case UpgradeTypeEnum.Speed:
				return level * MaxVelocityBonusPerUpgrade;
			case UpgradeTypeEnum.Gather:
				return level * MaxGatherBonusPerUpgrade; 
			default:
				throw new ArgumentOutOfRangeException(nameof(upgrade), upgrade, null);
					
		}
		
		return 0f;
		
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
			ActionText.SetText(ActionTextEnum.Land);
			_landableFlower = flower;
		}	
	}
	
	//Signalled
	private void OnLandingShapeAreaExited(object area)
	{
		if (area is Flower flower)
		{
			ActionText.DismissText();
			if (!_currentState.UseFlyingMovement)
			{
				Takeoff();
			}
			_landableFlower = null;
		}
	}
}

