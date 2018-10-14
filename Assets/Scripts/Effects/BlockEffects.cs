using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Les blocs sont des effets tout ce qu'il y a de plus normal. Quand ils sont utilisés ils sont ajoutés dans la liste des effets bloqués actifs du GameEffectsManager
/// <summary>
/// Classe pour effet permettant de contrer d'autres effets.
/// </summary>
public class BlockEffects : Effect
{
    private List<Effect> listEffectsToBlock;
    /// <summary>
    /// -1 = non dépendant
    /// </summary>
    private int numberToBlock;


    public BlockEffects(Placeable cible, Placeable lanceur) : base(cible, lanceur)
    {
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
        foreach (Effect a in listEffectsToBlock)
        {
            if (effet.GetType() == a.GetType())
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
        
    }


}
