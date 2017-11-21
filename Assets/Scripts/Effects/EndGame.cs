using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : Effect {

    private Game game;
    private Joueur joueur;
    public EndGame(Game game, Joueur joueur)
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
