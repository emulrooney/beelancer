using Godot;
using System;
using System.Collections.Generic;

public class ResourceCounters : VBoxContainer
{
	private static ResourceCounters _instance;
	
	private Dictionary<ResourceTypeEnum, Label> _resourceLabels;
	
	public static void UpdatePlayerCollection(Dictionary<ResourceTypeEnum, float> resources)
	{
		foreach (var resource in resources)
		{
			_instance.UpdateLabel(resource.Key, resource.Value);
		}
	}

	private void UpdateLabel(ResourceTypeEnum key, float value)
	{
		var valueString = $"{value:0.00}";
		_resourceLabels[key].Text = valueString;
	}

	public override void _Ready()
	{
		if (IsInstanceValid(_instance) && _instance != this)
		{
			_instance.QueueFree();
		}
		_instance = this;

		var resourceLabels = new Dictionary<ResourceTypeEnum, Label>
		{
			{ResourceTypeEnum.RedPollen, GetNode<Label>("RedPollen/Label")},
			{ResourceTypeEnum.BluePollen, GetNode<Label>("BluePollen/Label")},
			{ResourceTypeEnum.GreenPollen, GetNode<Label>("GreenPollen/Label")},
			{ResourceTypeEnum.Honey, GetNode<Label>("Honey/Label")},
			// {ResourceTypeEnum.Nectar, GetNode<Label>("Nectar/Label")},
			// {ResourceTypeEnum.Energy, GetNode<Label>("GreenPollen/Label")}, //TODO Add label
		};

		_instance._resourceLabels = resourceLabels;
	}
}
