using System;
using Godot;

public class GGame: Node
{
    private Game game;
    private GCard EnemyCard;
    private Label NerfedLabel;

    public override void _Ready()
    {
        game = new Game();
        game.Start();

        // Events
        game.EnemyReplaced += ReconnectEnemy;

        GPile DrawPile = GetNode<GPile>("CanvasLayer/Screen/DrawPile");
        DrawPile.Connect(game.DrawPile);
        GPile DiscardPile = GetNode<GPile>("CanvasLayer/Screen/DiscardPile");
        DiscardPile.Connect(game.DiscardPile);
        EnemyCard = GetNode<GCard>("CanvasLayer/Screen/Enemy");
        EnemyCard.Connect(game.Enemy);
        NerfedLabel = GetNode<Label>("CanvasLayer/Screen/NERFED");
        game.Enemy.SetNerfed += (nerfed_bool) => NerfedLabel.Visible = nerfed_bool;

        GHand Hand = GetNode<GHand>("CanvasLayer/Screen/Hand");
        Hand.SetReferencePile(DrawPile);
        Hand.Connect(game.Hand);

        GCard GJoker1 = GetNode<GCard>("CanvasLayer/Screen/Joker1");
        GCard GJoker2 = GetNode<GCard>("CanvasLayer/Screen/Joker2");
        RegicideCard Joker1 = RegicideCard.CreateCard(RegicideCard.Suit.CLUBS, 0, game.Enemy, game.DrawPile, game.DiscardPile, game.Hand);
        RegicideCard Joker2 = RegicideCard.CreateCard(RegicideCard.Suit.CLUBS, 0, game.Enemy, game.DrawPile, game.DiscardPile, game.Hand);
        GJoker1.Connect(Joker1);
        GJoker2.Connect(Joker2);
        GJoker1.CardSelected += (card) => PlayJoker(GJoker1, card);
        GJoker2.CardSelected += (card) => PlayJoker(GJoker2, card);

        StatViewer healthViewer = GetNode<StatViewer>("CanvasLayer/Screen/Enemy/Health");
        healthViewer.SetLabel(game.Enemy.Health);
        game.Enemy.SetHealth += healthViewer.SetLabel;
        StatViewer attackViewer = GetNode<StatViewer>("CanvasLayer/Screen/Enemy/Attack");
        attackViewer.SetLabel(game.Enemy.Power);
        game.Enemy.SetAttackPower += attackViewer.SetLabel;
    }

    public void ReconnectEnemy(Enemy enemy)
    {
        EnemyCard.Connect(enemy);
    }

    public void PlayJoker(GCard jokerCard, RegicideCard joker)
    {
        joker.Play();
        jokerCard.QueueFree();
    }

    public void UpdateNerfedLabel(bool nerfed)
    {
        NerfedLabel.Visible = nerfed;
    }
}