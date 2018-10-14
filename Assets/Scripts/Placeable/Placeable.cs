using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

/// <summary>
/// Représente a peut pres n'importe quoi pouvant occuper un bloc dans le grille
/// </summary>

public abstract class Placeable : NetworkBehaviour
{
   // public GameManager gameManager;

    public int serializeNumber;
    //public Vector3Int position;
    private bool walkable;
    private List<Effect> onWalkEffects;
    private bool movable;
    private bool destroyable;
    private TraversableType tangible;
    private TraversableType traversableBullet;
   
    private GravityType gravityType;
//    private bool pickable;
    private EcraseType ecrasable;
    public bool explored;
    private List<Effect> onDestroyEffects;
    private List<HitablePoint> hitablePoints;
    private List<Effect> onDebutTour;
    private List<Effect> onFinTour;
    /// <summary>
    /// Le joueur a qui appartient le placeable. Les joueurs, équipe neutre(monstres neutres) et null(blocs indépendants)
    /// </summary>
    public Player player;

    public Vector3Int GetPosition()
    {
        throw new NotImplementedException();

    }

    public bool Walkable
    {
        get
        {
            return walkable;
        }

        set
        {
            walkable = value;
        }
    }

    public bool Movable
    {
        get
        {
            return movable;
        }

        set
        {
            movable = value;
        }
    }

   

    /// <summary>
    /// Indique si on a déja fait nos test de gravité sur ce placeable
    /// </summary>
    public bool Explored
    {
        get
        {
            return explored;
        }

        set
        {
            explored = value;
        }
    }




    public List<Effect> OnDebutTour
    {
        get
        {
            return onDebutTour;
        }

        set
        {
            onDebutTour = value;
        }
    }

    public List<Effect> OnFinTour
    {
        get
        {
            return onFinTour;
        }

        set
        {
            onFinTour = value;
        }
    }

    public List<Effect> OnDestroyEffects
    {
        get
        {
            return onDestroyEffects;
        }

        set
        {
            onDestroyEffects = value;
        }
    }

    public List<Effect> OnWalkEffects
    {
        get
        {
            return onWalkEffects;
        }

        set
        {
            onWalkEffects = value;
        }
    }

    public List<HitablePoint> HitablePoints
    {
        get
        {
            return hitablePoints;
        }

        set
        {
            hitablePoints = value;
        }
    }

    public TraversableType TraversableChar
    {
        get
        {
            return tangible;
        }

        set
        {
            tangible = value;
        }
    }

    public TraversableType TraversableBullet
    {
        get
        {
            return traversableBullet;
        }

        set
        {
            traversableBullet = value;
        }
    }

    public GravityType GravityType
    {
        get
        {
            return gravityType;
        }

        set
        {
            gravityType = value;
        }
    }

    public EcraseType Ecrasable
    {
        get
        {
            return ecrasable;
        }

        set
        {
            ecrasable = value;
        }
    }

    public Player Player
    {
        get
        {
            return player;
        }

        set
        {
            player = value;
        }
    }

   

    public bool Destroyable
    {
        get
        {
            return destroyable;
        }

        set
        {
            destroyable = value;
        }
    }

  


    /// <summary>
    /// Copie l'objet
    /// </summary>
    /// <returns>Retourne une copie de l'objet</returns>
    public virtual Placeable Cloner()
    {
        var copy = (Placeable)this.MemberwiseClone();
        return copy;
    }

    /// <summary>
    /// Méthode a appeler lors de la destruction de l'objet
    /// </summary>
    public virtual void Detruire()
    {
        if (this.Destroyable)
        {
            foreach (var effet in this.OnDestroyEffects)
            {
                effet.Use();
            }
        }
        Destroy(this);
        Destroy(this.gameObject);
    }
    /// <summary>
    /// permet le shoot et le déplacement
    /// </summary>
    void OnMouseOver()
    {


        if (Input.GetMouseButtonUp(0) && this.walkable)
        {
            Debug.Log("Hello there");

            //Warning: works because only local player is joueur
            ClientScene.localPlayers[0].gameObject.GetComponent<Player>().CmdMoveTo(this.netId);

            //
            //            gameManager.PlayerMove();

            //GameManager.PlaceToGo = this.Position ;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            GameManager.instance.ShotPlaceable = this;

        }

    }
}
