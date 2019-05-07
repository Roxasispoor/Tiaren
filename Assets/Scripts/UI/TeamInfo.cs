using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TeamInfo : MonoBehaviour
{
    #region variables

    public enum MouvementEndings { Character, Team, None}

    private TeamBuilder builder;

    public int teamNumber;

    private string teamName;
    public TMP_InputField nameText;
    public Vector3 textBasePos = new Vector2();
    public Vector3 textModifyPos;
    public GameObject modifyButton;
    public GameObject removeButton;
    public List<Toggle> teamSlots;

    public List<Vector2> basePositions = new List<Vector2>();

    public List<Vector2> modifyPositions;

    public CharacterMenuInfo[] chosenCharacters;

    //Gameobjects for warnings
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
        textBasePos = nameText.transform.localPosition;
        for (int i = 0; i < teamSlots.Count; i++)
        {
            basePositions[i] = teamSlots[i].transform.localPosition;
            teamSlots[i].interactable = false;
        }
        builder = gameObject.GetComponentInParent<TeamBuilder>();
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
            teamSlots[slot].image.sprite = null;
        }
        else
        {
            teamSlots[slot].image.sprite = chosenCharacters[slot].characterSprite;
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

    public void UpdatePositionInfo()
    {
        textBasePos = nameText.transform.localPosition;
        for (int i = 0; i < teamSlots.Count; i++)
        {
            basePositions[i] = teamSlots[i].transform.localPosition;
        }
    }

    public void DeactivateModifyUI()
    {
        teamSlots[0].group.SetAllTogglesOff();
        for (int i = 0; i < teamSlots.Count; i++)
        {
            teamSlots[i].interactable = false;
            StartCoroutine(CoMoveObjectY(0.5f, modifyPositions[i].y + builder.teamGap.y * (teamNumber + 1), basePositions[i].y, teamSlots[i].gameObject, MouvementEndings.Team));
            StartCoroutine(CoMoveObjectX(0.5f, modifyPositions[i].x, basePositions[i].x, teamSlots[i].gameObject, MouvementEndings.Team));
        }

        modifyButton.SetActive(true);
        removeButton.SetActive(true);

        StartCoroutine(CoMoveObjectY(0.5f, textModifyPos.y + builder.teamGap.y * (teamNumber+1), textBasePos.y, nameText.gameObject, MouvementEndings.None));
        StartCoroutine(CoMoveObjectX(0.5f, textModifyPos.x, textBasePos.x, nameText.gameObject, MouvementEndings.None));
        nameText.interactable = false;
    }

    public void ActivateModifyUI()
    {
        builder.ActivateModifyTeam(this);

        for (int i = 0; i < teamSlots.Count; i++)
        {
            StartCoroutine(CoMoveObjectY(1f, basePositions[i].y, modifyPositions[i].y + builder.teamGap.y * (teamNumber + 1), teamSlots[i].gameObject, MouvementEndings.Character));
            StartCoroutine(CoMoveObjectX(1f, basePositions[i].x, modifyPositions[i].x, teamSlots[i].gameObject, MouvementEndings.Character));
            teamSlots[i].interactable = true;
        }

        modifyButton.SetActive(false);
        removeButton.SetActive(false);
        
        nameText.interactable = true;

        activeSlot = 0;
        teamSlots[0].isOn = true;

        SaveInfoForCancel();

        StartCoroutine(CoMoveObjectY(0.3f, textBasePos.y, textModifyPos.y + builder.teamGap.y * (teamNumber + 1), nameText.gameObject, MouvementEndings.None));
        StartCoroutine(CoMoveObjectX(0.3f, textBasePos.x, textModifyPos.x, nameText.gameObject, MouvementEndings.None));
        
    }

    protected IEnumerator CoMoveObjectY(float time, float startY, float endY, GameObject objectToMove, MouvementEndings endings)
    {
        float iterator = 0;
        while (iterator < time)
        {
            iterator += Time.deltaTime;
            if (iterator > time) iterator = time;

            float val = JoniUtility.Easing.GetTerpedPosition(iterator, time, JoniUtility.Easing.Function.Circular, JoniUtility.Easing.Direction.In);

            float newY = Mathf.Lerp(startY, endY, val);

            Vector3 newPos = new Vector3(objectToMove.transform.localPosition.x, newY, objectToMove.transform.localPosition.z);

            objectToMove.transform.localPosition = newPos;
            yield return 0;
        }
        if (endings == MouvementEndings.Character)
        {
            builder.ShowCharacters(this);
        }
        else if (endings == MouvementEndings.Team)
        {
            builder.ActivateTeams();
        }
    }

    protected IEnumerator CoMoveObjectX(float time, float startX, float endX, GameObject objectToMove, MouvementEndings endings)
    {
        float iterator = 0;
        while (iterator < time)
        {
            iterator += Time.deltaTime;
            if (iterator > time) iterator = time;

            float val = JoniUtility.Easing.GetTerpedPosition(iterator, time, JoniUtility.Easing.Function.Quadratic, JoniUtility.Easing.Direction.Out);

            float newX = Mathf.Lerp(startX, endX, val);

            Vector3 newPos = new Vector3(newX, objectToMove.transform.localPosition.y, objectToMove.transform.localPosition.z);

            objectToMove.transform.localPosition = newPos;
            yield return 0;
        }
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
            FadingUI textToFade = builder.teamNameWarning;
            textToFade.StartPulseText(textToFade.gameObject.GetComponent<TMP_Text>());
            return false;
        }
        for (int i = 0; i < chosenCharacters.Length; i++)
        {
            if (null == chosenCharacters[i])
            {
                warnings[i].GetComponent<FadingUI>().StartPulseImage(warnings[i]);
                return false;
            }
        }
        return true;
    }

    public void ChangeSlot(Toggle toggle)
    {
        activeSlot = teamSlots.FindIndex(toggle.Equals);
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

    public void EraseTeam()
    {
        builder.RemoveTeam(this);
    }
}
