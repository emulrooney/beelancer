using Godot;
using System;

public class MainMenu : Control
{
	private PopupDialog _credits;
	private PopupMenu _options;

	public override void _Ready()
	{
		_credits = GetNode<PopupDialog>("CenterContainer/CreditsDialog");
		_options = GetNode<PopupMenu>("CenterContainer/OptionsMenu");
	}

	private void OnCreditsDismissPressed()
	{
		_credits.Visible = false;
	}
	
	private void OnCreditsButtonPressed()
	{
		_credits.Visible = true;
	}
	
	private void OnSettingsButtonPressed()
	{
		_options.Visible = true;
	}
	
	private void OnStartGameButtonPressed()
	{
		Game.NewGame();
		
	}
	
	private void OnSaveOptionsPressed()
	{
		//Todo: Set options
		
		_options.Visible = false;
	}
	
	private void OnCancelOptionsPressed()
	{
		_options.Visible = false;
	}
}
