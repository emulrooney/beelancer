using Godot;
using System;

public class GameCamera : Camera2D
{
	[Export] private Vector2 MaxCameraZoomout = Vector2.One * 2;
	private Vector2 initialCameraZoom = Vector2.One;
	[Export] private float lerp = 0f;
	
	[Export] private Vector2 roundProcessTo = new Vector2(1f, 1f);

	public static GameCamera Current { get; set; }

	private AnimationPlayer _animator;
	private AnimationPlayer _birdAnimator;
	private bool _birdLeftToRight;

	private TextureRect _bg;
	private static Vector2 _textureSize;

	public bool IsLanded { get; set; }
	public Vector2 LastLandingFocusLocation;


	public override void _Ready()
	{
		initialCameraZoom = Zoom;
		
		if (IsInstanceValid(Current))
		{
			Current.QueueFree();
		}

		_animator = GetNode<AnimationPlayer>("AnimationPlayer");
		_birdAnimator = GetNode<AnimationPlayer>("BirdIndicator/AnimationPlayer");

		Current = this;
		
	}

	public override void _Process(float delta)
	{
		if (_animator.PlaybackActive && IsInstanceValid(Beelancer.Current))
		{
			if (IsLanded)
			{
				GlobalPosition = Beelancer.Current.GlobalPosition.LinearInterpolate(LastLandingFocusLocation, lerp);
			}
			else
			{
				GlobalPosition = Beelancer.Current.GlobalPosition;
			}
		}
	}

	public static void StartLerp()
	{
		Current._animator.CurrentAnimation = "ToLandingZone";
	}

	public static void ReverseLerp()
	{
		Current._animator.CurrentAnimation = "ToPlayer";
	}

	public static void BirdTime()
	{
		if (Current._birdLeftToRight)
		{
			Current._birdAnimator.CurrentAnimation = "RightToLeft";
			Current._birdLeftToRight = false;
		}
		else
		{
			Current._birdAnimator.CurrentAnimation = "LeftToRight";
			Current._birdLeftToRight = true;
		}
	}
}
