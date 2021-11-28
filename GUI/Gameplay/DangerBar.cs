using Godot;
using System;

/**
 * TODO this stuff with _instance is dumb, just have an instance 
 */
public class DangerBar : TextureProgress
{
	[Export] public float[] BirdIndicatorTimePercentages;
	private float[] _birdIndicatorTimes;
	private bool[] _birdAppearances;
	[Export] public float MinimumBirdAppearanceTimePercent;
	private float _minimumBirdAppearanceTime;

	private static DangerBar _instance;
	private Timer _activeTimer;
	private Timer _hiddenTimer;
 
	private double _currentValue = 0;
	private float _timerIncrementAmount;
	private float _timerDecrementAmount;

	public override void _Ready()
	{
		_instance = this;
		_activeTimer = GetNode<Timer>("ActiveTimer");
		_hiddenTimer = GetNode<Timer>("HiddenTimer");
	}

	public static void Start()
	{
		var sneakBonus = Beelancer.Current.GetBonus(UpgradeTypeEnum.Sneak);
		
		_instance._activeTimer.WaitTime = _instance._timerIncrementAmount;
		_instance._hiddenTimer.WaitTime = _instance._timerDecrementAmount + sneakBonus;
		
		_instance._birdAppearances = new bool[_instance.BirdIndicatorTimePercentages.Length];
		_instance._birdIndicatorTimes = new float[_instance.BirdIndicatorTimePercentages.Length];

		for (int i = 0; i < _instance._birdIndicatorTimes.Length; i++)
		{
			_instance._birdAppearances[i] = false;
			_instance._birdIndicatorTimes[i] = _instance.BirdIndicatorTimePercentages[i] * (100 / _instance._timerIncrementAmount);
		}

		_instance._currentValue = 0;
	}

	public static void SetActiveDanger(bool inDanger)
	{
		if (inDanger)
		{
			_instance._hiddenTimer.Stop();
			_instance._activeTimer.Start();
		}
		else
		{
			_instance._activeTimer.Stop();
			_instance._hiddenTimer.Start();
		}
	}

	public static void Setup(float dangerTime, float calmTime)
	{
		_instance._timerIncrementAmount = dangerTime / 100;
		_instance._timerDecrementAmount = calmTime / 100;
		_instance._minimumBirdAppearanceTime *= dangerTime; 
		
		SetActiveDanger(true);
	}
	
	public static void SetBarVisibility(bool visible)
	{
		_instance.Visible = visible;
	}

	private void OnActiveTimerTimeout()
	{
		_currentValue += _timerIncrementAmount;
		_currentValue = Math.Min(_currentValue, 100);
		Value = _currentValue;

		for (int i = 0; i < _birdIndicatorTimes.Length; i++)
		{
			if (!_birdAppearances[i] && _currentValue > _birdIndicatorTimes[i])
			{
				_birdAppearances[i] = true;

				AudioManager.PlaySFX(SoundEffectEnum.Explore_BirdCaw);
				GameCamera.BirdTime();
			}
		}

		if (_currentValue >= 100)
		{
			//Player dead.
			GameCamera.BirdKillsPlayer();
		}
	}

	private void OnInactiveTimerTimeout()
	{
		_currentValue -= _timerIncrementAmount;
		_currentValue = Math.Max(_currentValue, 0);
		Value = _currentValue; 
	}
	 
}
