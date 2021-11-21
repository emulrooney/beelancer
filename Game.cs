using System;
using System.Collections.Generic;
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
	public static Dictionary<UpgradeTypeEnum, int> CurrentLevels;
	
	private static MainMenu _mainMenu;
	private GameStateEnum _stateEnum = GameStateEnum.MainMenu;

	public override void _Ready()
	{
		CurrentYard = GetParent().GetNodeOrNull<Yard>("Yard");
		_mainMenu = GetNode<MainMenu>("MainMenu");

		if (!IsInstanceValid(CurrentYard))
		{
			_mainMenu.Visible = true;
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
			ActionText.SetText(ActionTextEnum.Takeoff);
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
		GUIManager.SetGameState(GameStateEnum.Gameplay);

		if (IsInstanceValid(CurrentYard))
		{
			CurrentYard.QueueFree();
		}
		
		_mainMenu.Visible = false;
		
		CurrentYard = _instance.Yard.Instance<Yard>();
		_instance.AddChild(CurrentYard);
	
		
		CurrentLevels = new Dictionary<UpgradeTypeEnum, int>();
		foreach (UpgradeTypeEnum upgrade in Enum.GetValues(typeof(UpgradeTypeEnum)))
		{
			CurrentLevels.Add(upgrade, 1);
		}
		
		AudioManager.PlayTrack(MusicTrackEnum.Exploration);
	}

	public static void ShowMainMenu()
	{
		GUIManager.SetGameState(GameStateEnum.MainMenu);
		
		CurrentYard.QueueFree();

		_mainMenu.Visible = true;
	}
	
}
