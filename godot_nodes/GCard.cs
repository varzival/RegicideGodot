using Godot;
using System;

public class GCard: TextureRect, GodotView<RegicideCard>
{
	public delegate void DCardSelected(RegicideCard card);
	public event DCardSelected CardSelected;

	public RegicideCard Card;

	public GCard()
	{ }

	public GCard(RegicideCard card)
	{
		Connect(card);
	}

	public void Connect(RegicideCard card)
	{
		Card = card;
		SetTexture(card);
	}

	public void Connect(Enemy card)
	{
		Card = card;
		SetTexture(card);
	}

	public void Update(RegicideCard.Suit suit)
	{
		SetTexture(Card);
	}

	public void SetTexture(Enemy card)
	{
		string path = TexturePaths.CardTexture(card.CardSuit, card.Power, true);
		Texture = ResourceLoader.Load(path) as Texture;
	}

	public void SetTexture(RegicideCard card)
	{
		string path = TexturePaths.CardTexture(card.CardSuit, card.Power, false);
		Texture = ResourceLoader.Load(path) as Texture;
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton)
		{
			if  (
					((InputEventMouseButton)@event).ButtonIndex == (int)ButtonList.Left
					&& ((InputEventMouseButton)@event).IsPressed()
					&& ((InputEventMouseButton)@event).Doubleclick
				)
			{
				CardSelected?.Invoke(this.Card);
			}
		}
	}
}
