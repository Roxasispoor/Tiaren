using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to create debug tool for non-editor build.
/// </summary>
public class Debugger : MonoBehaviour
{
    /// <summary>
    /// Should it display information or not.
    /// </summary>
    public bool activateDisplay;
    /// <summary>
    /// Canvas used to display information.
    /// </summary>
    [SerializeField]
    private Canvas canvas;
    /// <summary>
    /// The text used to display information.
    /// </summary>
    private UnityEngine.UI.Text Text;

    /// <summary>
    /// The list of all the UI element corresponding to the chara.
    /// </summary>
    private List<CharacterDisplay> characterDisplays;

    // Start is called before the first frame update
    void Start()
    {
        activateDisplay = false;
        canvas.gameObject.SetActive(activateDisplay);
        Text = canvas.GetComponentInChildren<UnityEngine.UI.Text>();
        characterDisplays = new List<CharacterDisplay>();
    }

    // Update is called once per frame
    void Update()
    {
        // Press B to activate/deactive display
        if (Input.GetKeyDown("b"))
        {
            activateDisplay = !activateDisplay;
            canvas.gameObject.SetActive(activateDisplay);
        }
        if (activateDisplay && GameManager.instance != null)
        {
            if (GameManager.instance.isGameStarted)
            {
                characterDisplays.Clear();
                foreach (CharacterDisplay chara in GameManager.instance.GetLocalPlayer().GetComponentsInChildren<CharacterDisplay>())
                {
                    characterDisplays.Add(chara);
                }
                Debug.Log("Loaded " + characterDisplays.Count + "Characters");
            }

            if (GameManager.instance.Hovered != null)
            {
                DisplayInfo(GameManager.instance.Hovered);
            } else
            {
                foreach (CharacterDisplay chara in characterDisplays)
                {
                    if (chara.isHovered)
                    {
                        DisplayInfo(chara.Character);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Format and display the formation for the Placeable.
    /// </summary>
    /// <param name="placeable">Placeable to show in the debugger</param>
    private void DisplayInfo(Placeable placeable)
    {
        Text.text = "";
        Text.text = placeable.name;
        Text.text += "\nNetId: " + placeable.netId;


        Vector3Int pos = placeable.GetPosition();
        Text.text += "\nGetPos: " + pos;

        // Try to cast as StandardCube
        StandardCube cube = placeable as StandardCube;

        if (cube)
        {
            NewLine("Type", "StandardCube");
        }

        // Try to cast as StandardCube
        LivingPlaceable chara = placeable as LivingPlaceable;

        if (chara)
        {
            NewLine("Type", "Living");
            NewLine("SpeedStack", chara.SpeedStack.ToString());
        }
    }

    private void NewLine (string title, string info)
    {
        Text.text += "\n" + title + ": " + info;
    }
}
