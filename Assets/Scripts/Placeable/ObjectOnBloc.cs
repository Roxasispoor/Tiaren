using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class Containing that may contain a pickable object
/// </summary>
public class ObjectOnBloc : NetIdeable {
    public bool isPickable;
    public bool isPicked;
    public bool dropOnDeathPicker;
    public bool dropOnDeathBlocUnder;
    public override void DispatchEffect(Effect effect)
    {
      
        effect.TargetAndInvokeEffectManager(this);
    }
    //Does nothing if you try to pick An object on block that isn't blockable
    public virtual void GetPicked(Placeable Luncher)
    {
    }

    private List<Effect> onWalk;


    private List<Skill> givenSkills;
    public List<Effect> OnWalk
    {
        get
        {
            return onWalk;
        }

        set
        {
            onWalk = value;
        }
    }

    public List<Skill> GivenSkills
    {
        get
        {
            return givenSkills;
        }

        set
        {
            givenSkills = value;
        }
    }
    protected void Awake()
    {
        shouldBatch = false;
        givenSkills = new List<Skill>();
        if(isPickable)
        {
            PickObjectOnBlockEffect eff = new PickObjectOnBlockEffect(this);
            Skill taker = new Skill(0, 0, new List<Effect>() { eff }, SkillType.ALREADYTARGETED, "Pick Object", 0, 1);//No cooldown because skills from these objects aren't resetted
            givenSkills.Add(taker);
        }
    }
    public virtual void Destroy()
    {
        if(isPicked)
        {
            OnDestroyPicker();
        }
        else
        {
            OnDestroyBlocUnder();
        }
    }
    public virtual void OnDestroyBlocUnder()
    {
        if (dropOnDeathBlocUnder)
        {
            Drop();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public virtual void OnDestroyPicker()
    {
        if(dropOnDeathPicker)
        {
            Drop();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public virtual void SomethingPutAbove()
    {

    }
    public void Drop()
    {
        Debug.Log("Drop!");
        Vector3Int currentPos = GetPosition();
        while(Grid.instance.GridMatrix[currentPos.x, currentPos.y,currentPos.z]==null && currentPos.y>0)
        {
            currentPos = new Vector3Int(currentPos.x, currentPos.y - 1, currentPos.z);
        }
        if(currentPos.y==0 && Grid.instance.GridMatrix[currentPos.x, currentPos.y, currentPos.z]==null)
        {
            Debug.LogError("Object was droped with nothing under!");
        }
        else
        {
            isPicked = false;
            transform.SetParent(Grid.instance.GridMatrix[currentPos.x, currentPos.y, currentPos.z].transform.Find("Inventory"));
            transform.localPosition = new Vector3();
            GetComponentInChildren<Transform>().localPosition = Vector3.up;
        }

    }
    public virtual string Save()
    {
        return serializeNumber + ";" + netId + ";" + GetPosition();
    }

    public virtual void Load(string[] objectInfo)
    {
        
    }
    public virtual void Initialize()
    {
        if(isPickable && Grid.instance.GetPlaceableFromVector(GetPosition()).IsLiving())
        {
            isPicked = true;
        }
    }
}
