using System;
using Godot;

public class GGame: Node
{
	private Game game;
	private GCard EnemyCard;
	private Label NerfedLabel;
	private GPile DiscardPile;
	private GPile DrawPile;
	private GHand GHand;
	private Label gameStatus;
	private CanvasItem defeatOverlay;

	private int num_jokers;

	public override void _Ready()
	{
		game = new Game();
		game.Start();

		// Events
		game.EnemyReplaced += ReconnectEnemy;

		DrawPile = GetNode<GPile>("CanvasLayer/Screen/DrawPile");
		DrawPile.Connect(game.DrawPile);
		DiscardPile = GetNode<GPile>("CanvasLayer/Screen/DiscardPile");
		DiscardPile.Connect(game.DiscardPile);
		EnemyCard = GetNode<GCard>("CanvasLayer/Screen/Enemy");
		EnemyCard.Connect(game.Enemy);
		NerfedLabel = GetNode<Label>("CanvasLayer/Screen/NERFED");
		NerfedLabel.Visible = false;
		game.Enemy.SetNerfed += (nerfed_bool) => NerfedLabel.Visible = nerfed_bool;

		GHand = GetNode<GHand>("CanvasLayer/Screen/Hand");
		GHand.SetReferencePile(DrawPile);
		GHand.Connect(game.Hand);
		game.Enemy.TriggerDiscard += game.Hand.Discard;
		game.Hand.HandEmpty += Defeat; // DefeatIfNoJokers

		num_jokers = 2;
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

		gameStatus = GetNode<StatViewer>("CanvasLayer/Screen/GameStatus");
		game.Hand.UpdateMode += UpdateStatus;

		game.Hand.CardPlayed += EnemyTurn;

		defeatOverlay = GetNode<CanvasItem>("CanvasLayer/Screen/OverlayDefeat");
	}

	public void DefeatIfNoJokers()
	{
		if (num_jokers <= 0)
			Defeat();
	}

	public void Defeat()
	{
		defeatOverlay.Visible = true;
	}

	public void UpdateStatus(Hand.Mode mode)
	{
		switch(mode)
		{
			case Hand.Mode.PLAY:
				gameStatus.Text = "Play";
				break;
			case Hand.Mode.DISCARD:
				gameStatus.Text = "Discard (Damage " + game.Hand.DiscardDamage + ")";
				break;
		}

	}

	public void EnemyTurn()
	{
		if (game.Hand.CurrentSize == 0 && num_jokers <= 0)
		{
			Defeat();
			return;
		}
		if (game.Enemy.Health <= 0)
		{
			game.ReplaceEnemy();
			return;
		}
		game.Enemy.Attack();
	}

	public void ReconnectEnemy(Enemy enemy)
	{
		EnemyCard.Connect(enemy);
	}

	public void PlayJoker(GCard jokerCard, RegicideCard joker)
	{
		joker.Play();
		jokerCard.QueueFree();
		num_jokers -= 1;
	}

	public void UpdateNerfedLabel(bool nerfed)
	{
		NerfedLabel.Visible = nerfed;
	}
}
