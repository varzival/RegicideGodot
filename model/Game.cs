using Godot;
using System;
using System.Collections.Generic;

public class Game
{
    public delegate void DEnemyReplaced(Enemy enemy);
    public event DEnemyReplaced EnemyReplaced;

    public Enemy Enemy;
    public Pile EnemyPile;
    public Pile DrawPile;
    public Pile DiscardPile;
    public Hand Hand;

    public void Start()
    {
        DiscardPile = new Pile();
        Enemy = new Enemy();
        DrawPile = new Pile();
        Hand = new Hand(DrawPile, DiscardPile);
        var draw_cards = ConstructDrawPile();
        DrawPile.SetCards(draw_cards);
        Hand.Draw(Hand.MaxSize);

        EnemyPile = new Pile();
        var enemy_cards = ConstructEnemyPile();
        EnemyPile.SetCardsByParts(enemy_cards);

        ReplaceEnemy();
        Enemy.Defeat += ReplaceEnemy;
    }

    public void ReplaceEnemy()
    {
        Enemy new_enemy = (Enemy)EnemyPile.DrawFromTop();
        Enemy.Power = new_enemy.Power;
        Enemy.Health = new_enemy.Health;
        Enemy.CardSuit = new_enemy.CardSuit;
        Enemy.Nerfed = false;
        EnemyReplaced?.Invoke(Enemy);
    }


    public List<RegicideCard> ConstructDrawPile()
    {
        List<RegicideCard> cards = new List<RegicideCard>();
        for (int p = 1; p < 11; p++)
        {
            foreach (var suit in (RegicideCard.Suit[])Enum.GetValues(typeof(RegicideCard.Suit)))
            {
                cards.Add(RegicideCard.CreateCard(suit, p, Enemy, DrawPile, DiscardPile, Hand));
            }
        }
        return cards;
    }

    public List<RegicideCard>[] ConstructEnemyPile()
    {
        var enemy_values = new (int, int)[] { (10, 20), (15, 30), (20, 40) };
        var cards_parts = new List<RegicideCard>[3];
        int i = 0;
        foreach (var t in enemy_values)
        {
            var list = new List<RegicideCard>();
            foreach (var suit in (RegicideCard.Suit[])Enum.GetValues(typeof(RegicideCard.Suit)))
            {
                list.Add(new Enemy(t.Item2, t.Item1, suit));
            }
            cards_parts[i] = list;
            i++;
        }
        return cards_parts;
    }
}
