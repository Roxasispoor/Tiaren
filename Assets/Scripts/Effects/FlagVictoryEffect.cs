using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagVictoryEffect : EffectOnLiving {
    public override Effect Clone()
    {
        return new FlagVictoryEffect(this);
    }
    public FlagVictoryEffect(Placeable launcher):base()
    {
        Launcher = launcher;
    }
    public override void Use()
    {
       if(GameManager.instance.gameMode==GameMode.FLAG &&  Target.Player!=((Placeable)Launcher).Player &&
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


    public override void Preview(NetIdeable target)
    {
        //throw new System.NotImplementedException();
        Debug.Log("No preview for FlagVictoryEffect");
    }

    public override void ResetPreview(NetIdeable target)
    {
        throw new System.NotImplementedException();
    }
}
