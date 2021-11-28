using Godot;
using System;
using Godot.Collections;

public class HiveMenu : CenterContainer
{
	private static HiveMenu _instance;
	
	[Export] private float _buttonClickTradeQuantity = 1f;

	[Export] private int[] _upgradeCosts = { 0, 25, 30, 35, 40, 45, 50 };
	
	public Hive CurrentHive { get; private set; }

	private Label _hiveName;

	private TextureRect _buySprite;
	private TextureRect _sellSprite;
	private Label _buyRate;
	private Label _sellRate;
	private Dictionary<UpgradeTypeEnum, Button> _upgradeButtons;

	private readonly Dictionary<UpgradeTypeEnum, ResourceTypeEnum> _resourceNeeded = new Dictionary<UpgradeTypeEnum, ResourceTypeEnum>
	{
		{UpgradeTypeEnum.Carry, ResourceTypeEnum.RedPollen},
		{UpgradeTypeEnum.Trade, ResourceTypeEnum.RedPollen},
		{UpgradeTypeEnum.Accelerate, ResourceTypeEnum.GreenPollen},
		{UpgradeTypeEnum.Speed, ResourceTypeEnum.GreenPollen},
		{UpgradeTypeEnum.Sneak, ResourceTypeEnum.BluePollen},
		{UpgradeTypeEnum.Gather, ResourceTypeEnum.BluePollen}
	};


	public override void _Ready()
	{
		_instance = this;
		
		_hiveName = GetNode<Label>("Panel/Hive Name");
		_buySprite = GetNode<TextureRect>("Panel/Control/HBoxContainer/Trading/Panel/HBoxContainer/Buying Side/TextureRect");
		_sellSprite = GetNode<TextureRect>("Panel/Control/HBoxContainer/Trading/Panel/HBoxContainer/Selling Side/TextureRect");
		_buyRate = GetNode<Label>("Panel/Control/HBoxContainer/Trading/Panel/HBoxContainer/Buying Side/Amount");
		_sellRate = GetNode<Label>("Panel/Control/HBoxContainer/Trading/Panel/HBoxContainer/Selling Side/Amount");

		_upgradeButtons = new Dictionary<UpgradeTypeEnum, Button>();
		_upgradeButtons.Add(UpgradeTypeEnum.Carry, GetNode<Button>("Panel/Control/HBoxContainer/Upgrades/Panel/GridContainer/CarryUpgrade"));
		_upgradeButtons.Add(UpgradeTypeEnum.Accelerate, GetNode<Button>("Panel/Control/HBoxContainer/Upgrades/Panel/GridContainer/AccelerateUpgrade"));
		_upgradeButtons.Add(UpgradeTypeEnum.Sneak, GetNode<Button>("Panel/Control/HBoxContainer/Upgrades/Panel/GridContainer/SneakUpgrade"));
		_upgradeButtons.Add(UpgradeTypeEnum.Trade, GetNode<Button>("Panel/Control/HBoxContainer/Upgrades/Panel/GridContainer/TradeUpgrade"));
		_upgradeButtons.Add(UpgradeTypeEnum.Speed, GetNode<Button>("Panel/Control/HBoxContainer/Upgrades/Panel/GridContainer/SpeedUpgrade"));
		_upgradeButtons.Add(UpgradeTypeEnum.Gather, GetNode<Button>("Panel/Control/HBoxContainer/Upgrades/Panel/GridContainer/GatherUpgrade"));
	}

	/**
	 * Facade to show menu
	 */
	public static void ShowMenu()
	{
		_instance._ShowMenu();
	}

	
	private void _ShowMenu()
	{
		CurrentHive = new Hive();

		_hiveName.Text = CurrentHive.HiveName;
		_buySprite.Modulate = SetSpriteColor(CurrentHive.Buying.Key);
		_sellSprite.Modulate = SetSpriteColor(CurrentHive.Selling.Key);
		_buyRate.Text = $"For {CurrentHive.Buying.Value:0.00}/Unit";
		_sellRate.Text = $"For {CurrentHive.Selling.Value:0.00}/Unit";

		UpdateUpgrades();
		
		Visible = true;
		GetTree().Paused = true;
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

	private void UpdateUpgrades()
	{
		foreach (var upgrade in _upgradeButtons)
		{
			var level = Game.CurrentLevels[upgrade.Key];
			var button = _upgradeButtons[upgrade.Key];

			button.GetNode<Label>("Price").Text = _upgradeCosts[level].ToString();
			button.GetNode<TextureProgress>("TextureProgress").Value = level;
		}
	}
	
	//Signalled
	private void OnPurchasePressed()
	{
		var cost = CurrentHive.Buying.Value;
		var quantity = Math.Min(1, Beelancer.Current.GetResourceQuantity(ResourceTypeEnum.Honey));

		Beelancer.Current.ModifyResourceQuantity(ResourceTypeEnum.Honey, -cost * quantity);
		Beelancer.Current.ModifyResourceQuantity(CurrentHive.Buying.Key, 1 * quantity);

		AudioManager.PlaySFX(SoundEffectEnum.GUI_Sell);
	}

	//Signalled
	private void OnSellPressed()
	{
		var cost = CurrentHive.Selling.Value;
		var quantity = Math.Min(1, Beelancer.Current.GetResourceQuantity(CurrentHive.Selling.Key));

		Beelancer.Current.ModifyResourceQuantity(ResourceTypeEnum.Honey, cost * quantity);
		Beelancer.Current.ModifyResourceQuantity(CurrentHive.Selling.Key, -1 * quantity);
		
		AudioManager.PlaySFX(SoundEffectEnum.GUI_Sell);
	}
	
	private void OnUpgradePressed(int upgrade)
	{
		var upgradeType = (UpgradeTypeEnum) upgrade;
		
		var cost = _upgradeCosts[Game.CurrentLevels[upgradeType]];
		var resourceNeeded = _resourceNeeded[upgradeType];
		var resourceOnHand = Beelancer.Current.GetResourceQuantity(resourceNeeded);
		
		if (Game.CurrentLevels[upgradeType] < 6 && resourceOnHand >= cost)
		{
			Game.CurrentLevels[upgradeType] += 1;
			Beelancer.Current.ModifyResourceQuantity(resourceNeeded, -cost);
			UpdateUpgrades();
		}
		
		AudioManager.PlaySFX(SoundEffectEnum.GUI_UpgradePurchase);
	}
	
	private void OnLeaveHivePressed()
	{
		Game.NewYard();
		GUIManager.SetGameState(GameStateEnum.Gameplay);
		AudioManager.PlaySFX(SoundEffectEnum.GUI_Negative);
		GetTree().Paused = false;
	}
}


