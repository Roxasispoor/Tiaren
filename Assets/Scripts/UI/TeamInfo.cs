using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TeamInfo : MonoBehaviour
{
    #region variables
    private string teamName;
    public TMP_InputField nameText;
    public Vector3 textBasePos = new Vector2();
    public Vector3 textModifyPos;
    public GameObject modifyButton;
    public List<Toggle> TeamSlots;

    public List<Vector2> basePositions = new List<Vector2>();

    public List<Vector2> modifyPositions;

    public CharacterMenuInfo[] chosenCharacters;

    //Gameobjects for warnings
    public GameObject teamNameWarning;
    public List<Image> warnings;


    private CharacterMenuInfo[] previousCharacters;
    private string previousName;

    private int activeSlot;

    public string TeamName { get => teamName; set => teamName = value; }

    #endregion

    private void Awake()
    {
        chosenCharacters = new CharacterMenuInfo[3];
        previousCharacters = new CharacterMenuInfo[3];

    }

    private void Start()
    {
        textBasePos = nameText.transform.position;
        for (int i = 0; i < TeamSlots.Count; i++)
        {
            basePositions[i] = TeamSlots[i].transform.position;
            TeamSlots[i].interactable = false;
        }
    }

    public void UpdateDisplay()
    {
        nameText.text = teamName;

        for (int i = 0; i < chosenCharacters.Length; i++)
        {
            selectiveUpdate(i);
        }
    }

    private void selectiveUpdate(int slot)
    {
        if (null == chosenCharacters[slot])
        {
            TeamSlots[slot].image.sprite = null;
        }
        else
        {
            TeamSlots[slot].image.sprite = chosenCharacters[slot].characterSprite;
        }
    }

    public void UpdateInfo(string name, CharacterMenuInfo[] characters)
    {
        teamName = name;
        for (int i = 0; i < characters.Length; i++)
        {
            chosenCharacters[i] = characters[i];
        }
        UpdateDisplay();
    }

    public void DeactivateModifyUI()
    {
        TeamSlots[0].group.SetAllTogglesOff();
        for (int i = 0; i < TeamSlots.Count; i++)
        {
            TeamSlots[i].transform.position = basePositions[i];
            TeamSlots[i].interactable = false;
        }

        modifyButton.SetActive(true);

        nameText.transform.position = textBasePos;
        nameText.interactable = false;
    }

    public bool ActivateModifyUI()
    {
        for (int i = 0; i < TeamSlots.Count; i++)
        {
            TeamSlots[i].transform.position = modifyPositions[i];
            TeamSlots[i].interactable = true;
        }

        modifyButton.SetActive(false);

        nameText.transform.position = textModifyPos;
        nameText.interactable = true;

        activeSlot = 0;
        TeamSlots[0].isOn = true;

        SaveInfoForCancel();

        return true;
    }

    private void SaveInfoForCancel()
    {
        previousName = teamName;
        for (int i = 0; i < chosenCharacters.Length; i++)
        {
            previousCharacters[i] = chosenCharacters[i];
        }
    }

    public bool CheckReadyForSave()
    {
        if (null == teamName)
        {
            teamNameWarning.GetComponent<FadingUI>().StartPulse(teamNameWarning.GetComponent<TMP_Text>());
            return false;
        }
        for (int i = 0; i < chosenCharacters.Length; i++)
        {
            if (null == chosenCharacters[i])
            {
                warnings[i].GetComponent<FadingUI>().StartPulse(warnings[i]);
                return false;
            }
        }
        return true;
    }

    public void ChangeSlot(Toggle toggle)
    {
        activeSlot = TeamSlots.FindIndex(toggle.Equals);
    }

    public void SetCharacterForSlot(CharacterMenuInfo character)
    {
        chosenCharacters[activeSlot] = character;
        selectiveUpdate(activeSlot);
    }

    public void ChangeTeamName()
    {
        TeamName = nameText.text;
    }

    public void ResetModification()
    {
        teamName = previousName;
        for(int i = 0; i < chosenCharacters.Length; i++)
        {
            chosenCharacters[i] = previousCharacters[i];
        }
        UpdateDisplay();
    }
}
