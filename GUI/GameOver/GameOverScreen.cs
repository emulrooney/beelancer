using Godot;
using System;

public class GameOverScreen : CenterContainer
{
	private void OnRestartButtonPressed()
	{
		Game.NewGame();
		Visible = false;
		AudioManager.PlaySFX(SoundEffectEnum.GUI_Positive);
	}


	private void OnMainMenuButtonPressed()
	{
		Game.ShowMainMenu();
		Visible = false;
		AudioManager.PlaySFX(SoundEffectEnum.GUI_Negative);
		AudioManager.PlayTrack(MusicTrackEnum.Menu);
	}	
}
