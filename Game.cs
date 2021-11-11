using System;
using Godot;

/**
 * Autoloaded, singleton
 */
public class Game : Node
{
	private Game _instance;
	
	[Export] private PackedScene Yard { get; set; }
	[Export] private Beelancer Bee { get; set; }
	
	public static Random Random = new Random();
	public static Yard CurrentYard { get; private set; }
	
	private GameState State = GameState.MainMenu;

	public override void _Ready()
	{
		CurrentYard = GetParent().GetNodeOrNull<Yard>("Yard");
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

	public static void NewGame()
	{
		SetLandedFlower(null);
		SetGameState(GameState.Gameplay);

		if (IsInstanceValid(CurrentYard))
		{
			CurrentYard.QueueFree();
			CurrentYard = 
		}
	}

	public static void MainMenu()
	{
		SetGameState(GameState.MainMenu);
	}

	public static void SetGameState(GameState state)
	{
		switch (state)
		{
			case GameState.MainMenu:
				
				break;
			case GameState.Gameplay:
				break;
			case GameState.HiveMenu:
				break;
			case GameState.GameOver:
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(state), state, null);
		}
		 
	}
}
