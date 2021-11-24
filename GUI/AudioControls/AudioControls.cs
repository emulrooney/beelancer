using Godot;
using System;

public class AudioControls : HBoxContainer
{
	//Signalled.
	private void OnSFXButtonToggled(bool pressed)
	{
		GD.Print("SFX " + pressed);
		AudioManager.SetSfxActive(pressed);
	}

	//Signalled.
	private void OnMusicButtonToggled(bool pressed)
	{
		GD.Print("Music " + pressed);
		AudioManager.SetMusicActive(pressed);
	}
}
