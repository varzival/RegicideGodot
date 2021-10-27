using System;
using System.Collections.Generic;


public class Hand
{
	public delegate void DUpdateCardHand(List<Card> cards);
	public delegate void DUpdateCard(RegicideCard card);
	public delegate void Notify();

	public event Notify NoDiscardPossible;
	public event Notify DrawPileEmpty;
	public event DUpdateCardHand UpdateCardHand;
	public event DUpdateCard AddCardToHand;
	public event DUpdateCard RemoveCardFromHand;
	public IEnumerable<RegicideCard> Cards
	{
		get
		{
			return cardsInHand;
		}
	}

	public readonly int MaxSize = 8;

	private Pile DrawPile;
	private Pile DiscardPile;
	private List<RegicideCard> cardsInHand = new List<RegicideCard>();
	private int DiscardDamage = 0;
	public enum Mode
	{
		PLAY,
		DISCARD
	}
	public Mode CurrentMode;

	public Hand(Pile drawPile, Pile discardPile)
	{
		DrawPile = drawPile;
		DiscardPile = discardPile;
		CurrentMode = Mode.PLAY;
	}

	public void Draw(int cardAmt)
	{
		int currentAmt = cardsInHand.Count;
		for (var i = 0; i < Math.Min(cardAmt, MaxSize - currentAmt); i++)
		{
			RegicideCard c = DrawPile.DrawFromTop();
			if (c is null)
			{
				DrawPileEmpty?.Invoke();
				return;
			}

			cardsInHand.Add(c);
			AddCardToHand?.Invoke(c);
		}
	}

	public void Discard(int damage)
	{
		if (cardsInHand.Count == 0)
		{
			NoDiscardPossible?.Invoke();
			return;
		}
		CurrentMode = Mode.DISCARD;
		DiscardDamage = damage;
	}

	public void SelectCard(RegicideCard card)
	{
		switch(CurrentMode)
		{
			case Mode.PLAY:
				DiscardPile.PutOnTop(card);
				cardsInHand.Remove(card);
				card.Play();
				break;
			case Mode.DISCARD:
				DiscardPile.PutOnTop(card);
				cardsInHand.Remove(card);
				DiscardDamage -= card.Power;
				if (DiscardDamage < 1)
				{
					DiscardDamage = 0;
					CurrentMode = Mode.PLAY;
				}
				break;
		}
		RemoveCardFromHand?.Invoke(card);
	}
}
