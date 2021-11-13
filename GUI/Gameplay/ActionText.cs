using Godot;
using System;
using System.Collections.Generic;

public class ActionText : Label
{
	public static ActionText Current { get; private set; }
	[Export] private Color NormalColor { get; set; }
	[Export] private Color MovementColor { get; set; }
	[Export] private Color ProblemColor { get; set; }
	[Export] private Color DangerColor { get; set; }

	//TODO Could create a struct for these and combine into one
	private static Dictionary<ActionTextType, string> _text;
	private static Dictionary<ActionTextType, Color> _textColors;

	public override void _Ready()
	{
		Current = this;
		
		_text = new Dictionary<ActionTextType, string>
		{
			{ActionTextType.Land, "Press [Spacebar] to land."},
			{ActionTextType.Takeoff, "Press [Spacebar] to takeoff."},
			{ActionTextType.Heavy, "You're weighed down with pollen! Head to a hive to trade."},
			{ActionTextType.BirdDanger, "A bird is about to strike - find cover or hide in a hive!"},
			{ActionTextType.EnterHive, "Press [Spacebar] to enter hive."},
			{ActionTextType.LeaveYard, "Press [Spacebar] to exit the yard."},
		};	
		
		_textColors = new Dictionary<ActionTextType, Color>
		{
			{ActionTextType.Land, NormalColor},
			{ActionTextType.Takeoff, MovementColor},
			{ActionTextType.Heavy, ProblemColor},
			{ActionTextType.BirdDanger, DangerColor},
			{ActionTextType.EnterHive, MovementColor},
			{ActionTextType.LeaveYard, MovementColor},
		};
	}

	public static void SetText(ActionTextType textType)
	{
		Current.Text = _text[textType];
		Current.SelfModulate = _textColors[textType];
		Current.Visible = true;
	}

	public static void DismissText()
	{
		Current.Text = "";
	}
}
