using UnityEngine;

public class CreateZipLine : CreateBlock
{
    public CreateZipLine(CreateBlock other) : base(other)
    {

    }

    public CreateZipLine(GameObject prefab, Vector3Int face) : base(prefab, face)
    {
    }
    public CreateZipLine(int prefabNumber, Vector3Int face):base(prefabNumber,face)
    {
    }

    public override Effect Clone()
    {
        return new CreateZipLine(this);
    }
    public override void Use()
    {
        if (prefab.GetComponent<ZipLine>() != null)
        {
            if (GameManager.instance.isClient)
            {
                GetLauncherAnimation();
                animLauncher.Play("createBlock");
            }
            GameObject zip1 = Grid.instance.InstanciateObjectOnBloc(prefab, Launcher.GetPosition() - new Vector3Int(0, 1, 0));
            GameObject zip2 = Grid.instance.InstanciateObjectOnBloc(prefab, Target.GetPosition());
            zip1.GetComponent<ZipLine>().linkedTo = zip2.GetComponent<ZipLine>();
            zip2.GetComponent<ZipLine>().linkedTo = zip1.GetComponent<ZipLine>();
        }
    }
}