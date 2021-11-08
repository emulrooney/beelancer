using System;
using Godot;

/**
 * Autoloaded, singleton
 */
public class Game : Node
{
	public static Random Random = new Random();
	public static Yard CurrentYard { get; private set; }

	public override void _Ready()
	{
		CurrentYard = GetParent().GetNode<Yard>("Yard");
	}

	public static void SetLandedFlower(Flower flower)
	{
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
