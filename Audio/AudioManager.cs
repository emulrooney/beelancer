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

	private static Dictionary<SoundEffectEnum, AudioStreamPlayer> _sounds;


	public override void _Ready()
	{
		_musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");
		_music = new Dictionary<MusicTrackEnum, AudioStream>();
		_music.Add(MusicTrackEnum.Menu, _menuMusic);
		_music.Add(MusicTrackEnum.Exploration, _exploreMusic);
		
		//start menu music since we're there when _ready is called anyway
		PlayTrack(MusicTrackEnum.Menu);
		
		_sounds = new Dictionary<SoundEffectEnum, AudioStreamPlayer>();
		_sounds.Add(SoundEffectEnum.GUI_Positive, GetNode<AudioStreamPlayer>("SFX/GUI_Positive"));
		_sounds.Add(SoundEffectEnum.GUI_Negative, GetNode<AudioStreamPlayer>("SFX/GUI_Negative"));
	}
	
	public static void PlayTrack(MusicTrackEnum musicTrack)
	{
		if (_music.ContainsKey(musicTrack))
		{
			_musicPlayer.Stop();
			_musicPlayer.Stream = _music[musicTrack];
			_musicPlayer.Play();
		}
		else
		{
			GD.PrintErr($"AudioManager is missing music track ({musicTrack})");
		}
	}

	public static void PlaySFX(SoundEffectEnum soundEffect)
	{
		if (_sounds.ContainsKey(soundEffect))
		{
			_sounds[soundEffect].Play();
		}
		else
		{
			GD.PrintErr($"AudioManager is missing sound effect ({soundEffect})");
		}
		
	}

}
