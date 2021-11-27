using Godot;
using System;

public class MainMenu : Control
{
	private static MainMenu _instance;
	
	private PopupDialog _credits;
	private PopupPanel _options;

	public override void _Ready()
	{
		_instance = this;
		_credits = GetNode<PopupDialog>("CenterContainer/CreditsDialog");
		_options = GetNode<PopupPanel>("CenterContainer/OptionsMenu");
	}

	public static void SetVisible(bool visible)
	{
		_instance.Visible = visible;
	}

	private void OnCreditsDismissPressed()
	{
		_credits.Visible = false;
		AudioManager.PlaySFX(SoundEffectEnum.GUI_Negative);
	}
	
	private void OnCreditsButtonPressed()
	{
		_credits.Visible = true;
		AudioManager.PlaySFX(SoundEffectEnum.GUI_Positive);
	}
	
	private void OnSettingsButtonPressed()
	{
		_options.Visible = true;
		AudioManager.PlaySFX(SoundEffectEnum.GUI_Positive);
	}
	
	private void OnStartGameButtonPressed()
	{
		Game.NewGame();
		AudioManager.PlaySFX(SoundEffectEnum.GUI_Positive);
		
	}
	
	private void OnSaveOptionsPressed()
	{
		//Todo: Set options
		_options.Visible = false;
		AudioManager.PlaySFX(SoundEffectEnum.GUI_Positive);
	}
	
	private void OnCancelOptionsPressed()
	{
		_options.Visible = false;
		AudioManager.PlaySFX(SoundEffectEnum.GUI_Negative);

	}
}
