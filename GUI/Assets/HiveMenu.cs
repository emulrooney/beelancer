using Godot;
using System;
using Godot.Collections;

public class HiveMenu : CenterContainer
{
	private static HiveMenu _instance;
	
	[Export] private float _buttonClickTradeQuantity = .5f;
	
	public Hive CurrentHive { get; private set; }

	private Label _hiveName;

	private TextureRect _buySprite;
	private TextureRect _sellSprite;
	private Label _buyRate;
	private Label _sellRate;

	public override void _Ready()
	{
		_instance = this;
		
		_hiveName = GetNode<Label>("Panel/Hive Name");
		_buySprite = GetNode<TextureRect>("Panel/Control/HBoxContainer/Trading/Panel/HBoxContainer/Buying Side/TextureRect");
		_sellSprite = GetNode<TextureRect>("Panel/Control/HBoxContainer/Trading/Panel/HBoxContainer/Selling Side/TextureRect");
		_buyRate = GetNode<Label>("Panel/Control/HBoxContainer/Trading/Panel/HBoxContainer/Buying Side/Amount");
		_sellRate = GetNode<Label>("Panel/Control/HBoxContainer/Trading/Panel/HBoxContainer/Selling Side/Amount");
	}

	/**
	 * Facade to show menu
	 */
	public static void ShowMenu()
	{
		_instance._ShowMenu();
	}

	/**
	 * Facade
	 */
	public static void HideMenu()
	{
		_instance._HideMenu();
	}
	
	private void _ShowMenu()
	{
		CurrentHive = new Hive();

		_hiveName.Text = CurrentHive.HiveName;
		_buySprite.Modulate = SetSpriteColor(CurrentHive.Buying.Key);
		_sellSprite.Modulate = SetSpriteColor(CurrentHive.Selling.Key);
		_buyRate.Text = $"For {CurrentHive.Buying.Value:0.00}/Unit";
		_sellRate.Text = $"For {CurrentHive.Selling.Value:0.00}/Unit";

		Visible = true;
	}

	private void _HideMenu()
	{
		Visible = false;
	}

	private Color SetSpriteColor(ResourceTypeEnum resource)
	{
		switch (resource)
		{
			case ResourceTypeEnum.RedPollen:
				return new Color(1f, .54f, .38f);
			case ResourceTypeEnum.GreenPollen:
				return new Color(.37f, 1f, .96f);
			case ResourceTypeEnum.BluePollen:
				return new Color(.4f, .6f, 1f);
			default:
				throw new ArgumentOutOfRangeException(nameof(resource), resource, null);
		}
	}
	
	//Signalled
	private void OnPurchasePressed()
	{
		
	}

	//Signalled
	private void OnSellPressed()
	{
		// Replace with function body.
	}
	
	private void OnLeaveHivePressed()
	{
		HideMenu();
	}
}

