using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe représentant les dégats
/// </summary>
public class Damage : Effect {
    private float damage;
    /// <summary>
    /// Crée un objet de dégats
    /// </summary>
    /// <param name="cible">Indique la cible du coup</param>
    /// <param name="lanceur">Indique le lanceur du coup</param>
    /// <param name="damage">Quantité de dégats pris</param>
    public Damage(Placeable cible, Placeable lanceur,float damage): base(cible,lanceur)
    {
        this.Damage1 = damage;  
    }


    public float Damage1
    {
        get
        {
            return damage;
        }

        set
        {
            damage = value;
        }
    }

    override
        public void Use()
    {
       if(this.Cible is LivingPlaceable)
        {
            LivingPlaceable Cible = (LivingPlaceable)(this.Cible);
            Cible.PvActuels -= Damage1;
            if(Cible.PvActuels<=0)
            {
                Cible.Destroy();
            }

           
        }
    }
}
