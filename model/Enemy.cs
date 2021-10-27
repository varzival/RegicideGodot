using Godot;
using System;

public class Enemy : RegicideCard
{
    public delegate void SetInt(int integer);
    public delegate void SetBool(bool boolean);
    public delegate void Notify();

    public event Notify Defeat;
    public event SetInt TriggerDiscard;
    public event SetInt SetAttackPower;
    public event SetInt SetHealth;
    public event SetBool SetNerfed;

    private int attackPower;
    public new int Power {
        get
        {
            return attackPower;
        }
        set
        {
            attackPower = value;
            if (attackPower < 0) attackPower = 0;
            SetAttackPower?.Invoke(attackPower);
        }
    }

    private int health;
    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
            if (health < 0) health = 0;
            SetHealth?.Invoke(health);
        }
    }

    private bool nerfed = false;
    public bool Nerfed
    {
        get
        {
            return nerfed;
        }
        set
        {
            nerfed = value;
            SetNerfed?.Invoke(nerfed);
        }
    }


    public Enemy()
    {
        Health = 0;
        Power = 0;
    }

    public Enemy(int health, int attackPower, RegicideCard.Suit suit)
    {
        Health = health;
        CardSuit = suit;
        Power = attackPower;
    }

    public void Damage(int dmg)
    {
        this.Health -= dmg;

        if (this.Health < 1)
        {
            Defeat?.Invoke();
        }
    }

    public void Attack()
    {
        TriggerDiscard?.Invoke(Power);
    }
}