using Godot;
using System;
using System.Collections.Generic;
using System.Data;

public class Beelancer : RigidBody2D
{
	[Export] public float AccelerationForce = 2f;
	[Export] public float MaxVelocity = 10f;
	[Export] public float WalkSpeed = 200f;
	[Export] public float RotationSpeed = 15f;

	[Export] public float PollenWeightRotationSlowdown = .3f; //Per full unit over.
	[Export] public float PollenWeightRotationMaxSlowdown = 15f;
	[Export] public float PollenWeightAccelerationForceSlowdown = .018f; //Per full unit over.
	[Export] public float PollenWeightAccelerationMaxSlowdown = 1.5f;
	
	[Export] public float BasePollenCapacity = 45f;
	 
	[Export] public float AccelerationBonusPerUpgrade = 1f;
	[Export] public float TimerDecreaseBonusPerUpgrade = 0.06f;
	[Export] public float MaxVelocityBonusPerUpgrade = 5f;
	[Export] public float TradeBonusPerUpgrade = 0.01f;
	[Export] public float WalkSpeedBonusPerUpgrade = 4f;
	[Export] public float MaxGatherBonusPerUpgrade = .01f;
	[Export] public float FreePollenUnitsPerUpgrade = 5f;
	


	public static Beelancer Current { get; private set; }
	private Vector2 move;

	//Visuals
	private AnimationPlayer _animator;
	private BeeState _currentState;
	private CPUParticles2D _impact;
	
	private Dictionary<PlayerStateEnum, BeeState> _states;
	private List<PollenDeposit> _activeDeposits = new List<PollenDeposit>();
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
		_impact = GetNode<CPUParticles2D>("BeeSprite/ImpactParticles");

		_collectionTimer = GetNode<Timer>("PollenCollector/Timer");
		_collectionTimer.Start();
		
		SetupStates();
		SetState(PlayerStateEnum.Flying);
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
		if (_currentState.GetType() == typeof(TakeoffState))
		{
			ApplyCentralImpulse(Transform.x * GetWeightedAcceleration() * 1.1f);
			return;
		}
		
		if (move != Vector2.Zero)
		{
			if (move.x != 0 && _currentState.CanRotate)
			{
				//Rotation
				state.AngularVelocity = move.x * GetWeightedRotation();
			}

			if (move.y > 0)
			{
				if (_currentState.UseFlyingMovement && _currentState.CanAccelerate)
				{
					//Apply actual force
					ApplyCentralImpulse(Transform.x * GetWeightedAcceleration());
					
					float vX = AppliedForce.x;
					float vY = AppliedForce.y;
					
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

			_pollenWeight = Math.Max(0, GetPollenWeight() - GetFreeCarryLimit());
		}
	}

	//AnimationPlayer
	private void SetState(PlayerStateEnum state)
	{
		_currentState = _states[state];
		_animator.CurrentAnimation = _currentState.AnimationName;
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
		return Game.CollectedResources[resource];
	}

	public void ModifyResourceQuantity(ResourceTypeEnum resource, float value)
	{
		Game.CollectedResources[resource] += value;
		Game.CollectedResources[resource] = Math.Max(Game.CollectedResources[resource], 0f);
		
		ResourceCounters.UpdatePlayerCollection(Game.CollectedResources, GetPollenWeight(), GetFreeCarryLimit());
	}

	public float GetPollenWeight()
	{
		var pollen = Game.CollectedResources[ResourceTypeEnum.RedPollen]
					 + Game.CollectedResources[ResourceTypeEnum.BluePollen]
					 + Game.CollectedResources[ResourceTypeEnum.GreenPollen];
		return pollen;
	}

	public float GetFreeCarryLimit()
	{
		return Game.CurrentLevels[UpgradeTypeEnum.Carry] * FreePollenUnitsPerUpgrade;
	}

	private float GetWeightedRotation()
	{
		return RotationSpeed - Math.Min(PollenWeightRotationMaxSlowdown, (float)(Math.Floor(_pollenWeight) * PollenWeightRotationSlowdown));
	}

	private float GetWeightedAcceleration()
	{
		return AccelerationForce - Math.Min(PollenWeightAccelerationMaxSlowdown,
			(float) (Math.Floor(_pollenWeight) * PollenWeightAccelerationForceSlowdown));
	}

	public float GetBonus(UpgradeTypeEnum upgrade)
	{
		var level = Game.CurrentLevels[upgrade];
		
		switch (upgrade)
		{
			case UpgradeTypeEnum.Carry:
				return level * MaxGatherBonusPerUpgrade;
			case UpgradeTypeEnum.Accelerate:
				return level * AccelerationBonusPerUpgrade;
			case UpgradeTypeEnum.Sneak:
				return level * TimerDecreaseBonusPerUpgrade;
			case UpgradeTypeEnum.Trade:
				return level * TradeBonusPerUpgrade;
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

	public void Death()
	{
		_animator.CurrentAnimation = "death";
	}

	public void ShowImpact()
	{
		_impact.Emitting = true;
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
	
	//Signalled
	private void OnPollenCollectorTimerTimeout()
	{	
		if (!_currentState.CanGatherPollen) return;
		
		bool updateCollection = false;
		
		//Probably inefficient but fine for now 
		foreach (var activeDeposit in _activeDeposits)
		{
			if (IsInstanceValid(activeDeposit))
			{
				activeDeposit.Harvest();
				Game.CollectedResources[activeDeposit.ResourceType] += activeDeposit.CollectionRate + GetBonus(UpgradeTypeEnum.Gather);
				updateCollection = true;
				
			}
			else
			{
				_activeDeposits.Remove(activeDeposit);
			}
		}

		if (updateCollection)
		{
			ResourceCounters.UpdatePlayerCollection(Game.CollectedResources, GetPollenWeight(), GetFreeCarryLimit());
			AudioManager.PlaySFX(SoundEffectEnum.Explore_PollenPickup);
		}
	}

}

