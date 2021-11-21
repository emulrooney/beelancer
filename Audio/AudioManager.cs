using Godot;
using System;
using Godot.Collections;

public class AudioManager : Node
{
	[Export] private AudioStream _menuMusic;
	[Export] private AudioStream _exploreMusic;

	//Music
	private static AudioStreamPlayer _musicPlayer;
	private static Dictionary<MusicTrackEnum, AudioStream> _music;
	
	//SFX
	private static AudioStreamPlayer _guiPositiveSound;
	private static AudioStreamPlayer _guiNegativeSound;


	public override void _Ready()
	{
		_musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");

		_guiPositiveSound = GetNode<AudioStreamPlayer>("SFX/GUI_Positive");
		_guiNegativeSound = GetNode<AudioStreamPlayer>("SFX/GUI_Negative");

		_music = new Dictionary<MusicTrackEnum, AudioStream>();
		_music.Add(MusicTrackEnum.Menu, _menuMusic);
		_music.Add(MusicTrackEnum.Exploration, _exploreMusic);
		
		//start menu music since we're there when _ready is called anyway
		PlayTrack(MusicTrackEnum.Menu);
	}
	
	public static void PlayTrack(MusicTrackEnum musicTrack)
	{
		GD.Print("Playing:" + musicTrack);
		_musicPlayer.Stop();
		_musicPlayer.Stream = _music[musicTrack];
		_musicPlayer.Play();
	}

	public static void PlaySFX(SoundEffectEnum soundEffect)
	{
		switch (soundEffect)
		{
			case SoundEffectEnum.GUI_Positive:
				_guiPositiveSound.Play();
				break;
			case SoundEffectEnum.GUI_Negative:
				_guiNegativeSound.Play();
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(soundEffect), soundEffect, null);
		}
		
		
		
		
	}

}
