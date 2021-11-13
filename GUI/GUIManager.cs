using Godot;
using System;

public class GUIManager : CanvasLayer
{
	private static AnimationPlayer animationPlayer;

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}
	
	public static void SetGameState(GameState state)
	{
		switch (state)
		{
			case GameState.MainMenu:
			case GameState.HiveMenu:
				animationPlayer.CurrentAnimation = "HiveMenu";
				HiveMenu.ShowMenu();
				break;
			case GameState.GameOver:
				animationPlayer.CurrentAnimation = "MainMenu";
				break;
			case GameState.Gameplay:
				animationPlayer.CurrentAnimation = "Gameplay";
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(state), state, null);
		}
	}
}
