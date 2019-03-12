﻿using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents something able to fill a bloc of the grid
/// </summary>
[Serializable]
public abstract class Placeable : NetIdeable
{
    /// <summary>
    /// Size of the child quads.
    /// </summary>
    private const float sizeQuad = 1.02f;
    /// <summary>
    /// Can we walk on it.
    /// </summary>
    private bool walkable;
    /// <summary>
    /// Is the placeable movable (for push, etc.).
    /// </summary>
    protected bool movable;
    /// <summary>
    /// Is the placeable traversable by another Placeable.
    /// </summary>
    protected TraversableType tangible;
    /// <summary>
    /// Is the placeable traversable by bullets.
    /// </summary>
    protected TraversableType traversableBullet;
    /// <summary>
    /// The speed used for the animation.
    /// </summary>
    private float animationSpeed = 2.5f;
    /// <summary>
    /// Previous material (used for highlighting)
    /// </summary>
    // AMELIORATION: move to cube ?
    [NonSerialized]
    public Material oldMaterial;
    /// <summary>
    /// Base material.
    /// </summary>
    // AMELIORATION: move to cube ?
    public Material baseMaterial;
    /// <summary>
    /// How it react to gravity.
    /// </summary>
    protected GravityType gravityType;
    /// <summary>
    /// How it react when crushed.
    /// </summary>
    protected CrushType crushable;
    /// <summary>
    /// Used for gravity - Indicates if gravity tests have been already done on placeable.
    /// </summary>
    public bool explored;
    /// <summary>
    /// Used for gravity - Indicates if it is connected to the ground.
    /// </summary>
    public bool grounded;
    /// <summary>
    /// Current coroutine used for the movement.
    /// </summary>
    private Coroutine moveCoroutine;

    /// <summary>
    /// player who owns the placeable. players, neutral monsters, and null (independant blocs)
    /// </summary>
    public Player player;

    public override bool IsLiving()
    {
        return false;
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

    public CrushType Crushable
    {
        get
        {
            return crushable;
        }
    }

    public virtual Player Player
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
    

    public float AnimationSpeed
    {
        get
        {
            return animationSpeed;
        }
    }

    public Coroutine MoveCoroutine
    {
        get
        {
            return moveCoroutine;
        }

        set
        {
            moveCoroutine = value;
        }
    }




    /// <summary>
    /// Copy object
    /// </summary>
    /// <returns>Return a copy of the object</returns>
    public virtual Placeable Cloner()
    {
        Placeable copy = (Placeable)this.MemberwiseClone();
        return copy;
    }

    /// <summary>
    /// method to call for destroying object
    /// </summary>
    public abstract void Destroy();

    public virtual void Highlight()
    {
        if (GameManager.instance.activeSkill != null && GameManager.instance.activeSkill.SkillType == SkillType.BLOCK)
        {
            GameObject quadUp = transform.Find("Quads").Find("QuadUp").gameObject;
            GameObject quadRight = transform.Find("Quads").Find("QuadRight").gameObject;
            GameObject quadLeft = transform.Find("Quads").Find("QuadLeft").gameObject;
            GameObject quadFront = transform.Find("Quads").Find("QuadFront").gameObject;
            GameObject quadBack = transform.Find("Quads").Find("QuadBack").gameObject;

            quadUp.SetActive(true);

            quadRight.SetActive(true);
            quadRight.transform.localScale = new Vector3(quadRight.transform.localScale.x, sizeQuad, 1);
            quadRight.transform.localPosition = new Vector3(quadRight.transform.localPosition.x, 0, quadRight.transform.localPosition.z);

            quadLeft.SetActive(true);
            quadLeft.transform.localScale = new Vector3(quadLeft.transform.localScale.x, sizeQuad, 1);
            quadLeft.transform.localPosition = new Vector3(quadLeft.transform.localPosition.x, 0, quadLeft.transform.localPosition.z);

            quadFront.SetActive(true);
            quadFront.transform.localScale = new Vector3(quadFront.transform.localScale.x, sizeQuad, 1);
            quadFront.transform.localPosition = new Vector3(quadFront.transform.localPosition.x, 0, quadFront.transform.localPosition.z);

            quadBack.SetActive(true);
            quadBack.transform.localScale = new Vector3(quadBack.transform.localScale.x, sizeQuad, 1);
            quadBack.transform.localPosition = new Vector3(quadBack.transform.localPosition.x, 0, quadBack.transform.localPosition.z);

        }
        foreach (Transform fils in transform.Find("Quads"))
        {

            fils.gameObject.SetActive(true);
            fils.gameObject.GetComponent<MeshRenderer>().material = GameManager.instance.highlightingMaterial;
        }
    }
    public virtual void UnHighlight()
    {

        //Put back the default material
        foreach (Transform fils in transform.Find("Quads"))
        {
            fils.gameObject.GetComponent<MeshRenderer>().material = GameManager.instance.pathFindingMaterial;
        }
        //If we are in move mode doesn't belong to path we desactivate it
        if (GameManager.instance.State != States.Move || GameManager.instance.playingPlaceable != null && GameManager.instance.playingPlaceable.AreaOfMouvement != null &&
            !GameManager.instance.playingPlaceable.AreaOfMouvement.Exists(new NodePath(GetPosition().x, GetPosition().y, GetPosition().z, 0, null).Equals))

        {


            foreach (Transform fils in transform.Find("Quads"))
            {

                fils.gameObject.SetActive(false);

            }
        }

    }

    public void OnMouseOverWithLayer()
    {
        if (GameManager.instance.State == States.Spawn) // AMELIORATION: This part should be in LivingPlaceable and not here
        {
            if (GameManager.instance.GetLocalPlayer().Isready != true) {
                if (Input.GetMouseButtonUp(0))
                {

                    if (!this.IsLiving() && ((StandardCube)this).isSpawnPoint == true // AMELIORATION: Maybe could be remove
                                                                                      // Check if it is a spawn point for that player
                            && Grid.instance.GetSpawnPlayer(GameManager.instance.GetLocalPlayer()).Contains(GetPosition() + Vector3Int.up)
                            && GameManager.instance.CharacterToSpawn != null)
                    {
                        Debug.Log("You have authority to ask to spawn");

                        if (Grid.instance.GridMatrix[this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z] == null)
                        {
                            Grid.instance.GridMatrix[this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z] = GameManager.instance.CharacterToSpawn;
                            Grid.instance.GridMatrix[GameManager.instance.CharacterToSpawn.GetPosition().x, GameManager.instance.CharacterToSpawn.GetPosition().y,
                                GameManager.instance.CharacterToSpawn.GetPosition().z] = null;
                            GameManager.instance.CharacterToSpawn.transform.position = new Vector3(this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z);
                        }
                        else if (Grid.instance.GridMatrix[this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z].IsLiving()
                            && Grid.instance.GridMatrix[this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z].player == GameManager.instance.GetLocalPlayer())
                        {
                            Placeable temp = Grid.instance.GridMatrix[this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z];
                            Grid.instance.GridMatrix[GameManager.instance.CharacterToSpawn.GetPosition().x, GameManager.instance.CharacterToSpawn.GetPosition().y,
                                GameManager.instance.CharacterToSpawn.GetPosition().z] = temp;
                            Grid.instance.GridMatrix[this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z] = GameManager.instance.CharacterToSpawn;
                            temp.transform.position = new Vector3(GameManager.instance.CharacterToSpawn.GetPosition().x, GameManager.instance.CharacterToSpawn.GetPosition().y,
                                GameManager.instance.CharacterToSpawn.GetPosition().z);
                            GameManager.instance.CharacterToSpawn.transform.position = new Vector3(this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z);
                        }
                        GameManager.instance.CharacterToSpawn = null;
                    }
                    else if (player == GameManager.instance.GetLocalPlayer())
                    {
                        if (!GameManager.instance.CharacterToSpawn)
                        {
                            GameManager.instance.CharacterToSpawn = (LivingPlaceable)this;
                        }
                        else
                        {
                            /*
                            //Placeable temp = Grid.instance.GridMatrix[this.GetPosition().x, this.GetPosition().y - 1, this.GetPosition().z];
                            Grid.instance.GridMatrix[GameManager.instance.CharacterToSpawn.GetPosition().x, GameManager.instance.CharacterToSpawn.GetPosition().y,
                                GameManager.instance.CharacterToSpawn.GetPosition().z] = temp;
                            Grid.instance.GridMatrix[this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z] = GameManager.instance.CharacterToSpawn;
                            temp.transform.position = new Vector3(GameManager.instance.CharacterToSpawn.GetPosition().x, GameManager.instance.CharacterToSpawn.GetPosition().y,
                                GameManager.instance.CharacterToSpawn.GetPosition().z);
                            GameManager.instance.CharacterToSpawn.transform.position = new Vector3(this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z);
                            */

                            Vector3Int temp = GetPosition();
                            Grid.instance.GridMatrix[GameManager.instance.CharacterToSpawn.GetPosition().x, GameManager.instance.CharacterToSpawn.GetPosition().y,
                                GameManager.instance.CharacterToSpawn.GetPosition().z] = this;
                            this.transform.position = GameManager.instance.CharacterToSpawn.transform.position;
                            Grid.instance.GridMatrix[temp.x, temp.y, temp.z] = GameManager.instance.CharacterToSpawn;
                            GameManager.instance.CharacterToSpawn.transform.position = temp;


                            GameManager.instance.CharacterToSpawn = null;
                        }
                    }
                }
            }
        }
        else if (GameManager.instance.State == States.Move)
        {
            // Debug.Log(EventSystem.current.IsPointerOverGameObject());
            if (Input.GetMouseButtonUp(0) && this.walkable)
            {
                if (GameManager.instance.playingPlaceable.Player.isLocalPlayer && !GameManager.instance.playingPlaceable.Player.GetComponent<Player>().isWinner)
                {
                    Debug.Log("You have authority to ask for a deplacment move");
                    //Vector3 destination = this.GetPosition();
                    Vector3[] path = GameManager.instance.GetPathFromClicked(this);//Check and move on server
                    GameManager.instance.playingPlaceable.Player.CmdMoveTo(path);
                    // GameManager.instance.CheckIfAccessible(this);
                }
            }
        }
        else if (GameManager.instance.State == States.UseSkill)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Skill skill = GameManager.instance.activeSkill;
                if (GameManager.instance.playingPlaceable.Player.isLocalPlayer && !GameManager.instance.playingPlaceable.Player.GetComponent<Player>().isWinner
                    && skill != null && (GameManager.instance.PlayingPlaceable.TargetArea.Contains(this) ||
                    IsLiving() && GameManager.instance.PlayingPlaceable.TargetableUnits.Contains((LivingPlaceable)this))

                    /*&& (skill.SkillType == SkillType.LIVING && IsLiving() 
                    || (skill.SkillType == SkillType.BLOCK || skill.SkillType == SkillType.AREA) && !IsLiving())*/)


                {

                    Debug.Log("You have authority to ask to act on " + netId + " On position" + GetPosition() + "Time : " + Time.time);
                    List<Placeable> area = GameManager.instance.playingPlaceable.player.GetComponentInChildren<RaycastSelector>().Area;
                    if (area == null && skill.SkillArea != SkillArea.SURROUNDINGLIVING)
                    {
                        GameManager.instance.playingPlaceable.player.OnUseSkill(Player.SkillToNumber(GameManager.instance.playingPlaceable, skill), netId, new int[0], 0);
                    }
                    else if (skill.SkillArea == SkillArea.SURROUNDINGLIVING || skill.SkillArea == SkillArea.MIXEDAREA)
                    {
                        List<LivingPlaceable> Playerlist = GameManager.instance.playingPlaceable.TargetableUnits;
                        int j = 0;
                        int[] netidlist;

                        if (skill.SkillArea == SkillArea.MIXEDAREA)
                        {
                            netidlist = new int[Playerlist.Count + area.Count];
                            for (j = 0; j < area.Count; j++)
                            {
                                netidlist[j] = area[j].netId;
                            }
                        }
                        else
                        {
                            netidlist = new int[Playerlist.Count];
                        }

                        for (int i = j; i < netidlist.Length; i++)
                        {
                            netidlist[i] = Playerlist[i - j].netId;
                        }

                        GameManager.instance.playingPlaceable.player.OnUseSkill(Player.SkillToNumber(GameManager.instance.playingPlaceable, skill), netId, netidlist,
                            GameManager.instance.playingPlaceable.player.GetComponentInChildren<RaycastSelector>().State);
                    }
                    else
                    {
                        int[] netidlist = new int[area.Count];
                        for (int i = 0; i < netidlist.Length; i++)
                        {
                            netidlist[i] = area[i].netId;
                        }
                        GameManager.instance.playingPlaceable.player.OnUseSkill(Player.SkillToNumber(GameManager.instance.playingPlaceable, skill), netId, netidlist,
                            GameManager.instance.playingPlaceable.player.GetComponentInChildren<RaycastSelector>().State);
                    }
                    //GameManager.instance.activeSkill.Use(GameManager.instance.playingPlaceable, new List<Placeable>(){this});
                }
            }

        }
    }
    /// <summary>
    /// allows shoot and shifting
    /// </summary>
    protected void OnMouseOver()
    {

    }

    protected virtual void Awake()
    {

        baseMaterial = GetComponent<Renderer>().material;
    }

    /// <summary>
    /// To call when creating a Placeable to initialize needed values
    /// </summary>
    public virtual void Init()
    {

    }

    /// <summary>
    /// On dispatch selon Living et placeable
    /// </summary>
    /// <param name="effect"></param>
    public override void DispatchEffect(Effect effect)
    {
        effect.TargetAndInvokeEffectManager(this);
    }
}
