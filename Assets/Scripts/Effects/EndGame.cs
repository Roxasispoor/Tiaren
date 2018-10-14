using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Effet de fin du jeu
/// </summary>
public class EndGame : Effect
{
    private GameManager game;
    private Player joueur;
    public EndGame(GameManager game, Player joueur)
    {
        this.game = game;
        this.joueur = joueur;
    }
    override
        public void Use()
    {
        this.game.Winner = joueur;
    }

}
