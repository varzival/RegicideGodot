using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class GHand : Control, GodotView<Hand>
{
    private Hand Hand;
    private GPile ReferencePile;

    private readonly float DistanceOfCardsInHand = 60;

    public void Connect(Hand obj)
    {
        Hand = obj;
        Hand.AddCardToHand += AddCardToHand;
        Hand.RemoveCardFromHand += RemoveCardFromHand;
        UpdateCardHand();
    }
    
    public void SetReferencePile(GPile pile)
    {
        ReferencePile = pile;
    }

    public void UpdateCardHand()
    {
        if (Hand == null)
            return;
        foreach (var card in GetCurrentCards())
        {
            card.QueueFree();
        }
        foreach(var card in Hand.Cards)
        {
            AddCardToHand(card);
        }
    }

    public void AddCardToHand(RegicideCard card)
    {
        var current_card_count = GetCurrentCards().Count();
        GCard new_gcard = new GCard();
        AddChild(new_gcard);
        new_gcard.Connect(card);
        new_gcard.AddToGroup("card_in_hand");
        new_gcard.SetTexture(card);
        if (ReferencePile != null)
            new_gcard.RectScale = ReferencePile.RectScale;
        new_gcard.RectPosition = new Vector2(DistanceOfCardsInHand * current_card_count, 0);
        new_gcard.CardSelected += Hand.SelectCard;
    }

    public void RemoveCardFromHand(Card card)
    {
        var same_cards = GetCurrentCards().Where((c) => c.Card == card);
        foreach (GCard c in same_cards)
        {
            c.QueueFree();
        }
        ResetCardPosition();
    }

    private IEnumerable<GCard> GetCurrentCards()
    {
        return GetTree().GetNodesInGroup("card_in_hand").Cast<GCard>().Where(x => !x.IsQueuedForDeletion());
    }


    public void ResetCardPosition()
    {
        var currentCards = GetCurrentCards();
        for (int i = 0; i < currentCards.Count(); i++)
        {
            (currentCards.ElementAt(i)).RectPosition
                = new Vector2(DistanceOfCardsInHand * i, 0);
        }
    }
}
