using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Makibishi : ObjectOnBloc
{
    public int power;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        isPickable = false;
        isPicked = false;
        dropOnDeathPicker = false;
        dropOnDeathBlocUnder = false;
        stopOnWalk = true;
        OnWalk = new List<Effect>();
    }

    public void Init(int power)
    {
        OnWalk.Add(new DamageCalculated(power, DamageCalculated.DamageScale.BRUT, 0));
        OnWalk[0].Launcher = this;
        this.power = power;
    }

    public override void Destroy()
    {
        ((StandardCube)parent).ObjectOns.Remove(this);
        Destroy(gameObject);
    }

    public override void SomethingPutAbove()
    {
        base.SomethingPutAbove();
        if (!Grid.instance.GetPlaceableFromVector(GetPosition() + new Vector3Int(0, 1, 0)).IsLiving())
        {
            Destroy();
        }
    }

    public override void TriggerOnWalk(Placeable target)
    {
        gameObject.SetActive(true);
        for(int i = OnWalk.Count - 1; i>=0; i--)
        {
            target.DispatchEffect(OnWalk[i]);
        }
        Destroy();
    }
}
