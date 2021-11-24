using Godot;
using System;

public class AudioControls : HBoxContainer
{
	//Signalled.
	private void OnSFXButtonToggled(bool pressed)
	{
		AudioManager.SetSfxActive(pressed);
	}

	//Signalled.
	private void OnMusicButtonToggled(bool pressed)
	{
		AudioManager.SetMusicActive(pressed);
	}
}
