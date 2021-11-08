using Godot;

public class Leaf : Area2D
{
	[Export] private float _coverTransparency = .5f;
	private Color _initialColor;
	private Color _transparentColor;
	
	public override void _Ready()
	{
		_initialColor = Modulate;
		_transparentColor = new Color(Modulate.r, Modulate.g, Modulate.b, _coverTransparency);
	} 
	
	//Signalled
	private void OnLeafBodyEntered(object body)
	{
		if (body is Beelancer bee)
		{
			Modulate = _transparentColor;
			Beelancer.Current.InCover = true;
			
			DangerBar.SetActiveDanger(false);
		}
	}

	//Signalled
	private void OnLeafBodyExited(object body)
	{
		if (body is Beelancer bee)
		{
			Modulate = _initialColor;
			Beelancer.Current.InCover = false;
			
			DangerBar.SetActiveDanger(true);
		}
	}
}
