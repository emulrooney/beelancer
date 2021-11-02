using Godot;
using System;

public class GameCamera : Camera2D
{
	[Export] private Vector2 roundProcessTo = new Vector2(1f, 1f);
	private Vector2 lastPosition;

	private Vector2 currentOffset;
	
	public static GameCamera Current { get; set; }

	private ShaderMaterial _bgShader;

	public override void _Ready()
	{
		if (IsInstanceValid(Current))
		{
			Current.QueueFree();
		}
		Current = this;

		_bgShader = (ShaderMaterial)GetNode<TextureRect>("Background/TextureRect").Material;
	}

	public override void _Process(float delta)
	{
		var offset = (Position - lastPosition).Snapped(roundProcessTo);
		currentOffset = (currentOffset + offset);
		_bgShader.SetShaderParam("offset", currentOffset);
		lastPosition = Position;
	}
}
