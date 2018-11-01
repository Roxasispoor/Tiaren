using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Effect of end game
/// </summary>
public class EndGame : Effect
{
   
    private Player player;

    public EndGame(Effect other) : base(other)
    {
    }

    public EndGame( Player player)
    {

        this.player = player;
    }

    public override Effect Clone()
    {
        return new EndGame(this);
    }

    override
        public void Use()
    {
    //    this.game.Winner = player;
    }

}
