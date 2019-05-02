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
    public Vector2 textBasePos = new Vector2();
    public Vector2 textModifyPos;
    public Button modifyButton;
    public List<Toggle> TeamSlots;

    public List<Vector2> basePositions = new List<Vector2>();

    public List<Vector2> modifyPositions;

    public List<CharacterMenuInfo> ChosenCharacters;



    private int activeSlot;

    public string TeamName { get => teamName; set => teamName = value; }

    #endregion

    private void Awake()
    {
        ChosenCharacters = new List<CharacterMenuInfo>();
        textBasePos = nameText.transform.localPosition;
        for(int i = 0; i < TeamSlots.Count; i++)
        {
            basePositions[i] = TeamSlots[i].transform.localPosition;
            TeamSlots[i].interactable = false;
        }
    }

    public void UpdateDisplay()
    {
        if (teamName != null)
        {
            nameText.text = teamName;
        }
        for(int i = 0; i < ChosenCharacters.Count; i++)
        {
            TeamSlots[i].image.sprite = ChosenCharacters[i].characterSprite;
        }
    }

    public void UpdateInfo(string name, CharacterMenuInfo[] characters)
    {
        teamName = name;
        ChosenCharacters.Clear();
        foreach(CharacterMenuInfo character in characters)
        {
            ChosenCharacters.Add(character);
        }
        UpdateDisplay();
    }

    public void DeactivateModifyUI()
    {
        for (int i = 0; i < TeamSlots.Count; i++)
        {
            TeamSlots[i].transform.localPosition = basePositions[i];
            TeamSlots[i].interactable = false;
            TeamSlots[i].isOn = false;
        }

        modifyButton.gameObject.SetActive(true);

        nameText.transform.localPosition = textBasePos;
    }

    public void ActivateModifyUI()
    {
        for(int i = 0; i < TeamSlots.Count; i++)
        {
            TeamSlots[i].transform.localPosition = modifyPositions[i];
            TeamSlots[i].interactable = true;
        }

        modifyButton.gameObject.SetActive(false);

        nameText.transform.localPosition = textModifyPos;

        activeSlot = 0;
        TeamSlots[0].isOn = true;
    }
}
