using Godot;
using System;
using Godot.Collections;

public class AudioManager : Node
{
	[Export] private AudioStream _menuMusic;
	[Export] private AudioStream _exploreMusic;

	[Export] private float _sfxDb = 1f;
	[Export] private float _musicDb = .7f;

	//Music
	private static AudioStreamPlayer _musicPlayer;
	private static Dictionary<MusicTrackEnum, AudioStream> _music;

	private static Dictionary<SoundEffectEnum, AudioStreamPlayer> _sounds;

	private static bool _soundOn = true;
	private static bool _musicOn = true;
	

	public override void _Ready()
	{
		_musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");
		_music = new Dictionary<MusicTrackEnum, AudioStream>();
		_music.Add(MusicTrackEnum.Menu, _menuMusic);
		_music.Add(MusicTrackEnum.Exploration, _exploreMusic);
		
		//start menu music since we're there when _ready is called anyway
		PlayTrack(MusicTrackEnum.Menu);
		
		_sounds = new Dictionary<SoundEffectEnum, AudioStreamPlayer>
		{
			{SoundEffectEnum.GUI_Positive, GetNode<AudioStreamPlayer>("SFX/GUI_Positive")},
			{SoundEffectEnum.GUI_Negative, GetNode<AudioStreamPlayer>("SFX/GUI_Negative")},
			{SoundEffectEnum.GUI_UpgradePurchase, GetNode<AudioStreamPlayer>("SFX/GUI_Upgrade")},
			{SoundEffectEnum.GUI_Buy, GetNode<AudioStreamPlayer>("SFX/GUI_Buy")},
			{SoundEffectEnum.GUI_Sell, GetNode<AudioStreamPlayer>("SFX/GUI_Sell")},
			{SoundEffectEnum.Explore_PollenPickup, GetNode<AudioStreamPlayer>("SFX/Explore_Pollen")},
			{SoundEffectEnum.Explore_BirdCaw, GetNode<AudioStreamPlayer>("SFX/Explore_Bird")},
			{SoundEffectEnum.Explore_CarpenterRobbery, GetNode<AudioStreamPlayer>("SFX/Explore_Danger")},
			{SoundEffectEnum.Explore_Squish, GetNode<AudioStreamPlayer>("SFX/Explore_Squish")},
			{SoundEffectEnum.Explore_Death, GetNode<AudioStreamPlayer>("SFX/Explore_GameOver")},
		};
	}
	
	public static void PlayTrack(MusicTrackEnum musicTrack)
	{
		if (!_musicOn) return;
		
		if (_music.ContainsKey(musicTrack))
		{
			_musicPlayer.Stop();
			_musicPlayer.Stream = _music[musicTrack];
			
			if (_musicOn) _musicPlayer.Play();
		}
		else
		{
			GD.PrintErr($"AudioManager is missing music track ({musicTrack})");
		}
	}

	public static void PlaySFX(SoundEffectEnum soundEffect)
	{
		if (!_soundOn) return;
		
		if (_sounds.ContainsKey(soundEffect))
		{
			_sounds[soundEffect].Play();
		}
		else
		{
			GD.PrintErr($"AudioManager is missing sound effect ({soundEffect})");
		}
	}

	public static void SetSfxActive(bool active)
	{
		_soundOn = active;
	}

	public static void SetMusicActive(bool active)
	{
		_musicOn = active;
		if (active)
		{
			_musicPlayer.Play();
		}
		else
		{
			_musicPlayer.Stop();
		}
	}

}
