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
				animationPlayer.CurrentAnimation = "MainMenu";
				MainMenu.SetVisible(true);
				break;
			case GameStateEnum.HiveMenu:
				animationPlayer.CurrentAnimation = "HiveMenu";
				HiveMenu.ShowMenu();
				break;
			case GameStateEnum.GameOver:
				animationPlayer.CurrentAnimation = "GameOver";
				break;
			case GameStateEnum.Gameplay:
				animationPlayer.CurrentAnimation = "Gameplay";
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(stateEnum), stateEnum, null);
		}
	}
}
