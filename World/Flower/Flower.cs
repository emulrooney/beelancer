using Godot;

public class Flower : Area2D
{
	private Navigation2D _navigation2D;

	public override void _Ready()
	{
		//_navigation2D.GetNode<Navigation2D>("Navigation2D");
	}

	public void AcceptLander(Node2D lander)
	{
		// lander.GetParent().RemoveChild(lander);
		// AddChild(lander);
	}

	public void RemoveLander(Node2D lander)
	{
		// lander.GetParent().RemoveChild(lander);
		// Game.CurrentWorld.AddChild(lander);
	}
	
}
