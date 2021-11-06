using System;
using Godot;

/**
 * Autoloaded, singleton
 */
public class Game : Node
{
	public static Random Random = new Random();

	public static Node2D CurrentWorld { get; private set; }

	public override void _Ready()
	{
		CurrentWorld = GetParent().GetNode<Node2D>("World");
	}
	
	public override void _Process(float delta)
	{
		if (IsInstanceValid(Beelancer.Current) && IsInstanceValid(GameCamera.Current))
		{
			//TODO Set zoom based on location
			//TODO zoom in and out based on velocity
			GameCamera.Current.Position = Beelancer.Current.Position;
		}
	}
}
