using Godot;
using System;
using System.Collections.Generic;

public class PollenDeposit : Area2D
{
	[Export] public ResourceTypeEnum ResourceType { get; private set; }
	
	[Export] public float Value { get; private set; } = 2f;
	[Export] public float CollectionRate { get; private set; } = 0.2f; //Lower numbers are harder to gather.

	private float _maxValue;
	private float _currentValue;

	private Sprite _sprite;
	private Vector2 _initialScale;
	
	public override void _Ready()
	{
		_sprite = GetNode<Sprite>("Sprite");
		_maxValue = Value;
		_currentValue = Value;

		_initialScale = _sprite.Scale;
	}

	public void Setup(ResourceTypeEnum resourceType, float value, float collectionRate)
	{
		ResourceType = resourceType;
		Value = value;
		CollectionRate = collectionRate;
	}

	public void Harvest()
	{
		_currentValue -= CollectionRate;
		_sprite.Scale = _initialScale * (_currentValue / _maxValue);
		if (_currentValue <= 0)
		{
			QueueFree();
		}
	}
	

}
