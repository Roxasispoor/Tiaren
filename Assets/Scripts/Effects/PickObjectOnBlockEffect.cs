using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickObjectOnBlockEffect : EffectOnObjectBloc
{
    public PickObjectOnBlockEffect(ObjectOnBloc target)
    {
        Target = target;
    }
    public PickObjectOnBlockEffect(PickObjectOnBlockEffect other) : base(other)
    {
       
    }
    public override Effect Clone()
    {
        return new PickObjectOnBlockEffect(this);
    }

    public override void Use()
    {
       if(Target.isPickable)
        {
            Debug.Log("Object picked!");
            Target.transform.SetParent(Launcher.transform.Find("Inventory"));
            Target.transform.localPosition = new Vector3();
            GameManager.instance.playingPlaceable.Player.GetComponent<UIManager>().UpdateAbilities(GameManager.instance.playingPlaceable,
                GameManager.instance.playingPlaceable.GetPosition());
        }
    }
}
