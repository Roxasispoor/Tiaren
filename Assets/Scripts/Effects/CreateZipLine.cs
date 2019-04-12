using UnityEngine;

public class CreateZipLine : CreateBlock
{
    private GameObject zip;

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

    public override void Preview(NetIdeable target)
    {
        Launcher = GameManager.instance.PlayingPlaceable;
        Target = target as StandardCube;   
        if (Target == null)
        {
            Debug.LogError("CreateZipline, something went wrong");
        }
        FXManager.Instance.ZiplinePreview((StandardCube)Target, (LivingPlaceable)Launcher);
    }

    public void createMesh()
    {
        GameObject zip1 = Grid.instance.InstanciateObjectOnBloc(prefab, Launcher.GetPosition() - new Vector3Int(0, 1, 0));
        GameObject zip2 = Grid.instance.InstanciateObjectOnBloc(prefab, Target.GetPosition());
        zip1.GetComponent<ZipLine>().linkedTo = zip2.GetComponent<ZipLine>();
        zip2.GetComponent<ZipLine>().linkedTo = zip1.GetComponent<ZipLine>();
        zip1.GetComponentInChildren<ZipLine>().rope = zip1.GetComponentInChildren<ZiplineFX>().ConnectZipline(zip2.GetComponentInChildren<ZiplineFX>());
    }

    public override void ResetPreview(NetIdeable target)
    {
        FXManager.Instance.ZiplineUnpreview();
    }

    public override void Use()
    {
        if (prefab.GetComponent<ZipLine>() != null)
        {
            ResetPreview(Target);
            createMesh();
            GameManager.instance.PlayingPlaceable.Player.GetComponent<UIManager>().updateSpecialAbilities(GameManager.instance.PlayingPlaceable,
            GameManager.instance.PlayingPlaceable.GetPosition());
        }
        else
        {
            Debug.LogError("Zipline not found");
        }
    }
}