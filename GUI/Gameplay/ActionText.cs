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
	private static Dictionary<ActionTextEnum, string> _text;
	private static Dictionary<ActionTextEnum, Color> _textColors;

	private static Label _overburdened;

	public override void _Ready()
	{
		Current = this;
		_overburdened = GetNode<Label>("Overburdened");
		
		_text = new Dictionary<ActionTextEnum, string>
		{
			{ActionTextEnum.Land, "Press [Spacebar] to land."},
			{ActionTextEnum.Takeoff, "Press [Spacebar] to takeoff."},
			{ActionTextEnum.Heavy, "You're weighed down with pollen! Head to a hive to trade."},
			{ActionTextEnum.BirdDanger, "A bird is about to strike - find cover or hide in a hive!"},
			{ActionTextEnum.EnterHive, "Press [Spacebar] to enter hive."},
			{ActionTextEnum.LeaveYard, "Press [Spacebar] to exit the yard."},
		};	
		
		_textColors = new Dictionary<ActionTextEnum, Color>
		{
			{ActionTextEnum.Land, NormalColor},
			{ActionTextEnum.Takeoff, MovementColor},
			{ActionTextEnum.Heavy, ProblemColor},
			{ActionTextEnum.BirdDanger, DangerColor},
			{ActionTextEnum.EnterHive, MovementColor},
			{ActionTextEnum.LeaveYard, MovementColor},
		};
	}

	public static void SetText(ActionTextEnum textEnum)
	{
		Current.Text = _text[textEnum];
		Current.SelfModulate = _textColors[textEnum];
		Current.Visible = true;
	}

	public static void ShowOverburdenedText(bool burdened)
	{
		_overburdened.Visible = burdened;
	}

	public static void DismissText()
	{
		Current.Text = "";
	}
}
