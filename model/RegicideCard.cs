using Godot;
using System;
using System.Diagnostics;

public class RegicideCard : Card
{
    public delegate void SetSuit(Suit suit);

    public event SetSuit SetCardSuit;


    public RegicideCard()
    {
        Effect = new Effect.Empty();
        Power = 0;
    }

    private RegicideCard(Effect effect, Suit suit, int power) : base(effect)
    {
        CardSuit = suit;
        Power = power;
    }

    public enum Suit
    {
        CLUBS,
        SPADES,
        HEARTS,
        DIAMONDS,
    }
    public int Power;

    private Suit card_suit;
    public Suit CardSuit
    {
        get
        {
            return card_suit;
        }
        set
        {
            card_suit = value;
            SetCardSuit?.Invoke(card_suit);
        }
    }

    abstract class CounterableEffect: Effect
    {
        public Suit Suit;
        public Enemy Enemy;
        public int Power;

        protected CounterableEffect(Suit suit, Enemy enemy, int power)
        {
            Suit = suit;
            Enemy = enemy;
            Power = power;
        }

        public abstract void ExecuteIfNotCountered();

        public override void Execute()
        {
            if (this.Suit != Enemy.CardSuit && !Enemy.Nerfed)
            {
                ExecuteIfNotCountered();
            }
            else
            {
                Console.WriteLine("Effect has been countered");
            }
        }
    }

    class Damage: Effect
    {
        protected Enemy enemy;
        protected int power; 

        public Damage(Enemy enemy, int power)
        {
            this.enemy = enemy;
            this.power = power;
        }

        public override void Execute()
        {
            this.enemy.Damage(this.power);
        }
    }

    class DoubleDamage: Damage
    {
        public Suit Suit;

        public DoubleDamage(Enemy enemy, int power, Suit suit) : base(enemy, power)
        {
            Suit = suit;
        }

        public override void Execute()
        {
            if (this.enemy.CardSuit == this.Suit)
                this.enemy.Damage(this.power);
            else
                this.enemy.Damage(2 * this.power);
        }
    }

    class NerfEnemy: Effect
    {
        public Enemy Enemy;

        public NerfEnemy(Enemy enemy)
        {
            Enemy = enemy;
        }

        public override void Execute()
        {
            this.Enemy.Nerfed = true;
        }
    }

    class WeakenEnemy: CounterableEffect
    {
        public WeakenEnemy(Suit suit, Enemy enemy, int power) : base(suit, enemy, power)
        {
        }

        public override void ExecuteIfNotCountered()
        {
            this.Enemy.Power -= this.Power;
        }
    }

    class HealPile: CounterableEffect
    {
        public Pile DrawPile;
        public Pile DiscardPile;

        public HealPile(Suit suit, Enemy enemy, int power, Pile drawPile, Pile discardPile) : base(suit, enemy, power)
        {
            DrawPile = drawPile;
            DiscardPile = discardPile;
        }

        public override void ExecuteIfNotCountered()
        {
            DiscardPile.Shuffle();
            for (var i = 0; i < Power; i++)
            {
                RegicideCard c = DiscardPile.DrawFromTop();
                if (c != null)
                {
                    DrawPile.PutToBottom(c);
                }
                else
                {
                    break;
                }
            }
        }
    }

    class DrawCards: CounterableEffect
    {
        public Hand Hand;

        public DrawCards(Suit suit, Enemy enemy, int power, Hand hand) : base(suit, enemy, power)
        {
            Hand = hand;
        }

        public override void ExecuteIfNotCountered()
        {
            Hand.Draw(Power);
        }
    }

    class DrawFull : Effect
    {
        public Hand Hand;

        public DrawFull(Hand hand)
        {
            Hand = hand;
        }

        public override void Execute()
        {
            Hand.DiscardAll();
            Hand.Draw(Hand.MaxSize);
        }
    }

    public static RegicideCard CreateCard(Suit suit, int power, Enemy enemy, Pile drawPile, Pile discardPile, Hand hand)
    {
        Debug.Assert(0 <= power, "Power must be positive");

        Effect fullEffect;

        /*
        if (power == 0)
        {
            fullEffect = new NerfEnemy(enemy);
        }
        */

        if (power == 0)
        {
            fullEffect = new DrawFull(hand);
        }
        else {
            switch(suit)
            {
                case Suit.CLUBS:
                    fullEffect = new DoubleDamage(enemy, power, Suit.CLUBS);
                break;
                case Suit.SPADES:
                    fullEffect = new Effect.EffectSequence(
                            new WeakenEnemy(Suit.SPADES, enemy, power),
                            new Damage(enemy, power)
                        );
                break;
                case Suit.HEARTS:
                    fullEffect = new Effect.EffectSequence(
                            new HealPile(Suit.HEARTS, enemy, power, drawPile, discardPile),
                            new Damage(enemy, power)
                        );
                break;
                case Suit.DIAMONDS:
                    fullEffect = new Effect.EffectSequence(
                            new DrawCards(Suit.DIAMONDS, enemy, power, hand),
                            new Damage(enemy, power)
                        );
                break;
                default:
                    fullEffect = new Effect.Empty();
                break;
            }
        }

        RegicideCard card = new RegicideCard(fullEffect, suit, power);
        return card;
    }
}
