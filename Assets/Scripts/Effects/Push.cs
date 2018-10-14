using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Effet permettant de repousser un bloc
/// </summary>
public class Push : Effect
{
    private float damage;
    private int nbCases;
    private GameManager gameManager;
    /// <summary>
    /// Un super push détruit le bloc et augmente les dégats
    /// </summary>
    private bool isSuper;
    /// <summary>
    /// Crée un objet de dégats
    /// </summary>
    /// <param name="cible">Indique la cible du coup</param>
    /// <param name="lanceur">Indique le lanceur du coup</param>
    public Push(Placeable cible, Placeable lanceur, int nbCases, bool isSuper) : base(cible, lanceur)
    {
        this.nbCases = nbCases;
        this.isSuper = isSuper;
    }

    override
    public void Use() { }

    //TODO: a revoir, vieux et mal fait
    /*
    override
        public void Use()
    {
        if (Cible != Lanceur)
        {
            Vector3Int vectDiff = Cible.Position - Lanceur.Position;
            if (vectDiff.magnitude == 1)
            {
                for (int i = 0; i < nbCases; i++)
                {
                    //Si il y a de la place derrière la cible on la déplace
                    Vector3Int posConsidered = Cible.Position + vectDiff;
                    if (gameManager.GrilleJeu.Grid[posConsidered.x, posConsidered.y, posConsidered.z] == null)
                    {
                        gameManager.GrilleJeu.DeplaceBloc(Cible, posConsidered);

                    }
                    //si derrière il y avait un placeable il prend des dégats
                    else if (gameManager.GrilleJeu.Grid[posConsidered.x, posConsidered.y, posConsidered.z].GetType() == typeof(LivingPlaceable))
                    {
                        if (isSuper)
                        {
                            if (Cible.GetType() == typeof(LivingPlaceable))
                            {
                                gameManager.GameEffectManager.ToBeTreated.Add(new Damage(Cible, Lanceur, nbCases * 35));
                            }
                            gameManager.GameEffectManager.ToBeTreated.Add(new Damage(gameManager.GrilleJeu.Grid[posConsidered.x, posConsidered.y, posConsidered.z], Cible, nbCases * 35));
                            break;
                        }
                        else
                        {
                            if (Cible.GetType() == typeof(LivingPlaceable))
                            {
                                gameManager.GameEffectManager.ToBeTreated.Add(new Damage(Cible, Lanceur, nbCases * 20));
                            }
                            gameManager.GameEffectManager.ToBeTreated.Add(new Damage(gameManager.GrilleJeu.Grid[posConsidered.x, posConsidered.y, posConsidered.z], Cible, nbCases * 20));
                            break;
                        }
                    }
                    else if (gameManager.GrilleJeu.Grid[posConsidered.x, posConsidered.y, posConsidered.z].GetType() == typeof(Placeable))
                    {

                        if (isSuper)
                        {
                            //on deplace le placeable
                            gameManager.GrilleJeu.DeplaceBloc(Cible, posConsidered);
                            if (Cible.GetType() == typeof(LivingPlaceable))
                            {
                                gameManager.GameEffectManager.ToBeTreated.Add(new Damage(Cible, Lanceur, nbCases * 35));
                            }
                        }
                        else //on applique des dommages réduit si il y a lieu
                        {
                            if (Cible.GetType() == typeof(LivingPlaceable))
                            {
                                gameManager.GameEffectManager.ToBeTreated.Add(new Damage(Cible, Lanceur, nbCases * 20));
                            }

                            break;
                        }
                    }

                }




            }
        }
    }
     */

}
