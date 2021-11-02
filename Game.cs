using Godot;
using System;

public class Game : Node
{
	public override void _Process(float delta)
	{
		//TODO better way to do this
		if (IsInstanceValid(Beelancer.Current) && IsInstanceValid(GameCamera.Current))
		{
			//TODO Set zoom based on location
			//TODO zoom in and out based on velocity
			GameCamera.Current.Position = Beelancer.Current.Position;
		}
		
	}

}
