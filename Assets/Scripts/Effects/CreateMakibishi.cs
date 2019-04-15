using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMakibishi : CreateBlock
{
    private int power;

    public CreateMakibishi(GameObject prefab, Vector3Int face, int power) : base(prefab, face)
    {
        this.power = power;
    }

    public void createMesh()
    {
        GameObject Makibishi = Grid.instance.InstanciateObjectOnBloc(prefab, Target.GetPosition());
        Makibishi.GetComponent<Makibishi>().Init(power);
        if(((LivingPlaceable)Launcher).Player != GameManager.instance.GetLocalPlayer())
        {
            Makibishi.SetActive(false);
        }
    }

    public override void Preview(NetIdeable target)
    {
        Launcher = GameManager.instance.PlayingPlaceable;
        Target = target as StandardCube;
        if (Target == null)
        {
            Debug.LogError("Create makibishi, target is not standard cube");
        }
        FXManager.instance.MakibishiPreview((StandardCube)Target);
    }

    public override void ResetPreview(NetIdeable target)
    {
        FXManager.instance.MakibishiUnpreview();
    }

    public override void Use()
    {
        if (prefab.GetComponent<Makibishi>() != null)
        {
            ResetPreview(Target);
            createMesh();
        }
        else
        {
            Debug.LogError("makibishi not found");
        }
    }
}
