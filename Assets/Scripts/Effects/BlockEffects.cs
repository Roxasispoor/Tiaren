using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Les blocs sont des effets tout ce qu'il y a de plus normal. Quand ils sont utilisés ils sont ajoutés dans la liste des effets bloqués actifs du GameEffectsManager
public class BlockEffects : Effect {
    private List<Effect> listEffectsToBlock;
    
    private int numberToBlock;// -1 = not dependant
    private GameEffectManager gameEffectManager;

    public BlockEffects(Placeable cible, Placeable lanceur, GameEffectManager gameEffectManager): base(cible,lanceur)
    {
        this.gameEffectManager = gameEffectManager;
    }

    public List<Effect> ListEffectsToBlock
    {
        get
        {
            return listEffectsToBlock;
        }

        set
        {
            listEffectsToBlock = value;
        }
    }
   
    public bool ContientEffetType(Effect effet) // vérifie que la liste des effets contient une instance du type de l'objet
    {
        foreach(Effect a in listEffectsToBlock)
        {
            if(effet.GetType() == a.GetType())
            {
                return true;
            }
        }
        return false;
    }


    public int NumberToBlock
    {
        get
        {
            return numberToBlock;
        }

        set
        {
            numberToBlock = value;
        }
    }

    override
    public void Use()
    {
        gameEffectManager.ActiveBlocks.Add(this);
    }


}
