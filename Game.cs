using System;
using Godot;

/**
 * Autoloaded, singleton
 */
public class Game : Node
{
	private static Game _instance;
	
	[Export] private PackedScene Yard { get; set; }
	[Export] private Beelancer Bee { get; set; }
	
	public static Random Random = new Random();
	public static Yard CurrentYard { get; private set; }

	public static MainMenu MainMenu { get; private set; }
	
	private GameState State = GameState.MainMenu;

	public override void _Ready()
	{
		CurrentYard = GetParent().GetNodeOrNull<Yard>("Yard");
		MainMenu = GetNode<MainMenu>("MainMenu");

		if (!IsInstanceValid(CurrentYard))
		{
			MainMenu.Visible = true;
		}
		
		_instance = this;
	}

	public static void SetLandedFlower(Flower flower)
	{
		if (!IsInstanceValid(GameCamera.Current)) return;
		
		if (flower != null)
		{
			GameCamera.Current.LastLandingFocusLocation = flower.GlobalPosition;
			GameCamera.Current.IsLanded = true;
			GameCamera.StartLerp();
			ActionText.SetText(ActionTextType.Takeoff);
		}
		else
		{
			GameCamera.Current.IsLanded = false;
			GameCamera.ReverseLerp();
			ActionText.DismissText();
		}

	}

	public static void NewGame()
	{
		SetLandedFlower(null);
		GUIManager.SetGameState(GameState.Gameplay);

		if (IsInstanceValid(CurrentYard))
		{
			CurrentYard.QueueFree();
		}
		
		MainMenu.Visible = false;
		
		CurrentYard = _instance.Yard.Instance<Yard>();
		_instance.AddChild(CurrentYard);
	}

	public static void ShowMainMenu()
	{
		GUIManager.SetGameState(GameState.MainMenu);
		
		CurrentYard.QueueFree();

		MainMenu.Visible = true;
	}
	
}
