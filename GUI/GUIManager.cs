using Godot;
using System;

public class GUIManager : CanvasLayer
{
	private static AnimationPlayer animationPlayer;

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}
	
	public static void SetGameState(GameStateEnum stateEnum)
	{
		switch (stateEnum)
		{
			case GameStateEnum.MainMenu:
			case GameStateEnum.HiveMenu:
				animationPlayer.CurrentAnimation = "HiveMenu";
				HiveMenu.ShowMenu();
				break;
			case GameStateEnum.GameOver:
				animationPlayer.CurrentAnimation = "MainMenu";
				break;
			case GameStateEnum.Gameplay:
				animationPlayer.CurrentAnimation = "Gameplay";
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(stateEnum), stateEnum, null);
		}
	}
}
