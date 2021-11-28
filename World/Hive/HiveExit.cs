using Godot;
using System;

public class HiveExit : StaticBody2D
{
	private void OnEnterHiveBodyEntered(object body)
	{
		if (body is Beelancer bee)
		{
			GUIManager.SetGameState(GameStateEnum.HiveMenu);
		}
	}
}

