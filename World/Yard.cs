using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

public class Yard : Node2D
{
	[Export] private PackedScene _carpenterBee;
	[Export] private PackedScene _levelExit;

	[Export] public float CarpenterSpawnRate = 5f;
	[Export] public float CarpenterFlySpeed = .1f;

	[Export] public float BirdDangerTime = 30f; //Seconds from 0 to 100 bird danger, assuming no hiding/landing.
	[Export] public float BirdCalmdownTime = 10f; //Seconds from 0 to 100 bird danger, assuming no hiding/landing.
	[Export] public int CrabSpiderCount = 1; //todo trash this??

	[Export] private float _offscreenSpawnPoint = 120f;

	public List<Position2D> SpawnPoints { get; } = new List<Position2D>();

	private Node2D _decor;
	private Node2D _bounds;
	private CanvasLayer _bg;
	private Timer _carpenterTimer;

	public Yard Current { get; private set; }

	public override void _Ready()
	{
		if (IsInstanceValid(Current))
		{
			Current.QueueFree();
		}
		Current = this;
		
		var spawns = GetNode<Node>("SpawnPoints").GetChildren();
		foreach (var spawn in spawns)
		{
			if (spawn is Position2D spawnPoint)
			{
				SpawnPoints.Add(spawnPoint);
			}
		}

		_decor = GetNode<Node2D>("Decor");
		_bounds = GetNode<Node2D>("Bounds");
		_bg = GetNode<CanvasLayer>("Background");
		_carpenterTimer = GetNode<Timer>("CarpenterBeeSpawn");
		_carpenterTimer.WaitTime = CarpenterSpawnRate;
		_carpenterTimer.Start();
		
		RandomizeLevel();

		DangerBar.SetBarVisibility(BirdDangerTime > 0f);
		DangerBar.Setup(BirdDangerTime, BirdCalmdownTime);
		DangerBar.Start();
	}

	private void RandomizeLevel()
	{
		var rotation = Game.Random.Next(0, 360);

		_decor.RotationDegrees = rotation;
		_bounds.RotationDegrees = rotation;
		_bg.RotationDegrees = rotation;

		var decorElements = _decor.GetChildren();
		foreach (var decorElement in decorElements)
		{
			if (decorElement is Node2D decorNode)
			{
				decorNode.RotationDegrees = Game.Random.Next(0, 360);

				if (decorElement is FlowerCollection fc)
				{
					foreach (Node n in fc.GetChildren())
					{
						if (n is Leaf leaf) leaf.Cull();
					}
				}
			}
		}

		var hiveSpawn = Game.Random.Next(1, SpawnPoints.Count - 1);
		var playerSpawn = hiveSpawn + 1;

		var newHive = _levelExit.Instance<HiveExit>();
		AddChild(newHive);
		newHive.Position = SpawnPoints[hiveSpawn].Position;

		Beelancer.Current.Position = SpawnPoints[playerSpawn].Position;
	}

	private void OnCarpenterBeeSpawnTimeout()
	{
		if (!IsInstanceValid(Beelancer.Current)) return;
		Vector2 origin = Vector2.Zero;

		var direction = Game.Random.Next(0, 3);
		if (direction <= 1)
		{
			var xOffset = GetViewport().Size.x / 2;
			var yOffset = Game.Random.Next(-100, 100);
			if (direction == 0) 
				origin = Beelancer.Current.GlobalPosition + new Vector2(-(xOffset + _offscreenSpawnPoint), yOffset);
			else
				origin = Beelancer.Current.GlobalPosition + new Vector2(xOffset + _offscreenSpawnPoint, yOffset);
		}
		else
		{
			var xOffset = Game.Random.Next(-100, 100);
			var yOffset = GetViewport().Size.y / 2;
			
			if (direction == 2)
				origin = Beelancer.Current.GlobalPosition + new Vector2(xOffset, -(yOffset + _offscreenSpawnPoint));
			else
				origin = Beelancer.Current.GlobalPosition + new Vector2(xOffset, yOffset + _offscreenSpawnPoint);
		}

		var newCarpenter = _carpenterBee.Instance<CarpenterBee>();
		AddChild(newCarpenter);
		newCarpenter.GlobalPosition = origin;
		newCarpenter.Setup(newCarpenter.GetAngleTo(Beelancer.Current.Position), CarpenterFlySpeed);
	}
} 
