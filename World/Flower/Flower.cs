using System;
using System.Collections.Generic;
using Godot;

public class Flower : Area2D
{
	[Export] public bool Landable = true;
	[Export] public Color NotLandableTint;

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
		

		//_navigation2D.GetNode<Navigation2D>("Navigation2D");
	}

	public void SetSpriteTexture(Texture spriteTexture)
	{
		GetNode<Sprite>("Sprite").Texture = spriteTexture;
	}

	public void SetResource(ResourceTypeEnum resource)
	{
		_resource = resource;
		GetNode<Sprite>("Sprite").SelfModulate = _flowerColors[resource];
	}

	public void AcceptLander(Node2D lander)
	{
		
	}

	public void RemoveLander(Node2D lander)
	{
		// lander.GetParent().RemoveChild(lander);
		// Game.CurrentWorld.AddChild(lander);
	}
	
}
