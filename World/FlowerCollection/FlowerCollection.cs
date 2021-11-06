using Godot;
using System;

public class FlowerCollection : Node2D
{
	[Export] public Texture[] ValidFlowerTextures;

	private readonly ResourceTypeEnum[] _resourceTypes =
	{
		ResourceTypeEnum.RedPollen,
		ResourceTypeEnum.GreenPollen,
		ResourceTypeEnum.BluePollen,
	};

	private ResourceTypeEnum _resource;

	public override void _Ready()
	{
		var resource = Game.Random.Next(0, _resourceTypes.Length);
		_resource = _resourceTypes[resource];
		
		Texture selected = ValidFlowerTextures[Game.Random.Next(0, ValidFlowerTextures.Length)];
		var nodeChildren = GetChildren();
		foreach (var nodeChild in nodeChildren)
		{
			if (nodeChild is Flower flower)
			{
				flower.SetSpriteTexture(selected);
				if (flower.Landable)
				{
					flower.SetResource(_resource);
				}
			}
		}
	}
}
