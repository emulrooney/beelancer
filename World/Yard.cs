using Godot;
using System;
using System.Diagnostics.Contracts;

public class Yard : Node2D
{
	[Export] public float BirdDangerTime = 90f; //Seconds from 0 to 100 bird danger, assuming no hiding/landing.
	[Export] public float BirdCalmdownTime = 20f; //Seconds from 0 to 100 bird danger, assuming no hiding/landing.
	[Export] public int CrabSpiderCount = 1;

	public Yard Current { get; private set; }

	public override void _Ready()
	{
		if (IsInstanceValid(Current))
		{
			Current.QueueFree();
		}

		Current = this;
		
		DangerBar.SetBarVisibility(BirdDangerTime > 0f);
		DangerBar.Setup(BirdDangerTime, BirdCalmdownTime);
		DangerBar.Start();
	}

}
