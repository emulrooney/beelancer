using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

public class Yard : Node2D
{
	[Export] private PackedScene _carpenterBee;
	[Export] private PackedScene _levelExit;

	[Export] public float CarpenterSpawnRate = 20f;
	[Export] public float CarpenterFlySpeed = 30f;

	[Export] public float BirdDangerTime = 30f; //Seconds from 0 to 100 bird danger, assuming no hiding/landing.
	[Export] public float BirdCalmdownTime = 10f; //Seconds from 0 to 100 bird danger, assuming no hiding/landing.
	[Export] public int CrabSpiderCount = 1; //todo trash this??

	public List<Position2D> SpawnPoints { get; } = new List<Position2D>();

	private Node2D _decor;
	private Node2D _bounds;
	private CanvasLayer _bg;

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

}
