using System;
using System.Collections.Generic;
using Godot;

public class Flower : Area2D
{
	[Export] public PackedScene Pollen { get; private set; }
	[Export] public bool Landable { get; private set; } = true;
	[Export] public Color NotLandableTint { get; private set; }
	[Export] public Vector2 PollenSpawnPoint { get; private set; }

	private Navigation2D _navigation2D;

	private static readonly Dictionary<ResourceTypeEnum, Color> _flowerColors = new Dictionary<ResourceTypeEnum, Color>
	{
		{ResourceTypeEnum.RedPollen, new Color(1f, 0.54f, .38f)},
		{ResourceTypeEnum.BluePollen, new Color(.4f, .6f, 1f)},
		{ResourceTypeEnum.GreenPollen, new Color(.37f, 1f, .96f)},
	};

	private ResourceTypeEnum _resource;

	public override void _Ready()
	{
		if (!Landable)
		{
			Modulate = NotLandableTint;
		}

		//Randomize rotation for some visual variety
		Rotation = Game.Random.Next(0, 360);

		_navigation2D = GetNode<Navigation2D>("Navigation2D");
	}

	public void SetSpriteTexture(Texture spriteTexture)
	{
		GetNode<Sprite>("Sprite").Texture = spriteTexture;
	}

	public void SetResource(ResourceTypeEnum resource, int minPollenNodes = 5, int maxPollenNodes = 10,
		float valuePerNode = 1f, float collectionRate = 0.2f)
	{
		_resource = resource;
		GetNode<Sprite>("Sprite").SelfModulate = _flowerColors[resource];

		int nodes = Game.Random.Next(minPollenNodes, maxPollenNodes);
		for (int i = 0; i < nodes; i++)
		{
			int xPosition = Game.Random.Next((int)-PollenSpawnPoint.x, (int)PollenSpawnPoint.x);
			int yPosition = Game.Random.Next((int)-PollenSpawnPoint.x, (int)PollenSpawnPoint.x);
			
			var pollenInstance = Pollen.Instance<PollenDeposit>();
			pollenInstance.Setup(resource, valuePerNode, collectionRate);
			pollenInstance.Rotation = Game.Random.Next(0, 360);
			AddChild(pollenInstance);
			pollenInstance.Position = new Vector2(xPosition, yPosition);
		}
	}
}
