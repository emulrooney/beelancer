using Godot;
using System;

public class GameOverScreen : CenterContainer
{
	private void OnRestartButtonPressed()
	{
		Game.NewGame();
		Visible = false;
	}


	private void OnMainMenuButtonPressed()
	{
		Game.MainMenu();
		Visible = false;
	}	
}
