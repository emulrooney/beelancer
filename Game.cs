using System;
using Godot;

/**
 * Autoloaded, singleton
 */
public class Game : Node
{
	public static Random Random = new Random();
	private static Flower _landedFlower = null;

	public static Node2D CurrentWorld { get; private set; }

	public override void _Ready()
	{
		CurrentWorld = GetParent().GetNode<Node2D>("World");
	}

	public static void SetLandedFlower(Flower flower)
	{
		_landedFlower = flower;
		if (flower != null)
		{
			GameCamera.Current.LastLandingFocusLocation = flower.GlobalPosition;
			GameCamera.Current.IsLanded = true;
			GameCamera.StartLerp();
		}
		else
		{
			GameCamera.Current.IsLanded = false;
			GameCamera.ReverseLerp();
		}

	}
}
