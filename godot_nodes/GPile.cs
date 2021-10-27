using System;
using Godot;

public class GPile : TextureRect, GodotView<Pile>
{
	[Export]
	public bool IsFaceUp;

	private Pile Pile;

	public void Connect(Pile pile)
	{
		Pile = pile;
		pile.UpdatePileTexture += SetTexture;
		SetTexture(pile.TopCard());
	}

	public void SetTexture(RegicideCard card)
	{
		String path;
		if (card == null)
		{
			Texture = null;
			return;
		}

		if (IsFaceUp)
			path = TexturePaths.CardTexture(card.CardSuit, card.Power);
		else
			path = TexturePaths.BACK;
		Texture = ResourceLoader.Load(path) as Texture;
	}

	public override void _Ready()
	{

	}
}
