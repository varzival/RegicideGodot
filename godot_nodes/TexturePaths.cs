using System;

public class TexturePaths
{
    public static readonly String BASE = "res://textures/";
    public static readonly String BACK = BASE + "BackFace/CardBackFaceBlueLargePattern.png";
    public static readonly String ERR = BASE + "BackFace/CardBackFaceWhiteBlueLargePattern.png";

    public static String CardTexture(RegicideCard.Suit suit, int power, Boolean enemy = false)
    {
        String s = BASE;
        switch(suit)
        {
            case RegicideCard.Suit.CLUBS:
                s += "Clubs/";
                break;
            case RegicideCard.Suit.SPADES:
                s += "Spades/";
                break;
            case RegicideCard.Suit.HEARTS:
                s += "Hearts/";
                break;
            case RegicideCard.Suit.DIAMONDS:
                s += "Diamonds/";
                break;
        }

        if (power == 0)
        {
            s += "Joker.png";
        }
        else if (power == 1)
        {
            s += "Ace.png";
        }
        else if (2 <= power && power <= 10 && !enemy)
        {
            s += power + ".png";
        }
        else if (power == 10 && enemy)
        {
            s += "Jack.png";
        }
        else if (power == 15)
        {
            s += "Queen.png";
        }
        else if (power == 20 && enemy)
        {
            s += "King.png";
        }
        else
        {
            Console.WriteLine("Could not find Texture for given power: " + power);
            s = ERR;
        }

        return s;
    }
}
