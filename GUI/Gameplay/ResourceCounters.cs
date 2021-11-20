using Godot;
using System;
using System.Collections.Generic;

public class ResourceCounters : VBoxContainer
{
	private static ResourceCounters _instance;
	
	private Dictionary<ResourceTypeEnum, Label> _resourceLabels;
	private ProgressBar _progressBar;

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
			{ResourceTypeEnum.Honey, GetNode<Label>("Honey/Label")}
		};

		_progressBar = GetParent().GetNode<ProgressBar>("Control/Weight");
		_instance._resourceLabels = resourceLabels;
	}
	
	public static void UpdatePlayerCollection(Dictionary<ResourceTypeEnum, float> resources, float currentWeight, float maxWeightBeforeSlowdown)
	{
		foreach (var resource in resources)
		{
			_instance.UpdateLabel(resource.Key, resource.Value);
		}

		var progress = Math.Min(currentWeight / maxWeightBeforeSlowdown, 1f) * 100;
		_instance._progressBar.Value = progress;

		ActionText.ShowOverburdenedText(progress >= 100);
	}

	private void UpdateLabel(ResourceTypeEnum key, float value)
	{
		var valueString = $"{value:0.00}";
		_resourceLabels[key].Text = valueString;
	}
}
