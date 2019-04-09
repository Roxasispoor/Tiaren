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
        Target = target as Placeable;   //TODO: (Pierrick:) pourquoi la taget c'est un placeable ?
        if (Target = null)
        {
            Debug.LogError("CreateZipline, something went wrong");
        }
        createMesh(true);
    }

    public void createMesh(bool preview)
    {
        GameObject zip1 = Grid.instance.InstanciateObjectOnBloc(prefab, Launcher.GetPosition() - new Vector3Int(0, 1, 0));
        GameObject zip2 = Grid.instance.InstanciateObjectOnBloc(prefab, Target.GetPosition());
        zip1.GetComponent<ZipLine>().linkedTo = zip2.GetComponent<ZipLine>();
        zip2.GetComponent<ZipLine>().linkedTo = zip1.GetComponent<ZipLine>();
        if (preview == true)
        {
            zip1.GetComponentInChildren<MeshRenderer>().material = GameManager.instance.materialPreviewCreate;
            zip1.GetComponentInChildren<MeshRenderer>().material.shader = GameManager.instance.PlayingPlaceable.outlineShader;
            zip2.GetComponentInChildren<MeshRenderer>().material = GameManager.instance.materialPreviewCreate;
            zip2.GetComponentInChildren<MeshRenderer>().material.shader = GameManager.instance.PlayingPlaceable.outlineShader;

            zip = zip1;
        }
        zip1.GetComponentInChildren<ZipLine>().rope = zip1.GetComponentInChildren<ZiplineFX>().ConnectZipline(zip2.GetComponentInChildren<ZiplineFX>(), preview);
    }

    public override void ResetPreview(NetIdeable target)
    {
        if (zip != null)
        {
            zip.GetComponent<ZipLine>().Destroy();
        }
    }

    public override void Use()
    {
        if (prefab.GetComponent<ZipLine>() != null)
        {
            ResetPreview(Target);
            createMesh(false);
            GameManager.instance.PlayingPlaceable.Player.GetComponent<UIManager>().updateSpecialAbilities(GameManager.instance.PlayingPlaceable,
            GameManager.instance.PlayingPlaceable.GetPosition());
        }
        else
        {
            Debug.LogError("Zipline not found");
        }
    }
}