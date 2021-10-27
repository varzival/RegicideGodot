using System;

public class Card
{
    protected Effect Effect;
    public delegate void DCardSelected(Card card);
    public event DCardSelected CardSelected;

    public Card()
    {
        Effect = new Effect.Empty();
    }

    public Card(Effect effect)
    {
        Effect = effect;
    }

    public Effect getEffect()
    {
        return Effect;
    }

    public void setEffect(Effect effect)
    {
        Effect = effect;
    }

    public void Play()
    {
        Effect.Execute();
    }
}

public abstract class Effect
{
    public class EffectSequence : Effect
    {
        private Effect e1;
        private Effect e2;

        public EffectSequence(Effect e1, Effect e2)
        {
            this.e1 = e1;
            this.e2 = e2;
        }

        public override void Execute()
        {

            this.e1.Execute();
            this.e2.Execute();
        }
    }

    public class Empty : Effect
    {
        public override void Execute()
        { }
    }

    public abstract void Execute();
}