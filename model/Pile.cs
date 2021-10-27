using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Pile : TextureRect
{
    private List<RegicideCard> Cards = new List<RegicideCard>();
    public delegate void DUpdatePileTexture(RegicideCard card);
    public event DUpdatePileTexture UpdatePileTexture;

    public void PublishUpdateTexture()
    {
        if (Cards.Count == 0)
        {
            UpdatePileTexture?.Invoke(null);
            return;
        }
        UpdatePileTexture?.Invoke(Cards.First());
    }

    public void Shuffle()
    {
        var rand = new Random();
        Cards = this.Cards.OrderBy(x => rand.Next()).ToList();
        PublishUpdateTexture();
    }

    public void PutOnTop(RegicideCard card)
    {
        Cards.Insert(0, card);
        PublishUpdateTexture();
    }

    public RegicideCard DrawFromTop()
    {
        if (Cards.Count == 0)
        {
            return null;
        }
        RegicideCard card = Cards.First();
        Cards.RemoveAt(0);
        PublishUpdateTexture();
        return card;
    }

    public void PutToBottom(RegicideCard card)
    {
        Cards.Add(card);
        PublishUpdateTexture();
    }

    public void SetCards(List<RegicideCard> cards)
    {
        Cards = cards;
        Shuffle();
        PublishUpdateTexture();
    }

    public void SetCardsByParts(List<RegicideCard>[] cards_arrays)
    {
        var rand = new Random();
        Cards = this.Cards.OrderBy(x => rand.Next()).ToList();
        Cards = new List<RegicideCard>();
        foreach (var list in cards_arrays)
        {
            var shuffeled_list = list.OrderBy(x => rand.Next()).ToList();
            Cards = Cards.Concat(shuffeled_list).ToList();
        }
        PublishUpdateTexture();
    }

    public RegicideCard TopCard()
    {
        if (Cards.Count > 0)
            return Cards.First();
        return null;
    }
}
