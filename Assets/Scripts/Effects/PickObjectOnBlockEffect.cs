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

    public override void Preview(Placeable target)
    {
        throw new System.NotImplementedException();
    }

    public override void ResetPreview(Placeable target)
    {
        throw new System.NotImplementedException();
    }

    public override void Use()
    {
       if(Target.isPickable)
        {
            Debug.Log("Object picked!");
            Target.transform.SetParent(Launcher.transform.Find("Inventory"));
            Target.transform.localPosition = new Vector3();
            Target.GetComponentInChildren<Transform>().localPosition = Vector3.down;
            Target.transform.localRotation = Quaternion.identity;
            GameManager.instance.PlayingPlaceable.Player.GetComponent<UIManager>().updateSpecialAbilities(GameManager.instance.PlayingPlaceable,
                GameManager.instance.PlayingPlaceable.GetPosition());
        }
    }
}
