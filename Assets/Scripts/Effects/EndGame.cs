using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Effect of end game
/// </summary>
public class EndGame : Effect
{
    private GameManager game;
    private Player player;
    public EndGame(GameManager game, Player player)
    {
        this.game = game;
        this.player = player;
    }
    override
        public void Use()
    {
        this.game.Winner = player;
    }

}
