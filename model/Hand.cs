using System;
using System.Collections.Generic;


public class Hand
{
	public delegate void DUpdateCardHand(List<Card> cards);
	public delegate void DUpdateCard(RegicideCard card);
	public delegate void DUpdateMode(Mode mode);
	public delegate void Notify();

	public event Notify HandEmpty;
	public event Notify DrawPileEmpty;
	public event Notify CardPlayed;
	public event Notify DiscardFinished;

	public event DUpdateCardHand UpdateCardHand;
	public event DUpdateCard AddCardToHand;
	public event DUpdateCard RemoveCardFromHand;
	public event DUpdateMode UpdateMode;
	public IEnumerable<RegicideCard> Cards
	{
		get
		{
			return cardsInHand;
		}
	}

	public int DiscardDamage = 0;

	public readonly int MaxSize = 8;

	public int CurrentSize
	{
		get
		{
			return cardsInHand.Count;
		}
	}

	private Pile DrawPile;
	private Pile DiscardPile;
	private List<RegicideCard> cardsInHand = new List<RegicideCard>();
	
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
		UpdateMode?.Invoke(CurrentMode);
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
		if (damage < 0)
		{
			return;
		}
		if (cardsInHand.Count == 0)
		{
			HandEmpty?.Invoke();
			return;
		}
		CurrentMode = Mode.DISCARD;
		DiscardDamage = damage;
		UpdateMode?.Invoke(CurrentMode);
	}

	public void DiscardAll()
	{
		foreach (var card in cardsInHand)
		{
			DiscardPile.PutOnTop(card);
			RemoveCardFromHand?.Invoke(card);
		}
		cardsInHand.RemoveRange(0, cardsInHand.Count);
	}

	public void SelectCard(RegicideCard card)
	{
		switch(CurrentMode)
		{
			case Mode.PLAY:
				DiscardPile.PutOnTop(card);
				cardsInHand.Remove(card);
				card.Play();
				CardPlayed?.Invoke();
				break;
			case Mode.DISCARD:
				DiscardPile.PutOnTop(card);
				cardsInHand.Remove(card);
				DiscardDamage -= card.Power;
				UpdateMode?.Invoke(CurrentMode);
				if (DiscardDamage < 1)
				{
					DiscardDamage = 0;
					CurrentMode = Mode.PLAY;
					UpdateMode?.Invoke(CurrentMode);
				}
				
				if (cardsInHand.Count == 0)
				{
					HandEmpty?.Invoke();
				}
				break;
		}
		RemoveCardFromHand?.Invoke(card);
	}
}
