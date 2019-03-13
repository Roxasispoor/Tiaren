using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagVictoryEffect : EffectOnLiving {
    public override Effect Clone()
    {
        return new FlagVictoryEffect(this);
    }
    public FlagVictoryEffect():base()
    { }
    public override void Use()
    {
       if(GameManager.instance.gameMode==GameMode.FLAG &&  Target.Player!=Launcher.Player &&
            Target.transform.Find("Inventory") && Target.transform.Find("Inventory").GetComponentInChildren<Flag>()!=null)
        {
            GameManager.instance.winner = Target.Player;
            Target.Player.isWinner = true;
            GameManager.instance.CheckWinCondition();
        }
    }
    public FlagVictoryEffect(FlagVictoryEffect other) : base(other)
    {
    }


    public override void preview()
    {
        //throw new System.NotImplementedException();
        Debug.Log("No preview for FlagVictoryEffect");
    }
}
